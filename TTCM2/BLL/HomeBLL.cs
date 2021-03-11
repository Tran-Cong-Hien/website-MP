using Dapper;
using Data.Home;
using Data.News;
using Data.Products;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class HomeBLL
    {
        public List<Products> SelectNewsProducts()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select *from Products where [Status] =1").ToList();
            return result;
        }


        public List<Products> SelectSellProducts()
        {
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("select *from [dbo].[Products] where [Selling] =1").ToList();
            return result;

        }

        

        public List<News> SelectNews()
        {
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<News>("select top(2) *from [dbo].[News] where Active =1").ToList();
            return result;
        }

    }
}
