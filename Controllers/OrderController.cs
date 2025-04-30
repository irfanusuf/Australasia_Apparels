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


        public OrderController(SqlDbContext dbContext, IMailService mailService)
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

                if (cart == null || cart.CartValue == 0)
                {
                    return RedirectToAction("Cart", "User");
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

                if (address == null)
                {
                    ViewBag.AddressErrorMessage = "Kindly Fill in Address or select any Address from the list";
                    return View("CheckOut");
                }

                var cart = await dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(cp => cp.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || cart.CartValue == 0)
                {
                    return RedirectToAction("Cart", "User");

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

                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }


        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Verify(Guid OrderId)
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;

            var order = await dbContext.Orders.Include(o => o.Address).ThenInclude(o => o.Buyer).FirstOrDefaultAsync(o => o.OrderId == OrderId); // finding order using orderId

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

            // var address = await dbContext.Addresses.FirstOrDefaultAsync(a => a.UserId == userId);


            var viewModel = new HybridViewModel
            {
                OrderItems = orderItems,
                Order = order,
                Address = order.Address
            };


            return View(viewModel);
        }



        [Authorize]
        [HttpGet]

        public async Task<IActionResult> SendEmail(Guid OrderId)
        {


            try
            {

                Guid? userId = HttpContext.Items["UserId"] as Guid?;

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    RedirectToAction("Login", "User");
                }

                var order = await dbContext.Orders.Include(o => o.Address).FirstOrDefaultAsync(o => o.OrderId == OrderId);


                var htmlBody = $@"
                        <!DOCTYPE html>
<html>
<head>
  <style>
    body {{
      font-family: Arial, sans-serif;
      background-color: #f4f4f4;
      margin: 0;
      padding: 0;
    }}
    .container {{
      max-width: 600px;
      margin: 30px auto;
      background-color: #ffffff;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 6px rgba(0, 0, 0, 0.15);
    }}
    .btn {{
      display: inline-block;
      padding: 12px 24px;
      margin-top: 20px;
      background-color: #007bff;
      color: #ffffff !important;
      text-decoration: none;
      border-radius: 6px;
    }}
    .footer {{
      font-size: 12px;
      color: #777;
      margin-top: 30px;
    }}
  </style>
</head>
<body>
  <div class='container'>
    <h2>Order Verification</h2>
    <p>Hello {order?.Address?.FirstName ?? "Customer"},</p>
    <p>Thank you for shopping with <strong>Australasia Apparels</strong>.</p>
    <p>Please verify your order by clicking the button below:</p>

    <a href='https://australasia-apparels.shop/order/verifiedByEmail?OrderId={OrderId}' class='btn'>Verify My Order</a>

    <p>If you have any questions or need help, feel free to <a href='https://australasia-apparels.shop/support'>contact our support team</a>.</p>

    <p class='footer'>If you didn’t place this order, you can safely ignore this email.</p>
  </div>
</body>
</html>";


                await mailService.SendEmailAsync(user?.Email, "Order Verification ", htmlBody, true);



                TempData["EmailMessage"] = "Mail sent to your Mail Id . Kindly check Your mail box and search for our mail and press verify!";
                return RedirectToAction("Verify", new { order?.OrderId });

            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> VerifiedByEMail(Guid OrderId)
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);
                if (order == null)
                {
                    ViewBag.ErrorMessage = "Order not found.";
                    return View("Error");
                }

                order.OrderStatus = OrderStatus.Verified;
                await dbContext.SaveChangesAsync();

                TempData["EmailMessage"] = "Email is SuccesFully Verified you can pay Now for your Order!";
                return RedirectToAction("Verify", new { order?.OrderId });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(Guid OrderId)
        {
            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);
                if (order == null)
                {
                    ViewBag.ErrorMessage = "Order not found.";
                    return View("Error");
                }

                order.OrderStatus = OrderStatus.InTransit;
                order.PaymentStatus = PaymentStatus.RazorPay;
                await dbContext.SaveChangesAsync();


                return RedirectToAction("Orders", "User");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Cancel(Guid OrderId)
        {
            try
            {
                var order = await dbContext.Orders.Include(o => o.Buyer).FirstOrDefaultAsync(o => o.OrderId == OrderId);
                if (order == null)
                {
                    ViewBag.ErrorMessage = "Order not found.";
                    return View("Error");
                }

                string htmlBody = $@"
    <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #d9534f;'>Order Cancellation Requested</h2>
            <p>Dear {order?.Buyer?.Username},</p>
            <p>We received a request to cancel your order with the ID <strong>{order?.OrderId}</strong>.</p>
            <p>If you initiated this cancellation, please confirm it by clicking the button below:</p>
            <p style='margin: 20px 0;'>
                <a href='https://australasia-apparels.shop/Order/ConfirmCancellation?OrderId={order?.OrderId}' 
                   style='display: inline-block; padding: 10px 20px; background-color: #d9534f; 
                          color: white; text-decoration: none; border-radius: 5px;'>
                    Confirm Cancellation
                </a>
            </p>
            <p>If you did not request this, you can safely ignore this email and your order will remain active.</p>
            <p>Thank you,<br/>Customer Support Team</p>
        </body>
    </html>";


                await mailService.SendEmailAsync(order?.Buyer?.Email, "Verify Order cancellation ", htmlBody, true);

                TempData["EmailMessage"] = "Your have request order cancellation . Kindly check your mail and verify order cancellation!";
                return RedirectToAction("Verify", new { order?.OrderId });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ConfirmCancellation(Guid OrderId)
        {

            try
            {
                var order = await dbContext.Orders.FindAsync(OrderId);
                if (order == null)
                {
                    ViewBag.ErrorMessage = "Order not found.";
                    return View("Error");
                }

                order.OrderStatus = OrderStatus.Cancelled;
                await dbContext.SaveChangesAsync();

                TempData["EmailMessage"] = "Your order has been succesfully cancelled!";
                return RedirectToAction("Verify", new { order?.OrderId });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }

        }

    }
}
