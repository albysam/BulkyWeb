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
    public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
    {
        private ApplicationDbContext _db;
        public WishlistRepository(ApplicationDbContext db) : base(db) 
        {
            _db= db;
        }
       

        public void Update(Wishlist obj)
        {
            _db.Wishlist.Update(obj);
        }
    }
}
