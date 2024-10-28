using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IInitialStockRepository
    {
        private readonly ApplicationDbContext _context;

        public IInitialStockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public InitialStock Tambah(InitialStock InitialStock)
        {
            _context.InitialStocks.Add(InitialStock);
            _context.SaveChanges();
            return InitialStock;
        }

        public async Task<InitialStock> GetInitialStockById(Guid Id)
        {
            var InitialStock = await _context.InitialStocks
                .Include(p => p.Supplier)
                .Include(c => c.Product)
                .Include(m => m.LeadTime)
                .SingleOrDefaultAsync(i => i.InitialStockId == Id);

            if (InitialStock != null)
            {
                var InitialStockDetail = new InitialStock()
                {
                    InitialStockId = InitialStock.InitialStockId,
                    GenerateBy = InitialStock.GenerateBy,
                    ProductId = InitialStock.ProductId,
                    ProductName = InitialStock.ProductName,
                    SupplierId = InitialStock.SupplierId,
                    SupplierName = InitialStock.SupplierName,
                    LeadTimeId = InitialStock.LeadTimeId,
                    CalculateBaseOn = InitialStock.CalculateBaseOn,
                    MaxRequest = InitialStock.MaxRequest,
                    AverageRequest = InitialStock.AverageRequest
                };
                return InitialStockDetail;
            }
            return null;
        }

        public async Task<InitialStock> GetInitialStockByIdNoTracking(Guid Id)
        {
            return await _context.InitialStocks.AsNoTracking().FirstOrDefaultAsync(a => a.InitialStockId == Id);
        }

        public async Task<List<InitialStock>> GetInitialStocks()
        {
            return await _context.InitialStocks.OrderBy(p => p.CreateDateTime).Select(InitialStock => new InitialStock()
            {
                InitialStockId = InitialStock.InitialStockId,
                GenerateBy = InitialStock.GenerateBy,
                ProductId = InitialStock.ProductId,
                ProductName = InitialStock.ProductName,
                SupplierId = InitialStock.SupplierId,
                SupplierName = InitialStock.SupplierName,
                LeadTimeId = InitialStock.LeadTimeId,
                CalculateBaseOn = InitialStock.CalculateBaseOn,
                MaxRequest = InitialStock.MaxRequest,
                AverageRequest = InitialStock.AverageRequest
            }).ToListAsync();
        }

        public IEnumerable<InitialStock> GetAllInitialStock()
        {
            return _context.InitialStocks.OrderByDescending(d => d.CreateDateTime)
                .Include(p => p.Supplier)
                .Include(c => c.Product)
                .Include(m => m.LeadTime)
                .AsNoTracking();
        }

        public InitialStock Update(InitialStock update)
        {
            var InitialStock = _context.InitialStocks.Attach(update);
            InitialStock.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public InitialStock Delete(Guid Id)
        {
            var InitialStock = _context.InitialStocks.Find(Id);
            if (InitialStock != null)
            {
                _context.InitialStocks.Remove(InitialStock);
                _context.SaveChanges();
            }
            return InitialStock;
        }
    }
}
