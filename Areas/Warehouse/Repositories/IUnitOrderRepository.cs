using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Warehouse.Repositories
{
    public class IUnitOrderRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IUnitOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public UnitOrder Tambah(UnitOrder UnitOrder)
        {
            _context.UnitOrders.Add(UnitOrder);
            _context.SaveChanges();
            return UnitOrder;
        }

        public async Task<UnitOrder> GetUnitOrderById(Guid Id)
        {
            var UnitOrder = _context.UnitOrders
                .Where(i => i.UnitOrderId == Id)
                .Include(d => d.UnitOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .FirstOrDefault(p => p.UnitOrderId == Id);

            if (UnitOrder != null)
            {
                var UnitOrderDetail = new UnitOrder()
                {
                    UnitOrderId = UnitOrder.UnitOrderId,
                    UnitOrderNumber = UnitOrder.UnitOrderNumber,
                    UnitRequestId = UnitOrder.UnitRequestId,
                    UnitRequestNumber = UnitOrder.UnitRequestNumber,
                    UserAccessId = UnitOrder.UserAccessId,
                    ApplicationUser = UnitOrder.ApplicationUser,
                    UnitLocationId = UnitOrder.UnitLocationId,
                    UnitLocation = UnitOrder.UnitLocation,
                    WarehouseLocationId = UnitOrder.WarehouseLocationId,
                    WarehouseLocation = UnitOrder.WarehouseLocation,
                    UserApprove1Id = UnitOrder.UserApprove1Id,
                    UserApprove1 = UnitOrder.UserApprove1,
                    ApproveStatusUser1 = UnitOrder.ApproveStatusUser1,
                    Status = UnitOrder.Status,
                    QtyTotal = UnitOrder.QtyTotal,
                    Note = UnitOrder.Note,
                    UnitOrderDetails = UnitOrder.UnitOrderDetails
                };
                return UnitOrderDetail;
            }
            return UnitOrder;
        }

        public async Task<UnitOrder> GetUnitOrderByIdNoTracking(Guid Id)
        {
            return await _context.UnitOrders.AsNoTracking()
                .Include(d => d.UnitOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .Where(i => i.UnitOrderId == Id).FirstOrDefaultAsync(a => a.UnitOrderId == Id);
        }

        public async Task<List<UnitOrder>> GetUnitOrders()
        {
            return await _context.UnitOrders.Where(s => s.Status != "Selesai").OrderBy(p => p.CreateDateTime).Select(UnitOrder => new UnitOrder()
            {
                UnitOrderId = UnitOrder.UnitOrderId,
                UnitOrderNumber = UnitOrder.UnitOrderNumber,
                UnitRequestId = UnitOrder.UnitRequestId,
                UnitRequestNumber = UnitOrder.UnitRequestNumber,
                UserAccessId = UnitOrder.UserAccessId,
                ApplicationUser = UnitOrder.ApplicationUser,
                UnitLocationId = UnitOrder.UnitLocationId,
                UnitLocation = UnitOrder.UnitLocation,
                WarehouseLocationId = UnitOrder.WarehouseLocationId,
                WarehouseLocation = UnitOrder.WarehouseLocation,
                UserApprove1Id = UnitOrder.UserApprove1Id,
                UserApprove1 = UnitOrder.UserApprove1,
                ApproveStatusUser1 = UnitOrder.ApproveStatusUser1,
                Status = UnitOrder.Status,
                QtyTotal = UnitOrder.QtyTotal,
                Note = UnitOrder.Note,
                UnitOrderDetails = UnitOrder.UnitOrderDetails
            }).ToListAsync();
        }

        public async Task<List<UnitOrder>> GetUnitOrderDetails()
        {
            return await _context.UnitOrders.OrderBy(p => p.CreateDateTime).Select(UnitOrder => new UnitOrder()
            {
                UnitOrderId = UnitOrder.UnitOrderId,
                UnitOrderNumber = UnitOrder.UnitOrderNumber,
                UnitRequestId = UnitOrder.UnitRequestId,
                UnitRequestNumber = UnitOrder.UnitRequestNumber,
                UserAccessId = UnitOrder.UserAccessId,
                ApplicationUser = UnitOrder.ApplicationUser,
                UnitLocationId = UnitOrder.UnitLocationId,
                UnitLocation = UnitOrder.UnitLocation,
                WarehouseLocationId = UnitOrder.WarehouseLocationId,
                WarehouseLocation = UnitOrder.WarehouseLocation,
                UserApprove1Id = UnitOrder.UserApprove1Id,
                UserApprove1 = UnitOrder.UserApprove1,
                ApproveStatusUser1 = UnitOrder.ApproveStatusUser1,
                Status = UnitOrder.Status,
                QtyTotal = UnitOrder.QtyTotal,
                Note = UnitOrder.Note,
                UnitOrderDetails = UnitOrder.UnitOrderDetails
            }).ToListAsync();
        }

        public IEnumerable<UnitOrder> GetAllUnitOrder()
        {
            return _context.UnitOrders.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.UnitOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(p => p.UnitLocation)
                .Include(t => t.WarehouseLocation)
                .Include(y => y.UserApprove1)
                .ToList();
        }

        public async Task<UnitOrder> Update(UnitOrder update)
        {
            var UnitOrder = _context.UnitOrders.Attach(update);
            UnitOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }
    }
}
