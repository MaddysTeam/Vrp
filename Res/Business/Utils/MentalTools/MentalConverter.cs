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

      public static void ConverHtmlToImage(string htmlPath,string output,string imageName)
      {
         var htmlStr = ReadHtml(htmlPath);
         var bytes = FormatConverter.ConvertHtmlTextToPDF(htmlStr);

         var ms = new MemoryStream(bytes);
         FormatConverter.ConvertPDF2Image(ms, output, imageName, 1, 1, ImageFormat.Gif);

      }


      private static string ReadHtml(string htmlFile)
      {
         if (!string.IsNullOrEmpty(htmlFile))
         {
            return File.ReadAllText(htmlFile);
         }

         return string.Empty;
      }

   }

}
