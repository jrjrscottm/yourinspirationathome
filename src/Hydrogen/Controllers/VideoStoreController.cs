
using Hydrogen.Services.VideoStore;
using Hydrogen.ViewModels.VideoStore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Hydrogen.Controllers
{
    [Authorize(Policy="MyVideoStore")]
    public class VideoStoreController : Controller
    {
        readonly IVideoStoreService _videoStoreService;
        readonly UserManager<IdentityUser> _userManager;

        public VideoStoreController(
            UserManager<IdentityUser> userManager,
            IVideoStoreService videoStoreService)
        {
            _videoStoreService = videoStoreService;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var videos = _videoStoreService.GetUserVideos(_userManager.GetUserId(User));

            var model = new VideoStoreViewModel
            {
                Videos = videos.Select(v => new VideoViewModel
                {
                    ImagePath = v.ImagePath,
                    EmbedCode = v.EmbedCode,
                    Url = v.Url,
                    Name = v.Title
                }).ToList()
            };

            return View(model);
        }
    }
}
