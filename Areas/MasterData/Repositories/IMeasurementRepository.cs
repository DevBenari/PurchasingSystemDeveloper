using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IMeasurementRepository
    {
        private readonly ApplicationDbContext _context;

        public IMeasurementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Measurement Tambah(Measurement Measurement)
        {
            _context.Measurements.Add(Measurement);
            _context.SaveChanges();
            return Measurement;
        }

        public async Task<Measurement> GetMeasurementById(Guid Id)
        {
            var Measurement = await _context.Measurements
                .SingleOrDefaultAsync(i => i.MeasurementId == Id);

            if (Measurement != null)
            {
                var MeasurementDetail = new Measurement()
                {
                    MeasurementId = Measurement.MeasurementId,
                    MeasurementCode = Measurement.MeasurementCode,
                    MeasurementName = Measurement.MeasurementName,
                    Note = Measurement.Note
                };
                return MeasurementDetail;
            }
            return null;
        }

        public async Task<Measurement> GetMeasurementByIdNoTracking(Guid Id)
        {
            return await _context.Measurements.AsNoTracking().FirstOrDefaultAsync(a => a.MeasurementId == Id);
        }

        public async Task<List<Measurement>> GetMeasurements()
        {
            return await _context.Measurements.OrderBy(p => p.CreateDateTime).Select(Measurement => new Measurement()
            {
                MeasurementId = Measurement.MeasurementId,
                MeasurementCode = Measurement.MeasurementCode,
                MeasurementName = Measurement.MeasurementName,
                Note = Measurement.Note
            }).ToListAsync();
        }

        public IEnumerable<Measurement> GetAllMeasurement()
        {
            return _context.Measurements.OrderByDescending(d => d.CreateDateTime)
                .AsNoTracking();
        }

        public Measurement Update(Measurement update)
        {
            var Measurement = _context.Measurements.Attach(update);
            Measurement.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Measurement Delete(Guid Id)
        {
            var Measurement = _context.Measurements.Find(Id);
            if (Measurement != null)
            {
                _context.Measurements.Remove(Measurement);
                _context.SaveChanges();
            }
            return Measurement;
        }
    }
}
