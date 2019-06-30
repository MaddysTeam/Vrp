using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using O2S.Components.PDFRender4NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

         Document doc = new Document(PageSize.A4);//要寫PDF的文件，建構子沒填的話預設直式A4
         PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
         //指定文件預設開檔時的縮放為100%
         PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
         //開啟Document文件 
         doc.Open();
         //使用XMLWorkerHelper把Html parse到PDF檔裡
         XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8);
         //將pdfDest設定的資料寫到PDF檔
         PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
         writer.SetOpenAction(action);
         doc.Close();
         msInput.Close();
         outputStream.Close();
         //回傳PDF檔案 
         return outputStream.ToArray();
      }


      /// <summary>
      /// 将PDF文档转换为图片的方法
      /// </summary>
      /// <param name="pdfInputPath">PDF文件路径</param>
      /// <param name="imageName">生成图片的名字</param>
      /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
      /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
      /// <param name="imageFormat">设置所需图片格式</param>
      /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
      public static Stream ConvertPDF2Image(Stream stream,
          string imageName, int startPageNum, int endPageNum, ImageFormat imageFormat, Definition definition=Definition.Ten)
      {
         PDFFile pdfFile = PDFFile.Open(stream);
      
         // validate pageNum
         if (startPageNum <= 0)
         {
            startPageNum = 1;
         }
         if (endPageNum > pdfFile.PageCount)
         {
            endPageNum = pdfFile.PageCount;
         }
         if (startPageNum > endPageNum)
         {
            int tempPageNum = startPageNum;
            startPageNum = endPageNum;
            endPageNum = startPageNum;
         }

         var ms = new MemoryStream();
         // start to convert each page
         for (int i = startPageNum; i <= endPageNum; i++)
         {
            Bitmap pageImage = pdfFile.GetPageImage(i - 1, 40 * (int)definition);
            pageImage.Save(ms, imageFormat);
            pageImage.Dispose();
         }

         pdfFile.Dispose();

         return ms;
      }

      public enum Definition
      {
         One = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10
      }

   }

   //public class UnicokdeFontFactory : FontFactoryImp
   //{

   //   private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "simsun.ttc");
   //   private static readonly string 標楷體Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KAIU.TTF");

   //   public override iTextSharp.text.Font GetFont(
   //      string fontname,
   //      string encoding,
   //      bool embedded,
   //      float size,
   //      int style,
   //      BaseColor color,
   //      bool cached)
   //   {
   //      BaseFont basefont = BaseFont.CreateFont(@"c:\windows\fonts\simsun.ttc,1", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
   //      return new iTextSharp.text.Font(basefont, size, style, color);
   //   }

   //   public override void Register(string path)
   //   {
   //      base.Register(@"c:\windows\fonts\STLITI.TTF");
   //   }

   //}

}
