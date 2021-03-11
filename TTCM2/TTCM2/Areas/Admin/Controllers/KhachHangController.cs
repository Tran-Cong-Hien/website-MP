using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
     
    public class KhachHangController : Controller
    {
        CustomerBLL _get;
        public KhachHangController()
        {
            _get = new CustomerBLL();
        }
        // GET: Admin/KhachHang
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, staff")]
        public ActionResult DanhSachKhachHang()
        {
            var result = _get.SelectCustomer();
            return View(result);
            
        }

        [Authorize(Roles = "admin, staff")]
        public ActionResult ThemMoiKhachHang()
        {
            return View();
        }
    }
}