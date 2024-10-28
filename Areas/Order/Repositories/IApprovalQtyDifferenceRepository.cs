using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Order.Repositories
{
    public class IApprovalQtyDifferenceRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IApprovalQtyDifferenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public ApprovalQtyDifference Tambah(ApprovalQtyDifference approval)
        {
            _context.ApprovalQtyDifferences.Add(approval);
            _context.SaveChanges();
            return approval;
        }

        public async Task<ApprovalQtyDifference> GetApprovalById(Guid Id)
        {
            var approval = _context.ApprovalQtyDifferences
                .Where(i => i.ApprovalQtyDifferenceId == Id)
                .Include(d => d.PurchaseOrder)
                .Include(a => a.QtyDifference)
                .Include(a1 => a1.UserApprove)
                .Include(u => u.ApplicationUser)
                .FirstOrDefault(p => p.ApprovalQtyDifferenceId == Id);

            if (approval != null)
            {
                var approvalDetail = new ApprovalQtyDifference()
                {
                    ApprovalQtyDifferenceId = approval.ApprovalQtyDifferenceId,
                    QtyDifferenceId = approval.QtyDifferenceId,
                    QtyDifferenceNumber = approval.QtyDifferenceNumber,
                    PurchaseOrderId = approval.PurchaseOrderId,
                    PurchaseOrderNumber = approval.PurchaseOrderNumber,
                    UserAccessId = approval.UserAccessId,
                    UserApproveId = approval.UserApproveId,
                    ApproveBy = approval.ApproveBy,
                    ApprovalTime = approval.ApprovalTime,
                    ApprovalDate = approval.ApprovalDate,
                    ApprovalStatusUser = approval.ApprovalStatusUser,                    
                    Status = approval.Status,
                    Message = approval.Message,
                    Note = approval.Note
                };
                return approvalDetail;
            }
            return approval;
        }

        public async Task<ApprovalQtyDifference> GetApprovalByIdNoTracking(Guid Id)
        {
            return await _context.ApprovalQtyDifferences.AsNoTracking().FirstOrDefaultAsync(a => a.ApprovalQtyDifferenceId == Id);
        }

        public async Task<List<ApprovalQtyDifference>> GetApprovals()
        {
            return await _context.ApprovalQtyDifferences./*OrderBy(p => p.CreateDateTime).*/Select(approval => new ApprovalQtyDifference()
            {
                ApprovalQtyDifferenceId = approval.ApprovalQtyDifferenceId,
                QtyDifferenceId = approval.QtyDifferenceId,
                QtyDifferenceNumber = approval.QtyDifferenceNumber,
                PurchaseOrderId = approval.PurchaseOrderId,
                PurchaseOrderNumber = approval.PurchaseOrderNumber,
                UserAccessId = approval.UserAccessId,
                UserApproveId = approval.UserApproveId,
                ApproveBy = approval.ApproveBy,
                ApprovalTime = approval.ApprovalTime,
                ApprovalDate = approval.ApprovalDate,
                ApprovalStatusUser = approval.ApprovalStatusUser,
                Status = approval.Status,
                Message = approval.Message,
                Note = approval.Note
            }).ToListAsync();
        }

        public IEnumerable<ApprovalQtyDifference> GetAllApproval()
        {
            return _context.ApprovalQtyDifferences.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.PurchaseOrder)
                .Include(a => a.QtyDifference)
                .Include(a1 => a1.UserApprove)
                .Include(u => u.ApplicationUser)
                .ToList();
        }

        public async Task<ApprovalQtyDifference> Update(ApprovalQtyDifference update)
        {
            var approval = _context.ApprovalQtyDifferences.Attach(update);
            approval.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public ApprovalQtyDifference Delete(Guid Id)
        {
            var approval = _context.ApprovalQtyDifferences.Find(Id);
            if (approval != null)
            {
                _context.ApprovalQtyDifferences.Remove(approval);
                _context.SaveChanges();
            }
            return approval;
        }
    }
}
