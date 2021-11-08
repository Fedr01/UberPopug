using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UberPopug.TaskTrackerService.Tasks
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskTrackerDbContext _context;

        public TasksController(TaskTrackerDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tasks = _context.Tasks.ToList();
            return View(tasks);
        }

        public IActionResult NewTask()
        {
            return View("Create");
        }

        public IActionResult Create(Task task)
        {
            _context.Tasks.Add(task);
            
            var users = _context.Users.ToList();
            var random = new Random();
            var assignedTo = users[random.Next(0, users.Count)];
            task.AssignTo(assignedTo);
            
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AssignTasks()
        {
            var tasks = _context.Tasks.ToList();
            var users = _context.Users.ToList();

            var random = new Random();
            foreach (var task in tasks)
            {
                var assignedTo = users[random.Next(0, users.Count)];
                task.AssignTo(assignedTo);
            }

            _context.SaveChanges();
            return View("Index", tasks);
        }

        public async System.Threading.Tasks.Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        // [Route("/")]
        // public IActionResult AccessDenied()
        // {
        //     return Redirect("https://localhost:5000/Account/Logout");
        // }
    }
}