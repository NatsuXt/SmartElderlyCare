using ElderlyCare.Data;
using ElderlyCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCare.Services
{
    public class SystemAnnouncementService : ISystemAnnouncementService
    {
        private readonly ElderlyCareDbContext _context;

        public SystemAnnouncementService(ElderlyCareDbContext context)
        {
            _context = context;
        }

        public async Task CreateAnnouncementAsync(SystemAnnouncement announcement)
        {
            announcement.AnnouncementDate = DateTime.Now;
            announcement.Status = "Active";
            _context.SystemAnnouncements.Add(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnnouncementAsync(SystemAnnouncement announcement)
        {
            _context.SystemAnnouncements.Update(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnnouncementAsync(int announcementId)
        {
            var announcement = await _context.SystemAnnouncements.FindAsync(announcementId);
            if (announcement != null)
            {
                _context.SystemAnnouncements.Remove(announcement);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SystemAnnouncement> GetAnnouncementByIdAsync(int announcementId)
        {
            return await _context.SystemAnnouncements
                .Include(sa => sa.CreatedBy)
                .FirstOrDefaultAsync(sa => sa.AnnouncementId == announcementId);
        }

        public async Task<List<SystemAnnouncement>> GetAnnouncementsByTypeAsync(string type)
        {
            return await _context.SystemAnnouncements
                .Where(sa => sa.AnnouncementType == type)
                .Include(sa => sa.CreatedBy)
                .ToListAsync();
        }

        public async Task<List<SystemAnnouncement>> GetActiveAnnouncementsAsync()
        {
            return await _context.SystemAnnouncements
                .Where(sa => sa.Status == "Active")
                .Include(sa => sa.CreatedBy)
                .ToListAsync();
        }

        public async Task<List<SystemAnnouncement>> GetAnnouncementsByCreatorAsync(int staffId)
        {
            return await _context.SystemAnnouncements
                .Where(sa => sa.CreatedById == staffId)
                .Include(sa => sa.CreatedBy)
                .ToListAsync();
        }
    }
}