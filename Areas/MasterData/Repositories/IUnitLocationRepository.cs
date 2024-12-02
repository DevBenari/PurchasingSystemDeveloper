using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IUnitLocationRepository
    {
        private readonly ApplicationDbContext _context;

        public IUnitLocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public UnitLocation Tambah(UnitLocation UnitLocation)
        {
            _context.UnitLocations.Add(UnitLocation);
            _context.SaveChanges();
            return UnitLocation;
        }

        public async Task<UnitLocation> GetUnitLocationById(Guid Id)
        {
            var UnitLocation = await _context.UnitLocations
                .Include(d => d.UnitManager)
                .SingleOrDefaultAsync(i => i.UnitLocationId == Id);

            if (UnitLocation != null)
            {
                var UnitLocationDetail = new UnitLocation()
                {
                    UnitLocationId = UnitLocation.UnitLocationId,
                    UnitLocationCode = UnitLocation.UnitLocationCode,
                    UnitLocationName = UnitLocation.UnitLocationName,
                    UnitManagerId = UnitLocation.UnitManagerId,
                    Address = UnitLocation.Address
                };
                return UnitLocationDetail;
            }
            return null;
        }

        public async Task<UnitLocation> GetUnitLocationByIdNoTracking(Guid Id)
        {
            return await _context.UnitLocations.AsNoTracking().FirstOrDefaultAsync(a => a.UnitLocationId == Id);
        }

        public async Task<List<UnitLocation>> GetUnitLocations()
        {
            return await _context.UnitLocations.OrderBy(p => p.CreateDateTime).Select(UnitLocation => new UnitLocation()
            {
                UnitLocationId = UnitLocation.UnitLocationId,
                UnitLocationCode = UnitLocation.UnitLocationCode,
                UnitLocationName = UnitLocation.UnitLocationName,
                UnitManagerId= UnitLocation.UnitManagerId,
                Address = UnitLocation.Address
            }).ToListAsync();
        }

        public IEnumerable<UnitLocation> GetAllUnitLocation()
        {
            return _context.UnitLocations.OrderByDescending(d => d.CreateDateTime)
                .Include(d => d.UnitManager)
                .AsNoTracking();
        }

        public async Task<(IEnumerable<UnitLocation> unitLocations, int totalCountUnitLocations)> GetAllUnitLocationPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.UnitLocations
                .OrderByDescending(d => d.CreateDateTime)
                .Include(d => d.UnitManager)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.UnitLocationCode.Contains(searchTerm) || p.UnitLocationName.Contains(searchTerm) || p.UnitManager.FullName.Contains(searchTerm));
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
            var unitLocations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (unitLocations, totalCount);
        }

        public UnitLocation Update(UnitLocation update)
        {
            var UnitLocation = _context.UnitLocations.Attach(update);
            UnitLocation.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public UnitLocation Delete(Guid Id)
        {
            var UnitLocation = _context.UnitLocations.Find(Id);
            if (UnitLocation != null)
            {
                _context.UnitLocations.Remove(UnitLocation);
                _context.SaveChanges();
            }
            return UnitLocation;
        }
    }
}
