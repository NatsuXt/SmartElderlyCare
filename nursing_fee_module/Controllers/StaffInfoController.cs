using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class StaffInfoController : ControllerBase
{
    private readonly AppDbContext _context;

    public StaffInfoController(AppDbContext context)
    {
        _context = context;
    }

    // GET: StaffInfo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StaffInfo>>> GetStaffInfos()
    {
        return await _context.StaffInfos.ToListAsync();
    }

    // GET: StaffInfo/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StaffInfo>> GetStaffInfo(int id)
    {
        var staffInfo = await _context.StaffInfos.FindAsync(id);

        if (staffInfo == null)
        {
            return NotFound();
        }

        return staffInfo;
    }

    // PUT: StaffInfo/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStaffInfo(int id, StaffInfo staffInfo)
    {
        if (id != staffInfo.staff_id)
        {
            return BadRequest();
        }

        _context.Entry(staffInfo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StaffInfoExists(id))
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

    // POST: StaffInfo
    [HttpPost]
    public async Task<ActionResult<StaffInfo>> PostStaffInfo(StaffInfo staffInfo)
    {
        _context.StaffInfos.Add(staffInfo);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStaffInfo", new { id = staffInfo.staff_id }, staffInfo);
    }

    // DELETE: StaffInfo/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaffInfo(int id)
    {
        var staffInfo = await _context.StaffInfos.FindAsync(id);
        if (staffInfo == null)
        {
            return NotFound();
        }

        _context.StaffInfos.Remove(staffInfo);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StaffInfoExists(int id)
    {
        return _context.StaffInfos.Any(e => e.staff_id == id);
    }
}