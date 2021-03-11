using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TTCM2.Areas.Admin.Controllers
{
    public class DonHangController : Controller
    {

       
         OrderBLL _get;
        public DonHangController()
        {
            _get = new OrderBLL();
        }
    
            // GET: Admin/DonHang
            public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult DanhSachDonHang()
        {
            var result = _get.SelectOrder();

            if(result.Count()>0)
            {
                ////cv 1
                //foreach(var item in result)
                //{
                //    item.ListOrderDetail = _get.SelectDetaiOrderById(item.Id);
                //}

                ////cv2
                //for(int i=0; i < result.Count(); i++)
                //{
                //    result[i].ListOrderDetail= _get.SelectDetaiOrderById(result[i].Id);
                //}

                //cv3
                result.ForEach(item => { item.ListOrderDetail= _get.SelectDetaiOrderById(item.Id); });
            }
            return View(result);
        }
        public ActionResult ThemMoiDonHang()
        {
            return View();
        }
        //public ActionResult ChiTietDonHang()
        //{
        //    var result = _get1.SelectLineOrder();
        //    return View(result);
        //}
    }
}