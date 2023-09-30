using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class AddressVM
    {
        public Address? Address { get; set; }
		public string PostalCode { get; set; }
		public string State { get; set; }
		public string City { get; set; }
		public string StreetAddress { get; set; }
	}
}
