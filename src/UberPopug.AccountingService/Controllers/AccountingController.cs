using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberPopug.AccountingService.Accounting;
using UberPopug.AccountingService.Models;
using UberPopug.Common;

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

        public async Task<IActionResult> Index()
        {
            var user = await _dbContext
                .Users
                .Include(u => u.Transactions)
                .ThenInclude(u => u.Task)
                .FirstOrDefaultAsync(u => u.Email == User.GetEmail());

            return View(user);
        }

        public async Task<IActionResult> Statistics()
        {
            var transactions = await _dbContext.Transactions
                .Where(t => t.DateTime.Date == DateTime.UtcNow.Date)
                .ToListAsync();

            var completedTasksFee = transactions.Where(t => t.Type == TransactionType.Debit).Sum(t => t.Amount);
            var assignedTasksFee = transactions.Where(t => t.Type == TransactionType.Credit).Sum(t => t.Amount);

            var stats = new StatisticsViewModel
            {
                EarnedMoney = assignedTasksFee - completedTasksFee
            };
            return View(stats);
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