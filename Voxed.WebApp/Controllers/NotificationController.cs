﻿using Core.Data.Repositories;
using Core.Entities;
using Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;

namespace Voxed.WebApp.Controllers;

[Route("notification")]
public class NotificationController : BaseController
{
    private readonly IBlogRepository _blogRepository;
    private readonly IHubContext<NotificationHub, INotificationHub> _notificationHub;

    public NotificationController(
        UserManager<User> userManager,
        IBlogRepository blogRepository,
        IHubContext<NotificationHub, INotificationHub> notificationHub,
        IHttpContextAccessor accessor) : base(accessor, userManager)
    {
        _blogRepository = blogRepository;
        _notificationHub = notificationHub;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (id == Guid.Empty) return BadRequest();

        var notification = await _blogRepository.Notifications.GetById(id);
        if (notification == null) { return Redirect($"/"); }

        var userId = User.GetUserId();
        if (userId != notification.UserId) return Redirect("/");

        var voxHash = notification.PostId.ToShortString();
        var commentHash = notification.Comment.Hash;

        await _notificationHub.Clients
            .User(notification.UserId.ToString())
            .RemoveNotification(new RemoveNotificationModel() { Id = notification.Id.ToString() });

        await _blogRepository.Notifications.Remove(notification);
        await _blogRepository.SaveChangesAsync();

        return Redirect($"~/vox/{voxHash}#{commentHash}");
    }

    [Route("delete")]
    public async Task<IActionResult> DeleteAll()
    {
        var userId = User.GetUserId();
        if (userId == null) return BadRequest();

        var notifications = await _blogRepository.Notifications.GetByUserId(userId.Value);

        await _blogRepository.Notifications.RemoveRange(notifications);
        await _blogRepository.SaveChangesAsync();

        var returnUrl = Request.Headers["Referer"].ToString();

        return Redirect(returnUrl);
    }
}