using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Models;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class ISupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public ISupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Supplier Tambah(Supplier Supplier)
        {
            _context.Suppliers.Add(Supplier);
            _context.SaveChanges();
            return Supplier;
        }

        public async Task<Supplier> GetSupplierById(Guid Id)
        {
            var Supplier = await _context.Suppliers
                .Include(l => l.LeadTime)
                .SingleOrDefaultAsync(i => i.SupplierId == Id);

            if (Supplier != null)
            {
                var SupplierDetail = new Supplier()
                {
                    CreateBy = Supplier.CreateBy,
                    CreateDateTime = Supplier.CreateDateTime,
                    UpdateBy = Supplier.UpdateBy,
                    UpdateDateTime = Supplier.UpdateDateTime,
                    SupplierId = Supplier.SupplierId,
                    SupplierCode = Supplier.SupplierCode,
                    SupplierName = Supplier.SupplierName,
                    LeadTimeId = Supplier.LeadTimeId,
                    LeadTime = Supplier.LeadTime,
                    Address = Supplier.Address,
                    Handphone = Supplier.Handphone,
                    Email = Supplier.Email,
                    Note = Supplier.Note,
                    IsPKS = Supplier.IsPKS,
                    IsActive = Supplier.IsActive,
                };
                return SupplierDetail;
            }
            return null;
        }

        public async Task<Supplier> GetSupplierByIdNoTracking(Guid Id)
        {
            return await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(a => a.SupplierId == Id);
        }

        public async Task<List<Supplier>> GetSuppliers()
        {
            return await _context.Suppliers.Where(s => s.IsActive == true).OrderBy(p => p.CreateDateTime).Select(Supplier => new Supplier()
            {
                CreateBy = Supplier.CreateBy,
                CreateDateTime = Supplier.CreateDateTime,
                UpdateBy = Supplier.UpdateBy,
                UpdateDateTime = Supplier.UpdateDateTime,
                SupplierId = Supplier.SupplierId,
                SupplierCode = Supplier.SupplierCode,
                SupplierName = Supplier.SupplierName,
                LeadTimeId = Supplier.LeadTimeId,
                LeadTime = Supplier.LeadTime,
                Address = Supplier.Address,
                Handphone = Supplier.Handphone,
                Email = Supplier.Email,
                Note = Supplier.Note,
                IsPKS = Supplier.IsPKS,
                IsActive = Supplier.IsActive,
            }).ToListAsync();
        }

        public IEnumerable<Supplier> GetAllSupplier()
        {
            return _context.Suppliers.OrderByDescending(d => d.CreateDateTime).Where(s => s.IsActive == true && s.IsPKS == true)
                .Include(l => l.LeadTime)
                .AsNoTracking();
        }

        public IEnumerable<Supplier> GetAllSupplierNonActive()
        {
            return _context.Suppliers.OrderByDescending(d => d.CreateDateTime).Where(s => s.IsActive == false)
                .Include(l => l.LeadTime)
                .AsNoTracking();
        }

        public IEnumerable<Supplier> GetAllSupplierNonPks()
        {
            return _context.Suppliers.OrderByDescending(d => d.CreateDateTime).Where(s => s.IsPKS == false && s.IsActive == true)
                .Include(l => l.LeadTime)
                .AsNoTracking();
        }

        public async Task<(IEnumerable<Supplier> suppliers, int totalCountSuppliers)> GetAllSupplierPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.Suppliers
                .OrderByDescending(d => d.CreateDateTime)
                .Where(s => s.IsActive == true && s.IsPKS == true)
                .Include(l => l.LeadTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.SupplierCode.Contains(searchTerm) || p.SupplierName.Contains(searchTerm));
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
            var suppliers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (suppliers, totalCount);
        }

        public async Task<(IEnumerable<Supplier> suppliers, int totalCountSuppliers)> GetAllSupplierNonActivePageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.Suppliers
                .OrderByDescending(d => d.CreateDateTime)
                .Where(s => s.IsActive == false)
                .Include(l => l.LeadTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.SupplierCode.Contains(searchTerm) || p.SupplierName.Contains(searchTerm));
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
            var suppliers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (suppliers, totalCount);
        }

        public async Task<(IEnumerable<Supplier> suppliers, int totalCountSuppliers)> GetAllSupplierNonPksPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.Suppliers
                .OrderByDescending(d => d.CreateDateTime)
                .Where(s => s.IsPKS == false && s.IsActive == true)
                .Include(l => l.LeadTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.SupplierCode.Contains(searchTerm) || p.SupplierName.Contains(searchTerm));
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
            var suppliers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (suppliers, totalCount);
        }

        public Supplier Update(Supplier update)
        {
            var Supplier = _context.Suppliers.Attach(update);
            Supplier.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Supplier Delete(Guid Id)
        {
            var Supplier = _context.Suppliers.Find(Id);
            if (Supplier != null)
            {
                _context.Suppliers.Remove(Supplier);
                _context.SaveChanges();
            }
            return Supplier;
        }
    }
}
