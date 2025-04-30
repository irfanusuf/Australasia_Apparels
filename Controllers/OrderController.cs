using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Models.ViewModels;
using P2WebMVC.Services;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class OrderController : Controller
    {
        // GET: OrderController
        private readonly SqlDbContext dbContext;
        private readonly RazorPayService razorpayService;

        private readonly IMailService mailService;


        public OrderController(SqlDbContext dbContext ,IMailService mailService)
        {

            this.dbContext = dbContext;
            this.mailService = mailService;
            razorpayService = new RazorPayService();

        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> CheckOut(Guid CartId)
        {

            try
            {
             Guid? userId = HttpContext.Items["UserId"] as Guid?;

            


            var cart = await dbContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartId == CartId); // finding cart of user 

            if (cart == null ||  cart.CartValue == 0)
            {
                return RedirectToAction("Cart" , "User");     
            }


            var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);


            var cartItems = await dbContext.CartItems 
            .Include(cp => cp.Product)
            .Where(cp => cp.CartId == cart.CartId)
            .ToListAsync();

            var viewModel = new HybridViewModel
            {
                CartItems = cartItems,
                Cart = cart,
                Address = address
            };

            return View(viewModel);
                
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
            
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(PaymentStatus paymentOption)
        {

            try
            {
                 Guid? userId = HttpContext.Items["UserId"] as Guid?;
        
            if (userId == null)
            {
                return RedirectToAction("Login", "User"); // Or handle as appropriate
            }

            var address = await dbContext.Addresses.FirstOrDefaultAsync(u => u.UserId == userId);

            if(address == null){
                ViewBag.AddressErrorMessage ="Kindly Fill in Address or select any Address from the list";
                return View("CheckOut");
            }
        
            var cart = await dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(cp => cp.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
        
            if (cart == null || cart.CartValue == 0)
            {
                return RedirectToAction("Cart" , "User");
     
            }
        
            // Convert CartProducts to OrderProducts
            var orderItems = cart.CartItems.Select(cp => new OrderItem
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity,
                Size = cp.Size,
                Color = cp.Color,
                
            }).ToList();
        
            var order = new Order
            {
                OrderStatus = OrderStatus.Pending,
                PaymentStatus = paymentOption,
                AddressId = address.AddressId,
                TotalPrice = cart.CartValue,
                UserId = (Guid)userId,
                OrderItems = orderItems
            };
        
            var createOrder = await dbContext.Orders.AddAsync(order);
        
            dbContext.CartItems.RemoveRange(cart.CartItems);
            cart.CartValue = 0;
            await dbContext.SaveChangesAsync();
        
            return RedirectToAction("Verify", new { order.OrderId });
            }
            catch (System.Exception ex)
            {
                
                    ViewBag.ErrorMessage = ex.Message ;
                    return View ("Error");
            }

           
        }




        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Verify(Guid OrderId)
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;

            var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == OrderId); // finding order using orderId

            if (order == null)
            {
                ViewBag.CartEmpty = "No recent Orders";
                return View();
            }

            // for efficnecy used two queries // or maybe we can call a single query // will watch in future 

            var orderItems = await dbContext.OrderItems
            .Include(op => op.Product)
            .Where(op => op.OrderId == order.OrderId)
            .ToListAsync();

            var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);


            var viewModel = new HybridViewModel
            {
                OrderItems = orderItems,
                Order = order,
                Address = address
            };


            return View(viewModel);
        }






        [Authorize]
        [HttpGet]

        public async Task <IActionResult> SendEmail (Guid OrderId){


            try
            {

              Guid? userId = HttpContext.Items["UserId"] as Guid?;

              var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

              if(user == null){
                RedirectToAction ("Login" , "User");
              }

              var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderId == OrderId);

            //    await mailService.SendEmailAsync(user?.Email , "Order Verification " , "This email is for order verification on australasia apparels " ,true); 

                 

            TempData["EmailMessage"] = "Mail sent to your Mail Id . Kindly check Your mail box and search for our mail and press verify!";
              return RedirectToAction("Verify" ,  new { order?.OrderId });

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View();
             
            }

        }
    }
}
