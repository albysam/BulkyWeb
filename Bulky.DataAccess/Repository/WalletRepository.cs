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
    public class WalletRepository : Repository<Wallet>, IWalletRepository
	{
        private ApplicationDbContext _db;
        public WalletRepository(ApplicationDbContext db) : base(db) 
        {
            _db= db;
        }
		public void Add(Wallet wallet)
		{
			throw new NotImplementedException();
		}
		public Wallet? FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public void Update(Wallet obj)
        {
            _db.Wallet.Update(obj);
        }
    }
}
