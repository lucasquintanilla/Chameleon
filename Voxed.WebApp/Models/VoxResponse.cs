﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voxed.WebApp.Models
{
    public class VoxResponse
    {
        public string Hash { get; set; }
        public string Status { get; set; }
        public string Niche { get; set; }
        public string Title { get; set; }
        public string Comments { get; set; }
        public string Extension { get; set; }
        public string Sticky { get; set; }
        public string CreatedAt { get; set; }
        public string PollOne { get; set; }
        public string PollTwo { get; set; }
        public string Id { get; set; }
        public string Slug { get; set; }
        public string VoxId { get; set; }
        public bool New { get; set; }


        //Agregado extras
        public string ThumbnailUrl { get; set; }
        public string Category { get; set; }
    }
}