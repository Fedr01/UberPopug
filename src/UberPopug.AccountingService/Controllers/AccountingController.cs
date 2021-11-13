using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UberPopug.AccountingService.Models;
using UberPopug.AccountingService.Users;

namespace UberPopug.AccountingService.Controllers
{
    [Authorize]
    public class AccountingController : Controller
    {
        private readonly ILogger<AccountingController> _logger;
        private readonly AccountingDbContext _dbContext;

        public AccountingController(ILogger<AccountingController> logger, AccountingDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View(new User
            {
                Email = "test"
            });
        }

        public IActionResult Statistics()
        {
            var users = _dbContext.Users.ToList();
            return View(users);
        }
        
        public IActionResult Analytics()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}