﻿namespace Core.Services.Moderation.Models
{
    public class ReportRequest
    {
        public int ContentType { get; set; }
        public string ContentId { get; set; }
        public string Reason { get; set; }
    }
}
