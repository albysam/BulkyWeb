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
    public class AddressController : Controller
    {


        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public AddressVM AddressVM { get; set; }
        public AddressController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }




        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            AddressVM = new()
            {
                Address = _unitOfWork.Address.GetAll(u => u.user_Id == userId && u.Status == 0)
            };





            return View(AddressVM);
        }





		[HttpPost]
		public IActionResult SelectAddress(int addressId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			var addressFromDb = _unitOfWork.Address.Get(u => u.Id == addressId && u.user_Id == userId);
			if (addressFromDb != null)
			{
				addressFromDb.Status = 1;
				_unitOfWork.Address.Update(addressFromDb);
			}

            var addressFromDb1 = _unitOfWork.Address.Get(u => u.Id != addressId && u.user_Id == userId && u.Status == 1);
            if (addressFromDb1 != null)
            {
                addressFromDb1.Status = 0;
                _unitOfWork.Address.Update(addressFromDb1);
            }

            _unitOfWork.Save();

			return RedirectToAction("Summary", "Cart");
		}



      

    }
}
