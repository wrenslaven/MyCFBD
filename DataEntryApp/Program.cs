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
    Console.WriteLine("\n\nWhat would you like to do?\n1. PrintAll\n2. PrintByID\n3. PrintByConference\n4. PrintByState\n5. DeleteByID\n6. AddTeam\n7. UpdateByID\n8. Stop");
    string request = Console.ReadLine()!.ToUpper();
    switch (request)
    {
        case "1":
            servicesObj.PrintAll(teams);
            return true;
        case "2":
            Console.WriteLine("Enter the team's ID: ");
            var id = Console.ReadLine()!;
            servicesObj.PrintByID(teams, id);
            return true;
        case "3":
            Console.WriteLine("Enter the conference name: ");
            var conference = Console.ReadLine()!;
            servicesObj.PrintByConference(teams, conference);
            return true;
        case "4":
            Console.WriteLine("Enter the state name: ");
            var state = Console.ReadLine()!;
            servicesObj.PrintByState(teams, state);
            return true;
        case "5":
            Console.WriteLine("Enter the team's ID: ");
            var delId = Console.ReadLine()!;
            await servicesObj.DeleteByID(teams, delId);
            return true;
        case "6":
            var newID = teams.Count() + 1;
            servicesObj.AddTeam(teams, newID);
            return true;
        case "7":
            Console.WriteLine("Enter the team's ID: ");
            var updateID = Console.ReadLine()!;
            servicesObj.UpdateTeam(teams, updateID);
            return true;
        case "8":
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
                $"Data for {team.TeamName} ({team.TeamId}):\nLocation: {team.City}, {team.State}\nConference: {team.Conference}\nCoach: {team.CoachData.CoachName} (Year {team.CoachData.CoachYearAtSchool}, {team.CoachData.CoachTotalYears} years total), {team.CoachData.CoachWins}-{team.CoachData.CoachLosses}"
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
                    $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}\nCoach: {foundTeam.CoachData.CoachName} (Year {foundTeam.CoachData.CoachYearAtSchool}, {foundTeam.CoachData.CoachTotalYears} years total), {foundTeam.CoachData.CoachWins}-{foundTeam.CoachData.CoachLosses}"
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
                    $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}\nCoach: {foundTeam.CoachData.CoachName} (Year {foundTeam.CoachData.CoachYearAtSchool}, {foundTeam.CoachData.CoachTotalYears} years total), {foundTeam.CoachData.CoachWins}-{foundTeam.CoachData.CoachLosses}"
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
                    $"Data for {foundTeam.TeamName} ({foundTeam.TeamId}):\nLocation: {foundTeam.City}, {foundTeam.State}\nConference: {foundTeam.Conference}\n"
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
        Console.WriteLine("Enter the coach's name: ");
        string coachName = Console.ReadLine()!;
        Console.WriteLine("Enter the coach's year at school: ");
        int coachYearAtSchool = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the coach's total years: ");
        int coachTotalYears = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the coach's wins: ");
        int coachWins = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the coach's losses: ");
        int coachLosses = int.Parse(Console.ReadLine()!);
        CoachData newCoach = new CoachData()
        {
            CoachName = coachName,
            CoachYearAtSchool = coachYearAtSchool,
            CoachTotalYears = coachTotalYears,
            CoachWins = coachWins,
            CoachLosses = coachLosses
        };

        TeamData newTeam = new TeamData()
        {
            TeamId = id,
            TeamName = teamName,
            City = cityName,
            State = stateName,
            Conference = conferenceName,
            CoachData = newCoach
        };
        teams.Add(newTeam); // Add it to local memory

        HttpResponseMessage postResponse = await client.PostAsJsonAsync("teams", newTeam); // Add it to external memory
        postResponse.EnsureSuccessStatusCode();

    }

    public async void UpdateTeam(List<TeamData> teams, string id)
    {
        if(int.TryParse(id, out int IdInt)) // TO DO Check if the number is for a valid team, not just that it's a valid number
        {
            var idExists = teams.FindAll(team => team.TeamId == IdInt);

            if (idExists.Count == 0)
            {
                Console.WriteLine("No team with that ID was found.");
                return;
            }

            Console.WriteLine("What would you like to update?\n1. Name\n2. City\n3. State\n4. Conference\n5. Coach Data");
            bool validField = false;
            while (!validField)
            {
                string request = Console.ReadLine()!.ToUpper();
                switch (request){
                    case "1":
                        var foundForName = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team name: ");
                        var newName = Console.ReadLine()!;
                        foundForName[0].TeamName = newName;
                        HttpResponseMessage namePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForName[0]);
                        namePutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "2":
                        var foundForCity = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team city: ");
                        var newCity = Console.ReadLine()!;
                        foundForCity[0].City = newCity;

                        HttpResponseMessage cityPutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForCity[0]);
                        cityPutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "3":
                        var foundForState = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team state: ");
                        var newState = Console.ReadLine()!;
                        foundForState[0].State = newState;

                        HttpResponseMessage statePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForState[0]);
                        statePutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "4":
                        var foundForConference = teams.FindAll(team => team.TeamId == IdInt);
                        Console.WriteLine("Enter the new team conference: ");
                        var newConference = Console.ReadLine()!;
                        foundForConference[0].Conference = newConference;

                        HttpResponseMessage conferencePutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForConference[0]);
                        conferencePutResponse.EnsureSuccessStatusCode();
                        validField = true;
                        break;
                    case "5":
                        bool validCoachData = false;
                        while (!validCoachData)
                        {
                            var foundForCoach = teams.FindAll(team => team.TeamId == IdInt);
                            Console.WriteLine("Enter the coach's name: ");
                            string coachName = Console.ReadLine()!;
                            Console.WriteLine("Enter the coach's year at school: ");
                            int coachYearAtSchool = int.TryParse(Console.ReadLine()!, out int parsedCoachYearAtSchool) ? parsedCoachYearAtSchool : -1;
                            Console.WriteLine("Enter the coach's total years: ");
                            int coachTotalYears = int.TryParse(Console.ReadLine()!, out int parsedCoachTotalYears) ? parsedCoachTotalYears : -1;
                            Console.WriteLine("Enter the coach's wins: ");
                            int coachWins = int.TryParse(Console.ReadLine()!, out int parsedCoachWins) ? parsedCoachWins : -1;
                            Console.WriteLine("Enter the coach's losses: ");
                            int coachLosses = int.TryParse(Console.ReadLine()!, out int parsedCoachLosses) ? parsedCoachLosses : -1;
                            
                            
                            if (coachYearAtSchool > -1 && coachTotalYears > -1 && coachWins > -1 && coachLosses > -1){
                                 CoachData updatedCoach = new CoachData()
                                {
                                    CoachName = coachName,
                                    CoachYearAtSchool = coachYearAtSchool,
                                    CoachTotalYears = coachTotalYears,
                                    CoachWins = coachWins,
                                    CoachLosses = coachLosses
                                };

                                foundForCoach[0].CoachData = updatedCoach;

                                HttpResponseMessage coachPutResponse = await client.PutAsJsonAsync($"teams/{id}", foundForCoach[0]);
                                coachPutResponse.EnsureSuccessStatusCode();
                                validCoachData = true;
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Please enter valid numbers for the coach's year at school, total years, wins, and losses.");
                            }
                        }
                        validField = true;
                        break;
                    default:
                        Console.WriteLine("Please enter a valid choice.");
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

    public void ClearLastLine()
    {
        Console.Clear();
    }
}



public class TeamData
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Conference { get; set; } = string.Empty;
    public CoachData CoachData { get; set; } = new CoachData();
}

public class CoachData
{
    public int CoachLosses { get; set; }
    public int CoachWins { get; set; }
    public string CoachName { get; set; } = string.Empty;
    public int CoachTotalYears { get; set; }
    public int CoachYearAtSchool { get; set; }
}


public partial class DataEntryProgram { };