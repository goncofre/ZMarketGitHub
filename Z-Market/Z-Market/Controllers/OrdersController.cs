using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Z_Market.Models;
using Z_Market.ViewModels;

namespace Z_Market.Controllers
{
    public class OrdersController : Controller
    {
        Z_MarketContext db = new Z_MarketContext();
        public ActionResult NewOrder()
        {
            var orderView = new OrderView();
            orderView.Customer = new Customer();
            orderView.Products = new List<ProductOrder>();
            Session["orderView"] = orderView;

            var list = db.Customers.ToList();
            list.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
            list = list.OrderBy(c => c.FullName).ToList();
            ViewBag.CustomerID = new SelectList(list, "CustomerID", "FullName");

            return View(orderView);
        }

        public ActionResult AddProduct()
        {
            var list = db.Products.ToList();
            list.Add(new ProductOrder { ProductID = 0, Description = "[Seleccione un producto...]" });
            list = list.OrderBy(p => p.Description).ToList();
            ViewBag.ProductID = new SelectList(list, "ProductID", "Description");

            return View();
        }


        [HttpPost]
        public ActionResult NewOrder(OrderView orderView)
        {
            orderView = Session["orderView"] as OrderView;

            var customerID = int.Parse(Request["CustomerID"]);
            if(customerID ==0)
            {
                var listC = db.Customers.ToList();
                listC.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
                listC = listC.OrderBy(c => c.FullName).ToList();
                ViewBag.CustomerID = new SelectList(listC, "CustomerID", "FullName");
                ViewBag.Error = "Debe seleccionar un cliente";

                return View(orderView);
            }

            var customer = db.Customers.Find(customerID);
            if(customer == null)
            {
                var listC = db.Customers.ToList();
                listC.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
                listC = listC.OrderBy(c => c.FullName).ToList();
                ViewBag.CustomerID = new SelectList(listC, "CustomerID", "FullName");
                ViewBag.Error = "Cliente no existe";
            }

           if(orderView.Products.Count == 0)
            {
                var listC = db.Customers.ToList();
                listC.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
                listC = listC.OrderBy(c => c.FullName).ToList();
                ViewBag.CustomerID = new SelectList(listC, "CustomerID", "FullName");
                ViewBag.Error = "Debe ingresar detalle";
            }

            int orderID = 0;

            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order
                    {
                        CustomerID = customerID,
                        DateOrder = DateTime.Now,
                        OrderStatus = OrderStatus.Created
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    orderID = db.Orders.ToList().Select(o => o.OrderID).Max();

                    foreach (var item in orderView.Products)
                    {
                        var orderDetail = new OrderDetail
                        {
                            ProductID = item.ProductID,
                            Description = item.Description,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            OrderID = orderID
                        };
                        db.OrderDetails.Add(orderDetail);
                        db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ViewBag.Error = "ERROR "+ ex.Message;
                    var listCu = db.Customers.ToList();
                    listCu.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
                    listCu = listCu.OrderBy(c => c.FullName).ToList();
                    ViewBag.CustomerID = new SelectList(listCu, "CustomerID", "FullName");
                    return View(orderView);
                }
               
            }
            ViewBag.Message = string.Format("La orden: {0}, grabada Ok", orderID);

            var list = db.Customers.ToList();
            list.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
            list = list.OrderBy(c => c.FullName).ToList();
            ViewBag.CustomerID = new SelectList(list, "CustomerID", "FullName");

            orderView = new OrderView();
            orderView.Customer = new Customer();
            orderView.Products = new List<ProductOrder>();
            Session["orderView"] = orderView;

            return View(orderView);
        }

        [HttpPost]
        public ActionResult AddProduct(ProductOrder productOrder)
        {
            var orderView = Session["orderView"] as OrderView;

            var productID = int.Parse(Request["ProductID"]);
            if(productID == 0)
            {
                var list = db.Products.ToList();
                list.Add(new ProductOrder { ProductID = 0, Description = "[Seleccione un producto...]" });
                list = list.OrderBy(p => p.Description).ToList();
                ViewBag.ProductID = new SelectList(list, "ProductID", "Description");
                ViewBag.Error = "Debe seleccionar un producto";

                return View(productOrder);
            }

            var product = db.Products.Find(productID);
            if (product == null)
            {
                var list = db.Products.ToList();
                list.Add(new ProductOrder { ProductID = 0, Description = "[Seleccione un producto...]" });
                list = list.OrderBy(p => p.Description).ToList();
                ViewBag.ProductID = new SelectList(list, "ProductID", "Description");
                ViewBag.Error = "Producto no existe";

                return View(productOrder);
            }

            productOrder = orderView.Products.Find(p => p.ProductID == productID);

            if(productOrder == null) { 
                productOrder = new ProductOrder
                {
                    Description = product.Description,
                    Price = product.Price,
                    ProductID = product.ProductID,
                    Quantity = float.Parse(Request["Quantity"])
                };
                orderView.Products.Add(productOrder);
            }
            else
            {
                productOrder.Quantity += float.Parse(Request["Quantity"]);
            }

           

            var listC = db.Customers.ToList();
            listC.Add(new Customer { CustomerID = 0, FirstName = "[Seleccione un cliente...]" });
            listC = listC.OrderBy(c => c.FullName).ToList();
            ViewBag.CustomerID = new SelectList(listC, "CustomerID", "FullName");

            return View("NewOrder", orderView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}