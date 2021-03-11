using BLL;
using Data.Customer;
using Data.Messenger;
using Data.Order;
using Data.OrderDetail;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace TTCM2.Controllers
{
    public class AjaxController : Controller
    {
        // GET: Ajax
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCart(int number, int IdProduct)
        {
            try
            {
                if (Request.Cookies["shopcard"] == null)
                {
                    List<Cart> _list = new List<Cart>();

                    Cart _cart = new Cart();
                    _cart.Number = number;
                    _cart.ProductId = IdProduct;
                    _list.Add(_cart);

                    var json = new JavaScriptSerializer();
                    var JsonCart = json.Serialize(_list);

                    HttpCookie JsonCartCookie = new HttpCookie("shopcard", JsonCart);
                    JsonCartCookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Add(JsonCartCookie);
                }
                else
                {
                    var _list = JsonConvert.DeserializeObject<List<Cart>>(Request.Cookies["shopcard"].Value);
                    if (_list.Where(t => t.ProductId == IdProduct).Count() > 0)
                    {
                        _list.Where(t => t.ProductId == IdProduct).FirstOrDefault().Number += number;
                    }
                    else
                    {
                        Cart _cart = new Cart();
                        _cart.Number = number;
                        _cart.ProductId = IdProduct;
                        _list.Add(_cart);
                    }

                    var json = new JavaScriptSerializer();
                    var JsonCart = json.Serialize(_list);

                    HttpCookie JsonCartCookie = new HttpCookie("shopcard", JsonCart);
                    JsonCartCookie.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Set(JsonCartCookie);
                }
                return Json(new { success = true, messenger = "Thanh Cong" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, messenger = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult CountCart()
        {
            try
            {
                var Count = 0;
                if (Request.Cookies["shopcard"] != null)
                {
                    var _list = JsonConvert.DeserializeObject<List<Cart>>(Request.Cookies["shopcard"].Value);
                    Count = _list.Count();
                }
                return Json(new { success = true, messenger = "Thanh Cong", count = Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, messenger = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult InnerCart(Order model)
        {
            try
            {
                if (Request.Cookies["shopcard"] != null)
                {
                    List<OrderDetail> orderDetail = new List<OrderDetail>();
                    ProductsBLL _getprod = new ProductsBLL();
                    OrderBLL _set = new OrderBLL();
                    var _list = JsonConvert.DeserializeObject<List<Cart>>(Request.Cookies["shopcard"].Value);

                    _list.ForEach(p =>
                    {
                        var _object = _getprod.ProductById(p.ProductId);
                        if (_object != null)
                        {
                            OrderDetail _cart = new OrderDetail();
                            _cart.ProductId = _object.Id;
                            _cart.Name = _object.Name;
                            _cart.Price = _object.PriceSale == 0 ? _object.Price : _object.PriceSale;
                            _cart.Url = _object.Url;
                            _cart.Amount = p.Number;
                            _cart.AllPrice = (_object.PriceSale == 0 ? (p.Number * _object.Price) : (p.Number * _object.PriceSale));
                            orderDetail.Add(_cart);
                        }
                    });
                    model.IntoMoney = orderDetail.Sum(p => p.AllPrice);
                    model.CreateDate = DateTime.Now;
                    var rs = _set.InsertCart(model, orderDetail);
                    if (rs.success)
                    {
                        //Insert gio hang thanh cong thì xoa cookie 
                        HttpCookie JsonCartCookie = new HttpCookie("shopcard");
                        JsonCartCookie.Expires = DateTime.Now.AddDays(-1d);
                        Response.Cookies.Add(JsonCartCookie);

                        //Tao cookie đơn hàng

                        var json = new JavaScriptSerializer();
                        var JsonIdOrder = json.Serialize(rs.OrderId);

                        HttpCookie JsonIdOrderCookie = new HttpCookie("IdOrder", JsonIdOrder);
                        JsonIdOrderCookie.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(JsonIdOrderCookie);
                    }
                    return Json(new { success = rs.success, messenger = rs.messenger }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, messenger = "Giỏ hàng rỗng!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, messenger = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult Login(string name, string password, string returnUrl)
        {
            CustomerBLL _get = new CustomerBLL();
            var cos = _get.CheckExistCustomer(name);
            if (cos == 0)
            {
                return Json(new { success = false, messenger = "Tài khoản này không tồn tại!" }, JsonRequestBehavior.AllowGet);
            }
            var f_password = GetMD5(password);
            var coses = _get.CheckExistCustomer(name, f_password);
            if (coses > 0)
            {
                FormsAuthentication.SetAuthCookie(name, false);
                var json = new JavaScriptSerializer();
                var JsonEmail = json.Serialize(name);
                HttpCookie JsonEmailCookie = new HttpCookie("Email", JsonEmail);
                JsonEmailCookie.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(JsonEmailCookie);
                return Json(new { success = true, messenger = "Đăng nhập thành công", url = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl }, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(new { success = false, messenger = "Mật khẩu không chính xác!" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Register(string name, string password)
        {
            CustomerBLL _get = new CustomerBLL();
            var cos = _get.CheckExistCustomer(name);
            if (cos > 0)
            {
                return Json(new { success = false, messenger = "Tài khoản này đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }
            var f_password = GetMD5(password);

            Customer _model = new Customer();
            _model.Name = "Chờ cập nhật";
            _model.Password = f_password;
            _model.Phone = "Chờ cập nhật";
            _model.Rolse = "customer";
            _model.UpdateBy = "user";
            _model.UpdateDate = DateTime.Now;
            _model.CreateBy = "user";
            _model.CreateDate = DateTime.Now;
            _model.Adress = "Chờ cập nhật";
            _model.Email = name;
            _model.TypeCus = 1;

            var rs = _get.Insert(_model);
            return Json(new { success = rs.success, messenger = rs.messenger }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult LogOut()
        {
            if (Request.Cookies["Email"] != null)
            {
                FormsAuthentication.SignOut();
                HttpCookie JsonCartCookie = new HttpCookie("Email");
                JsonCartCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(JsonCartCookie);
            }
            return Json(new { success = true, messenger = "Đăng xuất thành công" }, JsonRequestBehavior.AllowGet);
        }
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

    }
}