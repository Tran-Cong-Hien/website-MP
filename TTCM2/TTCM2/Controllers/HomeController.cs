using BLL;
using Data.Home;
using Data.Messenger;
using Data.News;
using Data.Order;
using Data.Products;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace TTCM2.Controllers
{
    public class HomeController : Controller
    {
        HomeBLL _get;
        public HomeController()
        {
            _get = new HomeBLL();
        }


        public ActionResult Index()
        {
            return View();
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
        public ActionResult Trangchu()
        {
            HomeBLL _get = new HomeBLL();
            HomeModel result = new HomeModel
            {
                ListNewsProducts = _get.SelectNewsProducts(),
                ListNews = _get.SelectNews(),
                ListSellProducts = _get.SelectSellProducts()
            };
            return View(result);
        }
        public ActionResult GioiThieu()
        {
            return View();
        }
        public ActionResult DanhMucSanPham(string url)
        {
            ProductsBLL _get = new ProductsBLL();
            var result = _get.SelectProductByCategory(url);
            ViewBag.url = url;
            return View(result);
        }



        public ActionResult TinTuc()
        {
            NewsBLL _get = new NewsBLL();
            NewsModel result = new NewsModel
            {
                ListHomeNews = _get.SelectHomeNews(),
                ListOtherNews = _get.SelectOtherNews()

            };

            return View(result);
        }
        public ActionResult ChiTietSanPham(string url)
        {
            ProductsBLL _get = new ProductsBLL();
            var rs = _get.SelectProductDetail(url);
            return View(rs);
        }
        public ActionResult ChiTietTinTuc(string url)
        {
            NewsBLL _get = new NewsBLL();
            var rs = _get.SelectNewsDetail(url);
            return View(rs);
        }
        public ActionResult Dangnhap()
        {
            return View();
        }

        [Authorize(Roles = "customer, admin")]
        public ActionResult GioHang()
        {
            OrderCart model = new OrderCart();
            model.listSanpham = new List<ProductsCart>();
            ProductsBLL _getprod = new ProductsBLL();

            if (Request.Cookies["shopcard"] != null)
            {
                var _list = JsonConvert.DeserializeObject<List<Cart>>(Request.Cookies["shopcard"].Value);
                List<Products> _listProduct = new List<Products>();
                _list.ForEach(p => {
                    var _object= _getprod.ProductById(p.ProductId);
                    if(_object!=null)
                    {
                        ProductsCart _cart = new ProductsCart();
                        _cart.Id = _object.Id;
                        _cart.Name = _object.Name;
                        _cart.Price = _object.Price;
                        _cart.PriceSale = _object.PriceSale;
                        _cart.Url = _object.Url;
                        _cart.Quality = p.Number;
                        _cart.AllPrice = (_object.PriceSale == 0 ? (p.Number * _object.Price) : (p.Number * _object.PriceSale));
                        model.listSanpham.Add(_cart);
                    }
                });
                model.TamTinh = model.listSanpham.Sum(p => p.AllPrice);
                model.AllPayment = model.listSanpham.Sum(p => p.AllPrice);
            }
            
            return View(model);
        }
        public ActionResult DatHangThanhCong()
        {
            OrderBLL _getorder = new OrderBLL();
            OrderCartSuccess mpdel = new OrderCartSuccess();
            if (Request.Cookies["IdOrder"] != null)
            {
                var _IdOrder = JsonConvert.DeserializeObject<int>(Request.Cookies["IdOrder"].Value);
                mpdel = _getorder.SelectOrderById(_IdOrder);
            }
            return View(mpdel);
        }


        public ActionResult DangKy()
        {
            return View();
        }
    }
}