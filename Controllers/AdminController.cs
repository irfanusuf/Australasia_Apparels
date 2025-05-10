using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Models.ViewModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly SqlDbContext dbContext;

        private readonly ICloudinaryService cloudinary;

        public AdminController(SqlDbContext dbContext, ITokenService tokenService, ICloudinaryService cloudinary)
        {
            this.tokenService = tokenService;
            this.dbContext = dbContext;
            this.cloudinary = cloudinary;
        }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            Guid? userId = HttpContext.Items["UserId"] as Guid?;

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user?.Role != Role.Admin)
            {
                return RedirectToAction("Login", "User");
            }

            var totalRevenue = await dbContext.Orders.SumAsync(o => o.TotalPrice);

            var usersCount = await dbContext.Users.CountAsync();
            var ordersCount = await dbContext.Orders.CountAsync();
            var productsCount = await dbContext.Products.CountAsync();


            ViewBag.TotalUsers = usersCount;
            ViewBag.TotalOrders = ordersCount;
            ViewBag.TotalProducts = productsCount;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.Username =  user.Username;


            return View();
        }

        [HttpGet]
        public ActionResult CreateProduct()
        {
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            // ViewBag.SizeList = new SelectList(Enum.GetValues(typeof(ProductSize)));
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product, IFormFile ImageFile)
        {

            try
            {
                ViewBag.SizeList = new SelectList(Enum.GetValues(typeof(ProductSize)));
                ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));



                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Invalid Product Data";
                    return View(product);
                }

                if (ImageFile != null && ImageFile.Length > 0)
                {

                    var uploadResult = await cloudinary.UploadImageAsync(ImageFile);
                    if (uploadResult != null)
                    {
                        product.ImageUrl = uploadResult;
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Image Upload Failed";
                        return View();
                    }

                }


                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();
                TempData["Message"] = "Product Created Successfully";
                return RedirectToAction("ProductList");


            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> ProductList()
        {
            try
            {
                var products = await dbContext.Products.ToListAsync();

                var viewModel = new ProductViewModel
                {
                    Products = products
                };

                // var productDateGroups = await dbContext.Products
                //     .Where(p => !p.IsDeleted)
                //     .GroupBy(p => p.CreatedAt.Date)
                //     .Select(g => new
                //     {
                //         Date = g.Key,
                //         Count = g.Count()
                //     })
                //     .OrderBy(g => g.Date)
                //     .ToListAsync();

                // ViewBag.ProductChartData = productDateGroups;


                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> DeleteProduct(Guid ProductId)
        {

            var product = await dbContext.Products.FindAsync(ProductId);
            if (product == null)
            {
                return NotFound();
            }

            product.IsActive = false;
            product.IsDeleted = true;

            product.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();


            TempData["Message"] = " Production deletion Succesfull!";
            return RedirectToAction(nameof(ProductList));

        }



        [HttpGet]
        public async Task<ActionResult> EditProduct(Guid ProductId)
        {
            var product = await dbContext.Products.FindAsync(ProductId);


            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View(product);
        }



        [HttpPost]
        public async Task<ActionResult> EditProduct(Product model, Guid ProductId)
        {
            var product = await dbContext.Products.FindAsync(ProductId);
            if (product == null)
            {
                return NotFound();
            }

            // Update properties
            product.Name = model.Name;
            product.Brand = model.Brand;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Discount = model.Discount;
            product.Stock = model.Stock;
            product.Color = model.Color;
            product.Size = model.Size;
            product.Category = model.Category;
            product.SubCategory = model.SubCategory;
            product.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            TempData["Message"] = "Product editing successful!";
            return RedirectToAction(nameof(ProductList));
        }



        [HttpGet]
        public async Task<ActionResult> OrderList()
        {
            try
            {
                var orders = await dbContext.Orders.Include(o=> o.Address).ToListAsync();

                var viewModel = new OrderViewModel
                {
                    Orders = orders
                };
                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> UserDb()
        {

            try
            {
                var users = await dbContext.Users
                    .Include(u => u.Orders)
                    .Include(u => u.Cart)
                    .ToListAsync();

                var viewModel = new UserViewModel
                {
                    Users = users
                };
                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }


    }
}


