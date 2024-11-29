using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Repositories
{
    public class IWarehouseTransferRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IWarehouseTransferRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public WarehouseTransfer Tambah(WarehouseTransfer WarehouseTransfer)
        {
            _context.WarehouseTransfers.Add(WarehouseTransfer);
            _context.SaveChanges();
            return WarehouseTransfer;
        }

        public async Task<WarehouseTransfer> GetWarehouseTransferById(Guid Id)
        {
            var WarehouseTransfer = _context.WarehouseTransfers
                .Where(i => i.WarehouseTransferId == Id)
                .Include(d => d.WarehouseTransferDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1Id)
                .FirstOrDefault(p => p.WarehouseTransferId == Id);

            if (WarehouseTransfer != null)
            {
                var WarehouseTransferDetail = new WarehouseTransfer()
                {
                    CreateDateTime = WarehouseTransfer.CreateDateTime,
                    CreateBy = WarehouseTransfer.CreateBy,
                    UpdateDateTime = WarehouseTransfer.UpdateDateTime,
                    UpdateBy = WarehouseTransfer.UpdateBy,
                    DeleteDateTime = WarehouseTransfer.DeleteDateTime,
                    DeleteBy = WarehouseTransfer.DeleteBy,
                    WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                    WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                    UnitOrderId = WarehouseTransfer.UnitOrderId,
                    UnitOrderNumber = WarehouseTransfer.UnitOrderNumber,
                    UserAccessId = WarehouseTransfer.UserAccessId,
                    ApplicationUser = WarehouseTransfer.ApplicationUser,
                    UnitLocationId = WarehouseTransfer.UnitLocationId,
                    UnitLocation = WarehouseTransfer.UnitLocation,                    
                    WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                    WarehouseLocation = WarehouseTransfer.WarehouseLocation,
                    UserApprove1Id = WarehouseTransfer.UserApprove1Id,
                    UserApprove1 = WarehouseTransfer.UserApprove1,
                    Status = WarehouseTransfer.Status,
                    QtyTotal = WarehouseTransfer.QtyTotal,
                    WarehouseTransferDetails = WarehouseTransfer.WarehouseTransferDetails
                };
                return WarehouseTransferDetail;
            }
            return WarehouseTransfer;
        }

        public async Task<WarehouseTransfer> GetWarehouseTransferByIdNoTracking(Guid Id)
        {
            return await _context.WarehouseTransfers.AsNoTracking().Where(i => i.WarehouseTransferId == Id).FirstOrDefaultAsync(a => a.WarehouseTransferId == Id);
        }

        public async Task<List<WarehouseTransfer>> GetWarehouseTransfers()
        {
            return await _context.WarehouseTransfers.Where(s => s.Status != "Selesai").OrderBy(p => p.CreateDateTime).Select(WarehouseTransfer => new WarehouseTransfer()
            {
                CreateDateTime = WarehouseTransfer.CreateDateTime,
                CreateBy = WarehouseTransfer.CreateBy,
                UpdateDateTime = WarehouseTransfer.UpdateDateTime,
                UpdateBy = WarehouseTransfer.UpdateBy,
                DeleteDateTime = WarehouseTransfer.DeleteDateTime,
                DeleteBy = WarehouseTransfer.DeleteBy,
                WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                UnitOrderId = WarehouseTransfer.UnitOrderId,
                UnitOrderNumber = WarehouseTransfer.UnitOrderNumber,
                UserAccessId = WarehouseTransfer.UserAccessId,
                ApplicationUser = WarehouseTransfer.ApplicationUser,
                UnitLocationId = WarehouseTransfer.UnitLocationId,
                UnitLocation = WarehouseTransfer.UnitLocation,
                WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                WarehouseLocation = WarehouseTransfer.WarehouseLocation,
                UserApprove1Id = WarehouseTransfer.UserApprove1Id,
                UserApprove1 = WarehouseTransfer.UserApprove1,
                Status = WarehouseTransfer.Status,
                QtyTotal = WarehouseTransfer.QtyTotal,
                WarehouseTransferDetails = WarehouseTransfer.WarehouseTransferDetails
            }).ToListAsync();
        }

        public async Task<List<WarehouseTransfer>> GetWarehouseTransferDetails()
        {
            return await _context.WarehouseTransfers.OrderBy(p => p.CreateDateTime).Select(WarehouseTransfer => new WarehouseTransfer()
            {
                CreateDateTime = WarehouseTransfer.CreateDateTime,
                CreateBy = WarehouseTransfer.CreateBy,
                UpdateDateTime = WarehouseTransfer.UpdateDateTime,
                UpdateBy = WarehouseTransfer.UpdateBy,
                DeleteDateTime = WarehouseTransfer.DeleteDateTime,
                DeleteBy = WarehouseTransfer.DeleteBy,
                WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                UnitOrderId = WarehouseTransfer.UnitOrderId,
                UnitOrderNumber = WarehouseTransfer.UnitOrderNumber,
                UserAccessId = WarehouseTransfer.UserAccessId,
                ApplicationUser = WarehouseTransfer.ApplicationUser,
                UnitLocationId = WarehouseTransfer.UnitLocationId,
                UnitLocation = WarehouseTransfer.UnitLocation,
                WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                WarehouseLocation = WarehouseTransfer.WarehouseLocation,
                UserApprove1Id = WarehouseTransfer.UserApprove1Id,
                UserApprove1 = WarehouseTransfer.UserApprove1,
                Status = WarehouseTransfer.Status,
                QtyTotal = WarehouseTransfer.QtyTotal,
                WarehouseTransferDetails = WarehouseTransfer.WarehouseTransferDetails
            }).ToListAsync();
        }

        public IEnumerable<WarehouseTransfer> GetAllWarehouseTransfer()
        {
            return _context.WarehouseTransfers.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.WarehouseTransferDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .ToList();
        }

        public async Task<(IEnumerable<WarehouseTransfer> warehouseTransfers, int totalCountWarehouseTransfers)> GetAllWarehouseTransferPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.WarehouseTransfers
                .Include(d => d.WarehouseTransferDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .OrderByDescending(d => d.CreateDateTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.UnitOrderNumber.Contains(searchTerm) || p.UnitLocation.UnitLocationName.Contains(searchTerm) || p.WarehouseLocation.WarehouseLocationName.Contains(searchTerm));
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
            var warehouseTransfers = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (warehouseTransfers, totalCount);
        }

        public async Task<WarehouseTransfer> Update(WarehouseTransfer update)
        {
            var WarehouseTransfer = _context.WarehouseTransfers.Attach(update);
            WarehouseTransfer.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }
    }
}
