using Inforce_Task.Models.Entities;
using Inforce_Task.Models.Interfaces.IDbContextInterface;
using Inforce_Task.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Inforce_Task.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAppDbContext _appDbContext;

        public AccountController(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _appDbContext.Users.
                    FirstOrDefault(u => u.Login == model.Login);
                if (user == null || user.Password != model.Password)
                {
                    ViewData["ErrorMessage"] = "Incorrect login or password";
                }
                else
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

                    return RedirectToAction("Index", "Url");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _appDbContext.Users.AnyAsync(u => u.Login == model.Login))
                {
                    ViewData["ErrorMessage"] = "User with this login is already exists";
                    
                    return View();
                }
                else
                {
                    User user = new User(model.Login, model.Password);
                    _appDbContext.Users.Add(user);
                    await _appDbContext.SaveChanges();

                    ModelState.Clear();

                    TempData["SuccessMessage"] = "Account created";

                    return RedirectToAction("Login");
                }
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


    }
}
