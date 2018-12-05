using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Res.Business
{

   public class BinaryContentResult : ActionResult
   {

      private byte[] _content;
      private string _contentType;
      private string _fileName;

      public BinaryContentResult(string fileName,string contentType,byte[] content)
      {
         _content = content;
         _contentType = contentType;
         _fileName = fileName;
      }

      public override void ExecuteResult(ControllerContext context)
      {
         var response = context.HttpContext.Response;
         response.Cache.SetCacheability(HttpCacheability.NoCache);
         response.ContentType = _contentType;
         response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(_fileName, System.Text.Encoding.UTF8));

         using (var stream= new MemoryStream(_content))
         {
            stream.WriteTo(response.OutputStream);
            stream.Flush();
         }
      }

   }

}
