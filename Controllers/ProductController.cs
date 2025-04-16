using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
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

        public ActionResult AddToCart(Guid ProductId)
        {
            return RedirectToAction("Cart" , "");
        }
   
    }
}
