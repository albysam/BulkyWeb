using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace BulkyWeb.Area.Customer.Controllers
{
    [Area("Customer")]


    
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
          

            IEnumerable <Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                 Product = _unitOfWork.Product.Get(u=>u.Id== productId, includeProperties: "Category,ProductImages"),
                 Count = 1,
                 ProductId = productId
        };
           
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult HomeCart(ShoppingCart shoppingCart, int productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
          u.ProductId == productId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += 1;


                //STOCK MANAGEMENT
                var productFromDb1 = _unitOfWork.Product.Get(u => u.Id == shoppingCart.ProductId);

                if (cartFromDb.Count > productFromDb1.Price)
                {
                    TempData["success"] = "Product already added to cart";


                }

                else
                {
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                    TempData["success"] = "Cart updated Successfully";
                    _unitOfWork.Save();


                }






              
            }

            else
            {

                ShoppingCart cart = new ShoppingCart () 
                {
                   
                   
                    ProductId = productId,
                    Count = 1,
                    ApplicationUserId = userId
                };

                _unitOfWork.ShoppingCart.Add(cart);
                TempData["success"] = "Cart updated Successfully";
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
               
            }




            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;


                //STOCK MANAGEMENT
                var productFromDb = _unitOfWork.Product.Get(u => u.Id == shoppingCart.ProductId);

                if (cartFromDb.Count > productFromDb.Price)
                {
                    TempData["success"] = "Product already added to cart";


                }

                else
                {
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                    TempData["success"] = "Cart updated Successfully";
                    _unitOfWork.Save();
                  

                }


              
            }

            else
            {

                _unitOfWork.ShoppingCart.Add(shoppingCart);
                TempData["success"] = "Cart updated Successfully";
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            }

           

            return RedirectToAction(nameof(Index));
        }





        [HttpPost]
        [Authorize]
        public IActionResult Wishlist(Wishlist wishlist)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            wishlist.ApplicationUserId = userId;

            Wishlist wishlistFromDb = _unitOfWork.Wishlist.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == wishlist.ProductId);


            if (wishlistFromDb != null)
            {
                TempData["success"] = "Product already added in the Wishlist";
            }

            else
            {

                _unitOfWork.Wishlist.Add(wishlist);
                TempData["success"] = "Wishlist updated Successfully";
                _unitOfWork.Save();
            }

          

            return RedirectToAction(nameof(Index));
        }







        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}