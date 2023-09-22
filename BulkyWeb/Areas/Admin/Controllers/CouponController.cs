using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles = SD.Role_Admin)]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var objCouponList = _unitOfWork.Coupon.GetAll().ToList();
            return View(objCouponList);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Coupon obj)
        {
            if (ModelState.IsValid)
            {


               // var existingUser = _unitOfWork.Coupon.FirstOrDefault(u => u.Name == obj.Name);
                Coupon? couponFromDb = _unitOfWork.Coupon.Get(u => u.CouponCode == obj.CouponCode);
                if (couponFromDb == null)
                {
                    _unitOfWork.Coupon.Add(obj);
                    _unitOfWork.Save();
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", "Coupon Code already exists.");
                }


               
            }
            return View();
        }



        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Coupon? couponFromDb = _unitOfWork.Coupon.Get(u => u.Id == id);
            if (couponFromDb == null)
            {
                return NotFound();
            }
            return View(couponFromDb);

        }
        [HttpPost]
        public IActionResult Edit(Coupon obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Coupon.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Coupon updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }



        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Coupon? couponFromDb = _unitOfWork.Coupon.Get(u => u.Id == id);
            if (couponFromDb == null)
            {
                return NotFound();
            }
            return View(couponFromDb);

        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Coupon? obj = _unitOfWork.Coupon.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Coupon.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Coupon deleted successfully";
            return RedirectToAction("Index");


        }

    }
}
