using Dapper;
using Dapper.Contrib.Extensions;
using Data.Messenger;
using Data.Products;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductsBLL
    {
        public List<Products> SelectProduct()
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var result = myConn.Query<Products>("pd_select_Products").ToList();
            return result;
        }

        public ProductsFollowCateGory SelectProductByCategory(string url)
        {
            ProductsFollowCateGory _model = new ProductsFollowCateGory();
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();

            using (var multi = myConn.QueryMultiple("pd_select_Products_by_urlcategory", new { url = url }, commandType: CommandType.StoredProcedure))
            {
                _model.ListProduct = multi.Read<Products>().ToList();
                _model.NameCategory = multi.Read<string>().FirstOrDefault();
            }
            return _model;
        }

        public MessengerResult Delete(int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            try
            {
                myConn.Open();
                myConn.Execute("delete Products where Id=" + Id + "");
                return new MessengerResult { success=true, messenger="Xóa thành công!"};
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

        public ProductsDetail SelectProductDetail(string url)
        {
            ProductsDetail _model = new ProductsDetail();
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            using (var multi = myConn.QueryMultiple("pd_select_ProductsDetail_by_url", new { url = url }, commandType: CommandType.StoredProcedure))
            {
                _model.Detail = multi.Read<Products>().FirstOrDefault();
                _model.ListProductLike = multi.Read<Products>().ToList();
            }
            return _model;
        }
        public Products ProductById (int Id)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);
            myConn.Open();
            var multi = myConn.Query<Products>("SELECT TOP 1 * FROM [Products] where Id=" + Id + "").FirstOrDefault();
            return multi;
        }

        public MessengerResult Insert(Products model)
        {
            // connection string!
            SqlConnection myConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString);

            myConn.Open();
            using (var transition = myConn.BeginTransaction())
            {
                try
                {
                    if(model.Id==0)
                        myConn.Insert(model, transition);
                    else
                        myConn.Update(model, transition);

                    transition.Commit();
                    return new MessengerResult { success = true, messenger = "Thành công!"};
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
