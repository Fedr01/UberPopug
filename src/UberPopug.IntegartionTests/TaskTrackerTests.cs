using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using UberPopug.TaskTrackerService;
using Xunit;
using Startup = UberPopug.TaskTrackerService.Startup;

namespace UberPopug.IntegartionTests
{
    public class TaskTrackerTests : IClassFixture<CustomWebApplicationFactory<Startup, TaskTrackerDbContext>>
    {
        private readonly CustomWebApplicationFactory<Startup, TaskTrackerDbContext> _webApp;

        public TaskTrackerTests(CustomWebApplicationFactory<Startup, TaskTrackerDbContext> webApp)
        {
            _webApp = webApp;
        }

        [Fact]
        public async Task Should_Produce_And_Consume_New_Tasks()
        {
            var users = await _webApp.TrackerDbContext.Users.ToListAsync();
            if (!users.Any())
            {
                throw new Exception("Users should be produced first! Run Auth tests");
            }

            await _webApp.TrackerDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tasks");
            await _webApp.AccountingDbContext.Database.ExecuteSqlRawAsync("DELETE FROM Tasks");

            for (int i = 0; i < 10; i++)
            {
                var form = new MultipartFormDataContent();
                form.Add(new StringContent($"title{i}"), "Title");
                form.Add(new StringContent($"[UberPopug]-{i}"), "JiraId");
                await _webApp.Client.PostAsync("/Tasks/Create", form);
            }

            var trackerTasks = await _webApp.TrackerDbContext.Tasks.ToListAsync();
            var accTasks = await _webApp.AccountingDbContext.Tasks.ToListAsync();

            await Task.Delay(2000);
            trackerTasks.Count.ShouldBe(10);
            accTasks.Count.ShouldBe(trackerTasks.Count);
            accTasks[0].AssignPrice.ShouldBeGreaterThan(0);
            accTasks[0].CompletePrice.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Should_Reassign_Tasks()
        {
            var users = await _webApp.TrackerDbContext.Users.ToListAsync();
            if (!users.Any())
            {
                throw new Exception("Users should be produced first! Run Auth tests");
            }

            var accountingUsers = await _webApp.AccountingDbContext.Users.ToListAsync();
            var totalBalance = accountingUsers.Sum(a => a.Balance);

            var response = await _webApp.Client.GetAsync("/Tasks/AssignTasks");

            foreach (var user in accountingUsers)
            {
                await _webApp.AccountingDbContext.Entry(user).ReloadAsync();
            }

            var newBalance = accountingUsers.Sum(a => a.Balance);

            newBalance.ShouldBeLessThan(totalBalance);
        }

        [Fact]
        public async Task Should_Complete_Task()
        {
            var users = await _webApp.TrackerDbContext.Users.ToListAsync();
            if (!users.Any())
            {
                throw new Exception("Users should be produced first! Run Auth tests");
            }


            var accTask = await _webApp.AccountingDbContext.Tasks
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.AssignedToEmail != null);

            var oldBalance = accTask.User.Balance;
            var task = await _webApp.TrackerDbContext.Tasks.FirstOrDefaultAsync(t => t.PublicId == accTask.PublicId);

            var response = await _webApp.Client.GetAsync($"/Tasks/Complete?taskId={task.Id}");

            await _webApp.AccountingDbContext.Entry(accTask.User).ReloadAsync();

            accTask.User.Balance.ShouldNotBe(oldBalance);
        }
    }
}