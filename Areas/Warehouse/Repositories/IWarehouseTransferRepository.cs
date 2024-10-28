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
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .FirstOrDefault(p => p.WarehouseTransferId == Id);

            if (WarehouseTransfer != null)
            {
                var WarehouseTransferDetail = new WarehouseTransfer()
                {
                    WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                    WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                    WarehouseRequestId = WarehouseTransfer.WarehouseRequestId,
                    WarehouseRequestNumber = WarehouseTransfer.WarehouseRequestNumber,
                    UserAccessId = WarehouseTransfer.UserAccessId,
                    ApplicationUser = WarehouseTransfer.ApplicationUser,
                    UnitLocationId = WarehouseTransfer.UnitLocationId,
                    UnitLocation = WarehouseTransfer.UnitLocation,
                    UnitRequestManagerId = WarehouseTransfer.UnitRequestManagerId,
                    UnitRequestManager = WarehouseTransfer.UnitRequestManager,
                    WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                    WarehouseLocation = WarehouseTransfer.WarehouseLocation,
                    WarehouseApprovalId = WarehouseTransfer.WarehouseApprovalId,
                    WarehouseApproval = WarehouseTransfer.WarehouseApproval,
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
                WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                WarehouseRequestId = WarehouseTransfer.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseTransfer.WarehouseRequestNumber,
                UserAccessId = WarehouseTransfer.UserAccessId,
                ApplicationUser = WarehouseTransfer.ApplicationUser,
                UnitLocationId = WarehouseTransfer.UnitLocationId,
                UnitLocation = WarehouseTransfer.UnitLocation,
                UnitRequestManagerId = WarehouseTransfer.UnitRequestManagerId,
                UnitRequestManager = WarehouseTransfer.UnitRequestManager,
                WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                WarehouseLocation = WarehouseTransfer.WarehouseLocation,
                WarehouseApprovalId = WarehouseTransfer.WarehouseApprovalId,
                WarehouseApproval = WarehouseTransfer.WarehouseApproval,
                Status = WarehouseTransfer.Status,
                QtyTotal = WarehouseTransfer.QtyTotal,
                WarehouseTransferDetails = WarehouseTransfer.WarehouseTransferDetails
            }).ToListAsync();
        }

        public async Task<List<WarehouseTransfer>> GetWarehouseTransferDetails()
        {
            return await _context.WarehouseTransfers.OrderBy(p => p.CreateDateTime).Select(WarehouseTransfer => new WarehouseTransfer()
            {
                WarehouseTransferId = WarehouseTransfer.WarehouseTransferId,
                WarehouseTransferNumber = WarehouseTransfer.WarehouseTransferNumber,
                WarehouseRequestId = WarehouseTransfer.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseTransfer.WarehouseRequestNumber,
                UserAccessId = WarehouseTransfer.UserAccessId,
                ApplicationUser = WarehouseTransfer.ApplicationUser,
                UnitLocationId = WarehouseTransfer.UnitLocationId,
                UnitLocation = WarehouseTransfer.UnitLocation,
                UnitRequestManagerId = WarehouseTransfer.UnitRequestManagerId,
                UnitRequestManager = WarehouseTransfer.UnitRequestManager,
                WarehouseLocationId = WarehouseTransfer.WarehouseLocationId,
                WarehouseLocation = WarehouseTransfer.WarehouseLocation,
                WarehouseApprovalId = WarehouseTransfer.WarehouseApprovalId,
                WarehouseApproval = WarehouseTransfer.WarehouseApproval,
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
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .ToList();
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
