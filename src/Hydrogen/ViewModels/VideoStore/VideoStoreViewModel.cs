using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.ViewModels.VideoStore
{
    public class VideoStoreViewModel
    {
        public List<VideoViewModel> Videos { get; set; }
    }

    public class VideoViewModel
    {
        public string Name { get; set; }
        public string EmbedCode { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
    }

}
