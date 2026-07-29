using Microsoft.AspNetCore.Mvc;
using MyCFBD.API.Models;

namespace MyCFBD.API.Controllers;

[ApiController]
[Route("[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamList _teamList;
    public TeamsController(ITeamList teamList)
    {
        _teamList = teamList;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Team>> GetTeams()
    {
        return Ok(_teamList.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Team> GetTeam(int id)
    {
        try
        {
            var team = _teamList.Get(id);
            return Ok(team);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public ActionResult<Team> CreateTeam(Team team)
    {
        var createdTeam = _teamList.Add(team);
        return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.TeamId }, createdTeam);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTeam(int id, Team team)
    {
        if (id != team.TeamId)
        {
            return BadRequest("ID must match TeamId in request.");
        }

        var updated = _teamList.Update(team);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTeam(int id)
    {
        _teamList.Remove(id);
        return NoContent();
    }
}