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
	public class AppliedCouponRepository : Repository<AppliedCoupon>, IAppliedCouponRepository
	{
		private ApplicationDbContext _db;
		public AppliedCouponRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		AppliedCoupon? IAppliedCouponRepository.FirstOrDefault(Func<object, bool> value)
		{
			throw new NotImplementedException();
		}

		public void Update(AppliedCoupon obj)
		{
			_db.AppliedCoupon.Update(obj);
		}

		
	}
}
