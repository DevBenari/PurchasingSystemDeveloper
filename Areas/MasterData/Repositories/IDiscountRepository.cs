using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public IDiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Discount Tambah(Discount Discount)
        {
            _context.Discounts.Add(Discount);
            _context.SaveChanges();
            return Discount;
        }

        public async Task<Discount> GetDiscountById(Guid Id)
        {
            var Discount = await _context.Discounts
                .SingleOrDefaultAsync(i => i.DiscountId == Id);

            if (Discount != null)
            {
                var DiscountDetail = new Discount()
                {
                    DiscountId = Discount.DiscountId,
                    DiscountCode = Discount.DiscountCode,
                    DiscountValue = Discount.DiscountValue,
                    Note = Discount.Note,
                };
                return DiscountDetail;
            }
            return null;
        }

        public async Task<Discount> GetDiscountByIdNoTracking(Guid Id)
        {
            return await _context.Discounts.AsNoTracking().FirstOrDefaultAsync(a => a.DiscountId == Id);
        }

        public async Task<List<Discount>> GetDiscounts()
        {
            return await _context.Discounts.OrderBy(p => p.CreateDateTime).Select(Discount => new Discount()
            {
                DiscountId = Discount.DiscountId,
                DiscountCode = Discount.DiscountCode,
                DiscountValue = Discount.DiscountValue,
                Note = Discount.Note
            }).ToListAsync();
        }

        public IEnumerable<Discount> GetAllDiscount()
        {
            return _context.Discounts.OrderByDescending(d => d.CreateDateTime)
                .AsNoTracking();
        }

        public async Task<(IEnumerable<Discount> discounts, int totalCountDiscounts)> GetAllDiscountPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.Discounts
                .OrderByDescending(d => d.CreateDateTime)                
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.DiscountCode.Contains(searchTerm) || Convert.ToString(p.DiscountValue).Contains(searchTerm));
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
            var discounts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (discounts, totalCount);
        }

        public Discount Update(Discount update)
        {
            var Discount = _context.Discounts.Attach(update);
            Discount.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Discount Delete(Guid Id)
        {
            var Discount = _context.Discounts.Find(Id);
            if (Discount != null)
            {
                _context.Discounts.Remove(Discount);
                _context.SaveChanges();
            }
            return Discount;
        }
    }
}
