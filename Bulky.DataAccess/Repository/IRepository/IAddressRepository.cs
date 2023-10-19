using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IAddressRepository : IRepository<Address>
    {
		void Add(Wallet wallet);
		Address? FirstOrDefault(Func<object, bool> value);
        void Update(Address obj);
		
	}
}
