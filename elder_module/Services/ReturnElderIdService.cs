using ElderlyCareSystem.Data;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareSystem.Services
{
    public class FamilyService
    {
        private readonly AppDbContext _context;

        public FamilyService(AppDbContext context)
        {
            _context = context;
        }

        // 根据 FamilyId 查询对应的 ElderlyId
        public async Task<int?> GetElderlyIdByFamilyIdAsync(int familyId)
        {
            var family = await _context.FamilyInfos
                                       .AsNoTracking()
                                       .FirstOrDefaultAsync(f => f.FamilyId == familyId);
            return family?.ElderlyId;  // 如果找不到返回 null
        }
    }
}
