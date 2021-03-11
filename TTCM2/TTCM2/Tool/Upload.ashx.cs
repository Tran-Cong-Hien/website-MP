using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Text;
using System.Drawing;

namespace ElninoWeb
{
    /// <summary>
    /// Summary description for Upload
    /// </summary>
    public class Upload : IHttpHandler
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

        public string KeyGui { get; set; }
        private string strFileNameGuide = "";
        string AttachDir = string.Empty;
        private readonly JavaScriptSerializer js = new JavaScriptSerializer();
        private List<string> lstExtAllow = ConfigHelper.GetImageExtenionAllow();
        private int intResizeWidth
        {
            get
            {
                object objW = ConfigurationManager.AppSettings["ResizeDimension_Width"];
                if (objW == null) return 800;
                else
                    return Convert.ToInt32(objW);
            }
        }
        private int intResizeHeight
        {
            get
            {
                object objH = ConfigurationManager.AppSettings["ResizeDimension_Height"];
                if (objH == null) return 900;
                else
                    return Convert.ToInt32(objH);
            }
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            string fileName = context.Request["fileName"];

            var r = context.Response;
            r.AddHeader("Pragma", "no-cache");
            r.AddHeader("Cache-Control", "private, no-cache");

            AttachDir = ConfigHelper.GetTempFolder();
            AttachDir = context.Server.MapPath(AttachDir);
            // nếu chưa có folder, tạo folder
            if (!Directory.Exists(AttachDir))
            {
                Directory.CreateDirectory(AttachDir);
            }
            UploadFile(context);
        }

        private void UploadFile(HttpContext context)
        {
            KeyGui = Guid.NewGuid().ToString();
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;
            UploadWholeFile(context, statuses);
            WriteJsonIframeSafe(context, statuses);
        }
        private string GetfileToSave(string filename)
        {
            string strExtension = Path.GetExtension(filename).ToLower();
            File.Move(AttachDir + "\\" + filename, AttachDir + "\\" + FormatURLText(Path.GetFileNameWithoutExtension(filename))+"-"+DateTime.Now.ToString("ddMMyyyyhhmmss") + strExtension);
            return FormatURLText(Path.GetFileNameWithoutExtension(filename)) + "-" + DateTime.Now.ToString("ddMMyyyyhhmmss") + strExtension;

        }
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            for (int i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];
                string strTempPath = AttachDir + "\\" + Path.GetFileName(file.FileName);
                file.SaveAs(strTempPath);
                var fname = Path.GetFileName(file.FileName);
                strFileNameGuide = GetfileToSave(Path.GetFileName(file.FileName));

