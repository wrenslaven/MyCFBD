using System.Text.Json;
using System.Net.Http.Json;

using var client = new HttpClient();

client.BaseAddress = new Uri("https://localhost:7069/");
try
{
    HttpResponseMessage response = await client.GetAsync("teams");
    response.EnsureSuccessStatusCode();
    string jsonResponse = await response.Content.ReadAsStringAsync();
    var teams = JsonSerializer.Deserialize<List<TeamData>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    RunRequests(teams);
}
catch (HttpRequestException e)
{
    Console.WriteLine($"Error: {e.Message}");
}

void RunRequests(List<TeamData> teams){
    Console.WriteLine("What would you like to do? Type PrintAll, PrintByID, PrintByConference, PrintByState, DeleteByID, AddTeam, UpdateByID");
    string request = Console.ReadLine();
    switch (request)
    {
        case "PrintAll":
            PrintAll(teams);
            break;
        case "PrintByID":
            Console.WriteLine("Enter the team's ID: ");
            var id = Console.ReadLine();
            PrintByID(teams, id);
            break;
        case "PrintByConference":
            Console.WriteLine("Enter the conference name: ");
            var conference = Console.ReadLine();
            PrintByConference(teams, conference);
            break;
        case "PrintByState":
            Console.WriteLine("Enter the state name: ");
            var state = Console.ReadLine();
            PrintByState(teams, state);
            break;
        case "DeleteByID":
            Console.WriteLine("Enter the team's ID: ");
            var delId = Console.ReadLine();
            DeleteByID(teams, delId);
            break;
        case "AddTeam":
            var newID = teams.Count() + 1;
            AddTeam(teams, newID);
            break;
        case "UpdateByID":
            Console.WriteLine("Enter the team's ID: ");
            var updateID = Console.ReadLine();
            UpdateTeam(teams, updateID);
            break;
        default:
            Console.WriteLine("Please enter a valid request.");
            break;
    }
}

void PrintAll(List<TeamData> teams)
{
    foreach (TeamData team in teams)
    {
        Console.WriteLine(
            $"Data for {team.TeamName} ({team.TeamId}):\nLocation: {team.City}, {team.State}\nConference: {team.Conference}"
        );
    }
}

void PrintByID(List<TeamData> teams, string id)
{
    if(int.TryParse(id, out int IdInt))
    {
        var found = teams.FindAll(team => team.TeamId == IdInt);
    }
    else
    {
        Console.WriteLine("Please enter a valid number.");
    }
}

void PrintByConference(List<TeamData> teams, string conference)
{
    var found = teams.FindAll(team => team.Conference == conference);
    foreach (TeamData foundTeam in found)
    {
        Console.WriteLine(
                $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}"
                );
    }
}

void PrintByState(List<TeamData> teams, string state)
{
    var found = teams.FindAll(team => team.State == state);
    foreach (TeamData foundTeam in found)
    {
        Console.WriteLine(
                $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}"
                );
    }
}

async void DeleteByID(List<TeamData> teams, string id)
{
    
    if(int.TryParse(id, out int IdInt))
    {
        var found = teams.Find(team => team.TeamId == IdInt);
        teams.Remove(found);
        HttpResponseMessage postResponse = await client.DeleteAsync($"teams/{IdInt}");
        postResponse.EnsureSuccessStatusCode();
    }
    else
    {
        Console.WriteLine("Please enter a valid number.");
    }

    
}

async void AddTeam(List<TeamData> teams, int id)
{

    Console.WriteLine("Enter the team name: ");
    string teamName = Console.ReadLine();
    Console.WriteLine("Enter the city: ");
    string cityName = Console.ReadLine();
    Console.WriteLine("Enter the state: ");
    string stateName = Console.ReadLine();
    Console.WriteLine("Enter the conference: ");
    string conferenceName = Console.ReadLine();
    TeamData newTeam = new TeamData()
    {
        TeamId = id,
        TeamName = teamName,
        City = cityName,
        State = stateName,
        Conference = conferenceName
    };
    teams.Add(newTeam);

    HttpResponseMessage postResponse = await client.PostAsJsonAsync("teams", newTeam);
    postResponse.EnsureSuccessStatusCode();

}

async void UpdateTeam(List<TeamData> teams, string id)
{
    if(int.TryParse(id, out int IdInt))
    {
        Console.WriteLine("What would you like to update? Enter Name, City, State, or Conference");
        string request = Console.ReadLine();
        switch (request){
            case "Name":
                var foundForName = teams.FindAll(team => team.TeamId == IdInt);
                Console.WriteLine("Enter the new team name: ");
                var newName = Console.ReadLine();
                foundForName[0].TeamName = newName;
                HttpResponseMessage namePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForName[0]);
                namePutResponse.EnsureSuccessStatusCode();
                break;
            case "City":
                var foundForCity = teams.FindAll(team => team.TeamId == IdInt);
                Console.WriteLine("Enter the new team city: ");
                var newCity = Console.ReadLine();
                foundForCity[0].City = newCity;

                HttpResponseMessage cityPutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForCity[0]);
                cityPutResponse.EnsureSuccessStatusCode();
                
                break;
            case "State":
                var foundForState = teams.FindAll(team => team.TeamId == IdInt);
                Console.WriteLine("Enter the new team state: ");
                var newState = Console.ReadLine();
                foundForState[0].State = newState;

                HttpResponseMessage statePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForState[0]);
                statePutResponse.EnsureSuccessStatusCode();

                break;
            case "Conference":
                var foundForConference = teams.FindAll(team => team.TeamId == IdInt);
                Console.WriteLine("Enter the new team conference: ");
                var newConference = Console.ReadLine();
                foundForConference[0].Conference = newConference;

                HttpResponseMessage conferencePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForConference[0]);
                conferencePutResponse.EnsureSuccessStatusCode();

                break;
            default:
                Console.WriteLine("Please enter a valid field.");
                break;
        }

    }
    else
    {
        Console.WriteLine("Please enter a valid number.");
    }
}

public class TeamData
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Conference { get; set; } = string.Empty;
}