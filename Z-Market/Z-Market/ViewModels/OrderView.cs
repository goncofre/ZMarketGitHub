using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Z_Market.Models;

namespace Z_Market.ViewModels
{
    public class OrderView
    {
        public Customer Customer { get; set; }
        public ProductOrder Product { get; set; }
        public List<ProductOrder> Products { get; set; }
    }
}