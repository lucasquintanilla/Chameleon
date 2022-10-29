﻿using Core.Entities;
using Core.Extensions;
using Core.Shared;
using System.Collections.Generic;
using System.Linq;
using Voxed.WebApp.Extensions;
using Voxed.WebApp.Models;

namespace Voxed.WebApp.Mappers
{
    public static class VoxedMapper
    {
        public static VoxResponse Map(Vox vox)
        {
            return new VoxResponse()
            {
                Hash = GuidConverter.ToShortString(vox.Id),
                Status = true,
                Niche = "20",
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
                ThumbnailUrl = vox.Attachment?.ThumbnailUrl,
                Category = vox.Category.Name
            };
        }

        public static List<VoxResponse> Map(IEnumerable<Vox> voxs)
        {
            return voxs.Select(vox => Map(vox)).ToList();
        }
    }
}
