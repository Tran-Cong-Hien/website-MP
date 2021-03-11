using Dapper;
using Data.CategoryNews;
using Data.Messenger;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CategoryNewsBLL
    {
        public List<CategoryNews> SelectCategoryNews()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<CategoryNews>("pd_Select_CategoryNews").ToList();
            return result;

            //try
            //{

            //}
            //catch (System.Exception)
            //{
            //    // some exception
            //}
            //finally
            //{
            //    if (myConn.State == ConnectionState.Open)
            //    {
            //        myConn.Close();
            //    }
            //    myConn.Dispose();
            //}
        }

        public MessengerResult Delete(int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            try
            {
                myConn.Open();
                myConn.Execute("delete CategoryNews where Id=" + Id + "");
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
