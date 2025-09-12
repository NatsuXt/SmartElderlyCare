using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class ElderlyInfoController : ControllerBase
{
    private readonly AppDbContext _context;

    public ElderlyInfoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ElderlyInfo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ElderlyInfo>>> GetElderlyInfos()
    {
        return await _context.ElderlyInfos.ToListAsync();
    }

    // GET: ElderlyInfo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ElderlyInfo>> GetElderlyInfo(int id)
    {
        var elderlyInfo = await _context.ElderlyInfos.FindAsync(id);

        if (elderlyInfo == null)
        {
            return NotFound();
        }

        return elderlyInfo;
    }

    // PUT: ElderlyInfo/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutElderlyInfo(int id, ElderlyInfo elderlyInfo)
    {
        if (id != elderlyInfo.elderly_id)
        {
            return BadRequest();
        }

        _context.Entry(elderlyInfo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ElderlyInfoExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: ElderlyInfo
    [HttpPost]
    public async Task<ActionResult<ElderlyInfo>> PostElderlyInfo(ElderlyInfo elderlyInfo)
    {
        _context.ElderlyInfos.Add(elderlyInfo);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetElderlyInfo", new { id = elderlyInfo.elderly_id }, elderlyInfo);
    }

    // DELETE: ElderlyInfo/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteElderlyInfo(int id)
    {
        var elderlyInfo = await _context.ElderlyInfos.FindAsync(id);
        if (elderlyInfo == null)
        {
            return NotFound();
        }

        _context.ElderlyInfos.Remove(elderlyInfo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ElderlyInfoExists(int id)
    {
        return _context.ElderlyInfos.Any(e => e.elderly_id == id);
    }
}