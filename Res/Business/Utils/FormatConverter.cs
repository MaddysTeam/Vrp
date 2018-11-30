using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Business
{

   public static class FormatConverter
   {

      /// <summary>
      /// html convert to pdf
      /// </summary>
      /// <param name="htmlText"></param>
      /// <returns></returns>
      public static byte[] ConvertHtmlTextToPDF(string htmlText)
      {
         if (string.IsNullOrEmpty(htmlText))
         {
            return null;
         }

         htmlText = "<p>" + htmlText + "</p>";

         MemoryStream outputStream = new MemoryStream();//要把PDF寫到哪個串流
         byte[] data = Encoding.UTF8.GetBytes(htmlText);//字串轉成byte[]
         MemoryStream msInput = new MemoryStream(data);
         Document doc = new Document();//要寫PDF的文件，建構子沒填的話預設直式A4
         PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
         //指定文件預設開檔時的縮放為100%
         PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
         //開啟Document文件 
         doc.Open();
         //使用XMLWorkerHelper把Html parse到PDF檔裡
         XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new UnicodeFontFactory());
         //將pdfDest設定的資料寫到PDF檔
         PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
         writer.SetOpenAction(action);
         doc.Close();
         msInput.Close();
         outputStream.Close();
         //回傳PDF檔案 
         return outputStream.ToArray();

      }

   }

   public class UnicodeFontFactory : FontFactoryImp
   {

      private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arialuni.ttf");
      private static readonly string 標楷體Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KAIU.TTF");

      public override Font GetFont(
         string fontname,
         string encoding,
         bool embedded,
         float size,
         int style,
         BaseColor color,
         bool cached)
      {
         BaseFont baseFont = BaseFont.CreateFont(arialFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
         return new Font(baseFont, size, style, color);
      }

   }

}
