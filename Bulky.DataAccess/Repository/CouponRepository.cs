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
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private ApplicationDbContext _db;
        public CouponRepository(ApplicationDbContext db) : base(db) 
        {
            _db= db;
        }

        public Coupon? FirstOrDefault(Func<object, bool> value)
        {
            throw new NotImplementedException();
        }

        public void Update(Coupon obj)
        {
            _db.Coupons.Update(obj);
        }
    }
}
