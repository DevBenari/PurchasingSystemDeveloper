using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.Order.Repositories
{
    public class IPurchaseRequestRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IPurchaseRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public PurchaseRequest Tambah(PurchaseRequest purchaseRequest)
        {
            _context.PurchaseRequests.Add(purchaseRequest);
            _context.SaveChanges();
            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetPurchaseRequestById(Guid Id)
        {
            var purchaseRequest = _context.PurchaseRequests
                .Where(i => i.PurchaseRequestId == Id)
                .Include(d => d.PurchaseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                .FirstOrDefault(p => p.PurchaseRequestId == Id);

            if (purchaseRequest != null)
            {
                var purchaseRequestDetail = new PurchaseRequest()
                {
                    CreateDateTime = purchaseRequest.CreateDateTime,
                    CreateBy = purchaseRequest.CreateBy,
                    UpdateDateTime = purchaseRequest.UpdateDateTime,
                    UpdateBy = purchaseRequest.UpdateBy,
                    DeleteDateTime = purchaseRequest.DeleteDateTime,
                    DeleteBy = purchaseRequest.DeleteBy,
                    PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                    PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                    UserAccessId = purchaseRequest.UserAccessId,
                    ApplicationUser = purchaseRequest.ApplicationUser,
                    ExpiredDay = purchaseRequest.ExpiredDay,
                    RemainingDay = purchaseRequest.RemainingDay,
                    //DueDate = purchaseRequest.DueDate,
                    ExpiredDate = purchaseRequest.ExpiredDate,
                    Department1Id = purchaseRequest.Department1Id,
                    Department1 = purchaseRequest.Department1,
                    Position1Id = purchaseRequest.Position1Id,
                    Position1 = purchaseRequest.Position1,
                    UserApprove1Id = purchaseRequest.UserApprove1Id,
                    UserApprove1 = purchaseRequest.UserApprove1,
                    ApproveStatusUser1 = purchaseRequest.ApproveStatusUser1,
                    Department2Id = purchaseRequest.Department2Id,
                    Department2 = purchaseRequest.Department2,
                    Position2Id = purchaseRequest.Position2Id,
                    Position2 = purchaseRequest.Position2,
                    UserApprove2Id = purchaseRequest.UserApprove2Id,
                    UserApprove2 = purchaseRequest.UserApprove2,
                    ApproveStatusUser2 = purchaseRequest.ApproveStatusUser2,
                    Department3Id = purchaseRequest.Department3Id,
                    Department3 = purchaseRequest.Department3,
                    Position3Id = purchaseRequest.Position3Id,
                    Position3 = purchaseRequest.Position3,
                    UserApprove3Id = purchaseRequest.UserApprove3Id,
                    UserApprove3 = purchaseRequest.UserApprove3,
                    ApproveStatusUser3 = purchaseRequest.ApproveStatusUser3,
                    TermOfPaymentId = purchaseRequest.TermOfPaymentId,
                    TermOfPayment = purchaseRequest.TermOfPayment,                    
                    Status = purchaseRequest.Status,
                    QtyTotal = purchaseRequest.QtyTotal,
                    GrandTotal = purchaseRequest.GrandTotal,
                    Note = purchaseRequest.Note,
                    MessageApprove1 = purchaseRequest.MessageApprove1,
                    MessageApprove2 = purchaseRequest.MessageApprove2,
                    MessageApprove3 = purchaseRequest.MessageApprove3,
                    PurchaseRequestDetails = purchaseRequest.PurchaseRequestDetails
                };
                return purchaseRequestDetail;
            }
            return purchaseRequest;
        }

        public async Task<PurchaseRequest> GetPurchaseRequestByIdNoTracking(Guid Id)
        {
            return await _context.PurchaseRequests.AsNoTracking()
                .Where(i => i.PurchaseRequestId == Id)
                .Include(d => d.PurchaseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                //.Include(e => e.DueDate)
                .FirstOrDefaultAsync(a => a.PurchaseRequestId == Id);
        }

        public async Task<List<PurchaseRequest>> GetPurchaseRequests()
        {
            return await _context.PurchaseRequests./*OrderBy(p => p.CreateDateTime).*/Select(purchaseRequest => new PurchaseRequest()
            {
                CreateDateTime = purchaseRequest.CreateDateTime,
                CreateBy = purchaseRequest.CreateBy,
                UpdateDateTime = purchaseRequest.UpdateDateTime,
                UpdateBy = purchaseRequest.UpdateBy,
                DeleteDateTime = purchaseRequest.DeleteDateTime,
                DeleteBy = purchaseRequest.DeleteBy,
                PurchaseRequestId = purchaseRequest.PurchaseRequestId,
                PurchaseRequestNumber = purchaseRequest.PurchaseRequestNumber,
                UserAccessId = purchaseRequest.UserAccessId,
                ApplicationUser = purchaseRequest.ApplicationUser,
                ExpiredDay = purchaseRequest.ExpiredDay,
                RemainingDay = purchaseRequest.RemainingDay,
                //DueDate = purchaseRequest.DueDate,
                ExpiredDate = purchaseRequest.ExpiredDate,
                Department1Id = purchaseRequest.Department1Id,
                Department1 = purchaseRequest.Department1,
                Position1Id = purchaseRequest.Position1Id,
                Position1 = purchaseRequest.Position1,
                UserApprove1Id = purchaseRequest.UserApprove1Id,
                UserApprove1 = purchaseRequest.UserApprove1,
                ApproveStatusUser1 = purchaseRequest.ApproveStatusUser1,
                Department2Id = purchaseRequest.Department2Id,
                Department2 = purchaseRequest.Department2,
                Position2Id = purchaseRequest.Position2Id,
                Position2 = purchaseRequest.Position2,
                UserApprove2Id = purchaseRequest.UserApprove2Id,
                UserApprove2 = purchaseRequest.UserApprove2,
                ApproveStatusUser2 = purchaseRequest.ApproveStatusUser2,
                Department3Id = purchaseRequest.Department3Id,
                Department3 = purchaseRequest.Department3,
                Position3Id = purchaseRequest.Position3Id,
                Position3 = purchaseRequest.Position3,
                UserApprove3Id = purchaseRequest.UserApprove3Id,
                UserApprove3 = purchaseRequest.UserApprove3,
                ApproveStatusUser3 = purchaseRequest.ApproveStatusUser3,
                TermOfPaymentId = purchaseRequest.TermOfPaymentId,
                TermOfPayment = purchaseRequest.TermOfPayment,
                Status = purchaseRequest.Status,
                QtyTotal = purchaseRequest.QtyTotal,
                GrandTotal = purchaseRequest.GrandTotal,
                Note = purchaseRequest.Note,
                MessageApprove1 = purchaseRequest.MessageApprove1,
                MessageApprove2 = purchaseRequest.MessageApprove2,
                MessageApprove3 = purchaseRequest.MessageApprove3,
                PurchaseRequestDetails = purchaseRequest.PurchaseRequestDetails
            }).ToListAsync();
        }

        public IEnumerable<PurchaseRequest> GetAllPurchaseRequest()
        {
            return _context.PurchaseRequests.OrderByDescending(o => o.CreateDateTime)
                .Include(d => d.PurchaseRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                //.Include(e => e.DueDate)
                .ToList();
        }

        public async Task<PurchaseRequest> Update(PurchaseRequest update)
        {
            List<PurchaseRequestDetail> purchaseRequestDetails = _context.PurchaseRequestDetails.Where(d => d.PurchaseRequestId == update.PurchaseRequestId).ToList();
            _context.PurchaseRequestDetails.RemoveRange(purchaseRequestDetails);
            _context.SaveChanges();

            var purchaseRequest = _context.PurchaseRequests.Attach(update);
            purchaseRequest.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.PurchaseRequestDetails.AddRangeAsync(update.PurchaseRequestDetails);
            _context.SaveChanges();
            return update;
        }

        public PurchaseRequest Delete(Guid Id)
        {
            var PurchaseRequest = _context.PurchaseRequests.Find(Id);
            if (PurchaseRequest != null)
            {
                _context.PurchaseRequests.Remove(PurchaseRequest);
                _context.SaveChanges();
            }
            return PurchaseRequest;
        }
    }
}