                string fileExt = Path.GetExtension(fname);
                if (context.Request.QueryString["isfll"] == "1")
                {
                    if (lstExtAllow.Contains(fileExt))
                    {
                        ResizeImage(AttachDir + "\\" + strFileNameGuide, intResizeHeight, intResizeWidth);
                        //ResizeImage(AttachDir + "\\" + strFileNameGuide, 450, 375, AttachDir + "\\" + "medium" + "\\" + strFileNameGuide);
                    }
                }
                statuses.Add(new FilesStatus
                {
                    uploadPath = ConfigHelper.GetTempFolder() + "/" + strFileNameGuide,
                    fileName = fname,
                    size = (int)(new FileInfo(AttachDir + "\\" + strFileNameGuide)).Length,
                    guidName = strFileNameGuide,
                    progress = "1.0"
                });
            }
        }
        public bool ResizeImage(String strResourceFile, int intHeight, int intWidth)
        {
            System.Drawing.Image imgSource = System.Drawing.Image.FromFile(strResourceFile);

            if (imgSource.Height <= intHeight && imgSource.Width <= intWidth)
            {
                imgSource.Dispose();
                return true;
            }
            //Tính tỷ lệ hình:
            float floHeSoChieuCao = (float)imgSource.Height / intHeight;
            float floHeSoChieuRong = (float)imgSource.Width / intWidth;

            int intNewHeight = Math.Min(intHeight, imgSource.Height);
            int intNewWidth = (int)(imgSource.Width * intNewHeight / imgSource.Height);
            if (floHeSoChieuCao < floHeSoChieuRong)
            {
                intNewWidth = Math.Min(intWidth, imgSource.Width);
                intNewHeight = (int)(imgSource.Height * intNewWidth / imgSource.Width);
            }

            Bitmap bmtResult = new Bitmap(intNewWidth, intNewHeight);

            Graphics g = Graphics.FromImage((System.Drawing.Image)bmtResult);

            g.DrawImage(imgSource, 0, 0, intNewWidth, intNewHeight);

            imgSource.Dispose();
            bmtResult.Save(strResourceFile);
            bmtResult.Dispose();
            return true;
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

        public string FormatURLText(string source)
        {
            if (source.Trim().Length < 1)
                return "";
            StringBuilder sbSource = new StringBuilder(FilterVietkey(source.Trim()));
            sbSource = sbSource.Replace(" ", "-").Replace("&", "-").Replace("\"", "")
                        .Replace("'", "").Replace(":", "").Replace("“", "").Replace("”", "")
                        .Replace("/", "-").Replace("%", "").Replace(".", "").Replace("?", "")
                        .Replace("---", "-").Replace("--", "-").Replace("(", "").Replace(")", "").Replace("|", "").Replace(",", "").Replace("+", "").Replace("#", "").Replace("$", "").Replace("!", "").Replace("@", "").Replace("^", "").Replace("%", "").Replace("&", "").Replace("*", "").Replace("(", "").Replace(")", "").Replace("/", "").Replace(">", "").Replace("<", "").Replace("=", "").Replace("'", "");
            if (sbSource.Length > 50)
                return sbSource.ToString().ToLower();
            return sbSource.ToString().ToLower();
        }

        public String FilterVietkey(String strSource)
        {
            strSource = ConvertISOToUnicode(strSource);
            if (strSource.Trim().Length == 0)
                return "";

            String strUni = "á à ả ã ạ Á À Ả Ã Ạ ă ắ ằ ẳ ẵ ặ Ă Ắ Ằ Ẳ Ẵ Ặ â ấ ầ ẩ ẫ ậ Â Ấ Ầ Ẩ Ẫ Ậ đ Đ é è ẻ ẽ ẹ É È Ẻ Ẽ Ẹ ê ế ề ể ễ ệ Ê Ế Ề Ể Ễ Ệ í ì ỉ ĩ ị Í Ì Ỉ Ĩ Ị ó ò ỏ õ ọ Ó Ò Ỏ Õ Ọ ô ố ồ ổ ỗ ộ Ô Ố Ồ Ổ Ỗ Ộ ơ ớ ờ ở ỡ ợ Ơ Ớ Ờ Ở Ỡ Ợ ú ù ủ ũ ụ Ú Ù Ủ Ũ Ụ ư ứ ừ ử ữ ự Ư Ứ Ừ Ử Ữ Ự ý ỳ ỷ ỹ ỵ Ý Ỳ Ỷ Ỹ Ỵ";
            String strASCI = "a a a a a A A A A A a a a a a a A A A A A A a a a a a a A A A A A A d d e e e e e E E E E E e e e e e e E E E E E E i i i i i I I I I I o o o o o O O O O O o o o o o o O O O O O O o o o o o o O O O O O O u u u u u U U U U U u u u u u u U U U U U U y y y y y Y Y Y Y Y";

            String[] arrCharUni = strUni.Split(" ".ToCharArray());
            String[] arrCharASCI = strASCI.Split(" ".ToCharArray());

            String strResult = strSource;
            for (int i = 0; i < arrCharUni.Length; i++)
                strResult = strResult.Replace(arrCharUni[i], arrCharASCI[i]);

            strUni = "À Á Â Ã Ä Å Æ Ç È É Ê Ë Ì Í Î Ï Ð Ñ Ò Ó Ô Õ Ö Ø Ù Ú Û Ü Ý Þ ß à á â ã ä å æ ç è é ê ë ì í î ï ð ñ ò ó ô õ ö ø ù ú û ü ý þ ÿ";
            strASCI = "A A A A A A Æ Ç E E E E I I I I D N O O O O O Ø U U U U Y Þ ß a a a a a a æ ç e e e e i i i i ð n o o o o o ø u u u u y þ y";

            String[] arrCharUni1 = strUni.Split(" ".ToCharArray());
            String[] arrCharASCI1 = strASCI.Split(" ".ToCharArray());

            for (int i = 0; i < arrCharUni1.Length; i++)
                strResult = strResult.Replace(arrCharUni1[i], arrCharASCI1[i]);

            strResult = strResult.Replace("\0", "");
            return strResult;
        }

        public String ConvertISOToUnicode(String strSource)
        {
            String strUni = "á à ả ã ạ Á À Ả Ã Ạ ă ắ ằ ẳ ẵ ặ Ă Ắ Ằ Ẳ Ẵ Ặ â ấ ầ ẩ ẫ ậ Â Ấ Ầ Ẩ Ẫ Ậ đ Đ é è ẻ ẽ ẹ É È Ẻ Ẽ Ẹ ê ế ề ể ễ ệ Ê Ế Ề Ể Ễ Ệ í ì ỉ ĩ ị Í Ì Ỉ Ĩ Ị ó ò ỏ õ ọ Ó Ò Ỏ Õ Ọ ô ố ồ ổ ỗ ộ Ô Ố Ồ Ổ Ỗ Ộ ơ ớ ờ ở ỡ ợ Ơ Ớ Ờ Ở Ỡ Ợ ú ù ủ ũ ụ Ú Ù Ủ Ũ Ụ ư ứ ừ ử ữ ự Ư Ứ Ừ Ử Ữ Ự ý ỳ ỷ ỹ ỵ Ý Ỳ Ỷ Ỹ Ỵ";
            String strISO = "á à &#7843; ã &#7841; Á À &#7842; Ã &#7840; &#259; &#7855; &#7857; &#7859; &#7861; &#7863; &#258; &#7854; &#7856; &#7858; &#7860; &#7862; â &#7845; &#7847; &#7849; &#7851; &#7853; Â &#7844; &#7846; &#7848; &#7850; &#7852; &#273; &#272; é è &#7867; "
                            + "&#7869; &#7865; É È &#7866; &#7868; &#7864; ê &#7871; &#7873; &#7875; &#7877; &#7879; Ê &#7870; &#7872; &#7874; &#7876; &#7878; í ì &#7881; &#297; &#7883; Í Ì &#7880; &#296; &#7882; ó ò &#7887; õ &#7885; Ó Ò &#7886; Õ &#7884; ô "
                            + "&#7889; &#7891; &#7893; &#7895; &#7897; Ô &#7888; &#7890; &#7892; &#7894; &#7896; &#417; &#7899; &#7901; &#7903; &#7905; &#7907; &#416; &#7898; &#7900; &#7902; &#7904; &#7906; ú ù &#7911; &#361; &#7909; Ú Ù &#7910; &#360; &#7908; &#432; &#7913; &#7915; &#7917; &#7919; &#7921; &#431; "
                            + "&#7912; &#7914; &#7916; &#7918; &#7920; ý &#7923; &#7927; &#7929; &#7925; Ý &#7922; &#7926; &#7928; &#7924;";

            String[] arrCharUni = strUni.Split(" ".ToCharArray());
            String[] arrCharISO = strISO.Split(" ".ToCharArray());

            String strResult = strSource;
            for (int i = 0; i < arrCharUni.Length; i++)
                strResult = strResult.Replace(arrCharISO[i], arrCharUni[i]);

            strUni = "À Á Â Ã Ä Å Æ Ç È É Ê Ë Ì Í Î Ï Ð Ñ Ò Ó Ô Õ Ö Ø Ù Ú Û Ü Ý Þ ß à á â ã ä å æ ç è é ê ë ì í î ï ð ñ ò ó ô õ ö ø ù ú û ü ý þ ÿ";
            strISO = "&#192; &#193; &#194; &#195; &#196; &#197; &#198; &#199; &#200; &#201; &#202; &#203; &#204; &#205; &#206; "
                + "&#207; &#208; &#209; &#210; &#211; &#212; &#213; &#214; &#216; &#217; &#218; &#219; &#220; &#221; &#222; "
                + "&#223; &#224; &#225; &#226; &#227; &#228; &#229; &#230; &#231; &#232; &#233; &#234; &#235; &#236; &#237; &#238; &#239; "
                + "&#240; &#241; &#242; &#243; &#244; &#245; &#246; &#248; &#249; &#250; &#251; &#252; &#253; &#254; &#255;";

            String[] arrCharUni1 = strUni.Split(" ".ToCharArray());
            String[] arrCharISO1 = strISO.Split(" ".ToCharArray());

            for (int i = 0; i < arrCharUni1.Length; i++)
                strResult = strResult.Replace(arrCharISO1[i], arrCharUni1[i]);

            strResult = strResult.Replace("\0", "");
            return strResult;
        }
    }
}