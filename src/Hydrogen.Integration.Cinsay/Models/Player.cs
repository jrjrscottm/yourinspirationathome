using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Integration.Cinsay.Models
{
    public class CinsayPlayer
    {
        public string Guid { get; set; }
        public string ClientGuid { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Locale { get; set; }
        public string EmbedCode { get; set; }
        public string AssetStatus { get; set; }
        public List<CinsayVideo> Videos { get; set; }

    }

    public class CinsayVideo
    {
        public string Guid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PosterImage { get; set; }
        public string ThumbImage { get; set; }
        public string HiDefinition { get; set; }
        public string StandardDefinition { get; set; }
        public string Original { get; set; }
        public string Mobile { get; set; }
        public string AssetStatus { get; set; }
    }
}
