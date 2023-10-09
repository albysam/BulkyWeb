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
    public class AddressNewRepository : Repository<AddressNew>, IAddressNewRepository
	{
        private ApplicationDbContext _db;
        public AddressNewRepository(ApplicationDbContext db) : base(db) 
        {
            _db= db;
        }

		

		public void Update(AddressNew obj)
        {
            _db.AddressNew.Update(obj);
        }
    }
}
