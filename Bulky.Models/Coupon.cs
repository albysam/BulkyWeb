using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Coupon Code")]
        [Required]
        [MaxLength(30)]

        public string? CouponCode { get; set; }

        public DateTime ValidFromDate { get; set; }
        public DateTime ValidToDate { get; set; }

        [DisplayName("Discount")]
        [Range(1, 10000, ErrorMessage = "Discount must between 1-10000")]
        public int Discount { get; set; }


        [DisplayName("Minimum Cart Amount")]
       

        public string? MinCartAmount { get; set; }


        [DisplayName("Maximum Cart Amount")]
      

        public string? MaxCartAmount { get; set; }
    }
}

