using Microsoft.AspNetCore.Mvc;
using P2WebMVC.Interfaces;

namespace P2WebMVC.Controllers
{
    public class AdminController : Controller
    {


        private readonly ITokenService tokenService;

        public AdminController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }



        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Dashboard()
        {

            var token = Request.Cookies["GradSchoolAuthorizationToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "user");
            }


           var userId =  tokenService.VerifyTokenAndGetId(token);

           if(userId == null){
                return RedirectToAction("login", "user");
           }



            return View();
        }

    }
}
