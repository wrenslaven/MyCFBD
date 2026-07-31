using System.Text.Json;
using System.Net.Http.Json;

using var client = new HttpClient();
client.BaseAddress = new Uri("https://localhost:7069/");
Services servicesObj = new(client);
try
{
    HttpResponseMessage response = await client.GetAsync("teams");
    response.EnsureSuccessStatusCode();
    string jsonResponse = await response.Content.ReadAsStringAsync();
    var teams = JsonSerializer.Deserialize<List<TeamData>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    bool running = true;
    while (running)
    {
        running = await RunRequests(teams); // RunRequests() returns true for every request except Stop, so the user can continue entering requests until they'd like to stop.
    }
}
catch (HttpRequestException e)
{
    Console.WriteLine($"Error: {e.Message}");
}

async Task<bool> RunRequests(List<TeamData> teams){
    Console.WriteLine("What would you like to do? Type PrintAll, PrintByID, PrintByConference, PrintByState, DeleteByID, AddTeam, UpdateByID, or Stop to stop.");
    string request = Console.ReadLine()!.ToUpper();
    switch (request)
    {
        case "PRINTALL":
            servicesObj.PrintAll(teams);
            return true;
        case "PRINTBYID":
            Console.WriteLine("Enter the team's ID: ");
            var id = Console.ReadLine()!;
            servicesObj.PrintByID(teams, id);
            return true;
        case "PRINTBYCONFERENCE":
            Console.WriteLine("Enter the conference name: ");
            var conference = Console.ReadLine()!;
            servicesObj.PrintByConference(teams, conference);
            return true;
        case "PRINTBYSTATE":
            Console.WriteLine("Enter the state name: ");
            var state = Console.ReadLine()!;
            servicesObj.PrintByState(teams, state);
            return true;
        case "DELETEBYID":
            Console.WriteLine("Enter the team's ID: ");
            var delId = Console.ReadLine()!;
            await servicesObj.DeleteByID(teams, delId);
            return true;
        case "ADDTEAM":
            var newID = teams.Count() + 1;
            servicesObj.AddTeam(teams, newID);
            return true;
        case "UPDATEBYID":
            Console.WriteLine("Enter the team's ID: ");
            var updateID = Console.ReadLine()!;
            servicesObj.UpdateTeam(teams, updateID);
            return true;
        case "STOP":
            return false;
        default:
            Console.WriteLine("Please enter a valid request.");
            await RunRequests(teams);
            return true;
    }
}

public class Services(HttpClient client)
{
     public bool PrintAll(List<TeamData> teams)
    {
        foreach (TeamData team in teams)
        {
            Console.WriteLine(
                $"Data for {team.TeamName} ({team.TeamId}):\nLocation: {team.City}, {team.State}\nConference: {team.Conference}"
            );
        }
        if (teams.Count == 0)
        {
            Console.WriteLine("No teams found.");
            return false;
        }
        return true;
    }

    public bool PrintByID(List<TeamData> teams, string id)
    {
        if(int.TryParse(id, out int IdInt))
        {
            var found = teams.FindAll(team => team.TeamId == IdInt);
            foreach (TeamData foundTeam in found)
            {
                Console.WriteLine(
                    $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}"
                    );
            }
            if (found.Count == 0)
            {
            Console.WriteLine("No teams found with that ID.");
            return false;
            }
            
        }
        else
        {
            Console.WriteLine("Please enter a valid number.");
            return false;
        }
        return true;
    }

    public bool PrintByConference(List<TeamData> teams, string conference)
    {
        var found = teams.FindAll(team => team.Conference == conference);
        foreach (TeamData foundTeam in found)
        {
            Console.WriteLine(
                    $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}"
                    );
        }
        if (found.Count == 0)
        {
            Console.WriteLine("No teams found in that conference.");
            return false;
        }
        return true;
    }

    public bool PrintByState(List<TeamData> teams, string state)
    {
        var found = teams.FindAll(team => team.State == state);
        foreach (TeamData foundTeam in found)
        {
            Console.WriteLine(
                    $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}"
                    );
        }
        if (found.Count == 0)
        {
            Console.WriteLine("No teams found in that state.");
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteByID(List<TeamData> teams, string id) // uses Task<bool> instead of just bool as the return type because it's an async method
    {
        
        if(int.TryParse(id, out int IdInt))
        {
            var found = teams.Find(team => team.TeamId == IdInt);
            if (found == null)
            {
                Console.WriteLine("No team with that ID was found.");
                return false;
            }
            else
            {
                teams.Remove(found);
                HttpResponseMessage postResponse = await client.DeleteAsync($"teams/{IdInt}");
                postResponse.EnsureSuccessStatusCode();
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid number.");
            return false;
        }
        return true;
    }

    public async void AddTeam(List<TeamData> teams, int id)
    {

        Console.WriteLine("Enter the team name: ");
        string teamName = Console.ReadLine()!;
        Console.WriteLine("Enter the city: ");
        string cityName = Console.ReadLine()!;
        Console.WriteLine("Enter the state: ");
        string stateName = Console.ReadLine()!;
        Console.WriteLine("Enter the conference: ");
        string conferenceName = Console.ReadLine()!;
        TeamData newTeam = new TeamData()
        {
            TeamId = id,
            TeamName = teamName,
            City = cityName,
            State = stateName,
            Conference = conferenceName
        };
        teams.Add(newTeam); // Add it to local memory

        HttpResponseMessage postResponse = await client.PostAsJsonAsync("teams", newTeam); // Add it to external memory
        postResponse.EnsureSuccessStatusCode();

    }

    public async void UpdateTeam(List<TeamData> teams, string id)
    {
        if(int.TryParse(id, out int IdInt))
        {
            Console.WriteLine("What would you like to update? Enter Name, City, State, or Conference");
            bool validField = false;
            while (!validField)
            {
                string request = Console.ReadLine()!;
                switch (request){
                    case "Name":
                        var foundForName = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team name: ");
                        var newName = Console.ReadLine()!;
                        foundForName[0].TeamName = newName;
                        HttpResponseMessage namePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForName[0]);
                        namePutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "City":
                        var foundForCity = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team city: ");
                        var newCity = Console.ReadLine()!;
                        foundForCity[0].City = newCity;

                        HttpResponseMessage cityPutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForCity[0]);
                        cityPutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "State":
                        var foundForState = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team state: ");
                        var newState = Console.ReadLine()!;
                        foundForState[0].State = newState;

                        HttpResponseMessage statePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForState[0]);
                        statePutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "Conference":
                        var foundForConference = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team conference: ");
                        var newConference = Console.ReadLine()!;
                        foundForConference[0].Conference = newConference;

                        HttpResponseMessage conferencePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForConference[0]);
                        conferencePutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid field.");
                        validField = false;
                        break;
                }
            }
        }
        else
        {
            Console.WriteLine("Please enter a valid number.");
        }
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

public partial class DataEntryProgram { };