using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.Order.Models;
using PurchasingSystemDeveloper.Areas.Warehouse.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.Order.Repositories
{
    public class IQtyDifferenceRequestRepository
    {
        private string _errors = "";
        private readonly ApplicationDbContext _context;

        public IQtyDifferenceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetErrors()
        {
            return _errors;
        }

        public QtyDifferenceRequest Tambah(QtyDifferenceRequest QtyDifferenceRequest)
        {
            _context.QtyDifferenceRequests.Add(QtyDifferenceRequest);
            _context.SaveChanges();
            return QtyDifferenceRequest;
        }

        public async Task<QtyDifferenceRequest> GetQtyDifferenceRequestById(Guid Id)
        {
            var QtyDifferenceRequest = _context.QtyDifferenceRequests
                .Where(i => i.QtyDifferenceRequestId == Id)
                .Include(d => d.PurchaseOrder)
                .Include(a => a.QtyDifference)
                //.Include(r => r.QtyDifferenceRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(s => s.HeadWarehouseManager)
                .Include(f => f.HeadPurchasingManager)
                .FirstOrDefault(p => p.QtyDifferenceRequestId == Id);

            if (QtyDifferenceRequest != null)
            {
                var QtyDifferenceRequestDetail = new QtyDifferenceRequest()
                {
                    QtyDifferenceRequestId = QtyDifferenceRequest.QtyDifferenceRequestId,
                    QtyDifferenceId = QtyDifferenceRequest.QtyDifferenceId,
                    PurchaseOrderId = QtyDifferenceRequest.PurchaseOrderId,
                    PurchaseOrder = QtyDifferenceRequest.PurchaseOrder,
                    UserAccessId = QtyDifferenceRequest.UserAccessId,
                    ApplicationUser = QtyDifferenceRequest.ApplicationUser, 
                    QtyDifferenceApproveDate = QtyDifferenceRequest.QtyDifferenceApproveDate,
                    QtyDifferenceApproveBy = QtyDifferenceRequest.QtyDifferenceApproveBy,                    
                    HeadWarehouseManagerId = QtyDifferenceRequest.HeadWarehouseManagerId,
                    HeadWarehouseManager = QtyDifferenceRequest.HeadWarehouseManager,
                    HeadPurchasingManagerId = QtyDifferenceRequest.HeadPurchasingManagerId,
                    HeadPurchasingManager = QtyDifferenceRequest.HeadPurchasingManager,
                    Status = QtyDifferenceRequest.Status,
                    Note = QtyDifferenceRequest.Note,
                    //QtyDifferenceRequestDetails = QtyDifferenceRequest.QtyDifferenceRequestDetails
                };
                return QtyDifferenceRequestDetail;
            }
            return QtyDifferenceRequest;
        }

        public async Task<QtyDifferenceRequest> GetQtyDifferenceRequestByIdNoTracking(Guid Id)
        {
            return await _context.QtyDifferenceRequests.AsNoTracking().Where(i => i.QtyDifferenceRequestId == Id).FirstOrDefaultAsync(a => a.QtyDifferenceRequestId == Id);
        }

        public async Task<List<QtyDifferenceRequest>> GetQtyDifferenceRequests()
        {
            return await _context.QtyDifferenceRequests.OrderBy(p => p.CreateDateTime).Select(QtyDifferenceRequest => new QtyDifferenceRequest()
            {
                QtyDifferenceRequestId = QtyDifferenceRequest.QtyDifferenceRequestId,
                QtyDifferenceId = QtyDifferenceRequest.QtyDifferenceId,
                PurchaseOrderId = QtyDifferenceRequest.PurchaseOrderId,
                PurchaseOrder = QtyDifferenceRequest.PurchaseOrder,
                UserAccessId = QtyDifferenceRequest.UserAccessId,
                ApplicationUser = QtyDifferenceRequest.ApplicationUser,
                QtyDifferenceApproveDate = QtyDifferenceRequest.QtyDifferenceApproveDate,
                QtyDifferenceApproveBy = QtyDifferenceRequest.QtyDifferenceApproveBy,
                HeadWarehouseManagerId = QtyDifferenceRequest.HeadWarehouseManagerId,
                HeadWarehouseManager = QtyDifferenceRequest.HeadWarehouseManager,
                HeadPurchasingManagerId = QtyDifferenceRequest.HeadPurchasingManagerId,
                HeadPurchasingManager = QtyDifferenceRequest.HeadPurchasingManager,
                Status = QtyDifferenceRequest.Status,
                Note = QtyDifferenceRequest.Note,
                //QtyDifferenceRequestDetails = QtyDifferenceRequest.QtyDifferenceRequestDetails
            }).ToListAsync();
        }

        public IEnumerable<QtyDifferenceRequest> GetAllQtyDifferenceRequest()
        {
            return _context.QtyDifferenceRequests.OrderByDescending(c => c.CreateDateTime)
                .Include(d => d.PurchaseOrder)
                .Include(a => a.QtyDifference)
                //.Include(r => r.QtyDifferenceRequestDetails)
                .Include(u => u.ApplicationUser)
                .Include(s => s.HeadWarehouseManager)
                .Include(f => f.HeadPurchasingManager)
                .ToList();
        }

        public async Task<QtyDifferenceRequest> Update(QtyDifferenceRequest update)
        {
            var QtyDifferenceRequest = _context.QtyDifferenceRequests.Attach(update);
            QtyDifferenceRequest.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }
    }
}
