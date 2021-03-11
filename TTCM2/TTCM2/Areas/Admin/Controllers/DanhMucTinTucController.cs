using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
    public class DanhMucTinTucController : Controller
    {
        CategoryNewsBLL _get;
        public DanhMucTinTucController()
        {
            _get = new CategoryNewsBLL();
        }
        // GET: Admin/DanhMucTinTuc
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, staff")]
        public ActionResult TinTuc()
        {
            var result = _get.SelectCategoryNews();
            return View(result);
        }
    }
}