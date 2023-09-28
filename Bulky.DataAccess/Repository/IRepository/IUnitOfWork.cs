﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IWishlistRepository Wishlist { get; }

        IApplicationUserRepository ApplicationUser { get; }

        IOrderDetailRepository OrderDetail { get; }

        IOrderHeaderRepository OrderHeader { get; }

        IProductImageRepository ProductImage { get; }
        ICouponRepository Coupon { get; }
		IAddressRepository Address { get; }

		void Save();
    }
}
