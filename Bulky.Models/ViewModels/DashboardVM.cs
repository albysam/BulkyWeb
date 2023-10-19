using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class DashboardVM
    {
        public IEnumerable<Product> TrendingProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }

    }
}
