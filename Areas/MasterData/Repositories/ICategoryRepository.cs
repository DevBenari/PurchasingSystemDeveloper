using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Category Tambah(Category Category)
        {
            _context.Categories.Add(Category);
            _context.SaveChanges();
            return Category;
        }

        public async Task<Category> GetCategoryById(Guid Id)
        {
            var Category = await _context.Categories
                .SingleOrDefaultAsync(i => i.CategoryId == Id);

            if (Category != null)
            {
                var CategoryDetail = new Category()
                {
                    CategoryId = Category.CategoryId,
                    CategoryCode = Category.CategoryCode,
                    CategoryName = Category.CategoryName,
                    Note = Category.Note
                };
                return CategoryDetail;
            }
            return null;
        }

        public async Task<Category> GetCategoryByIdNoTracking(Guid Id)
        {
            return await _context.Categories.AsNoTracking().FirstOrDefaultAsync(a => a.CategoryId == Id);
        }

        public async Task<List<Category>> GetCategories()
        {
            return await _context.Categories.OrderBy(p => p.CreateDateTime).Select(Category => new Category()
            {
                CategoryId = Category.CategoryId,
                CategoryCode = Category.CategoryCode,
                CategoryName = Category.CategoryName,
                Note = Category.Note
            }).ToListAsync();
        }

        public IEnumerable<Category> GetAllCategory()
        {
            return _context.Categories.OrderByDescending(d => d.CreateDateTime)
                .AsNoTracking();
        }

        public Category Update(Category update)
        {
            var Category = _context.Categories.Attach(update);
            Category.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Category Delete(Guid Id)
        {
            var Category = _context.Categories.Find(Id);
            if (Category != null)
            {
                _context.Categories.Remove(Category);
                _context.SaveChanges();
            }
            return Category;
        }
    }
}
