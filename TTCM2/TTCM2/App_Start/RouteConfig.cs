using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TTCM2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Gioi thieu",
                url: "gioi-thieu",
                defaults: new { controller = "Home", action = "GioiThieu"}
            );
            routes.MapRoute(
                name: "Gio Hang",
                url: "gio-hang",
                defaults: new { controller = "Home", action = "GioHang" }
            );
            routes.MapRoute(
                name: "Dang Nhap",
                url: "dang-nhap",
                defaults: new { controller = "Home", action = "DangNhap" }
            );
            routes.MapRoute(
               name: "Dang Ky",
               url: "dang-ky",
               defaults: new { controller = "Home", action = "DangKy" }
           );
            routes.MapRoute(
               name: "Dăt hang thanh cong",
               url: "dat-hang-thanh-cong",
               defaults: new { controller = "Home", action = "DatHangThanhCong" }
           );
            routes.MapRoute(
                name: "Tin Tức",
                url: "tin-tuc",
                defaults: new { controller = "Home", action = "TinTuc"}
            );

            routes.MapRoute(
               name: "chi tiet tin tuc",
               url: "tin-tuc/{url}",
               defaults: new { controller = "Home", action = "ChiTietTinTuc", url = "" }
           );

            routes.MapRoute(
                name: "Danh muc san pham",
                url: "danh-muc/{url}",
                defaults: new { controller = "Home", action = "DanhMucSanPham", url= "" }
            );

            routes.MapRoute(
                name: "chi tiet san pham",
                url: "san-pham/{url}",
                defaults: new { controller = "Home", action = "ChiTietSanPham", url = "" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Trangchu", id = UrlParameter.Optional }
            );
        }
    }
}
