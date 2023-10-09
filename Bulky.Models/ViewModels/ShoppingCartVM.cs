using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }

        public OrderHeader OrderHeader { get; set; }
        public Address Address { get; set; }
		public AddressVM AddressVM { get; set; }
		public IEnumerable<AddressNew> AddressNew { get; set; }
		public IEnumerable<Coupon> Coupon { get; set; }

		public AppliedCoupon AppliedCoupon { get; set; }
		public string SelectedCouponCode { get; set; }
		public WalletTotal WalletTotal { get; set; }

		public string PostalCode { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string StreetAddress { get; set; }
	}
}
