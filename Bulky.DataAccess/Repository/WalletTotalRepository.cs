using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class WalletTotalRepository : Repository<WalletTotal>, IWalletTotalRepository
    {
        private ApplicationDbContext _db;
        public WalletTotalRepository(ApplicationDbContext db) : base(db) 
        {
            _db= db;
        }

        public WalletTotal? FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public void Update(WalletTotal obj)
        {
            var objFromDb = _db.WalletTotal.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.UserId = obj.UserId;
                objFromDb.WalletBalance = obj.WalletBalance;
            }
        }
    }
}
