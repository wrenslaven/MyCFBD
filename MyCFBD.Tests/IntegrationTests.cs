using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

public class UnitTests
{
    List<TeamData> testTeams = new List<TeamData>
    {
        new TeamData {
        TeamId = 0,
        TeamName = "Michigan",
        City = "Ann Arbor",
        State = "Michigan",
        Conference = "Big Ten"
        },
        new TeamData
        {
            TeamId = 1,
            TeamName = "Louisville",
            City = "Louisville",
            State = "Kentucky",
            Conference = "ACC"
        }
    };

    List<TeamData> emptyTestTeams = new List<TeamData>();

    DataEntryProgram dataEntryProgram = new();
    HttpClient testClient = new HttpClient();
    Services services = new(new HttpClient());
    

    [Fact]
    public void PrintAll_ValidList_ReturnTrue()
    {
        bool result = services.PrintAll(testTeams);
        Assert.True(result, "PrintAll returns true if there were teams to print");
    }

    [Fact]
    public void PrintAll_EmptyList_ReturnFalse()
    {
        bool result = services.PrintAll(emptyTestTeams);
        Assert.False(result, "PrintAll returns false if there were no teams to print");
    }

    [Fact]
    public void PrintByID_ValidID_ReturnTrue()
    {
        bool result = services.PrintByID(testTeams, "0");
        Assert.True(result, "PrintByID returns true if there was a team with that ID");
    }

    [Fact]
    public void PrintByID_InvalidID_ReturnFalse()
    {
        bool result = services.PrintByID(testTeams, "7");
        Assert.False(result, "PrintByID returns false if there was no team with that ID");
    }

    [Fact]
    public void PrintByConference_ValidConference_ReturnTrue()
    {
        bool result = services.PrintByConference(testTeams, "ACC");
        Assert.True(result, "PrintByConference returns true if there was a team in that conference");
    }

    [Fact]
    public void PrintByConference_InvalidConference_ReturnFalse()
    {
        bool result = services.PrintByConference(testTeams, "SEC");
        Assert.False(result, "PrintByConference returns false if there was no team in that conference");
    }

    [Fact]
    public async Task DeleteTeam_InvalidID_ReturnFalse()
    {
        Task<bool> taskResult = services.DeleteByID(testTeams, "7");
        bool result = await taskResult;
        Assert.False(result, "DeleteTeam resturn false if there's no team with that ID");
    }

    [Fact]
    public async Task DeleteTeam_InvalidNumber_ReturnTrue()
    {
        Task<bool> taskResult = services.DeleteByID(testTeams, "ABC");
        bool result = await taskResult;
        Assert.False(result, "DeleteTeam returns false if the user inputs anything other than a number.");
    }

    //TODO: Not sure how to test a success case for Delete/Add/Update, since they require access to the API.
    // I used integration testing for those things below, but that's not unit testing as I understand it.

}

// I used code from https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0&pivots=xunit for this section:
// public class IntegrationTests : IClassFixture<WebApplicationFactory<ApiProgram>>
// {
//     TeamData testTeam = new()
//     {
//         TeamId = 3,
//         TeamName = "Louisville",
//         City = "Louisville",
//         State = "Kentucky"
//     };

//     TeamData updatedTestTeam = new()
//     {
//         TeamId = 0,
//         TeamName = "Michigan",
//         City = "Ann Arbor",
//         State = "Michigan"
//     };
    
//     private readonly WebApplicationFactory<ApiProgram> _factory;
//     public IntegrationTests(WebApplicationFactory<ApiProgram> factory)
//     {
//         _factory = factory;
//     }

//     [Theory]
//     [InlineData("https://localhost:7069/teams")]
//     [InlineData("https://localhost:7069/teams/1")]
//     public async Task Get_EndpointsReturnSuccess(string url)
//     {
//         var client = _factory.CreateClient();
//         var response = await client.GetAsync(url);
//         response.EnsureSuccessStatusCode(); // Status Code 200-299
//         if (response.Content.Headers.ContentType != null)
//         {
//             Assert.Equal("application/json; charset=utf-8", 
//             response.Content.Headers.ContentType.ToString());
//         }
//     }

//     [Theory]
//     [InlineData("https://localhost:7069/teams")]
//     public async Task Post_EndpointsReturnSuccess(string url)
//     {
//         var client = _factory.CreateClient();
//         var response = await client.PostAsJsonAsync(url, testTeam);
//         response.EnsureSuccessStatusCode(); // Status Code 200-299
//         if (response.Content.Headers.ContentType != null)
//         {
//             Assert.Equal("application/json; charset=utf-8", 
//             response.Content.Headers.ContentType.ToString());
//         }
//     }

//     [Theory]
//     [InlineData("https://localhost:7069/teams/0")]
//     public async Task Put_EndpointsReturnSuccess(string url)
//     {
//         var client = _factory.CreateClient();
//         var response = await client.PutAsJsonAsync(url, updatedTestTeam);
//         response.EnsureSuccessStatusCode(); // Status Code 200-299
//         // PUT doesn't return anything so there's nothing to check equivalency of.
//     }

//     [Theory]
//     [InlineData("https://localhost:7069/teams/0")]
//     public async Task Delete_EndpointsReturnSuccess(string url)
//     {
//         var client = _factory.CreateClient();
//         var response = await client.DeleteAsync(url);
//         response.EnsureSuccessStatusCode(); // Status Code 200-299
//         // DELETE doesn't return anything so there's nothing to check equivalency of.
//     }
// }