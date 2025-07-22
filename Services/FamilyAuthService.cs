using ElderlyCareSystem.Data;
using ElderlyCareSystem.Dtos;
using ElderlyCareSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ElderlyCareSystem.Services
{
    public class FamilyAuthService
    {
        private readonly AppDbContext _context;

        public FamilyAuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateFamilyLoginAsync(FamilyLoginRequestDto request)
        {
            var family = await _context.FamilyInfos
                .Include(f => f.Elderly)
                .FirstOrDefaultAsync(f =>
                    f.Name == request.FamilyName &&
                    f.ContactPhone == request.ContactPhone &&
                    f.Elderly.Name == request.ElderlyName &&
                    f.Elderly.IdCardNumber == request.ElderlyIdCardNumber);

            return family != null;
        }
    }
}