using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Models.Dtos;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeeSettlementController : ControllerBase
{
    private readonly AppDbContext _context;

    public FeeSettlementController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/FeeSettlement
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeeSettlementDto>>> GetFeeSettlements()
    {
        var settlements = await _context.FeeSettlements
            .Include(fs => fs.FeeDetails)
            .ToListAsync();

        var dtos = settlements.Select(s => new FeeSettlementDto
        {
            id = s.settlement_id,
            elderly_id = s.elderly_id,
            billing_cycle_start = s.billing_cycle_start,
            billing_cycle_end = s.billing_cycle_end,
            total_amount = s.total_amount,
            insurance_amount = s.insurance_amount,
            personal_payment = s.personal_payment,
            payment_status = s.payment_status,
            created_at = s.created_at,
            fee_details = s.FeeDetails?.Select(fd => new FeeDetailDto
            {
                fee_type = fd.fee_type,
                description = fd.description,
                amount = fd.amount,
                start_date = fd.start_date,
                end_date = fd.end_date,
                quantity = fd.quantity,
                unit_price = fd.unit_price
            }).ToList()
        }).ToList();

        return dtos;
    }

    // GET: api/FeeSettlement/5
    [HttpGet("{id}")]
    public async Task<ActionResult<FeeSettlementDto>> GetFeeSettlement(int id)
    {
        var feeSettlement = await _context.FeeSettlements
            .Include(fs => fs.FeeDetails)
            .FirstOrDefaultAsync(fs => fs.settlement_id == id);

        if (feeSettlement == null)
        {
            return NotFound();
        }

        var dto = new FeeSettlementDto
        {
            id = feeSettlement.settlement_id,
            elderly_id = feeSettlement.elderly_id,
            billing_cycle_start = feeSettlement.billing_cycle_start,
            billing_cycle_end = feeSettlement.billing_cycle_end,
            total_amount = feeSettlement.total_amount,
            insurance_amount = feeSettlement.insurance_amount,
            personal_payment = feeSettlement.personal_payment,
            payment_status = feeSettlement.payment_status,
            created_at = feeSettlement.created_at,
            fee_details = feeSettlement.FeeDetails?.Select(fd => new FeeDetailDto
            {
                fee_type = fd.fee_type,
                description = fd.description,
                amount = fd.amount,
                start_date = fd.start_date,
                end_date = fd.end_date,
                quantity = fd.quantity,
                unit_price = fd.unit_price
            }).ToList()
        };

        return dto;
    }

    // PUT: FeeSettlement/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutFeeSettlement(int id, FeeSettlement feeSettlement)
    {
        if (id != feeSettlement.settlement_id)
        {
            return BadRequest();
        }

        _context.Entry(feeSettlement).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FeeSettlementExists(id))
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

    // POST: FeeSettlement
    [HttpPost]
    public async Task<ActionResult<FeeSettlement>> PostFeeSettlement(FeeSettlement feeSettlement)
    {
        _context.FeeSettlements.Add(feeSettlement);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetFeeSettlement", new { id = feeSettlement.settlement_id }, feeSettlement);
    }

    // DELETE: FeeSettlement/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFeeSettlement(int id)
    {
        var feeSettlement = await _context.FeeSettlements.FindAsync(id);
        if (feeSettlement == null)
        {
            return NotFound();
        }

        _context.FeeSettlements.Remove(feeSettlement);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FeeSettlementExists(int id)
    {
        return _context.FeeSettlements.Any(e => e.settlement_id == id);
    }
}