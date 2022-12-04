using Core.Entities;
using Core.Services.Mixers.Models;
using Core.Shared;
using System.Collections.Generic;
using System.Linq;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Models;
using Voxed.WebApp.ViewModels;

namespace Voxed.WebApp.Mappers;

public static class VoxedMapper
{
    public static VoxResponse Map(MixItem vox)
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
            Id = vox.Id.ToString(),
            Slug = vox.Slug,
            VoxId = vox.Id.ToString(),
            //New = vox.CreatedOn.IsNew(),
            ThumbnailUrl = vox.ThumbnailUrl,
            Category = vox.Category,
            Href = vox.Href,
        };
    }

    public static VoxResponse Map(Vox vox)
    {
        return new VoxResponse()
        {
            Hash = vox.Id.ToShortString(),
            Status = true,
            Niche = vox.CategoryId.ToString(),
            Title = vox.Title,
            Comments = vox.Comments.Count().ToString(),
            Extension = string.Empty,
            Sticky = vox.IsSticky ? "1" : "0",
            CreatedAt = vox.CreatedOn.ToString(),
            PollOne = string.Empty,
            PollTwo = string.Empty,
            Id = vox.Id.ToString(),
            Slug = vox.Category.ShortName.ToUpper(),
            VoxId = vox.Id.ToString(),
            New = vox.CreatedOn.IsNew(),
            ThumbnailUrl = vox.Media?.ThumbnailUrl,
            Category = vox.Category.Name,
            Href = "/vox/"+ vox.Id.ToShortString(),
        };
    }

    public static VoxDetailViewModel Map(Vox vox, UserVoxAction actions)
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
            CategoryThumbnailUrl = vox.Category.Attachment.ThumbnailUrl,
            CommentsAttachmentCount = vox.Comments.Where(x => x.Attachment != null).Count(),
            CommentsCount = vox.Comments.Count,
            UserName = vox.Owner.UserName,
            UserType = (ViewModels.UserType)(int)vox.Owner.UserType,
            CreatedOn = vox.CreatedOn.DateTime.ToTimeAgo(),

            Media = new MediaViewModel()
            {
                ThumbnailUrl = vox.Media.ThumbnailUrl,
                Url = vox.Media.Url,
                MediaType = (ViewModels.MediaType)(int)vox.Media.Type,
                ExtensionData = vox.Media?.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value]
            },

            IsFavorite = actions.IsFavorite,
            IsFollowed = actions.IsFollowed,
            IsHidden = actions.IsHidden,

            Comments = vox.Comments.OrderByDescending(c => c.IsSticky).ThenByDescending(c => c.CreatedOn).Select(c => new CommentViewModel()
            {
                ID = c.Id,
                Content = c.Content,
                Hash = c.Hash,
                AvatarColor = c.Style.ToString().ToLower(),
                AvatarText = UserTypeDictionary.GetDescription(c.Owner.UserType).ToUpper(),
                IsOp = c.UserId == vox.UserId,
                Media = c.Attachment == null ? null : new MediaViewModel()
                {
                    Url = c.Attachment?.Url,
                    MediaType = (ViewModels.MediaType)(int)c.Attachment?.Type,
                    ExtensionData = c.Attachment?.Url.Split('=')[(vox.Media?.Url.Split('=').Length - 1).Value],
                    ThumbnailUrl = c.Attachment?.ThumbnailUrl,
                },
                IsSticky = c.IsSticky,
                CreatedOn = c.CreatedOn,
                Style = c.Style.ToString().ToLower(),
                User = c.Owner,
            }),
        };
    }

    public static IEnumerable<VoxResponse> Map(IEnumerable<Vox> voxs) => voxs.Select(Map);
}
