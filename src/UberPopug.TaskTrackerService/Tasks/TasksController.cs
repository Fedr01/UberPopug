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
        private readonly ITasksManager _tasksManager;

        public TasksController(ITasksManager tasksManager)
        {
            _tasksManager = tasksManager;
        }

        public async Task<IActionResult> Index()
        {
            var tasks = await _tasksManager.GetAllAsync();
            return View(tasks);
        }

        public IActionResult NewTask()
        {
            return View("Create");
        }

        public async Task<IActionResult> Create(CreateTaskCommand command)
        {
            await _tasksManager.CreateAsync(command);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Complete(int taskId)
        {
            await _tasksManager.CompleteAsync(taskId);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTasks()
        {
            var tasks = await _tasksManager.AssignAllAsync();
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