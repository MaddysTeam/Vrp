using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Business.Utils
{

   public class MentalConverter
   {

      public static Stream ConverHtmlToImage(string htmlStr,string imageName)
      {
         var bytes = FormatConverter.ConvertHtmlTextToPDF(htmlStr);
         var ms = new MemoryStream(bytes);
         return FormatConverter.ConvertPDF2Image(ms, imageName, 1, 1, ImageFormat.Gif);
      }

   }

}
