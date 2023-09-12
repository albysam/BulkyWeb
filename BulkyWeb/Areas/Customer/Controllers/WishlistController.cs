using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class WishlistController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public WishlistVM WishlistVM { get; set; }
        public WishlistController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }




        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            WishlistVM = new()
            {
                WishlistList = _unitOfWork.Wishlist.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            //foreach (var cart in WishlistVM.WishlistList)
            //{
            //    cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
            //    cart.Price = GetPriceBasedOnQuantity(cart);

            //    WishlistListtVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            //}

            return View(WishlistVM);
        }


        //public IActionResult AddCart(int productId)
        //{
        //    ShoppingCart cart = new()
        //    {
        //        Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category,ProductImages"),
        //        Count = 1,
        //        ProductId = productId
        //    };

        //    return View(cart);
        //}



        [HttpPost]
        [Authorize]
        public IActionResult AddCart(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);


            if (cartFromDb != null)
            {
               
                cartFromDb.Count += shoppingCart.Count+1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);

            }

            else
            {
                shoppingCart.Count = 1;

                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }


            Wishlist wishlistFromDb = _unitOfWork.Wishlist.Get(u => u.ApplicationUserId == userId &&
            u.ProductId == shoppingCart.ProductId);

            _unitOfWork.Wishlist.Remove(wishlistFromDb);



            TempData["success"] = "Cart updated Successfully";
            _unitOfWork.Save();


          

            return RedirectToAction(nameof(Index));
        }



        public IActionResult Remove(int wishlistId)
        {
            var wishlistFromDb = _unitOfWork.Wishlist.Get(u => u.Id == wishlistId);

            _unitOfWork.Wishlist.Remove(wishlistFromDb);


            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

    }
}
