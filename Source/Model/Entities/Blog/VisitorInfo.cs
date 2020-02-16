using System;

namespace Model.Entities.Blog
{
    public class VisitorInfo : BaseEntity
    {
        public DateTime LastVisit { get; set; }
        public string UserAgent { get; set; }
    }
}