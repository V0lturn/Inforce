using Inforce_Task.Models.Entities;
using Inforce_Task.Models.Interfaces.IDbContextInterface;
using Inforce_Task.Models.Interfaces.IUrlServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inforce_Task.Controllers
{
    public class UrlController : Controller
    {
        private readonly IUrlService _urlService;
        private readonly IAppDbContext _appDbContext;
        public UrlController(IUrlService urlService, IAppDbContext appDbContext)
        {
            _urlService = urlService;
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var allUrl = await _urlService.GetAllData();

            if(!allUrl.Any()) 
            {
                ViewData["Error"] = "Something went wrong or no data was found";
            }


            return View(allUrl);
        }

        [HttpPost]
        public async Task<IActionResult> ShortUrl(string link)
        {
            link = link.Trim();

            var existingUrl = await _appDbContext.Urls
                .Where(u => u.UrlText == link || u.ShortenUrl == link)
                .FirstOrDefaultAsync();

            if (existingUrl != null)
            {
                return Json(new
                {
                    success = false,
                    message = "This URL already exists.",
                    data = await _urlService.GetAllData()
                });
            }

            var userName = User.Identity.Name;
            if (userName == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User is not authenticated.",
                    data = await _urlService.GetAllData()
                });
            }

            var currentUser = await _appDbContext.Users
                .Where(u => u.Login == userName)
                .FirstOrDefaultAsync();

            if (currentUser != null)
            {
                await _urlService.AddUrl(link, currentUser.UserId);

                return Json(new
                {
                    success = true,
                    message = "URL was successfully added.",
                    data = await _urlService.GetAllData()
                });
            }

            return Json(new
            {
                success = false,
                message = "User not found.",
                data = await _urlService.GetAllData()
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUrl(string linkToDelete)
        {
            linkToDelete = linkToDelete.Trim();

            var existingUrl = await _appDbContext.Urls
                .Where(u => u.UrlText == linkToDelete || u.ShortenUrl == linkToDelete)
                .FirstOrDefaultAsync();

            if (existingUrl == null)
            {
                return Json(new
                {
                    success = false,
                    message = "This link was not found.",
                    data = await _urlService.GetAllData()
                });
            }

            var userName = User.Identity.Name;
            if (userName == null)
            {
                return Json(new
                {
                    success = false,
                    message = "User is not authenticated.",
                    data = await _urlService.GetAllData()
                });
            }

            var currentUser = await _appDbContext.Users
                .Where(u => u.Login == userName)
                .FirstOrDefaultAsync();

            if (currentUser != null)
            {
                if (currentUser.Role == "Admin")
                {
                    _appDbContext.Urls.Remove(existingUrl);
                    await _appDbContext.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = "URL was successfully deleted.",
                        data = await _urlService.GetAllData()
                    });
                }
                else
                {
                    if (existingUrl.CreatedBy == currentUser.UserId)
                    {
                        _appDbContext.Urls.Remove(existingUrl);
                        await _appDbContext.SaveChanges();

                        return Json(new
                        {
                            success = true,
                            message = "URL was successfully deleted.",
                            data = await _urlService.GetAllData()
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            success = false,
                            message = "You cannot delete a URL created by someone else.",
                            data = await _urlService.GetAllData()
                        });
                    }
                }
            }

            return Json(new
            {
                success = false,
                message = "User not found.",
                data = await _urlService.GetAllData()
            });
        }

        public IActionResult RedirectToUrl(string shortUrl)
        {
            var urlRecord = _appDbContext.Urls.FirstOrDefault(u => u.ShortenUrl == shortUrl);

            if (urlRecord == null)
            {
                return NotFound();
            }

            return Redirect(urlRecord.UrlText);
        }

        [Authorize]
        public IActionResult ShortUrlInfo(string link)
        {
            var url = _appDbContext.Urls.FirstOrDefault(u => u.UrlText == link);
            if (url == null)
            {
                return NotFound();
            }

            return View(url);
        }

    }
}
