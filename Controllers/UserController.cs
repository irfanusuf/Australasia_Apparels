using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2WebMVC.Data;
using P2WebMVC.Interfaces;
using P2WebMVC.Models;
using P2WebMVC.Models.ViewModels;
using P2WebMVC.Types;

namespace P2WebMVC.Controllers
{
    public class UserController : Controller
    {

        private readonly SqlDbContext dbContext;    // encapsulated feilds
        private readonly ITokenService tokenService;

        public UserController(SqlDbContext dbContext, ITokenService tokenService)
        {
            this.dbContext = dbContext;
            this.tokenService = tokenService;
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.errorMessage = "All details Required!";
                    return View();
                }
                //   var existingUser = await sqlDbContext.Users.FindAsync(user.UserId);   // findAsync is for PK

                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);   // findAsync is for PK



                if (existingUser != null)
                {

                    ViewBag.errorMessage = "User Already Exists";
                    return View();

                }

                var encryptPass = BCrypt.Net.BCrypt.HashPassword(user.Password);

                user.Password = encryptPass;



                var newUser = await dbContext.Users.AddAsync(user);
                await dbContext.SaveChangesAsync();


                // ViewBag.successMessage = "User Created Succefully!";

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }


        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginView user)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    ViewBag.errorMessage = "All credentials Required!";
                    return View();
                }

                var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);


                if (existingUser == null)
                {

                    ViewBag.errorMessage = "User not Found!";
                    return View();

                }

                var checkPass = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);

                if (checkPass)
                {

                    var token = tokenService.CreateToken(existingUser.UserId, user.Email, existingUser.Username, 60 * 24);

                    //    Console.WriteLine(token);

                    HttpContext.Response.Cookies.Append("AuthorizationToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddHours(72)
                    });


                    if (existingUser.Role == Role.User)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else if (existingUser.Role == Role.StoreKeeper)
                    {
                        return RedirectToAction("Index", "StoreKeeper");
                    }
                    else if (existingUser.Role == Role.Admin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        ViewBag.errorMessage = "Something Went Wrong Kindly Try Again after Sometime!";
                        return View("Error");
                    }
                }
                else
                {
                    ViewBag.errorMessage = "PassWord incorrect!";
                    return View();
                }




            }
            catch (Exception ex)
            {

                ViewBag.errorMessage = ex.Message;
                return View("Error");
            }




        }

        [HttpGet]
        public async Task<ActionResult> Cart()
        {
            try
            {
                var token = Request.Cookies["AuthorizationToken"];

                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login", "User");
                }
                var userId = tokenService.VerifyTokenAndGetId(token);

                if (Guid.Empty == userId)
                {
                    return RedirectToAction("Login", "User");
                }

                var cart = await dbContext.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId== userId); // finding cart of user 

                if (cart == null || cart.CartItems.Count == 0)
                {
                    ViewBag.cartEmpty = "Cart is Empty";
                    return View();
                }

                // for efficency there is serperated cart profucts db query
                var cartItems = await dbContext.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();


                var viewModel = new CartViewModel
                {
                    CartItems = cartItems,
                    Cart = cart
                };

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
        public ActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthorizationToken");
            return RedirectToAction("Index", "Home");
        }



    }

}





