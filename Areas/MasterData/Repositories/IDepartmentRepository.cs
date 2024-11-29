using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public IDepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Department Tambah(Department Department)
        {
            _context.Departments.Add(Department);
            _context.SaveChanges();
            return Department;
        }

        public Department Update(Department DepartmentChanges)
        {
            var Department = _context.Departments.Attach(DepartmentChanges);
            Department.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return DepartmentChanges;
        }

        public Department Delete(Guid Id)
        {
            var Department = _context.Departments.Find(Id);
            if (Department != null)
            {
                _context.Departments.Remove(Department);
                _context.SaveChanges();
            }
            return Department;
        }

        public IEnumerable<Department> GetAllDepartment()
        {
            return _context.Departments.OrderByDescending(m => m.CreateDateTime)
                .Include(p => p.Positions)
                .AsNoTracking();
        }

        public async Task<(IEnumerable<Department> departments, int totalCountDepartments)> GetAllDepartmentPageSize(string searchTerm, int page, int pageSize, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var query = _context.Departments
                .OrderByDescending(d => d.CreateDateTime)
                .Include(p => p.Positions)
                .AsQueryable();

            // Filter berdasarkan searchTerm jika ada
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.DepartmentCode.Contains(searchTerm) || p.DepartmentName.Contains(searchTerm));
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
            var departments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (departments, totalCount);
        }

        public async Task<List<Department>> GetDepartments()
        {
            return await _context.Departments.OrderBy(p => p.DepartmentName).Select(Department => new Department()
            {
                DepartmentId = Department.DepartmentId,
                DepartmentCode = Department.DepartmentCode,
                DepartmentName = Department.DepartmentName
            }).ToListAsync();
        }

        public async Task<Department> GetDepartmentById(Guid Id)
        {
            var Department = await _context.Departments
                .SingleOrDefaultAsync(i => i.DepartmentId == Id);

            if (Department != null)
            {
                var DepartmentDetail = new Department()
                {
                    DepartmentId = Department.DepartmentId,
                    DepartmentCode = Department.DepartmentCode,
                    DepartmentName = Department.DepartmentName,                    
                };
                return DepartmentDetail;
            }
            return null;
        }

        public async Task<Department> GetDepartmentByIdNoTracking(Guid Id)
        {
            return await _context.Departments.AsNoTracking().FirstOrDefaultAsync(a => a.DepartmentId == Id);
        }
    }
}
