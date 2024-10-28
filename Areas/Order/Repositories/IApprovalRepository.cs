using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Order.Repositories
{
    public class IApprovalRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IApprovalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public Approval Tambah(Approval Approval)
        {
            _context.Approvals.Add(Approval);
            _context.SaveChanges();
            return Approval;
        }

        public async Task<Approval> GetApprovalById(Guid Id)
        {
            var Approval = _context.Approvals
                .Where(i => i.ApprovalId == Id)
                .Include(u => u.ApplicationUser)
                .Include(t => t.PurchaseRequest)
                .Include(a1 => a1.UserApprove)
                .FirstOrDefault(p => p.ApprovalId == Id);

            if (Approval != null)
            {
                var ApprovalDetail = new Approval()
                {
                    ApprovalId = Approval.ApprovalId,
                    PurchaseRequestId = Approval.PurchaseRequestId,
                    PurchaseRequestNumber = Approval.PurchaseRequestNumber,
                    UserAccessId = Approval.UserAccessId,
                    ApplicationUser = Approval.ApplicationUser,
                    ExpiredDay = Approval.ExpiredDay,
                    RemainingDay = Approval.RemainingDay,
                    ExpiredDate = Approval.ExpiredDate,
                    UserApproveId = Approval.UserApproveId,
                    ApproveBy = Approval.ApproveBy,
                    ApprovalTime = Approval.ApprovalTime,
                    ApprovalDate = Approval.ApprovalDate,
                    ApprovalStatusUser = Approval.ApprovalStatusUser,
                    Status = Approval.Status,
                    Message = Approval.Message,
                    Note = Approval.Note
                };
                return ApprovalDetail;
            }
            return Approval;
        }

        public async Task<Approval> GetApprovalByIdNoTracking(Guid Id)
        {
            return await _context.Approvals.AsNoTracking().FirstOrDefaultAsync(a => a.ApprovalId == Id);
        }

        public async Task<List<Approval>> GetApprovals()
        {
            return await _context.Approvals./*OrderBy(p => p.CreateDateTime).*/Select(Approval => new Approval()
            {
                ApprovalId = Approval.ApprovalId,
                PurchaseRequestId = Approval.PurchaseRequestId,
                PurchaseRequestNumber = Approval.PurchaseRequestNumber,
                UserAccessId = Approval.UserAccessId,
                ApplicationUser = Approval.ApplicationUser,
                ExpiredDay = Approval.ExpiredDay,
                RemainingDay = Approval.RemainingDay,
                ExpiredDate = Approval.ExpiredDate,
                UserApproveId = Approval.UserApproveId,
                ApproveBy = Approval.ApproveBy,
                ApprovalTime = Approval.ApprovalTime,
                ApprovalDate = Approval.ApprovalDate,
                ApprovalStatusUser = Approval.ApprovalStatusUser,
                Status = Approval.Status,
                Message = Approval.Message,
                Note = Approval.Note
            }).ToListAsync();
        }

        public IEnumerable<Approval> GetAllApproval()
        {
            return _context.Approvals.OrderByDescending(c => c.CreateDateTime)
                .Include(u => u.ApplicationUser)
                .Include(t => t.PurchaseRequest)
                .Include(a1 => a1.UserApprove)
                .ToList();
        }

        public async Task<Approval> Update(Approval update)
        {          
            var approval = _context.Approvals.Attach(update);
            approval.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Approval Delete(Guid Id)
        {
            var Approval = _context.Approvals.Find(Id);
            if (Approval != null)
            {
                _context.Approvals.Remove(Approval);
                _context.SaveChanges();
            }
            return Approval;
        }
    }
}
