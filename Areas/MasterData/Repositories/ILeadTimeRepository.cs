using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class ILeadTimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ILeadTimeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public LeadTime Tambah(LeadTime LeadTime)
        {
            _context.LeadTimes.Add(LeadTime);
            _context.SaveChanges();
            return LeadTime;
        }

        public async Task<LeadTime> GetLeadTimeById(Guid Id)
        {
            var LeadTime = await _context.LeadTimes
                .SingleOrDefaultAsync(i => i.LeadTimeId == Id);

            if (LeadTime != null)
            {
                var LeadTimeDetail = new LeadTime()
                {
                    LeadTimeId = LeadTime.LeadTimeId,
                    LeadTimeCode = LeadTime.LeadTimeCode,
                    LeadTimeValue = LeadTime.LeadTimeValue
                };
                return LeadTimeDetail;
            }
            return null;
        }

        public async Task<LeadTime> GetLeadTimeByIdNoTracking(Guid Id)
        {
            return await _context.LeadTimes.AsNoTracking().FirstOrDefaultAsync(a => a.LeadTimeId == Id);
        }

        public async Task<List<LeadTime>> GetLeadTimes()
        {
            return await _context.LeadTimes.OrderBy(p => p.CreateDateTime).Select(LeadTime => new LeadTime()
            {
                LeadTimeId = LeadTime.LeadTimeId,
                LeadTimeCode = LeadTime.LeadTimeCode,
                LeadTimeValue = LeadTime.LeadTimeValue
            }).ToListAsync();
        }

        public IEnumerable<LeadTime> GetAllLeadTime()
        {
            return _context.LeadTimes.OrderByDescending(d => d.CreateDateTime)
                .AsNoTracking();
        }

        public async Task<(IEnumerable<LeadTime> leadTimes, int totalCountLeadTimes)> GetAllLeadTimePageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.LeadTimes
                .OrderByDescending(d => d.CreateDateTime)                
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.LeadTimeCode.Contains(searchTerm) || Convert.ToString(p.LeadTimeValue).Contains(searchTerm));
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.CreateDateTime >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.CreateDateTime <= endDate.Value);
            }

            var totalCount = await query.CountAsync();

            // Ambil data paginated
            var leadTimes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (leadTimes, totalCount);
        }

        public LeadTime Update(LeadTime update)
        {
            var LeadTime = _context.LeadTimes.Attach(update);
            LeadTime.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public LeadTime Delete(Guid Id)
        {
            var LeadTime = _context.LeadTimes.Find(Id);
            if (LeadTime != null)
            {
                _context.LeadTimes.Remove(LeadTime);
                _context.SaveChanges();
            }
            return LeadTime;
        }
    }
}
