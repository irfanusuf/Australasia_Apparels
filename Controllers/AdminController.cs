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

            var user =  await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

           if(user?.Role != Role.Admin){
            return RedirectToAction("Login" , "User");
           } 
            var usersCount = await dbContext.Users.CountAsync();
            var ordersCount = await dbContext.Orders.CountAsync();
            var productsCount = await dbContext.Products.CountAsync();

            ViewBag.TotalUsers = usersCount;
            ViewBag.TotalOrders = ordersCount;
            ViewBag.TotalProducts = productsCount;


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
                TempData["SuccessMessage"] = "Product Created Successfully";
                return RedirectToAction("Index");


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
                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");

            }

        }

        [HttpGet]
        public async Task<ActionResult> OrderList()
        {

            try
            {
                var orders = await dbContext.Orders.ToListAsync();

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
                var users = await dbContext.Users.ToListAsync();

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


