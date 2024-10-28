using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IWarehouseLocationRepository
    {
        private readonly ApplicationDbContext _context;

        public IWarehouseLocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public WarehouseLocation Tambah(WarehouseLocation WarehouseLocation)
        {
            _context.WarehouseLocations.Add(WarehouseLocation);
            _context.SaveChanges();
            return WarehouseLocation;
        }

        public async Task<WarehouseLocation> GetWarehouseLocationById(Guid Id)
        {
            var WarehouseLocation = await _context.WarehouseLocations
                .Include(u => u.WarehouseManager)
                .SingleOrDefaultAsync(i => i.WarehouseLocationId == Id);

            if (WarehouseLocation != null)
            {
                var WarehouseLocationDetail = new WarehouseLocation()
                {
                    WarehouseLocationId = WarehouseLocation.WarehouseLocationId,
                    WarehouseLocationCode = WarehouseLocation.WarehouseLocationCode,
                    WarehouseLocationName = WarehouseLocation.WarehouseLocationName,
                    WarehouseManagerId = WarehouseLocation.WarehouseManagerId,
                    Address = WarehouseLocation.Address
                };
                return WarehouseLocationDetail;
            }
            return null;
        }

        public async Task<WarehouseLocation> GetWarehouseLocationByIdNoTracking(Guid Id)
        {
            return await _context.WarehouseLocations.AsNoTracking().FirstOrDefaultAsync(a => a.WarehouseLocationId == Id);
        }

        public async Task<List<WarehouseLocation>> GetWarehouseLocations()
        {
            return await _context.WarehouseLocations.OrderBy(p => p.CreateDateTime).Select(WarehouseLocation => new WarehouseLocation()
            {
                WarehouseLocationId = WarehouseLocation.WarehouseLocationId,
                WarehouseLocationCode = WarehouseLocation.WarehouseLocationCode,
                WarehouseLocationName = WarehouseLocation.WarehouseLocationName,
                WarehouseManagerId = WarehouseLocation.WarehouseManagerId,
                Address = WarehouseLocation.Address
            }).ToListAsync();
        }

        public IEnumerable<WarehouseLocation> GetAllWarehouseLocation()
        {
            return _context.WarehouseLocations.OrderByDescending(d => d.CreateDateTime)
                .Include(u => u.WarehouseManager)
                .AsNoTracking();
        }

        public WarehouseLocation Update(WarehouseLocation update)
        {
            var WarehouseLocation = _context.WarehouseLocations.Attach(update);
            WarehouseLocation.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public WarehouseLocation Delete(Guid Id)
        {
            var WarehouseLocation = _context.WarehouseLocations.Find(Id);
            if (WarehouseLocation != null)
            {
                _context.WarehouseLocations.Remove(WarehouseLocation);
                _context.SaveChanges();
            }
            return WarehouseLocation;
        }
    }
}
