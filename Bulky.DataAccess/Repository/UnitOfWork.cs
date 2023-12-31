﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class unitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }

        public IProductRepository Product { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IWishlistRepository Wishlist { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        public IProductImageRepository ProductImage { get; private set; }

        public ICouponRepository Coupon { get; private set; }
		public IAppliedCouponRepository AppliedCoupon { get; private set; }

		public IAddressRepository Address { get; private set; }

		public IAddressNewRepository AddressNew { get; private set; }
		public IWalletRepository Wallet { get; private set; }

        public IWalletTotalRepository WalletTotal { get; private set; }
        public unitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ProductImage = new ProductImageRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Wishlist = new WishlistRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            Coupon = new CouponRepository(_db);
            AppliedCoupon = new AppliedCouponRepository(_db);
			Address = new AddressRepository(_db);
			AddressNew = new AddressNewRepository(_db);
			Wallet = new WalletRepository(_db);
            WalletTotal = new WalletTotalRepository(_db);

        }

       
      
     
       
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
