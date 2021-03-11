using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
    public class TinTucController : Controller
    {
        NewsBLL _get;
        public TinTucController()
        {
            _get = new NewsBLL();
        }
        // GET: Admin/TinTuc
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin, staff")]
        public ActionResult ThemMoiTinTuc()
        {
            return View();
        }

        [Authorize(Roles = "admin, staff")]
        public ActionResult DanhSachTinTuc()
        {
            var result = _get.SelectNews();
            return View(result);
        }
    }
}