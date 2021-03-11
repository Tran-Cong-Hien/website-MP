using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
    public class NhanVienController : Controller
    {
        StaffBLL _get;
        public NhanVienController()
        {
            _get = new StaffBLL();
        }
        // GET: Admin/NhanVien
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult DanhSachNhanVien()
        {
            var result = _get.SelectStaff();
            return View(result);
        }
    }
}