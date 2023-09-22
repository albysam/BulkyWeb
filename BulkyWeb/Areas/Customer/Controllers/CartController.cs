﻿using Bulky.DataAccess.Repository.IRepository;
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
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
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
            //ShoppingCartVM.OrderHeader.PaymentIntentId = "CashonDelivery";

            //ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }


			if (selectedOption == null)
			{
				TempData["success"] = "Select Payment Type";
				return RedirectToAction(nameof(Summary));
			}



			if (SD.PaymentPayNow == selectedOption)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {

                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
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



        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
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
























//				[HttpPost]
//public IActionResult ApplyCoupon(ShoppingCartVM model)
//{
//    // Assuming you have a method to retrieve the discount percentage for the selected coupon code.
//    double discountPercentage = GetDiscountPercentage(model.SelectedCouponCode);

//    // Assuming you have a method to calculate the total amount of the products in the cart.
//    double totalAmount = CalculateTotalAmount(ShoppingCartVM.ShoppingCartList);

//    if (discountPercentage > 0)
//    {
//        double discountedAmount = totalAmount - (totalAmount * (discountPercentage / 100));
//        ShoppingCartVM.OrderHeader.OrderTotal = discountedAmount;
//    }
//    else
//    {
//        ShoppingCartVM.OrderHeader.OrderTotal = totalAmount; // No discount applied
//    }

//    return PartialView("_ValidationScriptsPartial", model);
//}



//private double GetDiscountPercentage(string couponCode)
//{

//    if (couponCode == "SUMMER2023")
//    {
//        return 5; // 20% discount
//    }
//    else
//    {
//        return 0; // Coupon not found or error
//    }
//}


//private double CalculateTotalAmount(IEnumerable<ShoppingCart> shoppingCartList)
//{


//    var claimsIdentity = (ClaimsIdentity)User.Identity;
//    var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

//    ShoppingCartVM = new()
//    {
//        ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
//       includeProperties: "Product"),
//        OrderHeader = new()
//    };

//    IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
//    double totalAmount = 0;

//    foreach (var cart in ShoppingCartVM.ShoppingCartList)
//    {
//        cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
//        cart.Price = GetPriceBasedOnQuantity(cart);

//        ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);

//        totalAmount = ShoppingCartVM.OrderHeader.OrderTotal;
//    }

//    return totalAmount;
//}



			}


	}

