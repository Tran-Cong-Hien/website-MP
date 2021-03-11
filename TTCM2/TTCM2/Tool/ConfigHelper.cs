using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;


public class ConfigHelper
{
    public const string SERVICE_DISPLAY_CONTENTS = "Content";
    public const string SERVICE_DISPLAY_CUSTOMERS = "Customer";
    public const string SERVICE_DISPLAY_TEMPLATES = "Template";

    public const string SITEMAP_CACHE_KEY = "POPCORN_SITEMAP_DATA";

    public static string GetEditorFolder()
    {
        return "/Upload/Editor/";
    }
    public static string GetNewsFolder()
    {
        return "/Upload/News/";
    }
    public static string GetCustomerFolder()
    {
        return "/Upload/Customer/";
    }

    public static string GetTemplateFolder()
    {
        return "/Upload/Template/";
    }

    public static string GetTempFolder()
    {
        return "/Upload/Temp";
    }
    public static string GetUserImage()
    {
        return "/Upload/UserImage/";
    }
    public static List<string> GetImageExtenionAllow()
    {
        string temp = ".jpg;.png;.bmp;.gif;.jpeg;.JPEG;.JPG;.PNG";
        return temp.Split(';').ToList();
    }
    public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
    {
        return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
    }
    public static int GetPageSize()
    {
        return 20;
    }
}
