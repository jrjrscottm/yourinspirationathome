using Hydrogen.Core.Domain.VideoStore;
using Hydrogen.Integration.Cinsay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen.Services.VideoStore
{
    public interface IVideoStoreService
    {
        string GetUserVideoStoreId(string userId);
        IEnumerable<Video> GetUserVideos(string userId);
    }

    public class VideoStoreService : IVideoStoreService
    {
        private ICinsayClient _cinsayClient;
        public VideoStoreService(ICinsayClient cinsayClient)
        {
            _cinsayClient = cinsayClient;
        }
        public IEnumerable<Video> GetUserVideos(string userId)
        {
            var cinsayClientGuid = GetUserVideoStoreId(userId);
            var cinsayPlayers = _cinsayClient.GetClientPlayers(cinsayClientGuid);

            return cinsayPlayers.Where(p => p.Videos.Any()).Select(x =>
            new Video
            {
                UserId = userId,
                VideoId = x.Guid,
                EmbedCode = x.EmbedCode,
                Title = x.Name,
                Description = x.Description,
                ImagePath = x.Videos.First().PosterImage,
                //TODO: Inject URL
                Url = $"http://player.bizdemo.cinsay.com/v4/{x.Guid}"
            });
        }

        public string GetUserVideoStoreId(string userId)
        {
            return "f9269f84-b194-4c7a-908f-19fb6a3c9485";
        }
    }
}
