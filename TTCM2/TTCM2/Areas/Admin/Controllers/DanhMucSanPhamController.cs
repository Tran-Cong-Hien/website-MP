using BLL;
using Data.CategoryProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
    public class DanhMucSanPhamController : Controller
    {
        CategoryProductBLL _get;
        public DanhMucSanPhamController()
        {
            _get = new CategoryProductBLL();
        }
        // GET: Admin/DanhMucSanPham
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin, staff")]
        public ActionResult SanPham()
        {
            var min = _get.SelectCategoryProduct();
            return View(min);
            
        }

        [HttpPost]
        [Authorize(Roles = "admin, staff")]
        public ActionResult Delete(int Id)
        {
            var result = _get.Delete(Id);
            return Json(new { success = result.success, messenger = result.messenger }, JsonRequestBehavior.AllowGet);
        }
    }
}
