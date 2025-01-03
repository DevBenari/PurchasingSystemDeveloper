using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Report.Models;
using PurchasingSystemDeveloper.Areas.Transaction.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Report.Repositories
{
    public class IClosingPurchaseOrderRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IClosingPurchaseOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public ClosingPurchaseOrder Tambah(ClosingPurchaseOrder ClosingPurchaseOrder)
        {
            _context.ClosingPurchaseOrders.Add(ClosingPurchaseOrder);
            _context.SaveChanges();
            return ClosingPurchaseOrder;
        }

        public async Task<ClosingPurchaseOrder> GetClosingPurchaseOrderById(Guid Id)
        {
            var ClosingPurchaseOrder = _context.ClosingPurchaseOrders
                .Where(i => i.ClosingPurchaseOrderId == Id)
                .Include(d => d.ClosingPurchaseOrderDetails)
                .Include(u => u.ApplicationUser)                
                .FirstOrDefault(p => p.ClosingPurchaseOrderId == Id);

            if (ClosingPurchaseOrder != null)
            {
                var ClosingPurchaseOrderDetail = new ClosingPurchaseOrder()
                {
                    CreateDateTime = ClosingPurchaseOrder.CreateDateTime,
                    CreateBy = ClosingPurchaseOrder.CreateBy,
                    UpdateDateTime = ClosingPurchaseOrder.UpdateDateTime,
                    UpdateBy = ClosingPurchaseOrder.UpdateBy,
                    DeleteDateTime = ClosingPurchaseOrder.DeleteDateTime,
                    DeleteBy = ClosingPurchaseOrder.DeleteBy,
                    ClosingPurchaseOrderId = ClosingPurchaseOrder.ClosingPurchaseOrderId,
                    ClosingPurchaseOrderNumber = ClosingPurchaseOrder.ClosingPurchaseOrderNumber,
                    UserAccessId = ClosingPurchaseOrder.UserAccessId,                    
                    ApplicationUser = ClosingPurchaseOrder.ApplicationUser,
                    Month = ClosingPurchaseOrder.Month,
                    Year = ClosingPurchaseOrder.Year,
                    TotalPo = ClosingPurchaseOrder.TotalPo,
                    TotalQty = ClosingPurchaseOrder.TotalQty,
                    GrandTotal = ClosingPurchaseOrder.GrandTotal,
                    ClosingPurchaseOrderDetails = ClosingPurchaseOrder.ClosingPurchaseOrderDetails,
                };
                return ClosingPurchaseOrderDetail;
            }
            return ClosingPurchaseOrder;
        }

        public async Task<ClosingPurchaseOrder> GetClosingPurchaseOrderByIdNoTracking(Guid Id)
        {
            return await _context.ClosingPurchaseOrders.AsNoTracking().Where(i => i.ClosingPurchaseOrderId == Id).FirstOrDefaultAsync(a => a.ClosingPurchaseOrderId == Id);
        }

        public async Task<List<ClosingPurchaseOrder>> GetClosingPurchaseOrders()
        {
            return await _context.ClosingPurchaseOrders./*OrderBy(p => p.CreateDateTime).*/Select(ClosingPurchaseOrder => new ClosingPurchaseOrder()
            {
                CreateDateTime = ClosingPurchaseOrder.CreateDateTime,
                CreateBy = ClosingPurchaseOrder.CreateBy,
                UpdateDateTime = ClosingPurchaseOrder.UpdateDateTime,
                UpdateBy = ClosingPurchaseOrder.UpdateBy,
                DeleteDateTime = ClosingPurchaseOrder.DeleteDateTime,
                DeleteBy = ClosingPurchaseOrder.DeleteBy,
                ClosingPurchaseOrderId = ClosingPurchaseOrder.ClosingPurchaseOrderId,
                ClosingPurchaseOrderNumber = ClosingPurchaseOrder.ClosingPurchaseOrderNumber,
                UserAccessId = ClosingPurchaseOrder.UserAccessId,
                ApplicationUser = ClosingPurchaseOrder.ApplicationUser,
                Month = ClosingPurchaseOrder.Month,
                Year = ClosingPurchaseOrder.Year,
                TotalPo = ClosingPurchaseOrder.TotalPo,
                TotalQty = ClosingPurchaseOrder.TotalQty,
                GrandTotal = ClosingPurchaseOrder.GrandTotal,
                ClosingPurchaseOrderDetails = ClosingPurchaseOrder.ClosingPurchaseOrderDetails,
            }).ToListAsync();
        }

        public IEnumerable<ClosingPurchaseOrder> GetAllClosingPurchaseOrder()
        {
            return _context.ClosingPurchaseOrders.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.ClosingPurchaseOrderDetails)
                .Include(u => u.ApplicationUser)                
                .ToList();
        }

        public IEnumerable<ClosingPurchaseOrder> GetAllClosingPurchaseOrderAsc()
        {
            return _context.ClosingPurchaseOrders
                .Include(d => d.ClosingPurchaseOrderDetails)
                .Include(u => u.ApplicationUser)                
                .ToList();
        }

        public async Task<(IEnumerable<ClosingPurchaseOrder> ClosingPurchaseOrders, int totalCountClosingPurchaseOrders)> GetAllClosingPurchaseOrderPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.ClosingPurchaseOrders
                .Include(d => d.ClosingPurchaseOrderDetails)
                .Include(u => u.ApplicationUser)                
                .OrderByDescending(d => d.CreateDateTime)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.ClosingPurchaseOrderNumber.Contains(searchTerm));
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
            var ClosingPurchaseOrders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (ClosingPurchaseOrders, totalCount);
        }

        public async Task<ClosingPurchaseOrder> Update(ClosingPurchaseOrder update)
        {
            List<ClosingPurchaseOrderDetail> ClosingPurchaseOrderDetails = _context.ClosingPurchaseOrderDetails.Where(d => d.ClosingPurchaseOrderId == update.ClosingPurchaseOrderId).ToList();
            _context.ClosingPurchaseOrderDetails.RemoveRange(ClosingPurchaseOrderDetails);
            _context.SaveChanges();

            var ClosingPurchaseOrder = _context.ClosingPurchaseOrders.Attach(update);
            ClosingPurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.ClosingPurchaseOrderDetails.AddRangeAsync(update.ClosingPurchaseOrderDetails);
            _context.SaveChanges();
            return update;
        }

        public ClosingPurchaseOrder Delete(Guid Id)
        {
            var ClosingPurchaseOrder = _context.ClosingPurchaseOrders.Find(Id);
            if (ClosingPurchaseOrder != null)
            {
                _context.ClosingPurchaseOrders.Remove(ClosingPurchaseOrder);
                _context.SaveChanges();
            }
            return ClosingPurchaseOrder;
        }
    }
}
