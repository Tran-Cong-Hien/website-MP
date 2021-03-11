using Dapper;
using Data.CategoryProduct;
using Data.Messenger;
using Data.Products;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BLL
{
    public class CategoryProductBLL
    {
        public List<CategoryProduct> SelectCategoryProduct()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<CategoryProduct>("pd_select_CategoryProduct").ToList();
            return result;
        }
        public List<CategoryProduct> SelectCategoryProductLeftMenu()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<CategoryProduct>("select Name, Url from CategoryProduct where Active=1").ToList();
            return result;
        }
        public List<Products> SelectProduct()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select * from Products where IdCateGory=1").ToList();
            return result;
        }
        public List<Products> SelectProduct1()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select *from Products where IdCateGory=2").ToList();
            return result;
        }
        public List<Products> SelectProduct2()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select *from Products where IdCateGory=3").ToList();
            return result;
        }
        public List<Products> SelectProduct3()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select *from Products where IdCateGory=4").ToList();
            return result;
        }
        public List<Products> SelectProduct4()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select *from Products where IdCateGory=5").ToList();
            return result;
        }
        public MessengerResult Delete(int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            try
            {
                myConn.Open();
                myConn.Execute("delete CategoryProduct where Id=" + Id + "");
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
    }

}
