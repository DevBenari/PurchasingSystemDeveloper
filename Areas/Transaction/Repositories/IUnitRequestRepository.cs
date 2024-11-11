using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Transaction.Repositories
{
    public class IUnitRequestRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IUnitRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public UnitRequest Tambah(UnitRequest UnitRequest)
        {
            _context.UnitRequests.Add(UnitRequest);
            _context.SaveChanges();
            return UnitRequest;
        }

        public async Task<UnitRequest> GetUnitRequestById(Guid Id)
        {
            var UnitRequest = _context.UnitRequests
                .Where(i => i.UnitRequestId == Id)
                .Include(d => d.UnitRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(z => z.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .FirstOrDefault(p => p.UnitRequestId == Id);

            if (UnitRequest != null)
            {
                var UnitRequestDetail = new UnitRequest()
                {
                    CreateDateTime = UnitRequest.CreateDateTime,
                    CreateBy = UnitRequest.CreateBy,
                    UpdateDateTime = UnitRequest.UpdateDateTime,
                    UpdateBy = UnitRequest.UpdateBy,
                    DeleteDateTime = UnitRequest.DeleteDateTime,
                    DeleteBy = UnitRequest.DeleteBy,
                    UnitRequestId = UnitRequest.UnitRequestId,
                    UnitRequestNumber = UnitRequest.UnitRequestNumber,
                    UserAccessId = UnitRequest.UserAccessId,
                    Department1Id = UnitRequest.Department1Id,
                    Department1 = UnitRequest.Department1,
                    Position1Id = UnitRequest.Position1Id,
                    Position1 = UnitRequest.Position1,
                    UserApprove1Id = UnitRequest.UserApprove1Id,
                    UserApprove1 = UnitRequest.UserApprove1,
                    ApproveStatusUser1 = UnitRequest.ApproveStatusUser1,
                    ApplicationUser = UnitRequest.ApplicationUser,
                    UnitLocationId = UnitRequest.UnitLocationId,
                    UnitLocation = UnitRequest.UnitLocation,                   
                    WarehouseLocationId = UnitRequest.WarehouseLocationId,
                    WarehouseLocation = UnitRequest.WarehouseLocation,                    
                    QtyTotal = UnitRequest.QtyTotal,
                    Status = UnitRequest.Status,
                    Note = UnitRequest.Note,
                    MessageApprove1 = UnitRequest.MessageApprove1,
                    UnitRequestDetails = UnitRequest.UnitRequestDetails,
                };
                return UnitRequestDetail;
            }
            return UnitRequest;
        }

        public async Task<UnitRequest> GetUnitRequestByIdNoTracking(Guid Id)
        {
            return await _context.UnitRequests.AsNoTracking().Where(i => i.UnitRequestId == Id).FirstOrDefaultAsync(a => a.UnitRequestId == Id);
        }

        public async Task<List<UnitRequest>> GetUnitRequests()
        {
            return await _context.UnitRequests./*OrderBy(p => p.CreateDateTime).*/Select(UnitRequest => new UnitRequest()
            {
                CreateDateTime = UnitRequest.CreateDateTime,
                CreateBy = UnitRequest.CreateBy,
                UpdateDateTime = UnitRequest.UpdateDateTime,
                UpdateBy = UnitRequest.UpdateBy,
                DeleteDateTime = UnitRequest.DeleteDateTime,
                DeleteBy = UnitRequest.DeleteBy,
                UnitRequestId = UnitRequest.UnitRequestId,
                UnitRequestNumber = UnitRequest.UnitRequestNumber,
                UserAccessId = UnitRequest.UserAccessId,
                Department1Id = UnitRequest.Department1Id,
                Department1 = UnitRequest.Department1,
                Position1Id = UnitRequest.Position1Id,
                Position1 = UnitRequest.Position1,
                UserApprove1Id = UnitRequest.UserApprove1Id,
                UserApprove1 = UnitRequest.UserApprove1,
                ApproveStatusUser1 = UnitRequest.ApproveStatusUser1,
                ApplicationUser = UnitRequest.ApplicationUser,
                UnitLocationId = UnitRequest.UnitLocationId,
                UnitLocation = UnitRequest.UnitLocation,
                WarehouseLocationId = UnitRequest.WarehouseLocationId,
                WarehouseLocation = UnitRequest.WarehouseLocation,
                QtyTotal = UnitRequest.QtyTotal,
                Status = UnitRequest.Status,
                Note = UnitRequest.Note,
                MessageApprove1 = UnitRequest.MessageApprove1,
                UnitRequestDetails = UnitRequest.UnitRequestDetails,
            }).ToListAsync();
        }

        public IEnumerable<UnitRequest> GetAllUnitRequest()
        {
            return _context.UnitRequests.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.UnitRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(z => z.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .ToList();
        }

        public IEnumerable<UnitRequest> GetAllUnitRequestAsc()
        {
            return _context.UnitRequests
                .Include(d => d.UnitRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(z => z.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .ToList();
        }

        public async Task<UnitRequest> Update(UnitRequest update)
        {
            List<UnitRequestDetail> UnitRequestDetails = _context.UnitRequestDetails.Where(d => d.UnitRequestId == update.UnitRequestId).ToList();
            _context.UnitRequestDetails.RemoveRange(UnitRequestDetails);
            _context.SaveChanges();

            var UnitRequest = _context.UnitRequests.Attach(update);
            UnitRequest.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.UnitRequestDetails.AddRangeAsync(update.UnitRequestDetails);
            _context.SaveChanges();
            return update;
        }

        public UnitRequest Delete(Guid Id)
        {
            var UnitRequest = _context.UnitRequests.Find(Id);
            if (UnitRequest != null)
            {
                _context.UnitRequests.Remove(UnitRequest);
                _context.SaveChanges();
            }
            return UnitRequest;
        }
    }
}
