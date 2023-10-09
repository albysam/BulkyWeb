using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class WalletTotalController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WalletTotalController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var objWalletTotalList = _unitOfWork.WalletTotal.GetAll(u => u.UserId == userId, includeProperties: "ApplicationUser").ToList();
            return View(objWalletTotalList);
        }
    }
}
