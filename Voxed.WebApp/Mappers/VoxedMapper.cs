using Core.Constants;
using Core.Entities;
using Core.Extensions;
using Core.Services.MediaServices;
using Core.Services.Mixers.Models;
using Core.Shared;
using System.Collections.Generic;
using System.Linq;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Hubs;
using Voxed.WebApp.Models;
using Voxed.WebApp.ViewModels;

namespace Voxed.WebApp.Mappers;

public interface IMapper
{
    VoxResponse Map(MixItem vox);
    VoxResponse Map(Post vox);
    VoxDetailViewModel Map(Post vox, UserPostAction actions, IEnumerable<IBoardPostViewModel> posts = null);
    IEnumerable<VoxResponse> Map(IEnumerable<Post> voxs);
    UserNotification Map(Notification notification);
}

public class VoxedMapper : IMapper
{
    private readonly string MediaEndpoint;

    public VoxedMapper(IMediaService mediaService)
    {
        MediaEndpoint = mediaService.Location;
    }
    public VoxResponse Map(MixItem vox)
    {
        return new VoxResponse()
        {
            Hash = vox.Hash,
            Status = true,
            Niche = vox.Niche,
            Title = vox.Title,
            Comments = vox.Comments,
            Extension = string.Empty,
            Sticky = vox.Sticky,
            //CreatedAt = vox.CreatedOn.ToString(),
            PollOne = string.Empty,
            PollTwo = string.Empty,
            Id = vox.Id?.ToString(),
            Slug = vox.Slug,
            VoxId = vox.Id?.ToString(),
            //New = vox.CreatedOn.IsNew(),
            ThumbnailUrl = vox.ThumbnailUrl,
            Category = vox.Category,
            Href = vox.Href,
        };
    }

    public VoxResponse Map(Post post)
    {
        return new VoxResponse()
        {
            Hash = post.Id.ToShortString(),
            Status = true,
            Niche = post.CategoryId.ToString(),
            Title = post.Title,
            Comments = post.Comments.Count.ToString(),
            Extension = string.Empty,
            Sticky = post.IsSticky ? "1" : "0",
            CreatedAt = post.CreatedOn.ToString(),
            PollOne = string.Empty,
            PollTwo = string.Empty,
            Id = post.Id.ToString(),
            Slug = post.Category.ShortName.ToUpper(),
            VoxId = post.Id.ToString(),
            New = post.CreatedOn.IsNew(),
            ThumbnailUrl = $"{MediaEndpoint}/{post.Media?.Key}" + ImageParameter.Quality40,
            Category = post.Category.Name,
            Href = "/vox/" + post.Id.ToShortString(),
        };
    }

    public VoxDetailViewModel Map(Post vox, UserPostAction actions, IEnumerable<IBoardPostViewModel> posts = null)
    {
        return new VoxDetailViewModel()
        {
            Id = vox.Id,
            Title = vox.Title,
            Content = vox.Content,
            Hash = vox.Id.ToShortString(),
            UserId = vox.UserId,

            CommentTag = UserTypeDictionary.GetDescription(vox.Owner.UserType).ToLower(),
            CategoryName = vox.Category.Name,
            CategoryShortName = vox.Category.ShortName,
            CategoryThumbnailUrl = vox.Category.Media.Url,
            CommentsAttachmentCount = vox.Comments.Where(x => x.Media != null).Count(),
            CommentsCount = vox.Comments.Count,
            UserName = vox.Owner.UserName,
            UserType = (ViewModels.UserType)(int)vox.Owner.UserType,
            CreatedOn = vox.CreatedOn.DateTime.ToTimeAgo(),

            Media = new MediaViewModel()
            {
                ThumbnailUrl = $"{MediaEndpoint}/{vox.Media.Key}" + ImageParameter.Quality40,
                Url = $"{MediaEndpoint}/{vox.Media.Key}",
                MediaType = (ViewModels.MediaType)(int)vox.Media.Type,
                ExtensionData = vox.Media?.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value],
                ExternalUrl = vox.Media?.ExternalUrl
            },

            IsFavorite = actions.IsFavorite,
            IsFollowed = actions.IsFollowed,
            IsHidden = actions.IsHidden,

            Comments = vox.Comments.OrderByDescending(c => c.IsSticky).ThenByDescending(c => c.CreatedOn).Select(c => new CommentViewModel()
            {
                ID = c.Id,
                Content = c.Content,
                Hash = c.Hash,
                IsOp = c.UserId == vox.UserId,
                AvatarColor = c.Style.ToString().ToLower(),
                AvatarText = UserTypeDictionary.GetDescription(c.Owner.UserType).ToUpper(),
                Media = c.Media == null ? null : new MediaViewModel()
                {
                    //Url = c.Media?.Url,
                    Url = $"{MediaEndpoint}/{c.Media.Key}" + ImageParameter.Quality40,
                    MediaType = (ViewModels.MediaType)(int)c.Media.Type,
                    ExtensionData = c.Media.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value],
                    ThumbnailUrl = $"{MediaEndpoint}/{c.Media.Key}" + ImageParameter.Quality40,
                    ExternalUrl = c.Media.ExternalUrl,
                },
                IsSticky = c.IsSticky,
                CreatedOn = c.CreatedOn,
                Style = c.Style.ToString().ToLower(),
                Author = c.Owner.UserType == Core.Entities.UserType.Administrator ? c.Owner?.UserName : "Anonimo",
                Tag = UserTypeDictionary.GetDescription(c.Owner.UserType).ToLower(),
                IsAdmin = c.Owner.UserType == Core.Entities.UserType.Administrator
            }),
            Posts = posts
        };
    }

    public IEnumerable<VoxResponse> Map(IEnumerable<Post> voxs) => voxs.Select(Map);

    public UserNotification Map(Notification notification)
    {
        return new UserNotification
        {
            Type = notification.Type.ToString().ToLowerInvariant(),
            Content = new NotificationContent()
            {
                VoxHash = notification.Post.Id.ToShortString(),
                NotificationBold = GetTitleNotification(notification.Type),
                NotificationText = notification.Post.Title,
                Count = "1",
                ContentHash = notification.Comment.Hash,
                Id = notification.Id.ToString(),
                //ThumbnailUrl = notification.Post.Media?.Url + Core.Constants.ImageParameter.FormatWebP
                ThumbnailUrl = $"{MediaEndpoint}/{notification.Post.Media?.Key}" + ImageParameter.FormatWebP,
            }
        };
    }

    private string GetTitleNotification(NotificationType notificationType)
    {
        return notificationType switch
        {
            NotificationType.New => "Nuevo comentario",
            NotificationType.Reply => "Nueva respuesta",
            _ => "Nueva notificacion",
        };
    }
}
