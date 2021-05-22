﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Data.Repositories;
using Core.Entities;
using Core.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Voxed.WebApp.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    //[Route("v{version:apiVersion}/[controller]")]
    //[ApiVersion("1.0")]    
    public class VoxController : ControllerBase
    {
        private readonly IVoxedRepository _voxedRepository;

        public VoxController(IVoxedRepository voxedRepository)
        {
            _voxedRepository = voxedRepository;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiVoxResponse>> Get(Guid id)
        {
            var vox = await _voxedRepository.Voxs.GetById(id);

            if (vox == null)
            {
                return NotFound();
            }

            if (vox.State == VoxState.Deleted)
            {
                return NotFound();
            }

            return Ok(ConvertToVoxResponse(vox));
        }

        [HttpGet]
        public async Task<IActionResult> GetLastest()
        {
            var voxs = await _voxedRepository.Voxs.GetLastestAsync();

            var voxsList = voxs.Select(vox => new Models.VoxResponse()
            {
                //Hash = GuidConverter.ToShortString(vox.ID),
                Hash = vox.ID.ToString("N"),
                //Hash = x.Hash,
                Status = "1",
                Niche = "20",
                Title = vox.Title,
                Comments = vox.Comments.Count().ToString(),
                Extension = "",
                Sticky = vox.IsSticky ? "1" : "0",
                CreatedAt = vox.CreatedOn.ToString(),
                PollOne = "",
                PollTwo = "",
                Id = "20",
                Slug = vox.Category.ShortName.ToUpper(),
                //VoxId = GuidConverter.ToShortString(vox.ID),
                VoxId = vox.ID.ToString("N"),
                New = vox.CreatedOn.Date == DateTime.Now.Date,
                ThumbnailUrl = vox.Media?.ThumbnailUrl
            }).ToList();

            return Ok(voxsList);
        }

        private ApiVoxResponse ConvertToVoxResponse(Vox vox)
        {
            return new ApiVoxResponse()
            {
                Id = vox.ID.ToString("N"),
                Title = vox.Title,
                Content = vox.Content,
                CategoryName = vox.Category.Name,
                CategoryShortName = vox.Category.ShortName,
                CategoryThumbnailUrl = vox.Category.Media.ThumbnailUrl,
                CreatedOn = TimeAgo.ConvertToTimeAgo(vox.CreatedOn.DateTime),
                UserName = UserViewHelper.GetUserName(vox.User),
                UserTag = UserViewHelper.GetUserTypeTag(vox.User.UserType),
                CommentsCount = vox.Comments.Count().ToString(),
                Comments = vox.Comments.Select(comment => new CommentNotification()
                {
                    UniqueId = null, //si es unique id puede tener colores unicos
                    UniqueColor = null,
                    UniqueColorContrast = null,

                    Id = comment.ID.ToString(),
                    Hash = comment.Hash,
                    VoxHash = vox.Hash,
                    AvatarColor = comment.Style.ToString().ToLower(),
                    IsOp = vox.UserID == comment.UserID && vox.User.UserType != UserType.Anonymous, //probar cambiarlo cuando solo pruedan craer los usuarios.
                    Tag = UserViewHelper.GetUserTypeTag(comment.User.UserType), //admin o dev               
                    Content = comment.Content ?? string.Empty,
                    Name = UserViewHelper.GetUserName(comment.User),
                    CreatedAt = TimeAgo.ConvertToTimeAgo(comment.CreatedOn.DateTime),
                    Poll = null, //aca va una opcion respondida

                    //Media
                    MediaUrl = comment.Media?.Url,
                    MediaThumbnailUrl = comment.Media?.ThumbnailUrl,
                    //Extension = request.GetUploadData()?.Extension == UploadDataExtension.Base64 ? GetFileExtensionFromUrl(comment.Media?.Url) : request.GetUploadData()?.Extension,
                    //ExtensionData = request.GetUploadData()?.ExtensionData,
                    //Via = request.GetUploadData()?.Extension == UploadDataExtension.Youtube ? comment.Media?.Url : null,
                }).ToList()
            };
        }
    }

    public class ApiVoxResponse
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CategoryName { get; set; }
        public string CategoryShortName { get; set; }
        public string CategoryThumbnailUrl { get; set; }
        public string UserName { get; set; }
        public string UserTag { get; set; }
        public string CreatedOn { get; set; }
        public string PollOne { get; set; }
        public string PollTwo { get; set; }
        public string CommentsCount { get; set; }
        public IEnumerable<CommentNotification> Comments { get; set; }
    }

    public class ApiCommentResponse
    {
        public string Hash { get; set; }
        public string Content { get; set; }
        public string CreatedOn { get; set; }

    }

    public class CommentNotification
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public string UniqueId { get; set; }
        public string VoxHash { get; set; }
        public string AvatarColor { get; set; }
        public bool IsOp { get; set; }
        public string Tag { get; set; }
        public string UniqueColor { get; set; }
        public string UniqueColorContrast { get; set; }
        public string Name { get; set; }
        public string CreatedAt { get; set; }
        public string Poll { get; set; }
        public string Extension { get; set; }
        public string ExtensionData { get; set; }
        public string Via { get; set; } // es una url ??
        public string Content { get; set; }
        public string MediaUrl { get; set; }
        public string MediaThumbnailUrl { get; set; }
    }

    public class ApiMediaResponse
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }

    }
}