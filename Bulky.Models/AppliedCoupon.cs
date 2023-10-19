using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class AppliedCoupon
    {
        [Key]
        public int Id { get; set; }

       

        public int CouponId { get; set; }
		public int Discount { get; set; }

		public string? MinCartAmount { get; set; }
		public string? MaxCartAmount { get; set; }
		public string? UserId { get; set; }
		public int Status { get; set; }
	}
}

