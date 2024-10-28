using Microsoft.EntityFrameworkCore;
using PurchasingSystemDeveloper.Areas.MasterData.Models;
using PurchasingSystemDeveloper.Data;
using PurchasingSystemDeveloper.Repositories;

namespace PurchasingSystemDeveloper.Areas.MasterData.Repositories
{
    public class IBankRepository
    {
        private readonly ApplicationDbContext _context;

        public IBankRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Bank Tambah(Bank Bank)
        {
            _context.Banks.Add(Bank);
            _context.SaveChanges();
            return Bank;
        }

        public async Task<Bank> GetBankById(Guid Id)
        {
            var Bank = await _context.Banks
                .SingleOrDefaultAsync(i => i.BankId == Id);

            if (Bank != null)
            {
                var BankDetail = new Bank()
                {
                    BankId = Bank.BankId,
                    BankCode = Bank.BankCode,
                    BankName = Bank.BankName,
                    AccountNumber = Bank.AccountNumber,
                    CardHolderName = Bank.CardHolderName,
                    Note = Bank.Note
                };
                return BankDetail;
            }
            return null;
        }

        public async Task<Bank> GetBankByIdNoTracking(Guid Id)
        {
            return await _context.Banks.AsNoTracking().FirstOrDefaultAsync(a => a.BankId == Id);
        }

        public async Task<List<Bank>> GetBanks()
        {
            return await _context.Banks.OrderBy(p => p.CreateDateTime).Select(Bank => new Bank()
            {
                BankId = Bank.BankId,
                BankCode = Bank.BankCode,
                BankName = Bank.BankName,
                AccountNumber = Bank.AccountNumber,
                CardHolderName = Bank.CardHolderName,
                Note = Bank.Note
            }).ToListAsync();
        }

        public IEnumerable<Bank> GetAllBank()
        {
            return _context.Banks.OrderByDescending(d => d.CreateDateTime)
                .AsNoTracking();
        }

        public Bank Update(Bank update)
        {
            var Bank = _context.Banks.Attach(update);
            Bank.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return update;
        }

        public Bank Delete(Guid Id)
        {
            var Bank = _context.Banks.Find(Id);
            if (Bank != null)
            {
                _context.Banks.Remove(Bank);
                _context.SaveChanges();
            }
            return Bank;
        }
    }
}
