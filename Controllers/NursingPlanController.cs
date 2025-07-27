using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class NursingPlanController : ControllerBase
{
    private readonly AppDbContext _context;

    public NursingPlanController(AppDbContext context)
    {
        _context = context;
    }

    // GET: NursingPlan
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NursingPlan>>> GetNursingPlans()
    {
        return await _context.NursingPlans
            .Include(n => n.ElderlyInfo)
            .Include(n => n.StaffInfo)
            .Include(n => n.FeeSettlements)
            .ToListAsync();
    }

    // GET: NursingPlan/5
    [HttpGet("{id}")]
    public async Task<ActionResult<NursingPlan>> GetNursingPlan(int id)
    {
        var nursingPlan = await _context.NursingPlans
            .Include(n => n.ElderlyInfo)
            .Include(n => n.StaffInfo)
            .Include(n => n.FeeSettlements)
            .FirstOrDefaultAsync(n => n.plan_id == id);

        if (nursingPlan == null)
        {
            return NotFound();
        }

        return nursingPlan;
    }

    // PUT: NursingPlan/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutNursingPlan(int id, NursingPlan nursingPlan)
    {
        if (id != nursingPlan.plan_id)
        {
            return BadRequest();
        }

        _context.Entry(nursingPlan).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!NursingPlanExists(id))
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

    // POST: NursingPlan
    [HttpPost]
    public async Task<ActionResult<NursingPlan>> PostNursingPlan(NursingPlan nursingPlan)
    {
        _context.NursingPlans.Add(nursingPlan);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetNursingPlan", new { id = nursingPlan.plan_id }, nursingPlan);
    }

    // DELETE: NursingPlan/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNursingPlan(int id)
    {
        var nursingPlan = await _context.NursingPlans.FindAsync(id);
        if (nursingPlan == null)
        {
            return NotFound();
        }

        _context.NursingPlans.Remove(nursingPlan);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool NursingPlanExists(int id)
    {
        return _context.NursingPlans.Any(e => e.plan_id == id);
    }
}