using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IWalletRepository : IRepository<Wallet>
    {
		
		Wallet? FirstOrDefault(Func<object, bool> value);
        void Update(Wallet obj);
      
    }
}
