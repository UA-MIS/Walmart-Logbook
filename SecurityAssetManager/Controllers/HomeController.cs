using SecurityAssetManager.Models;
using System.Web.Mvc;

namespace SecurityAssetManager.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewRouting()
        {
            if (User.IsInRole("Auditor") || User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Events");
            }

            else
            {
                return RedirectToAction("KeyholderIndex", "Items");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}