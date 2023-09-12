using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class WishlistVM
    {
        public IEnumerable<Wishlist> WishlistList { get; set; }

        public OrderHeader OrderHeader { get; set; }
       
    }
}
