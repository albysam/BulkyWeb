using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Bulky.Models.ViewModels;

namespace Bulky.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Category Name")]
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? ISBN { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        [DisplayName("List Price")]
        [Range(1, 1000, ErrorMessage = "Price must between 1-1000")]
        public double ListPrice { get; set; }



        //PRODUCT STOCK


        [Required]
        [DisplayName("Product Stock")]
       // [Range(1, 1000, ErrorMessage = "Product Stock must between 1-1000")]
        public double Price { get; set; }


       // [Required]
        [DisplayName("Price for 50+")]
       // [Range(1, 1000, ErrorMessage = "Price must between 1-1000")]
        public double Price50 { get; set; }



        //OFFER_PRICE

       // [Required]
        [DisplayName("Offer Price")]
       // [Range(1, 1000, ErrorMessage = "Price must between 1-1000")]
        public double Price100 { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set;}

        //public static implicit operator Product(ProductVM v)
        //{
        //    return new Product
        //    {
        //         Assuming properties in ProductVM map to properties in Product
        //        Id = v.ProductId,
        //        Title = v.Title,
        //         Other properties...
        //    };
        //}
    }
}
