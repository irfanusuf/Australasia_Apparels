using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public ProductController(SqlDbContext dbContext , ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }
      

        [HttpGet]
        public async Task<ActionResult> Index(ProductCategory category)
        {

            // get all products
            var products = await dbContext.Products.Where(p => p.IsActive && p.Category == category).ToListAsync();

            var viewModel = new ProductViewModel
            {
                Products = products,
            };

            ViewBag.category = category.ToString();

            return View(viewModel);
        }
    }
}
