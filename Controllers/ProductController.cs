using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.JunctionModels;
using P2WebMVC.Models.ViewModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class ProductController : Controller
    {

        private readonly SqlDbContext dbContext;
        private readonly ITokenService tokenService;
        public ProductController(SqlDbContext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }


        [HttpGet]
        public async Task<ActionResult> Index(ProductCategory category)
        {

            try
            {
                if (category == ProductCategory.All)
                {
                    var products = await dbContext.Products.Where(p => p.IsActive).ToListAsync();

                    var viewModel = new ProductViewModel
                    {
                        Products = products,
                    };

                    ViewBag.category = category.ToString();

                    return View(viewModel);
                }
                else
                {
                    var products = await dbContext.Products.Where(p => p.IsActive && p.Category == category).ToListAsync();

                    var viewModel = new ProductViewModel
                    {
                        Products = products,
                    };

                    ViewBag.category = category.ToString();

                    return View(viewModel);
                }


            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                Console.WriteLine(ex.Message);
                return View("Error");
                throw;
            }
            // get all products

        }


        [HttpGet]
        public async Task<ActionResult> Details(Guid ProductId)
        {
            try
            {

               

                var product = await dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == ProductId && p.IsActive);

        
                var viewModel = new ProductViewModel
                {
                    Product = product,
                };

                ViewBag.SizeList = new SelectList(Enum.GetValues(typeof(ProductSize)));
                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
                throw;
            }
        }
   
        [HttpGet]

     
         public async Task<ActionResult> AddToCart(Guid ProductId)
        {
            try
            {
                var token = Request.Cookies["AuthorizationToken"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("login", "user");
                }
                var userId = tokenService.VerifyTokenAndGetId(token);
                if (Guid.Empty == userId)
                {
                    return RedirectToAction("login", "user");
                }
                var product = await dbContext.Products.FindAsync(ProductId);

             

                var cart = await dbContext.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);    // cart ko find kerhay hai 

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = (Guid)userId,
                        CartValue = 0
                    };
                    await dbContext.Carts.AddAsync(cart);
                    await dbContext.SaveChangesAsync();
                }

                var existingCartItem = await dbContext
                .CartItems.FirstOrDefaultAsync(cp => cp.CartId == cart.CartId && cp.ProductId == ProductId);   // finding cartProduct 

                if (existingCartItem == null)
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = ProductId,
                        Quantity = 1
                    };

                    await dbContext.CartItems.AddAsync(cartItem);    
                    if (product != null)
                    {
                        cart.CartValue += (int)product.Price;
                    }
                    await dbContext.SaveChangesAsync();
                }

                if (existingCartItem != null && existingCartItem.ProductId == ProductId)
                {
                    existingCartItem.Quantity += 1;
                      if (product != null)
                    {
                        cart.CartValue += (int)product.Price;
                    }
                    await dbContext.SaveChangesAsync();
                }

                return RedirectToAction("Cart", "User");
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
   
    }
}
