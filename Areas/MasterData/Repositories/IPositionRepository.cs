using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IPositionRepository
    {
        private readonly ApplicationDbContext _context;

        public IPositionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Position Tambah(Position Position)
        {
            _context.Positions.Add(Position);
            _context.SaveChanges();
            return Position;
        }

        public Position Update(Position PositionChanges)
        {
            var Position = _context.Positions.Attach(PositionChanges);
            Position.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return PositionChanges;
        }

        public Position Delete(Guid Id)
        {
            var Position = _context.Positions.Find(Id);
            if (Position != null)
            {
                _context.Positions.Remove(Position);
                _context.SaveChanges();
            }
            return Position;
        }

        public IEnumerable<Position> GetAllPosition()
        {
            return _context.Positions.OrderByDescending(m => m.CreateDateTime)
                .Include(d => d.Department)
                .AsNoTracking();
        }

        public async Task<List<Position>> GetPositions()
        {
            return await _context.Positions.OrderBy(p => p.PositionName).Select(x => new Position()
            {
                PositionId = x.PositionId,
                PositionCode = x.PositionCode,
                PositionName = x.PositionName,
                DepartmentId = x.DepartmentId,
            }).ToListAsync();
        }

        public async Task<Position> GetPositionById(Guid Id)
        {
            var Position = await _context.Positions
                .Include (d => d.Department)
                .SingleOrDefaultAsync(i => i.PositionId == Id);

            if (Position != null)
            {
                var PositionDetail = new Position()
                {
                    PositionId = Position.PositionId,
                    PositionCode = Position.PositionCode,
                    PositionName = Position.PositionName,
                    DepartmentId = Position.DepartmentId,
                };
                return PositionDetail;
            }
            return null;
        }

        public async Task<Position> GetPositionByIdNoTracking(Guid Id)
        {
            return await _context.Positions.AsNoTracking().FirstOrDefaultAsync(a => a.PositionId == Id);
        }
    }
}
