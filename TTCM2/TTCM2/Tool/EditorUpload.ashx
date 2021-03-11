<%@ WebHandler Language="C#" Class="EditorUpload" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using System.Drawing;
using System.Drawing.Drawing2D;


public class EditorUpload : IHttpHandler, IRequiresSessionState
{

    public class FilesStatus
    {
        public string uploadPath { get; set; }
        public string fileName { get; set; }
        public string guidName { get; set; }
        public int size { get; set; }
        public string error { get; set; }
        public string progress { get; set; }
    }

    // folder luu file
    private string UserLoginFolder = "";

    // ma GUID phat sinh sau khi luu file len server
    public string KeyGui { get; set; }
    private string strFileNameGuide = "";

    // kich thuoc file se resize - chieu ngang
    private int intResizeW = 600;
    // kich thuoc file se resize - chieu cao
    private int intResizeH = 600;

    // duong dan chua folder image cho từng user
    string AttachDir = "/Upload/";

    // loai thuc hien khi goi handler này
    public string Type = "";

    private readonly JavaScriptSerializer js = new JavaScriptSerializer();

    public bool IsReusable { get { return false; } }

    public void ProcessRequest(HttpContext context)
    {
        Type = context.Request["type"];
        var r = context.Response;
        r.AddHeader("Pragma", "no-cache");
        r.AddHeader("Cache-Control", "private, no-cache");

        // kiem tra folder EditorUpload có hay chưa? chua co thi tạo folder
        AttachDir = context.Server.MapPath(AttachDir);
        if (!Directory.Exists(AttachDir))
            Directory.CreateDirectory(AttachDir);
        AttachDir = AttachDir + "/" + UserLoginFolder;
        
        // kiem tra folder chứa hình ảnh của user có hay chua? chua co thì tạo folder
        //if (!Directory.Exists(AttachDir + "\\" + UserLoginFolder))
        //    Directory.CreateDirectory(AttachDir + "\\" + UserLoginFolder);

        HandleMethod(context);
    }

    private void HandleMethod(HttpContext context)
    {
        if (Type != "GET" && Type != "HEAD" && Type != "DELETE")
            Type = context.Request.HttpMethod;

        switch (Type)
        {
            case "POST":
                UploadFile(context);
                break;

            case "DELETE":
                DeleteFile(context);
                break;

            default:
                context.Response.ClearHeaders();
                context.Response.StatusCode = 405;
                break;
        }
    }
    
    /// <summary>
    /// xóa file
    /// </summary>
    private void DeleteFile(HttpContext context)
    {
        try
        {
            var filePath = AttachDir + "/" + context.Request["f"];
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Upload file
    /// </summary>
    private void UploadFile(HttpContext context)
    {
        try
        {
            KeyGui = Guid.NewGuid().ToString();
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"]))
            {
                UploadWholeFile(context, statuses);
            }
            else
            {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }
            WriteJsonIframeSafe(context, statuses);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
    {
        try
        {
            if (context.Request.Files.Count != 1)
                throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");

            var inputStream = context.Request.Files[0].InputStream;
            var fullName = AttachDir + Path.GetFileName(fileName);

            strFileNameGuide = GetfileToSave(Path.GetFileName(fileName));
            using (var fs = new FileStream(fullName, FileMode.Append, FileAccess.Write))
            {
                var buffer = new byte[1024];

                var l = inputStream.Read(buffer, 0, 1024);
                while (l > 0)
                {
                    fs.Write(buffer, 0, l);
                    l = inputStream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
            }

            statuses.Add(new FilesStatus
            {
                uploadPath ="/Upload/"+ strFileNameGuide,
                fileName = fileName,
                size = (int)(new FileInfo(fullName)).Length,
                guidName = strFileNameGuide,
                progress = "1.0"
            });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private string GetfileToSave(string filename)
    {
        string strExtension = Path.GetExtension(filename).ToLower();
        if (strExtension.Equals(".png") || strExtension.Equals(".jpg") || strExtension.Equals(".gif") || strExtension.Equals(".bmp"))
        {
            Image objImg = Image.FromFile(AttachDir + "/" + UserLoginFolder + "/" + filename);
            //objImg = ResizeImage(objImg, intResizeW, intResizeH);
            objImg.Save(AttachDir + "/" + UserLoginFolder + "/" + KeyGui + strExtension);
            objImg.Dispose();
            File.Delete(AttachDir + "/" + UserLoginFolder + "/" + filename);
        }
        else
        {
            File.Move(AttachDir + "/" + UserLoginFolder + "/" + filename, AttachDir + "/" + UserLoginFolder + "/" + KeyGui + strExtension);
        }
        return KeyGui + strExtension;

    }


    private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
    {
        for (int i = 0; i < context.Request.Files.Count; i++)
        {
            var file = context.Request.Files[i];

            // Check if folder not exists then create new by path
            string _path = AttachDir + "/" + UserLoginFolder + "/";
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }

            file.SaveAs(AttachDir + "/" + UserLoginFolder + "/" + Path.GetFileName(file.FileName));

            var fname = Path.GetFileName(file.FileName);
            strFileNameGuide = GetfileToSave(Path.GetFileName(file.FileName));
            statuses.Add(new FilesStatus
            {
                uploadPath = "/Upload/" + strFileNameGuide,
                fileName = fname,
                size = file.ContentLength,
                guidName = strFileNameGuide,
                progress = "1.0",
            });
        }
    }

    private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
    {
        context.Response.AddHeader("Vary", "Accept");
        try
        {
            if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
            {
                context.Response.ContentType = "application/json";
            }
            else
            {
                context.Response.ContentType = "text/plain";
            }
        }
        catch
        {
            context.Response.ContentType = "text/plain";
        }
        var jsonObj = js.Serialize(statuses.ToArray());
        context.Response.Write(jsonObj);
    }

    private Image ResizeImage(Image objImg, int width, int height)
    {
        double intHeight = objImg.Height;
        double intWidth = objImg.Width;
        Bitmap objBip = new Bitmap(objImg);
        double dblPerWidth = 0;
        double dblPerHeight = 0;
        if (intWidth > width && intHeight <= height)
        {
            dblPerWidth = width / intWidth;
            height = Convert.ToInt32(Math.Round(intHeight * dblPerWidth, 0));
        }
        else if (intWidth <= width && intHeight > height)
        {
            dblPerHeight = height / intHeight;
            width = Convert.ToInt32(Math.Round(intWidth * dblPerHeight, 0));
        }
        else if (intWidth <= width && intHeight <= height)
        {
            width = Convert.ToInt32(intWidth);
            height = Convert.ToInt32(intHeight);
        }
        else if (intWidth > width && intHeight > height)
        {
            dblPerWidth = width / intWidth;
            dblPerHeight = height / intHeight;
            double dblPerCent = dblPerWidth > dblPerHeight ? dblPerHeight : dblPerWidth;
            height = Convert.ToInt32(Math.Round(intHeight * dblPerCent, 0));
            width = Convert.ToInt32(Math.Round(intWidth * dblPerCent, 0));
        }
        Bitmap objBit = new Bitmap(objImg, new Size(width, height));
        //  Bitmap bmp = new Bitmap(resizedW, resizedH);
        Graphics graphic = Graphics.FromImage((Image)objBit);
        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphic.DrawImage(objImg, 0, 0, width, height);
        graphic.Dispose();
        objImg.Dispose();
        return (Image)objBit;
    }
}