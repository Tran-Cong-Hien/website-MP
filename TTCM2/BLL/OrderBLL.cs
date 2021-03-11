using Dapper;
using Dapper.Contrib.Extensions;
using Data.Messenger;
using Data.Order;
using Data.OrderDetail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class OrderBLL
    {

        public List<OrderModel> SelectOrder()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<OrderModel>("pd_Select_Order").ToList();
            return result;
        }
        public OrderCartSuccess SelectOrderById(int Id)
        {
            // connection string!
            OrderCartSuccess _model = new OrderCartSuccess();
            _model.listSanpham = new List<OrderDetail>();
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            using (var multi = myConn.QueryMultiple("pd_select_Order_by_id", new { idOrder = Id }, commandType: CommandType.StoredProcedure))
            {
                _model.DonHang = multi.Read<Order>().FirstOrDefault();
                _model.listSanpham = multi.Read<OrderDetail>().ToList();
            }
            return _model;
        }

        public List<OrderDetail> SelectDetaiOrderById(int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<OrderDetail>("pd_OrderDetail_getbyid", new { IdOrder = Id }, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public MessengerResult Delete(int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            try
            {
                myConn.Open();
                myConn.Execute("delete Order where Id=" + Id + "");
                return new MessengerResult { success = true, messenger = "Xóa thành công!" };
            }
            catch (System.Exception ex)
            {
                // some exception
                return new MessengerResult { success = false, messenger = ex.Message };
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                {
                    myConn.Close();
                }
                myConn.Dispose();
            }
        }

        public MessengerResult InsertCart(Order model, List<OrderDetail> orderDetail)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            myConn.Open();
            using (var transition = myConn.BeginTransaction())
            {
                try
                {
                    var _idOrder = myConn.Insert(model, transition);
                    orderDetail.ForEach(p => { p.OrderId = Int32.Parse(_idOrder.ToString()); });
                    myConn.Insert(orderDetail, transition);
                    transition.Commit();
                    return new MessengerResult { success = true, messenger = "Thành công!", OrderId= (int)_idOrder };
                }
                catch (System.Exception ex)
                {
                    transition.Rollback();
                    // some exception
                    return new MessengerResult { success = false, messenger = ex.Message };
                }
                finally
                {
                    if (myConn.State == ConnectionState.Open)
                    {
                        myConn.Close();
                    }
                    myConn.Dispose();
                }
            }
            
        }
    }
}

