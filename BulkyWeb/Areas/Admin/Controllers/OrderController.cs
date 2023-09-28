using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Issuing;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("admin")]
	[Authorize]
	public class OrderController : Controller
	{

		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
			OrderVM = new()
			{
OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};

            return View(OrderVM);
        }




		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
        public IActionResult UpdateOrderDetail()
        {
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
                orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;

                orderHeaderFromDb.City = OrderVM.OrderHeader.City;
                orderHeaderFromDb.State = OrderVM.OrderHeader.State;

                orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

			if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
			{
				orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();

			TempData["Success"] = "Order Details Updated Successfully.";

			return RedirectToAction(nameof(Details), new {orderId= orderHeaderFromDb.Id});
        }
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
			_unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult ShipOrder()
        {
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id ==  OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = OrderVM.OrderHeader.OrderStatus;
            orderHeader.ShippingDate = DateTime.Now;
			if(orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
			{
                orderHeader.PaymentDueDate = DateTime.Now;

            }
			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
            if (orderHeader.PaymentMethod == SD.PaymentPayNow)
            {
                _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped, SD.PaymentStatusCompleted);
            }
			else { 
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped);
            }

            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        
             [HttpPost]
       // [Authorize(Roles = SD.Role_Admin)]
		public IActionResult CancelOrder(int orderId)
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);


          //STOCK MANAGEMENT 
           
                var orderDetails= _unitOfWork.OrderDetail.Get(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);
            var productFromDb = _unitOfWork.Product.Get(u => u.Id == orderDetails.ProductId);

            productFromDb.Price = productFromDb.Price + orderDetails.Count;
            _unitOfWork.Product.Update(productFromDb);
            //
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }




		[ActionName("Details")]
		[HttpPost]
		[Authorize(Roles = SD.Role_Admin)]
		public IActionResult Details_PAY_NOW()
		{

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
           
            orderHeader.PaymentDate = DateTime.Now;
            
           _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusShipped, SD.PaymentStatusCompleted);
            _unitOfWork.Save();
            TempData["Success"] = "Order Payment Successfully Completed.";

            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });


        }





            #region API CALLS
            [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders;

			if (User.IsInRole(SD.Role_Admin))
			{
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            }
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				objOrderHeaders = _unitOfWork.OrderHeader
					.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
			}
		
			switch (status)
			{
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment ||  u.PaymentStatus == SD.PaymentStatusPending);
					break;
                case "inprocess":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;

                case "completed":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusCompleted);
                    break;

                case "approved":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.StatusApproved || u.OrderStatus == SD.StatusApproved);
                    break;
                case "cancelled":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusCancelled);
                    break;
                case "shipped":
                    objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                default:
					break;

            }
			return Json(new { data = objOrderHeaders });
		}



		






		#endregion





	}
}
