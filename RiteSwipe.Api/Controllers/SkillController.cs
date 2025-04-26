using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiteSwipe.Application.DTOs;
using RiteSwipe.Application.Services;

namespace RiteSwipe.Api.Controllers;

[Authorize]
public class SkillController : BaseApiController
{
    private readonly ISkillService _skillService;

    public SkillController(ISkillService skillService)
    {
        _skillService = skillService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillDTO>>> GetSkills()
    {
        var skills = await _skillService.GetAllSkillsAsync();
        return Ok(skills);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SkillDTO>> GetSkill(int id)
    {
        var skill = await _skillService.GetSkillByIdAsync(id);
        if (skill == null)
        {
            return NotFound();
        }
        return Ok(skill);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SkillDTO>>> SearchSkills([FromQuery] string searchTerm)
    {
        var skills = await _skillService.SearchSkillsAsync(searchTerm);
        return Ok(skills);
    }

    [HttpPost]
    public async Task<ActionResult<SkillDTO>> CreateSkill([FromBody] string skillName)
    {
        var skill = await _skillService.CreateSkillAsync(skillName);
        return CreatedAtAction(nameof(GetSkill), new { id = skill.SkillId }, skill);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSkill(int id)
    {
        await _skillService.DeleteSkillAsync(id);
        return NoContent();
    }

    [HttpGet("{id}/tasks")]
    public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasksBySkill(int id)
    {
        var tasks = await _skillService.GetTasksBySkillAsync(id);
        return Ok(tasks);
    }

    [HttpGet("{id}/users")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersBySkill(int id)
    {
        var users = await _skillService.GetUsersBySkillAsync(id);
        return Ok(users);
    }

    [HttpGet("{id}/demand")]
    public async Task<ActionResult<int>> GetSkillDemand(int id)
    {
        var demand = await _skillService.GetSkillDemandAsync(id);
        return Ok(demand);
    }
}
