using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
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


        public OrderController(SqlDbContext dbContext)
        {

            this.dbContext = dbContext;
            razorpayService = new RazorPayService();

        }

        [HttpGet]
        public async Task<IActionResult> CheckOut(Guid CartId)
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;

            var cart = await dbContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartId == CartId); // finding cart of user 

            if (cart == null)
            {
                ViewBag.cartEmpty = "Cart is Empty";
                return View();     // have to watch it in future if there are no items in cart .. 
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;

            var cart = await dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(cp => cp.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);



            // Convert CartProducts to OrderProducts
            var orderItems = cart.CartItems.Select(cp => new OrderItem
            {
                ProductId = cp.ProductId,
                Quantity = cp.Quantity
            }).ToList();

            var order = new Order
            {
                OrderStatus = OrderStatus.Pending,
                TotalPrice = cart.CartValue,
                UserId = (Guid)userId,
                OrderItems = orderItems
            };

            var createOrder = await dbContext.Orders.AddAsync(order);

            dbContext.CartItems.RemoveRange(cart.CartItems);
            cart.CartValue = 0;
            await dbContext.SaveChangesAsync();

            return RedirectToAction("Payment", new { order.OrderId });
        }


        [HttpGet]
        public async Task<IActionResult> Payment(Guid OrderId)
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

    }
}
