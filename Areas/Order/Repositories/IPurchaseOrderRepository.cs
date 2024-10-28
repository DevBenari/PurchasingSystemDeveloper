using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Order.Repositories
{
    public class IPurchaseOrderRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IPurchaseOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public PurchaseOrder Tambah(PurchaseOrder PurchaseOrder)
        {
            _context.PurchaseOrders.Add(PurchaseOrder);
            _context.SaveChanges();
            return PurchaseOrder;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderById(Guid Id)
        {
            var PurchaseOrder = _context.PurchaseOrders
                .Where(i => i.PurchaseOrderId == Id)
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(a1 => a1.UserApprove1)
                .Include(a2 => a2.UserApprove2)
                .Include(a3 => a3.UserApprove3)
                //.Include(e => e.DueDate)
                .FirstOrDefault(p => p.PurchaseOrderId == Id);

            if (PurchaseOrder != null)
            {
                var PurchaseOrderDetail = new PurchaseOrder()
                {
                    CreateDateTime = PurchaseOrder.CreateDateTime,
                    CreateBy = PurchaseOrder.CreateBy,
                    UpdateDateTime = PurchaseOrder.UpdateDateTime,
                    UpdateBy = PurchaseOrder.UpdateBy,
                    DeleteDateTime = PurchaseOrder.DeleteDateTime,
                    DeleteBy = PurchaseOrder.DeleteBy,
                    PurchaseOrderId = PurchaseOrder.PurchaseOrderId,
                    PurchaseOrderNumber = PurchaseOrder.PurchaseOrderNumber,
                    PurchaseRequestId = PurchaseOrder.PurchaseRequestId,
                    PurchaseRequestNumber = PurchaseOrder.PurchaseRequestNumber,
                    UserAccessId = PurchaseOrder.UserAccessId,
                    ExpiredDate = PurchaseOrder.ExpiredDate,
                    ApplicationUser = PurchaseOrder.ApplicationUser,
                    UserApprove1Id = PurchaseOrder.UserApprove1Id,
                    UserApprove1 = PurchaseOrder.UserApprove1,
                    UserApprove2Id = PurchaseOrder.UserApprove2Id,
                    UserApprove2 = PurchaseOrder.UserApprove2,
                    UserApprove3Id = PurchaseOrder.UserApprove3Id,
                    UserApprove3 = PurchaseOrder.UserApprove3,
                    ApproveStatusUser1 = PurchaseOrder.ApproveStatusUser1,
                    ApproveStatusUser2 = PurchaseOrder.ApproveStatusUser2,
                    ApproveStatusUser3 = PurchaseOrder.ApproveStatusUser3,
                    TermOfPaymentId = PurchaseOrder.TermOfPaymentId,
                    TermOfPayment = PurchaseOrder.TermOfPayment,
                    Status = PurchaseOrder.Status,
                    QtyTotal = PurchaseOrder.QtyTotal,
                    GrandTotal = PurchaseOrder.GrandTotal,
                    //DueDateId = PurchaseOrder.DueDateId,
                    //DueDate = PurchaseOrder.DueDate,
                    Note = PurchaseOrder.Note,
                    PurchaseOrderDetails = PurchaseOrder.PurchaseOrderDetails,
                };
                return PurchaseOrderDetail;
            }
            return PurchaseOrder;
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdNoTracking(Guid Id)
        {
            return await _context.PurchaseOrders.AsNoTracking()
                .Where(i => i.PurchaseOrderId == Id)
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(a1 => a1.UserApprove1)
                .Include(a2 => a2.UserApprove2)
                .Include(a3 => a3.UserApprove3)
                //.Include(e => e.DueDate)
                .FirstOrDefaultAsync(a => a.PurchaseOrderId == Id);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrders()
        {
            return await _context.PurchaseOrders.OrderBy(p => p.CreateDateTime).Select(PurchaseOrder => new PurchaseOrder()
            {
                CreateDateTime = PurchaseOrder.CreateDateTime,
                CreateBy = PurchaseOrder.CreateBy,
                UpdateDateTime = PurchaseOrder.UpdateDateTime,
                UpdateBy = PurchaseOrder.UpdateBy,
                DeleteDateTime = PurchaseOrder.DeleteDateTime,
                DeleteBy = PurchaseOrder.DeleteBy,
                PurchaseOrderId = PurchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = PurchaseOrder.PurchaseOrderNumber,
                PurchaseRequestId = PurchaseOrder.PurchaseRequestId,
                PurchaseRequestNumber = PurchaseOrder.PurchaseRequestNumber,
                UserAccessId = PurchaseOrder.UserAccessId,
                ExpiredDate = PurchaseOrder.ExpiredDate,
                ApplicationUser = PurchaseOrder.ApplicationUser,
                UserApprove1Id = PurchaseOrder.UserApprove1Id,
                UserApprove1 = PurchaseOrder.UserApprove1,
                UserApprove2Id = PurchaseOrder.UserApprove2Id,
                UserApprove2 = PurchaseOrder.UserApprove2,
                UserApprove3Id = PurchaseOrder.UserApprove3Id,
                UserApprove3 = PurchaseOrder.UserApprove3,
                ApproveStatusUser1 = PurchaseOrder.ApproveStatusUser1,
                ApproveStatusUser2 = PurchaseOrder.ApproveStatusUser2,
                ApproveStatusUser3 = PurchaseOrder.ApproveStatusUser3,
                TermOfPaymentId = PurchaseOrder.TermOfPaymentId,
                TermOfPayment = PurchaseOrder.TermOfPayment,
                Status = PurchaseOrder.Status,
                QtyTotal = PurchaseOrder.QtyTotal,
                GrandTotal = PurchaseOrder.GrandTotal,
                //DueDateId = PurchaseOrder.DueDateId,
                //DueDate = PurchaseOrder.DueDate,
                Note = PurchaseOrder.Note,
                PurchaseOrderDetails = PurchaseOrder.PurchaseOrderDetails,
            }).ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersFilters()
        {
            return await _context.PurchaseOrders.Where(p => p.Status == "In Order").OrderBy(p => p.CreateDateTime).Select(PurchaseOrder => new PurchaseOrder()
            {
                CreateDateTime = PurchaseOrder.CreateDateTime,
                CreateBy = PurchaseOrder.CreateBy,
                UpdateDateTime = PurchaseOrder.UpdateDateTime,
                UpdateBy = PurchaseOrder.UpdateBy,
                DeleteDateTime = PurchaseOrder.DeleteDateTime,
                DeleteBy = PurchaseOrder.DeleteBy,
                PurchaseOrderId = PurchaseOrder.PurchaseOrderId,
                PurchaseOrderNumber = PurchaseOrder.PurchaseOrderNumber,
                PurchaseRequestId = PurchaseOrder.PurchaseRequestId,
                PurchaseRequestNumber = PurchaseOrder.PurchaseRequestNumber,
                UserAccessId = PurchaseOrder.UserAccessId,
                ExpiredDate = PurchaseOrder.ExpiredDate,
                ApplicationUser = PurchaseOrder.ApplicationUser,
                UserApprove1Id = PurchaseOrder.UserApprove1Id,
                UserApprove1 = PurchaseOrder.UserApprove1,
                UserApprove2Id = PurchaseOrder.UserApprove2Id,
                UserApprove2 = PurchaseOrder.UserApprove2,
                UserApprove3Id = PurchaseOrder.UserApprove3Id,
                UserApprove3 = PurchaseOrder.UserApprove3,
                ApproveStatusUser1 = PurchaseOrder.ApproveStatusUser1,
                ApproveStatusUser2 = PurchaseOrder.ApproveStatusUser2,
                ApproveStatusUser3 = PurchaseOrder.ApproveStatusUser3,
                TermOfPaymentId = PurchaseOrder.TermOfPaymentId,
                TermOfPayment = PurchaseOrder.TermOfPayment,
                Status = PurchaseOrder.Status,
                QtyTotal = PurchaseOrder.QtyTotal,
                GrandTotal = PurchaseOrder.GrandTotal,
                //DueDateId = PurchaseOrder.DueDateId,
                //DueDate = PurchaseOrder.DueDate,
                Note = PurchaseOrder.Note,
                PurchaseOrderDetails = PurchaseOrder.PurchaseOrderDetails,
            }).ToListAsync();
        }

        public IEnumerable<PurchaseOrder> GetAllPurchaseOrder()
        {
            return _context.PurchaseOrders.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.PurchaseOrderDetails)
                .Include(u => u.ApplicationUser)
                .Include(t => t.TermOfPayment)
                .Include(a1 => a1.UserApprove1)
                .Include(a2 => a2.UserApprove2)
                .Include(a3 => a3.UserApprove3)
                //.Include(e => e.DueDate)
                .ToList();
        }

        public async Task<PurchaseOrder> Update(PurchaseOrder update)
        {
            List<PurchaseOrderDetail> PurchaseOrderDetails = _context.PurchaseOrderDetails.Where(d => d.PurchaseOrderId == update.PurchaseOrderId).ToList();
            _context.PurchaseOrderDetails.RemoveRange(PurchaseOrderDetails);
            _context.SaveChanges();

            var PurchaseOrder = _context.PurchaseOrders.Attach(update);
            PurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.PurchaseOrderDetails.AddRangeAsync(update.PurchaseOrderDetails);
            _context.SaveChanges();
            return update;
        }

        public PurchaseOrder Delete(Guid Id)
        {
            var PurchaseOrder = _context.PurchaseOrders.Find(Id);
            if (PurchaseOrder != null)
            {
                _context.PurchaseOrders.Remove(PurchaseOrder);
                _context.SaveChanges();
            }
            return PurchaseOrder;
        }
    }
}
