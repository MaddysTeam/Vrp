using Aspose.Words;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.ThirdParty.Aspose
{

   /// <summary>
   /// word converter 
   /// </summary>
   public static class WordConverter
   {
       
      public static void ConvertoHtml(Stream source,string destPath)
      {
         if (source == null) throw new ArgumentNullException("source stream can not be null");

         new Document(source).Save(destPath, SaveFormat.Html);
      }


      public static void ConvertoHtml(string source, string destPath)
      {
         if (source == null) throw new ArgumentNullException("source stream can not be null");

         new Document(source).Save(destPath, SaveFormat.Html);
      }


      public static Stream ConvertoHtml(Stream source)
      {
         if (source == null) throw new ArgumentNullException("source stream can not be null");
         var stream = new MemoryStream();
         new Document(source).Save(stream, SaveFormat.Html);

         return stream;
      }

   }

}
