//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0&pivots=xunit was helpful for this
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualBasic;

public class TestTeamData
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Conference { get; set; } = string.Empty;
}



public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    TestTeamData testTeam = new()
    {
        TeamId = 3,
        TeamName = "Louisville",
        City = "Louisville",
        State = "Kentucky"
    };

    TestTeamData updatedTestTeam = new()
    {
        TeamId = 0,
        TeamName = "Michigan",
        City = "Ann Arbor",
        State = "Michigan"
    };
    
    private readonly WebApplicationFactory<Program> _factory;
    public IntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("https://localhost:7069/teams")]
    [InlineData("https://localhost:7069/teams/1")]
    public async Task Get_EndpointsReturnSuccess(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        if (response.Content.Headers.ContentType != null)
        {
            Assert.Equal("application/json; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
        }
    }

    [Theory]
    [InlineData("https://localhost:7069/teams")]
    public async Task Post_EndpointsReturnSuccess(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync(url, testTeam);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        if (response.Content.Headers.ContentType != null)
        {
            Assert.Equal("application/json; charset=utf-8", 
            response.Content.Headers.ContentType.ToString());
        }
    }

    [Theory]
    [InlineData("https://localhost:7069/teams/0")]
    public async Task Put_EndpointsReturnSuccess(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync(url, updatedTestTeam);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        // PUT doesn't return anything so there's nothing to check equivalency of.
    }

    [Theory]
    [InlineData("https://localhost:7069/teams/0")]
    public async Task Delete_EndpointsReturnSuccess(string url)
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync(url);
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        // DELETE doesn't return anything so there's nothing to check equivalency of.
    }
}