using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Repositories
{
    public class IApprovalRequestRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IApprovalRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public ApprovalRequest Tambah(ApprovalRequest ApprovalRequest)
        {
            _context.ApprovalRequests.Add(ApprovalRequest);
            _context.SaveChanges();
            return ApprovalRequest;
        }

        public async Task<ApprovalRequest> GetApprovalRequestById(Guid Id)
        {
            var ApprovalRequest = _context.ApprovalRequests
                .Where(i => i.ApprovalRequestId == Id)
                .Include(d => d.UnitRequest)
                .Include(u => u.UnitLocation)
                .Include(z => z.UnitRequestManager)
                .Include(a => a.WarehouseApproval)
                .Include(t => t.ApplicationUser)
                .FirstOrDefault(p => p.ApprovalRequestId == Id);

            if (ApprovalRequest != null)
            {
                var ApprovalRequestDetail = new ApprovalRequest()
                {
                    ApprovalRequestId = ApprovalRequest.ApprovalRequestId,
                    UnitRequestId = ApprovalRequest.UnitRequestId,
                    UnitRequestNumber = ApprovalRequest.UnitRequestNumber,
                    UserAccessId = ApprovalRequest.UserAccessId,
                    ApplicationUser = ApprovalRequest.ApplicationUser,
                    UnitLocationId = ApprovalRequest.UnitLocationId,
                    UnitLocation = ApprovalRequest.UnitLocation,
                    UnitRequestManagerId = ApprovalRequest.UnitRequestManagerId,
                    UnitRequestManager = ApprovalRequest.UnitRequestManager,
                    ApproveDate = ApprovalRequest.ApproveDate,
                    WarehouseApprovalId = ApprovalRequest.WarehouseApprovalId,                    
                    WarehouseApproval = ApprovalRequest.WarehouseApproval,
                    Status = ApprovalRequest.Status,
                    Note = ApprovalRequest.Note,
                };
                return ApprovalRequestDetail;
            }
            return ApprovalRequest;
        }

        public async Task<ApprovalRequest> GetApprovalRequestByIdNoTracking(Guid Id)
        {
            return await _context.ApprovalRequests.AsNoTracking().Where(i => i.ApprovalRequestId == Id).FirstOrDefaultAsync(a => a.ApprovalRequestId == Id);
        }

        public async Task<List<ApprovalRequest>> GetApprovalRequests()
        {
            return await _context.ApprovalRequests./*OrderBy(p => p.CreateDateTime).*/Select(ApprovalRequest => new ApprovalRequest()
            {
                ApprovalRequestId = ApprovalRequest.ApprovalRequestId,
                UnitRequestId = ApprovalRequest.UnitRequestId,
                UnitRequestNumber = ApprovalRequest.UnitRequestNumber,
                UserAccessId = ApprovalRequest.UserAccessId,
                ApplicationUser = ApprovalRequest.ApplicationUser,
                UnitLocationId = ApprovalRequest.UnitLocationId,
                UnitLocation = ApprovalRequest.UnitLocation,
                UnitRequestManagerId = ApprovalRequest.UnitRequestManagerId,
                UnitRequestManager = ApprovalRequest.UnitRequestManager,
                ApproveDate = ApprovalRequest.ApproveDate,
                WarehouseApprovalId = ApprovalRequest.WarehouseApprovalId,
                WarehouseApproval = ApprovalRequest.WarehouseApproval,
                Status = ApprovalRequest.Status,
                Note = ApprovalRequest.Note,
            }).ToListAsync();
        }

        public IEnumerable<ApprovalRequest> GetAllApprovalRequest()
        {
            return _context.ApprovalRequests.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.UnitRequest)
                .Include(u => u.UnitLocation)
                .Include(z => z.UnitRequestManager)
                .Include(a => a.WarehouseApproval)
                .Include(t => t.ApplicationUser)
                .ToList();
        }

        public async Task<ApprovalRequest> Update(ApprovalRequest update)
        {
            var ApprovalRequest = _context.ApprovalRequests.Attach(update);
            ApprovalRequest.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public ApprovalRequest Delete(Guid Id)
        {
            var ApprovalRequest = _context.ApprovalRequests.Find(Id);
            if (ApprovalRequest != null)
            {
                _context.ApprovalRequests.Remove(ApprovalRequest);
                _context.SaveChanges();
            }
            return ApprovalRequest;
        }
    }
}
