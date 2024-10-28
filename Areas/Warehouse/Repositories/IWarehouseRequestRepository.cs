using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Repositories
{
    public class IWarehouseRequestRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IWarehouseRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public WarehouseRequest Tambah(WarehouseRequest WarehouseRequest)
        {
            _context.WarehouseRequests.Add(WarehouseRequest);
            _context.SaveChanges();
            return WarehouseRequest;
        }

        public async Task<WarehouseRequest> GetWarehouseRequestById(Guid Id)
        {
            var WarehouseRequest = _context.WarehouseRequests
                .Where(i => i.WarehouseRequestId == Id)
                .Include(d => d.WarehouseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .FirstOrDefault(p => p.WarehouseRequestId == Id);

            if (WarehouseRequest != null)
            {
                var WarehouseRequestDetail = new WarehouseRequest()
                {
                    WarehouseRequestId = WarehouseRequest.WarehouseRequestId,
                    WarehouseRequestNumber = WarehouseRequest.WarehouseRequestNumber,
                    UnitRequestId = WarehouseRequest.UnitRequestId,
                    UnitRequestNumber = WarehouseRequest.UnitRequestNumber,
                    UserAccessId = WarehouseRequest.UserAccessId,
                    ApplicationUser = WarehouseRequest.ApplicationUser,
                    UnitLocationId = WarehouseRequest.UnitLocationId,
                    UnitLocation = WarehouseRequest.UnitLocation,
                    UnitRequestManagerId = WarehouseRequest.UnitRequestManagerId,
                    UnitRequestManager = WarehouseRequest.UnitRequestManager,
                    WarehouseLocationId = WarehouseRequest.WarehouseLocationId,
                    WarehouseLocation = WarehouseRequest.WarehouseLocation,
                    WarehouseApprovalId = WarehouseRequest.WarehouseApprovalId,
                    WarehouseApproval = WarehouseRequest.WarehouseApproval,
                    Status = WarehouseRequest.Status,
                    QtyTotal = WarehouseRequest.QtyTotal,
                    WarehouseRequestDetails = WarehouseRequest.WarehouseRequestDetails
                };
                return WarehouseRequestDetail;
            }
            return WarehouseRequest;
        }

        public async Task<WarehouseRequest> GetWarehouseRequestByIdNoTracking(Guid Id)
        {
            return await _context.WarehouseRequests.AsNoTracking()
                .Include(d => d.WarehouseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .Where(i => i.WarehouseRequestId == Id).FirstOrDefaultAsync(a => a.WarehouseRequestId == Id);
        }

        public async Task<List<WarehouseRequest>> GetWarehouseRequests()
        {
            return await _context.WarehouseRequests.Where(s => s.Status != "Selesai").OrderBy(p => p.CreateDateTime).Select(WarehouseRequest => new WarehouseRequest()
            {
                WarehouseRequestId = WarehouseRequest.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseRequest.WarehouseRequestNumber,
                UnitRequestId = WarehouseRequest.UnitRequestId,
                UnitRequestNumber = WarehouseRequest.UnitRequestNumber,
                UserAccessId = WarehouseRequest.UserAccessId,
                ApplicationUser = WarehouseRequest.ApplicationUser,
                UnitLocationId = WarehouseRequest.UnitLocationId,
                UnitLocation = WarehouseRequest.UnitLocation,
                UnitRequestManagerId = WarehouseRequest.UnitRequestManagerId,
                UnitRequestManager = WarehouseRequest.UnitRequestManager,
                WarehouseLocationId = WarehouseRequest.WarehouseLocationId,
                WarehouseLocation = WarehouseRequest.WarehouseLocation,
                WarehouseApprovalId = WarehouseRequest.WarehouseApprovalId,
                WarehouseApproval = WarehouseRequest.WarehouseApproval,
                Status = WarehouseRequest.Status,
                QtyTotal = WarehouseRequest.QtyTotal,
                WarehouseRequestDetails = WarehouseRequest.WarehouseRequestDetails
            }).ToListAsync();
        }

        public async Task<List<WarehouseRequest>> GetWarehouseRequestDetails()
        {
            return await _context.WarehouseRequests.OrderBy(p => p.CreateDateTime).Select(WarehouseRequest => new WarehouseRequest()
            {
                WarehouseRequestId = WarehouseRequest.WarehouseRequestId,
                WarehouseRequestNumber = WarehouseRequest.WarehouseRequestNumber,
                UnitRequestId = WarehouseRequest.UnitRequestId,
                UnitRequestNumber = WarehouseRequest.UnitRequestNumber,
                UserAccessId = WarehouseRequest.UserAccessId,
                ApplicationUser = WarehouseRequest.ApplicationUser,
                UnitLocationId = WarehouseRequest.UnitLocationId,
                UnitLocation = WarehouseRequest.UnitLocation,
                UnitRequestManagerId = WarehouseRequest.UnitRequestManagerId,
                UnitRequestManager = WarehouseRequest.UnitRequestManager,
                WarehouseLocationId = WarehouseRequest.WarehouseLocationId,
                WarehouseLocation = WarehouseRequest.WarehouseLocation,
                WarehouseApprovalId = WarehouseRequest.WarehouseApprovalId,
                WarehouseApproval = WarehouseRequest.WarehouseApproval,
                Status = WarehouseRequest.Status,
                QtyTotal = WarehouseRequest.QtyTotal,
                WarehouseRequestDetails = WarehouseRequest.WarehouseRequestDetails
            }).ToListAsync();
        }

        public IEnumerable<WarehouseRequest> GetAllWarehouseRequest()
        {
            return _context.WarehouseRequests.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.WarehouseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(b => b.UnitRequestManager)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.WarehouseApproval)
                .ToList();
        }

        public async Task<WarehouseRequest> Update(WarehouseRequest update)
        {
            var WarehouseRequest = _context.WarehouseRequests.Attach(update);
            WarehouseRequest.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }
    }
}
