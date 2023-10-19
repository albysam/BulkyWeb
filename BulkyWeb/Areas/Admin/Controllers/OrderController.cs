using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Stripe;
using System.Security.Claims;
using System.Text;
using PageSize = PdfSharpCore.PageSize;

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

            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);
            foreach (var orderDetailStatus in orderDetails)
            {
                orderDetailStatus.OrderStatus = 1;
                orderDetailStatus.OrderStatusType = "Approved";
                _unitOfWork.OrderDetail.Update(orderDetailStatus);
            }
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
            var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);
            foreach( var orderDetailStatus in orderDetails) {
                orderDetailStatus.OrderStatus = 2;
                orderDetailStatus.OrderStatusType = "Shipped";
                _unitOfWork.OrderDetail.Update(orderDetailStatus);
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
            StripeConfiguration.ApiKey = "sk_test_51NdFMzSDcXY2QkDCxG0w3dmJ0QYNOAAJDoFV64JT1i1S96EQmjyvGtiqGmRfZuhXALKrRnr2mN4QqtU9WbfSiTDP00dKwu63sY";
            if (orderHeader.PaymentMethod == SD.PaymentPayNow)
                
            {

                if(orderHeader.PaymentStatus == SD.PaymentStatusApproved || orderHeader.PaymentStatus == SD.PaymentStatusCompleted) {
              
                var options = new RefundCreateOptions
                {

                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new RefundService();
                Refund refund = service.Create(options);
                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
            }

                else
                {

                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                }



            }


           

            //Wallet
          if (orderHeader.PaymentMethod == SD.PaymenCashonDelivery)
            {

            if (orderHeader.PaymentStatus == SD.PaymentStatusCompleted) { 


            //orderHeader.CancelTotal = orderHeader.OrderTotal;
             

            //    _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.PaymentStatusRefunded);

                
                    var walletFromDb = _unitOfWork.WalletTotal.Get(u => u.UserId == orderHeader.ApplicationUserId);
                    if (walletFromDb != null)
                    {
                        walletFromDb.WalletBalance += orderHeader.OrderTotal;
                        _unitOfWork.WalletTotal.Update(walletFromDb);
                    }

                    else
                    {
                        WalletTotal wallet = new WalletTotal()
                        {
                            UserId = orderHeader.ApplicationUserId,
                            WalletBalance = orderHeader.OrderTotal

                        };
                        _unitOfWork.WalletTotal.Add(wallet);
                    }

               
            }
                else
                {

                    _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
                   
                }
               //WALLET PAYMENT
               //



            }


            //STOCK MANAGEMENT START

            var orderDetails = _unitOfWork.OrderDetail.Get(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);
            var productFromDb = _unitOfWork.Product.Get(u => u.Id == orderDetails.ProductId);

            productFromDb.Price = productFromDb.Price + orderDetails.Count;
            _unitOfWork.Product.Update(productFromDb);
            //STOCK MANAGEMENT END


            var orderDetails1 = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == OrderVM.OrderHeader.Id);
            foreach (var orderDetailStatus in orderDetails1)
            {
                orderDetailStatus.OrderStatus = 3;
                orderDetailStatus.OrderStatusType = "Cancelled";
                _unitOfWork.OrderDetail.Update(orderDetailStatus);
            }

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


        public IActionResult ReportXML()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Id,Username,Email,JoinedOn,SerialNumber");
            var users = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

            foreach (var user in users)
            {
                builder.AppendLine($"{user.Id},{user.Name},{user.OrderTotal},{user.PaymentDate.ToShortDateString()},{user.Id}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "users.csv");
        }



        public IActionResult ReportExport()
        {
            using var workbook = new XLWorkbook();
            var orderVM = new OrderVM
            {
                OrderDetail = _unitOfWork.OrderDetail
             .GetAll(u => u.OrderStatus != 3, includeProperties: "Product")
             .GroupBy(od => new { od.ProductId, od.Product.Title })
             .Select(g => new OrderDetail
             {
                 ProductId = g.Key.ProductId,
                 Product = new Bulky.Models.Product { Title = g.Key.Title },
                 Count = g.Sum(od => od.Count),
                 Price = g.Sum(od => od.Price * od.Count)

             })
             .ToList()
            };
            var worksheet = workbook.Worksheets.Add("Users");
            var currentRow = 1;

            worksheet.Cell(currentRow, 1).Value = "Product Id";
            worksheet.Cell(currentRow, 2).Value = "Product Title";
            worksheet.Cell(currentRow, 3).Value = "Product Total Count";
            worksheet.Cell(currentRow, 4).Value = "Product Total Amount";
            

            int totalCount = orderVM.OrderDetail.Sum(detail => detail.Count);
            double totalAmount = orderVM.OrderDetail.Sum(detail => detail.Price);
            foreach (var user in orderVM.OrderDetail)
                                {
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = user.ProductId;
                worksheet.Cell(currentRow, 2).Value = user.Product.Title;
                worksheet.Cell(currentRow, 3).Value = user.Count;
                worksheet.Cell(currentRow, 4).Value = user.Price;
               
            }
            worksheet.Cell(currentRow + 1, 1).Value = "Total Count: " +totalCount;
            worksheet.Cell(currentRow + 2, 1).Value = "Total Amount: " +totalAmount;
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
        }


       
        public IActionResult Report(DateTime? DateFrom, DateTime? DateTo)
        {
         
            var query = _unitOfWork.OrderDetail.GetAll(u=> u.OrderStatus != 3 ,includeProperties: "Product");

            if (DateFrom.HasValue)
            {
                query = query
                    .Where(od => od.OrderDatec != null &&
                           od.OrderDatec.Date >= DateFrom.Value);
            }

            if (DateTo.HasValue)
            {
                query = query
                    .Where(od =>od.OrderDatec != null &&
                           od.OrderDatec.Date <= DateTo.Value);
            }



            OrderVM = new OrderVM
                {
                    OrderDetail = query
                        .GroupBy(od => new { od.ProductId, od.Product.Title })
                        .Select(g => new OrderDetail
                        {
                            ProductId = g.Key.ProductId,
                            Product = new Bulky.Models.Product { Title = g.Key.Title },
                            Count = g.Sum(od => od.Count),
                            Price = g.Sum(od => od.Price*od.Count)

                        })
                        .ToList()
                };

         

            return View(OrderVM);
        }


        public IActionResult Dashboard()
        {
            var trendingProductsData = GetTrendingProductsData();
            var totalRevenueData = GetTotalRevenueData();
            var totalOrdersData = GetTotalOrdersData();

            var orderVM = new OrderVM
            {
                TrendingProductsData = trendingProductsData,
                TotalRevenueData = totalRevenueData,
                TotalOrdersData = totalOrdersData

            };

            return View(orderVM);
        }

        private List<OrderDetail> GetTrendingProductsData()
        {
            var query = _unitOfWork.OrderDetail.GetAll(u => u.OrderStatus != 3, includeProperties: "Product");

            var trendingProductsData = query
                .GroupBy(od => new { od.ProductId, od.Product.Title })
                .Select(g => new OrderDetail
                {
                    ProductId = g.Key.ProductId,
                    Product = new Bulky.Models.Product { Title = g.Key.Title },
                    Count = g.Sum(od => od.Count),
                    Price = g.Sum(od => od.Price),
                    OrderDatec = g.Select(od => od.OrderDatec.Date).FirstOrDefault()
                })
                .ToList();

            return trendingProductsData;
        }

        private List<OrderDetail> GetTotalRevenueData()
        {
            var query = _unitOfWork.OrderDetail.GetAll(u => u.OrderStatus != 3, includeProperties: "Product");

            var totalRevenueData = query
                .GroupBy(od => new { od.OrderDatec.Date})
                .Select(g => new OrderDetail
                {
                   
                    Price = g.Sum(od => od.Price * od.Count),
                    OrderDatec = g.Key.Date
                })
                .ToList();

            return totalRevenueData;
        }



        private List<OrderDetail> GetTotalOrdersData()
        {
            var query = _unitOfWork.OrderDetail.GetAll().DistinctBy(g => g.OrderHeaderId);

            var statusCounts = query
                .GroupBy(od => od.OrderStatus)
                .Select(g => new OrderDetail
                {
                    OrderStatusType = GetStatusName(g.Key),
                    OrderStatus = g.Count()
                })
                
                .ToList();

            return statusCounts;
        }

        private string GetStatusName(int status)
        {
            switch (status)
            {
                case 0:
                    return "Pending";
                case 1:
                    return "Approved";
                case 2:
                    return "Shipped";
                case 3:
                    return "Cancelled";
                default:
                    return "Unknown"; // Handle unexpected status codes
            }
        }



        [HttpGet]

       
        public ActionResult GeneratePdf(int orderId)
        {
           

            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")

            };

            var document = new PdfDocument();

            string htmlcontent = "<div style ='width:100%;text-align:center'>";
            htmlcontent += "<h2> Bulky Book Store </h2>";


            if (OrderVM != null)
            {
                htmlcontent += "<h2> Invoice No: INV" + OrderVM.OrderHeader.Id + "& Invoice Date:" + DateTime.Now + "</h2>";
                htmlcontent += "<h3> Customer: " + OrderVM.OrderHeader.Name + " " + "</h3>";
                htmlcontent += "<p>" + OrderVM.OrderHeader.StreetAddress + "," + OrderVM.OrderHeader.City + "</p>";
                htmlcontent += "<p>" + OrderVM.OrderHeader.State + "," + OrderVM.OrderHeader.PostalCode + "</p>";
                htmlcontent += "<h3> Contact :" + OrderVM.OrderHeader.PhoneNumber + "</h3>";
                htmlcontent += "</div>";
            }


            htmlcontent += "<table style = 'width:100% ;border:1px solid #000'>";
            htmlcontent += "<thead style='font-weight :bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style = 'border:1px solid #000'> Product Code </td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Product Name </td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Quantity</td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Price </td>";
            htmlcontent += "<td style = 'border:1px solid #000'> Total Amount </td>";
            htmlcontent += "</tr>";
            htmlcontent += "</thead >";

            htmlcontent += "<tbody>";
            if (OrderVM != null)
            {
                foreach (var product in OrderVM.OrderDetail)
                {
                    htmlcontent += "<tr>";
                    htmlcontent += "<td>" + product.ProductId + "</td>";
                    htmlcontent += "<td>" + product.Product.Title + "</td>";
                    htmlcontent += "<td>" + product.Count + "</td>";

                    htmlcontent += "<td>" + product.Price.ToString() +  "</td>";
                    htmlcontent += "<td>" + (product.Count * product.Price).ToString() +  "</td>";
                    htmlcontent += "</tr>";
                };
            }

            htmlcontent += "</tbody>";
            htmlcontent += "</div>";
            htmlcontent += "<br/>";
            htmlcontent += "<br/>";
            htmlcontent += "<div style='text-align:left>";
            htmlcontent += "<table style = 'width:100% ;border:1px solid #000;float:right'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style = 'border:1px solid #000'> Total Amount: " + OrderVM.OrderHeader.ProductTotal.ToString() + "</td>";
            htmlcontent += "</tr>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style = 'border:1px solid #000'> Discount Amount: " + OrderVM.OrderHeader.CouponDiscount.ToString() + " </td>";
            htmlcontent += "</tr>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style = 'border:1px solid #000'> Paid Amount: " + OrderVM.OrderHeader.TotalPaidAmount.ToString() + " </td>";
            htmlcontent += "</tr>";


           
            htmlcontent += "</table >";
            htmlcontent += "</div>";
            htmlcontent += "</div>";

            PdfGenerator.AddPdfPages(document, htmlcontent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }


            string Filename = "Invoice_" + orderId + ".pdf";
            return File(response, "application/pdf", Filename);
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
                case "refunded":
                    objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusRefunded);
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
