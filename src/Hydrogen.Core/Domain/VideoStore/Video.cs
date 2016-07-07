using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Core.Domain.VideoStore
{
    public class Video
    {
        public string UserId { get; set; }
        public string VideoId { get; set; }
        public string EmbedCode { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
    }
}
