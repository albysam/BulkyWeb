﻿using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IWalletTotalRepository : IRepository<WalletTotal>
    {
        WalletTotal? FirstOrDefault(Func<object, bool> value);
        void Update(WalletTotal obj);
      
    }
}
