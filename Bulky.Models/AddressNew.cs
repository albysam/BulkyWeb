using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class AddressNew
    {
        public int Id { get; set; }


		[Required]
		public string? ApplicationUserId { get; set; }
		[ForeignKey("ApplicationUserId")]
		[ValidateNever]
		public ApplicationUser? ApplicationUser { get; set; }
		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		public string? PostalCode { get; set; }
		public int? Status { get; set; }

		
	}
}
