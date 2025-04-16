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

        private readonly SqlDbContext sqlDbContext;    // encapsulated feilds
        private readonly ITokenService tokenService;

        public UserController(SqlDbContext sqlDbContext, ITokenService tokenService)
        {
            this.sqlDbContext = sqlDbContext;
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

                var existingUser = await sqlDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);   // findAsync is for PK

            

                if (existingUser != null)
                {

                    ViewBag.errorMessage = "User Already Exists";
                    return View();

                }

                var encryptPass = BCrypt.Net.BCrypt.HashPassword(user.Password);

                user.Password = encryptPass;



                var newUser = await sqlDbContext.Users.AddAsync(user);
                await sqlDbContext.SaveChangesAsync();


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

                var existingUser = await sqlDbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);


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


                    if(existingUser.Role == Role.User){
                        return RedirectToAction("Index", "Home");
                    }
                    else if(existingUser.Role == Role.StoreKeeper){
                        return RedirectToAction("Index", "StoreKeeper");
                    }
                    else if(existingUser.Role == Role.Admin){
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

        public async Task<ActionResult> Cart(){
            // var token = Request.Cookies["AuthorizationToken"];

            // if (string.IsNullOrEmpty(token))
            // {
            //     return RedirectToAction("login", "user");
            // }
            // var userId = tokenService.VerifyTokenAndGetId(token);

            // if (Guid.Empty == userId)
            // {
            //     return RedirectToAction("login", "user");
            // }

            // var user = await sqlDbContext.Users.Include(u => u.Cart).FirstOrDefaultAsync(u => u.UserId == userId);

            // if (user == null)
            // {
            //     return NotFound();
            // }

            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthorizationToken");
            return RedirectToAction("Index", "Home");
        }



    }
}





