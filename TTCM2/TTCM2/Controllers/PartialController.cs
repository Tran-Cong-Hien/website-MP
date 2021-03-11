using BLL;
using Data.Customer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Controllers
{
    public class PartialController : Controller
    {
        CategoryProductBLL _get;
        public PartialController()
        {
            _get = new CategoryProductBLL();
        }
        // GET: Partial
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuLeft()
        {
            var rs = _get.SelectCategoryProductLeftMenu();
            return PartialView(rs);
        }
        public ActionResult LoginView()
        {
            Customer model = new Customer();
            if (Request.Cookies["Email"] != null)
            {
                model.Email = JsonConvert.DeserializeObject<string>(Request.Cookies["Email"].Value);
                return PartialView(model);
            }
            else
                return PartialView(model);
        }
    }
}