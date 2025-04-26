using Microsoft.AspNetCore.Mvc.Testing;
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text;
using System.Text.Json;

namespace RiteSwipe.Tests.Performance
{
    public class LoadTests
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public LoadTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [Fact]
        public void Task_Creation_Load_Test()
        {
            var step = Step.Create("create_task", async context =>
            {
                var request = Http.CreateRequest("POST", "http://localhost:5000/api/tasks")
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(new StringContent(JsonSerializer.Serialize(new
                    {
                        Title = "Test Task",
                        Description = "Load Test Task",
                        Budget = 100,
                        Duration = 24
                    }), Encoding.UTF8, "application/json"));

                var response = await Http.Send(_client, request);

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario = ScenarioBuilder.CreateScenario("task_creation", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }

        [Fact]
        public void Task_Search_Load_Test()
        {
            var step = Step.Create("search_tasks", async context =>
            {
                var request = Http.CreateRequest("GET", "http://localhost:5000/api/tasks/search?query=test");
                var response = await Http.Send(_client, request);

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario = ScenarioBuilder.CreateScenario("task_search", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }

        [Fact]
        public void Task_Application_Load_Test()
        {
            var step = Step.Create("apply_task", async context =>
            {
                var request = Http.CreateRequest("POST", "http://localhost:5000/api/tasks/1/apply")
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(new StringContent(JsonSerializer.Serialize(new
                    {
                        UserId = "test-user",
                        Proposal = "Test proposal"
                    }), Encoding.UTF8, "application/json"));

                var response = await Http.Send(_client, request);

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            var scenario = ScenarioBuilder.CreateScenario("task_application", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 30, during: TimeSpan.FromSeconds(30))
                );

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }
    }
}
