using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Repositories
{
    public class IApprovalProductReturnRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IApprovalProductReturnRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public ApprovalProductReturn Tambah(ApprovalProductReturn Approval)
        {
            _context.ApprovalProductReturns.Add(Approval);
            _context.SaveChanges();
            return Approval;
        }

        public async Task<ApprovalProductReturn> GetApprovalById(Guid Id)
        {
            var Approval = _context.ApprovalProductReturns
                .Where(i => i.ApprovalProductReturnId == Id)
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .FirstOrDefault(p => p.ApprovalProductReturnId == Id);

            if (Approval != null)
            {
                var ApprovalDetail = new ApprovalProductReturn()
                {
                    CreateDateTime = Approval.CreateDateTime,
                    CreateBy = Approval.CreateBy,
                    UpdateDateTime = Approval.UpdateDateTime,
                    UpdateBy = Approval.UpdateBy,
                    DeleteDateTime = Approval.DeleteDateTime,
                    DeleteBy = Approval.DeleteBy,
                    ApprovalProductReturnId = Approval.ApprovalProductReturnId,
                    ProductReturnId = Approval.ProductReturnId,
                    ProductReturnNumber = Approval.ProductReturnNumber,
                    UserAccessId = Approval.UserAccessId,
                    ApplicationUser = Approval.ApplicationUser,                    
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

        public async Task<ApprovalProductReturn> GetApprovalByIdNoTracking(Guid Id)
        {
            return await _context.ApprovalProductReturns.AsNoTracking().FirstOrDefaultAsync(a => a.ApprovalProductReturnId == Id);
        }

        public async Task<List<ApprovalProductReturn>> GetApprovalProductReturns()
        {
            return await _context.ApprovalProductReturns./*OrderBy(p => p.CreateDateTime).*/Select(Approval => new ApprovalProductReturn()
            {
                CreateDateTime = Approval.CreateDateTime,
                CreateBy = Approval.CreateBy,
                UpdateDateTime = Approval.UpdateDateTime,
                UpdateBy = Approval.UpdateBy,
                DeleteDateTime = Approval.DeleteDateTime,
                DeleteBy = Approval.DeleteBy,
                ApprovalProductReturnId = Approval.ApprovalProductReturnId,
                ProductReturnId = Approval.ProductReturnId,
                ProductReturnNumber = Approval.ProductReturnNumber,
                UserAccessId = Approval.UserAccessId,
                ApplicationUser = Approval.ApplicationUser,
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

        public IEnumerable<ApprovalProductReturn> GetAllApproval()
        {
            return _context.ApprovalProductReturns.OrderByDescending(c => c.CreateDateTime)
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .ToList();
        }

        public IEnumerable<ApprovalProductReturn> GetAllApprovalById(Guid Id)
        {
            return _context.ApprovalProductReturns
                .Where(i => i.UserApproveId == Id)
                .OrderByDescending(c => c.CreateDateTime)
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .ToList();
        }

        public async Task<(IEnumerable<ApprovalProductReturn> ApprovalProductReturns, int totalCountApprovalProductReturns)> GetAllApprovalPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.ApprovalProductReturns
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .OrderByDescending(d => d.CreateDateTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductReturnNumber.Contains(searchTerm));
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
            var ApprovalProductReturns = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (ApprovalProductReturns, totalCount);
        }

        public IEnumerable<ApprovalProductReturn> GetChartBeforeExpired(Guid Id)
        {
            return _context.ApprovalProductReturns
                .Where(i => i.UserApproveId == Id)
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .ToList();
        }

        public IEnumerable<ApprovalProductReturn> GetChartOnExpired(Guid Id)
        {
            return _context.ApprovalProductReturns
                .Where(i => i.UserApproveId == Id)
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .ToList();
        }

        public IEnumerable<ApprovalProductReturn> GetChartMoreThanExpired(Guid Id)
        {
            return _context.ApprovalProductReturns
                .Where(i => i.UserApproveId == Id)
                .Include(u => u.ApplicationUser)
                .Include(t => t.ProductReturn)
                .Include(a1 => a1.UserApprove)
                .ToList();
        }

        public async Task<ApprovalProductReturn> Update(ApprovalProductReturn update)
        {
            var approval = _context.ApprovalProductReturns.Attach(update);
            approval.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public ApprovalProductReturn Delete(Guid Id)
        {
            var Approval = _context.ApprovalProductReturns.Find(Id);
            if (Approval != null)
            {
                _context.ApprovalProductReturns.Remove(Approval);
                _context.SaveChanges();
            }
            return Approval;
        }
    }
}
