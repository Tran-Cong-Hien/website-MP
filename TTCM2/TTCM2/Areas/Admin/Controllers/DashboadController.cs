using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
    public class DashboadController : Controller
    {
        // GET: Admin/Dashboad
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}