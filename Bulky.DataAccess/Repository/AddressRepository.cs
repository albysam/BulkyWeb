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
    public class AddressRepository : Repository<Address>, IAddressRepository
	{
        private ApplicationDbContext _db;
        public AddressRepository(ApplicationDbContext db) : base(db) 
        {
            _db= db;
        }

		public void Add(Wallet wallet)
		{
			throw new NotImplementedException();
		}

		public Address? FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public void Update(Address obj)
        {
            _db.Addresses.Update(obj);
        }
    }
}
