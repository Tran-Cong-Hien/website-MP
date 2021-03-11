<%@ WebHandler Language="C#" Class="Hinhanh" %>

using System;
using System.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
public class Hinhanh : IHttpHandler {
    
    public void Save(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
    {
        // Get the image's original width and height
        int originalWidth = image.Width;
        int originalHeight = image.Height;
        // To preserve the aspect ratio
        float ratioX = (float)maxWidth / (float)originalWidth;
        float ratioY = (float)maxHeight / (float)originalHeight;
        float ratio = Math.Min(ratioX, ratioY);
        // New width and height based on aspect ratio
        int newWidth = (int)(originalWidth * ratio);
        int newHeight = (int)(originalHeight * ratio);
        // Convert other formats (including CMYK) to RGB.
        Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);
        // Draws the image in the specified size with quality mode set to HighQuality
        using (Graphics graphics = Graphics.FromImage(newImage))
        {
            graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        // Get an ImageCodecInfo object that represents the JPEG codec.
        ImageCodecInfo imageCodecInfo =ConfigHelper.GetEncoderInfo(ImageFormat.Jpeg);

        // Create an Encoder object for the Quality parameter.
        System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

        // Create an EncoderParameters object. 
        EncoderParameters encoderParameters = new EncoderParameters(1);

        // Save the image as a JPEG file with quality level.
        EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
        encoderParameters.Param[0] = encoderParameter;
        newImage.Save(filePath, imageCodecInfo, encoderParameters);
    }
    public void ProcessRequest (HttpContext context) {
        string urlImg = "";
        try
        {
            string a = context.Request.QueryString["Guid"];
            string folder = context.Request.QueryString["Folder"];

            string _sourceUrl = context.Server.MapPath("~/Upload/" + folder);
            if ((!File.Exists(_sourceUrl + "/" + a)) || (a == null))
            {
                urlImg = context.Server.MapPath("~/noimage.png");
            }
            else
            {
                urlImg = _sourceUrl + "/" + a;
                if (context.Request.QueryString["_w"] != null)
                {
                    Bitmap hinhgoc = new Bitmap(_sourceUrl + "/" + a);
                    string w = context.Request.QueryString["_w"];
                    double scale = (double)hinhgoc.Width / int.Parse(w);
                    if (!System.IO.Directory.Exists(_sourceUrl + "/" + w))
                    {
                        Directory.CreateDirectory((_sourceUrl + "/" + w));
                    }
                    if (scale >= (double)1)//chỉ khi size nhỏ hơn mới resize lại.
                    {
                        if (!File.Exists(_sourceUrl + "/" + w + "/" + a))
                        {
                            double max_h = hinhgoc.Height / scale;
                            Save(hinhgoc, int.Parse(w), (int)max_h, 50, _sourceUrl + "/" + w + "/" + a);
                        } urlImg = _sourceUrl + "/" + w + "/" + a; 
                    }
                }
            }
        }
        catch (Exception) { urlImg = context.Server.MapPath("~/noimage.png"); }
        context.Response.ContentType = "text/plain";
        context.Response.WriteFile(urlImg);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}