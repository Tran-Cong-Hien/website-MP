using Dapper;
using Dapper.Contrib.Extensions;
using Data.Customer;
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
    public class CustomerBLL
    {
        public List<Customer> SelectCustomer()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Customer>("pd_select_Customer").ToList();
            return result;
        }

        public MessengerResult Delete(int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            try
            {
                myConn.Open();
                myConn.Execute("delete Customer where Id=" + Id + "");
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
        public int CheckExistCustomer(string name)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<int>("select count(Id) from Customer where Email='"+ name + "'").FirstOrDefault();
            return result;
        }
        public string GetRolseCustomer(string name)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<string>("select Rolse from Customer where Email='" + name + "'").FirstOrDefault();
            return result;
        }

        public int CheckExistCustomer(string name, string password)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<int>("select count(Id) from Customer where Email='" + name + "'and Password='"+ password + "'").FirstOrDefault();
            return result;
        }

        public MessengerResult Insert(Customer model)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            myConn.Open();
            using (var transition = myConn.BeginTransaction())
            {
                try
                {
                    var _idOrder = myConn.Insert(model, transition);
                    transition.Commit();
                    return new MessengerResult { success = true, messenger = "Thành công!", OrderId = (int)_idOrder };
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
