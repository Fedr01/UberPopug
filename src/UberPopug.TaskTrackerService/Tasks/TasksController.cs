using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult AssignTasks()
        {
            var tasks = _context.Tasks.ToList();
            var users = _context.Users.ToList();

            var random = new Random();
            foreach (var task in tasks)
            {
                var assignedTo = users[random.Next(0, users.Count)];
                task.UserEmail = assignedTo.Email;
            }

            _context.SaveChanges();
            return View("Index", tasks);
        }

        public IActionResult Logout()
        {
            return Redirect("https://localhost:5000/Account/Logout");
        }
    }
}