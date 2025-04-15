using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models.DomainModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ITokenService tokenService;
        private readonly SqlDbContext dbContext;

        public AdminController(SqlDbContext dbContext, ITokenService tokenService)
        {
            this.tokenService = tokenService;
            this.dbContext = dbContext;
        }


        [HttpGet]
        public ActionResult Index()
        {
            var token = Request.Cookies["GradSchoolAuthorizationToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "user");
            }
            var userId = tokenService.VerifyTokenAndGetId(token);

            if (Guid.Empty == userId)
            {
                return RedirectToAction("login", "user");
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateProduct()
        {
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {  
                await dbContext.Products.AddAsync(product);
                await dbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = "Product Creation Failed";
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
            return View();
        }
    }
}
