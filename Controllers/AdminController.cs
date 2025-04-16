using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

        private readonly ICloudinaryService cloudinary;

        public AdminController(SqlDbContext dbContext, ITokenService tokenService, ICloudinaryService cloudinary)
        {
            this.tokenService = tokenService;
            this.dbContext = dbContext;
            this.cloudinary = cloudinary;
        }


        [HttpGet]
        public ActionResult Index()
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
            return View();
        }

        [HttpGet]
        public ActionResult CreateProduct()
        {
            ViewBag.CategoryList = new SelectList(Enum.GetValues(typeof(ProductCategory)));
             ViewBag.SizeList = new SelectList(Enum.GetValues(typeof(ProductSize)));
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
                throw;
            }

        }
    }
}
