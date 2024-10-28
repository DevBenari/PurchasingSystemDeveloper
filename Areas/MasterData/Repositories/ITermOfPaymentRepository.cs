using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class ITermOfPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public ITermOfPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public TermOfPayment Tambah(TermOfPayment TermOfPayment)
        {
            _context.TermOfPayments.Add(TermOfPayment);
            _context.SaveChanges();
            return TermOfPayment;
        }

        public async Task<TermOfPayment> GetTermOfPaymentById(Guid Id)
        {
            var TermOfPayment = await _context.TermOfPayments
                .SingleOrDefaultAsync(i => i.TermOfPaymentId == Id);

            if (TermOfPayment != null)
            {
                var TermOfPaymentDetail = new TermOfPayment()
                {
                    TermOfPaymentId = TermOfPayment.TermOfPaymentId,
                    TermOfPaymentCode = TermOfPayment.TermOfPaymentCode,
                    TermOfPaymentName = TermOfPayment.TermOfPaymentName,
                    Note = TermOfPayment.Note
                };
                return TermOfPaymentDetail;
            }
            return null;
        }

        public async Task<TermOfPayment> GetTermOfPaymentByIdNoTracking(Guid Id)
        {
            return await _context.TermOfPayments.AsNoTracking().FirstOrDefaultAsync(a => a.TermOfPaymentId == Id);
        }

        public async Task<List<TermOfPayment>> GetTermOfPayments()
        {
            return await _context.TermOfPayments.OrderBy(p => p.CreateDateTime).Select(TermOfPayment => new TermOfPayment()
            {
                TermOfPaymentId = TermOfPayment.TermOfPaymentId,
                TermOfPaymentCode = TermOfPayment.TermOfPaymentCode,
                TermOfPaymentName = TermOfPayment.TermOfPaymentName,
                Note = TermOfPayment.Note
            }).ToListAsync();
        }

        public IEnumerable<TermOfPayment> GetAllTermOfPayment()
        {
            return _context.TermOfPayments.OrderByDescending(d => d.CreateDateTime)
                .AsNoTracking();
        }

        public TermOfPayment Update(TermOfPayment update)
        {
            var TermOfPayment = _context.TermOfPayments.Attach(update);
            TermOfPayment.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public TermOfPayment Delete(Guid Id)
        {
            var TermOfPayment = _context.TermOfPayments.Find(Id);
            if (TermOfPayment != null)
            {
                _context.TermOfPayments.Remove(TermOfPayment);
                _context.SaveChanges();
            }
            return TermOfPayment;
        }
    }
}
