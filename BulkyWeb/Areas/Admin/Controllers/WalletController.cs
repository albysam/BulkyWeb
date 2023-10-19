using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize]
	public class WalletController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var objWalletList = _unitOfWork.Wallet
                    .GetAll(u => u.userId == userId, includeProperties: "ApplicationUser");

           

            return View(objWalletList);
        }
    }
}
