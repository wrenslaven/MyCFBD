using System.Text.Json;
namespace MyCFBD.API.Models;

public interface ITeamList
{
    IEnumerable<Team> GetAll();
    Team Get(int teamId);
    Team Add(Team team);
    void Remove(int teamId);
    bool Update(Team team);
}

public class TeamList : ITeamList
{
    private List<Team> _teams = new List<Team>();
    private int _nextId = 1;

    public TeamList()
    {
        string jsonFile = File.ReadAllText("./Resources/allteamdata.json");
        var jsonData = JsonSerializer.Deserialize<List<Team>>(jsonFile, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true });
        if (jsonData != null)
        {
            _teams = jsonData;
            if (_teams.Any())
            {
                _nextId = _teams.Max(t => t.TeamId) + 1;
            }
        }
    }

    public IEnumerable<Team> GetAll()
    {
        return _teams;
    }
    public Team Get(int id)
    {
        var team = _teams.Find(p => p.TeamId == id) ?? throw new KeyNotFoundException($"Team {id} not found.");
        return team;
    }
    public Team Add(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);
        team.TeamId = _nextId++;
        _teams.Add(team);
        return team;
    }
    public void Remove(int teamId)
    {
        _teams.RemoveAll(p => p.TeamId == teamId);
    }
    public bool Update(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);
        int index = _teams.FindIndex(p => p.TeamId == team.TeamId);
        if (index == -1)
        {
            return false;
        }
        _teams.RemoveAt(index);
        _teams.Add(team);
        return true;
    }
}

public class Team
{
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Conference { get; set; } = string.Empty;
    public CoachData CoachData { get; set; } = new CoachData();
    public ReturningPlayers ReturningPlayers { get; set; } = new ReturningPlayers();
    public List<FootballGame> Games { get; set; } = new List<FootballGame>();
}

public class CoachData
{
    public string CoachName { get; set; } = string.Empty;
    public int CoachYearAtSchool { get; set; }
    public int CoachTotalYears { get; set; }
    public int CoachWins { get; set; }
    public int CoachLosses { get; set; }
}

public class ReturningPlayers
{
    public int ReturningOffensiveStarters { get; set; }
    public int ReturningDefensiveStarters { get; set; }
    public bool ReturningQuarterback { get; set; }
}

public class FootballGame
{
    public DateTime GameDate { get; set; }
    public string Opponent { get; set; } = string.Empty;
    public int OpponentId { get; set; }
    public string HomeAwayNeutral { get; set; } = string.Empty;
    public float Spread { get; set; }
    public float Total { get; set; }
    public int Score { get; set; }
    public int OpponentScore { get; set; }
    public bool Win { get; set; }
    public TeamGameStats GameStats { get; set; } = new TeamGameStats();
    public TeamGameStats OpponentGameStats { get; set; } = new TeamGameStats();
    public BettingData BettingData { get; set; } = new BettingData();
    public bool Overtime { get; set; }

}

public class TeamGameStats
{
    public int PassingYards { get; set; }
    public int QBR { get; set; }
    public int RushingYards { get; set; }
}

public class BettingData
{
    public bool WonATS { get; set; }
    public bool Over { get; set; }
}