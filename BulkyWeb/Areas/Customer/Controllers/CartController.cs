using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyWeb.Area.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Policy;
using System.Text.Encodings.Web;
using Stripe;
using Stripe.Checkout;
using Bulky.DataAccess.Repository;
using Stripe.Issuing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }




        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);

                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }


        [HttpPost]

        public IActionResult Address()
        {


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM.Address.user_Id = userId;
            ShoppingCartVM.Address.Status = 1;

            var addressFromDb = _unitOfWork.Address.Get(u => u.user_Id == userId && u.Status == 1);
            if (addressFromDb != null)
            {
                addressFromDb.Status = 0;
                _unitOfWork.Address.Update(addressFromDb);
            }

            if (ShoppingCartVM.Address.StreetAddress == null || ShoppingCartVM.Address.State == null || ShoppingCartVM.Address.City == null || ShoppingCartVM.Address.PostalCode == null)
            {
                TempData["success"] = "All fields are required";
                return RedirectToAction(nameof(Summary));
            }




            _unitOfWork.Address.Add(ShoppingCartVM.Address);
            _unitOfWork.Save();

            return RedirectToAction("Summary");
        }

		public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };




		

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;



			//COUPON DISPLAY
			ShoppingCartVM.Coupon = _unitOfWork.Coupon.GetAll();

			foreach (var coupon in ShoppingCartVM.Coupon)
			{
				coupon.CouponCode = coupon.CouponCode;
			}

			




			ShoppingCartVM.Address = _unitOfWork.Address.Get(u => u.user_Id == userId && u.Status == 1);
            if(ShoppingCartVM.Address != null)
            {
				ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.Address.StreetAddress;
				ShoppingCartVM.OrderHeader.City = ShoppingCartVM.Address.City;
				ShoppingCartVM.OrderHeader.State = ShoppingCartVM.Address.State;
				ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.Address.PostalCode;
			}

            else { 

            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
			}
            


            //WALLET START
            ShoppingCartVM.WalletTotal = _unitOfWork.WalletTotal.Get(u => u.UserId == userId);

            if (ShoppingCartVM.WalletTotal != null)
            {
                ShoppingCartVM.OrderHeader.CancelTotal = ShoppingCartVM.WalletTotal.WalletBalance;
                //COUPON START

                ShoppingCartVM.AppliedCoupon = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId);



                if (ShoppingCartVM.AppliedCoupon != null)
                {




                    foreach (var cart in ShoppingCartVM.ShoppingCartList)
                    {

                        cart.Price = GetPriceBasedOnQuantity(cart);
                        ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);
                        var DiscountAmount = (ShoppingCartVM.OrderHeader.ProductTotal * ShoppingCartVM.AppliedCoupon.Discount) / 100;

                        ShoppingCartVM.OrderHeader.Coupon = ShoppingCartVM.OrderHeader.ProductTotal - DiscountAmount;


                        ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.Coupon - ShoppingCartVM.OrderHeader.CancelTotal;
                        if (ShoppingCartVM.OrderHeader.OrderTotal <= 0)
                        {
                            ShoppingCartVM.OrderHeader.OrderTotal = 0;

                        }





                    }

                }
                //COUPON END
                else {



                    foreach (var cart in ShoppingCartVM.ShoppingCartList)
                    {

                        cart.Price = GetPriceBasedOnQuantity(cart);
                        ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);


                        ShoppingCartVM.OrderHeader.Coupon = 0;


                        ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.ProductTotal - ShoppingCartVM.OrderHeader.CancelTotal;
                        if (ShoppingCartVM.OrderHeader.OrderTotal <= 0)
                        {
                            ShoppingCartVM.OrderHeader.OrderTotal = 0;

                        }
                    }



                }



            }

            //WALLET END
            else
            {


				//COUPON START

				ShoppingCartVM.AppliedCoupon = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId);



				if (ShoppingCartVM.AppliedCoupon != null)
				{




					foreach (var cart in ShoppingCartVM.ShoppingCartList)
					{

						cart.Price = GetPriceBasedOnQuantity(cart);
						ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);
						var DiscountAmount = (ShoppingCartVM.OrderHeader.ProductTotal * ShoppingCartVM.AppliedCoupon.Discount) / 100;

						ShoppingCartVM.OrderHeader.Coupon = ShoppingCartVM.OrderHeader.ProductTotal - DiscountAmount;


						ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.Coupon;
						if (ShoppingCartVM.OrderHeader.OrderTotal <= 0)
						{
							ShoppingCartVM.OrderHeader.OrderTotal = 0;

						}

					}

				}
				//COUPON END
				else
				{

					foreach (var cart in ShoppingCartVM.ShoppingCartList)
                {
                    ShoppingCartVM.OrderHeader.ProductTotal = 0;
                    ShoppingCartVM.OrderHeader.CancelTotal = 0;
                    cart.Price = GetPriceBasedOnQuantity(cart);
                    ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }

            }
			}

			return View(ShoppingCartVM);
        }




        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            string selectedOption = ShoppingCartVM.OrderHeader.PaymentMethod;

			

			ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
            includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDatec = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
           

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
			///////////////////////////////////////WALLET START////////////////////////////////////////////////////////
			ShoppingCartVM.WalletTotal = _unitOfWork.WalletTotal.Get(u => u.UserId == userId);
            if (ShoppingCartVM.WalletTotal != null)
            {
                ShoppingCartVM.OrderHeader.CancelTotal = ShoppingCartVM.WalletTotal.WalletBalance;

                //COUPON START

                ShoppingCartVM.AppliedCoupon = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId);



                if (ShoppingCartVM.AppliedCoupon != null)
                {




                    foreach (var cart in ShoppingCartVM.ShoppingCartList)
                    {

                        cart.Price = GetPriceBasedOnQuantity(cart);
                        ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);
                        var DiscountAmount = (ShoppingCartVM.OrderHeader.ProductTotal * ShoppingCartVM.AppliedCoupon.Discount) / 100;

                        ShoppingCartVM.OrderHeader.Coupon = ShoppingCartVM.OrderHeader.ProductTotal - DiscountAmount;


                        ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.Coupon - ShoppingCartVM.OrderHeader.CancelTotal;
                        if (ShoppingCartVM.OrderHeader.OrderTotal <= 0)
                        {
                            ShoppingCartVM.OrderHeader.OrderTotal = 0;

                        }





                    }

                }
                //COUPON END
                else
                {

                    foreach (var cart in ShoppingCartVM.ShoppingCartList)
                    {
                        cart.Price = GetPriceBasedOnQuantity(cart);
                        ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);

                        ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.ProductTotal - ShoppingCartVM.OrderHeader.CancelTotal;
                        if (ShoppingCartVM.OrderHeader.OrderTotal <= 0)
                        {
                            ShoppingCartVM.OrderHeader.OrderTotal = 0;

                        }
                    }

                }


            }


            ////////////////////////////////////WALLET END/////////////////////////////////////





            else
            {


				//COUPON START

				ShoppingCartVM.AppliedCoupon = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId);



				if (ShoppingCartVM.AppliedCoupon != null)
				{




					foreach (var cart in ShoppingCartVM.ShoppingCartList)
					{

						cart.Price = GetPriceBasedOnQuantity(cart);
						ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);
						var DiscountAmount = (ShoppingCartVM.OrderHeader.ProductTotal * ShoppingCartVM.AppliedCoupon.Discount) / 100;

						ShoppingCartVM.OrderHeader.Coupon = ShoppingCartVM.OrderHeader.ProductTotal - DiscountAmount;


						ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.Coupon;
						if (ShoppingCartVM.OrderHeader.OrderTotal <= 0)
						{
							ShoppingCartVM.OrderHeader.OrderTotal = 0;

						}

					}

				}
				//COUPON END
				else
				{
					foreach (var cart in ShoppingCartVM.ShoppingCartList)
					{
						ShoppingCartVM.OrderHeader.ProductTotal = 0;
						ShoppingCartVM.OrderHeader.CancelTotal = 0;
						cart.Price = GetPriceBasedOnQuantity(cart);
						ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
						ShoppingCartVM.OrderHeader.ProductTotal += (cart.Price * cart.Count);
					}

				}
				
			}
            
			if (selectedOption == null && ShoppingCartVM.OrderHeader.OrderTotal > 0)
			{
				TempData["success"] = "Select Payment Type";
				return RedirectToAction(nameof(Summary));
			}

			//////////////////////////////////////////STRIPE START///////////////////////////////////////////////////////

			if (SD.PaymentPayNow == selectedOption)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
				
				AppliedCoupon couponFromDb = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId || u.Status == 1);

				if (couponFromDb != null)
				{
					_unitOfWork.AppliedCoupon.Remove(couponFromDb);
				}

			}
			//////////////////////////////////////////STRIPE END///////////////////////////////////////////////////////
			//////////////////////////////////////////COD START///////////////////////////////////////////////////////

			else if (SD.PaymenCashonDelivery == selectedOption)
			{

                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
				var walletFromDb = _unitOfWork.WalletTotal.Get(u => u.UserId == ShoppingCartVM.OrderHeader.ApplicationUserId);
				if (walletFromDb != null) {
					walletFromDb.WalletBalance = 0;
				_unitOfWork.WalletTotal.Update(walletFromDb);
				}

				AppliedCoupon couponFromDb = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId || u.Status == 1);

				if (couponFromDb != null)
				{
					_unitOfWork.AppliedCoupon.Remove(couponFromDb);
				}

				
					


				}
			//////////////////////////////////////////COD START///////////////////////////////////////////////////////
			else
			{
				ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusCompleted;
				ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
				var walletFromDb = _unitOfWork.WalletTotal.Get(u => u.UserId == ShoppingCartVM.OrderHeader.ApplicationUserId);
				
					walletFromDb.WalletBalance = walletFromDb.WalletBalance - ShoppingCartVM.OrderHeader.ProductTotal;
					_unitOfWork.WalletTotal.Update(walletFromDb);
				AppliedCoupon couponFromDb = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId);

				if (couponFromDb != null)
				{
					_unitOfWork.AppliedCoupon.Remove(couponFromDb);
				}
			}

			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();




            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                var productFromDb = _unitOfWork.Product.Get(u => u.Id == cart.ProductId);

                productFromDb.Price = productFromDb.Price - cart.Count;
                _unitOfWork.Product.Update(productFromDb);



                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            //////////////////////////////////////////////STRIPE PAYMENT START///////////////////////////////////////////////////////////////////
            if (SD.PaymentPayNow == selectedOption)

            {


                StripeConfiguration.ApiKey = "sk_test_51NdFMzSDcXY2QkDCxG0w3dmJ0QYNOAAJDoFV64JT1i1S96EQmjyvGtiqGmRfZuhXALKrRnr2mN4QqtU9WbfSiTDP00dKwu63sY";
                var domain = "https://localhost:7037/";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",

                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    AllowPromotionCodes = true,
                };
			
				
					foreach (var item in ShoppingCartVM.ShoppingCartList)
					{

						var sessionLineItem = new SessionLineItemOptions
						{
							PriceData = new SessionLineItemPriceDataOptions
							{
								UnitAmount = (long)(item.Price * 100),
								Currency = "usd",
								ProductData = new SessionLineItemPriceDataProductDataOptions
								{
									Name = item.Product.Title
								}
							},
							Quantity = item.Count
						};
						options.LineItems.Add(sessionLineItem);
					}





				var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);

            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });

        }

		//////////////////////////////////////////////STRIPE PAYMENT END///////////////////////////////////////////////////////////////////

		//COUPON

		[HttpPost]

        public IActionResult ApplyCoupon(AppliedCoupon couponUsed, int couponId, int couponDiscount, string couponMinAmount, string couponMaxAmount)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            couponUsed.UserId = userId;
			AppliedCoupon couponFromDb = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId);

            if (couponFromDb != null)
            {
				TempData["success"] = "Remove already applied coupon then apply another coupon";
				return RedirectToAction(nameof(Summary));
			}

            else
            {

				AppliedCoupon cart = new AppliedCoupon()
                {


                    CouponId = couponId,
                    UserId = userId,
                    Status = 1,
                    Discount = couponDiscount,
                    MinCartAmount = couponMinAmount,
                    MaxCartAmount = couponMaxAmount

				};


                _unitOfWork.AppliedCoupon.Add(cart);
            }




            TempData["success"] = "Coupon Applied Successfully";
            _unitOfWork.Save();

            return RedirectToAction(nameof(Summary));
        }

        [HttpPost]
        public IActionResult RemoveCoupon(AppliedCoupon couponUsed, int couponId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            couponUsed.UserId = userId;
			AppliedCoupon couponFromDb = _unitOfWork.AppliedCoupon.Get(u => u.UserId == userId &&
             u.CouponId == couponId);

            if (couponFromDb == null)
            {
                TempData["success"] = "Remove already applied coupon";
                return RedirectToAction(nameof(Summary));
            }

            else
            {



                _unitOfWork.AppliedCoupon.Remove(couponFromDb);

                TempData["success"] = "Coupon Removed Successfully";
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Summary));

        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus == SD.PaymentStatusPending)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }


            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }
		

		public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);


            //STOCK MANAGEMENT

            var productFromDb = _unitOfWork.Product.Get(u => u.Id == cartFromDb.ProductId);

            if (cartFromDb.Count > productFromDb.Price)
            {
                TempData["success"] = "Not enough stock available";
            }
            else
            {

                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }



        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);

            _unitOfWork.ShoppingCart.Remove(cartFromDb);


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {

            if (shoppingCart.Product.Price100 > 0)
            {
                return shoppingCart.Product.Price100;
            }

            else
            {
                return shoppingCart.Product.ListPrice;
            }

        }




			}


	}


