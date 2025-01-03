using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Repositories
{
    public class IProductReturnRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IProductReturnRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public ProductReturn Tambah(ProductReturn ProductReturn)
        {
            _context.ProductReturns.Add(ProductReturn);
            _context.SaveChanges();
            return ProductReturn;
        }

        public async Task<ProductReturn> GetProductReturnById(Guid Id)
        {
            var ProductReturn = _context.ProductReturns
                .Where(i => i.ProductReturnId == Id)
                .Include(d => d.ProductReturnDetails)
                .Include(u => u.ApplicationUser)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                .FirstOrDefault(p => p.ProductReturnId == Id);

            if (ProductReturn != null)
            {
                var ProductReturnDetail = new ProductReturn()
                {
                    CreateDateTime = ProductReturn.CreateDateTime,
                    CreateBy = ProductReturn.CreateBy,
                    UpdateDateTime = ProductReturn.UpdateDateTime,
                    UpdateBy = ProductReturn.UpdateBy,
                    DeleteDateTime = ProductReturn.DeleteDateTime,
                    DeleteBy = ProductReturn.DeleteBy,
                    ProductReturnId = ProductReturn.ProductReturnId,
                    ReturnDate = ProductReturn.ReturnDate,
                    ProductReturnNumber = ProductReturn.ProductReturnNumber,
                    UserAccessId = ProductReturn.UserAccessId,
                    ApplicationUser = ProductReturn.ApplicationUser,
                    PurchaseOrderNumber = ProductReturn.PurchaseOrderNumber,                    
                    Department1Id = ProductReturn.Department1Id,
                    Department1 = ProductReturn.Department1,
                    Position1Id = ProductReturn.Position1Id,
                    Position1 = ProductReturn.Position1,
                    UserApprove1Id = ProductReturn.UserApprove1Id,
                    UserApprove1 = ProductReturn.UserApprove1,
                    ApproveStatusUser1 = ProductReturn.ApproveStatusUser1,
                    Department2Id = ProductReturn.Department2Id,
                    Department2 = ProductReturn.Department2,
                    Position2Id = ProductReturn.Position2Id,
                    Position2 = ProductReturn.Position2,
                    UserApprove2Id = ProductReturn.UserApprove2Id,
                    UserApprove2 = ProductReturn.UserApprove2,
                    ApproveStatusUser2 = ProductReturn.ApproveStatusUser2,
                    Department3Id = ProductReturn.Department3Id,
                    Department3 = ProductReturn.Department3,
                    Position3Id = ProductReturn.Position3Id,
                    Position3 = ProductReturn.Position3,
                    UserApprove3Id = ProductReturn.UserApprove3Id,
                    UserApprove3 = ProductReturn.UserApprove3,
                    ApproveStatusUser3 = ProductReturn.ApproveStatusUser3,
                    Status = ProductReturn.Status,
                    ReasonForReturn = ProductReturn.ReasonForReturn,
                    Note = ProductReturn.Note,
                    MessageApprove1 = ProductReturn.MessageApprove1,
                    MessageApprove2 = ProductReturn.MessageApprove2,
                    MessageApprove3 = ProductReturn.MessageApprove3,
                    ProductReturnDetails = ProductReturn.ProductReturnDetails
                };
                return ProductReturnDetail;
            }
            return ProductReturn;
        }

        public async Task<ProductReturn> GetProductReturnByIdNoTracking(Guid Id)
        {
            return await _context.ProductReturns.AsNoTracking()
                .Where(i => i.ProductReturnId == Id)
                .Include(d => d.ProductReturnDetails)
                .Include(u => u.ApplicationUser)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                .FirstOrDefaultAsync(a => a.ProductReturnId == Id);
        }

        public async Task<List<ProductReturn>> GetProductReturns()
        {
            return await _context.ProductReturns./*OrderBy(p => p.CreateDateTime).*/Select(ProductReturn => new ProductReturn()
            {
                CreateDateTime = ProductReturn.CreateDateTime,
                CreateBy = ProductReturn.CreateBy,
                UpdateDateTime = ProductReturn.UpdateDateTime,
                UpdateBy = ProductReturn.UpdateBy,
                DeleteDateTime = ProductReturn.DeleteDateTime,
                DeleteBy = ProductReturn.DeleteBy,
                ProductReturnId = ProductReturn.ProductReturnId,
                ReturnDate = ProductReturn.ReturnDate,
                ProductReturnNumber = ProductReturn.ProductReturnNumber,
                UserAccessId = ProductReturn.UserAccessId,
                ApplicationUser = ProductReturn.ApplicationUser,
                PurchaseOrderNumber = ProductReturn.PurchaseOrderNumber,
                Department1Id = ProductReturn.Department1Id,
                Department1 = ProductReturn.Department1,
                Position1Id = ProductReturn.Position1Id,
                Position1 = ProductReturn.Position1,
                UserApprove1Id = ProductReturn.UserApprove1Id,
                UserApprove1 = ProductReturn.UserApprove1,
                ApproveStatusUser1 = ProductReturn.ApproveStatusUser1,
                Department2Id = ProductReturn.Department2Id,
                Department2 = ProductReturn.Department2,
                Position2Id = ProductReturn.Position2Id,
                Position2 = ProductReturn.Position2,
                UserApprove2Id = ProductReturn.UserApprove2Id,
                UserApprove2 = ProductReturn.UserApprove2,
                ApproveStatusUser2 = ProductReturn.ApproveStatusUser2,
                Department3Id = ProductReturn.Department3Id,
                Department3 = ProductReturn.Department3,
                Position3Id = ProductReturn.Position3Id,
                Position3 = ProductReturn.Position3,
                UserApprove3Id = ProductReturn.UserApprove3Id,
                UserApprove3 = ProductReturn.UserApprove3,
                ApproveStatusUser3 = ProductReturn.ApproveStatusUser3,
                Status = ProductReturn.Status,
                ReasonForReturn = ProductReturn.ReasonForReturn,
                Note = ProductReturn.Note,
                MessageApprove1 = ProductReturn.MessageApprove1,
                MessageApprove2 = ProductReturn.MessageApprove2,
                MessageApprove3 = ProductReturn.MessageApprove3,
                ProductReturnDetails = ProductReturn.ProductReturnDetails
            }).ToListAsync();
        }

        public IEnumerable<ProductReturn> GetAllProductReturn()
        {
            return _context.ProductReturns.OrderByDescending(o => o.CreateDateTime)
                .Include(d => d.ProductReturnDetails)
                .Include(u => u.ApplicationUser)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                .ToList();
        }

        public async Task<(IEnumerable<ProductReturn> ProductReturns, int totalCountProductReturns)> GetAllProductReturnPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.ProductReturns
                .Include(d => d.ProductReturnDetails)
                .Include(u => u.ApplicationUser)
                .Include(d1 => d1.Department1)
                .Include(p1 => p1.Position1)
                .Include(a1 => a1.UserApprove1)
                .Include(d2 => d2.Department2)
                .Include(p2 => p2.Position2)
                .Include(a2 => a2.UserApprove2)
                .Include(d3 => d3.Department3)
                .Include(p3 => p3.Position3)
                .Include(a3 => a3.UserApprove3)
                .OrderByDescending(d => d.CreateDateTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ProductReturnNumber.Contains(searchTerm) || p.UserApprove1.FullName.Contains(searchTerm) || p.UserApprove2.FullName.Contains(searchTerm) || p.UserApprove3.FullName.Contains(searchTerm));
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
            var ProductReturns = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (ProductReturns, totalCount);
        }

        public async Task<ProductReturn> Update(ProductReturn update)
        {
            List<ProductReturnDetail> ProductReturnDetails = _context.ProductReturnDetails.Where(d => d.ProductReturnId == update.ProductReturnId).ToList();
            _context.ProductReturnDetails.RemoveRange(ProductReturnDetails);
            _context.SaveChanges();

            var ProductReturn = _context.ProductReturns.Attach(update);
            ProductReturn.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.ProductReturnDetails.AddRangeAsync(update.ProductReturnDetails);
            _context.SaveChanges();
            return update;
        }

        public ProductReturn Delete(Guid Id)
        {
            var ProductReturn = _context.ProductReturns.Find(Id);
            if (ProductReturn != null)
            {
                _context.ProductReturns.Remove(ProductReturn);
                _context.SaveChanges();
            }
            return ProductReturn;
        }
    }
}
