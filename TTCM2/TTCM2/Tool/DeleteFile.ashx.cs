using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
namespace WebsiteViet.WebApp.Tool
{
    /// <summary>
    /// Summary description for DeleteFile
    /// </summary>
    public class DeleteFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string folder = context.Request.QueryString["folder"];
            string filename = context.Request.QueryString["filename"];
            string kq = "0";
            try
            {
                if(File.Exists(context.Server.MapPath("~/Upload/"+folder+"/"+filename)))
                {
                    File.Delete(context.Server.MapPath("~/Upload/" + folder +"/"+ filename));
                }
                kq = "1";
            }
            catch (Exception) { kq = "0"; }
            context.Response.ContentType = "text/plain";
            context.Response.Write(kq);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}