using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;
using Res.Business.Utils;
using Util.ThirdParty.WangsuCloud;
using System.IO;

namespace Res.Controllers
{

   public class DesignerController : BaseController
   {

      public ActionResult Design()
      {
         //var filePath = Server.MapPath("~/Attachments/second.htm");
         //MentalConverter.ConverHtmlToImage(filePath, @"D:\temp", "myImage");

         //CreateAndBindMedal();e

         var filePath = Server.MapPath("~/Attachments/abc.html");
         var htmlStr = System.IO.File.ReadAllText(filePath).Replace("{{Auther}}", "test").Replace("{{ResourceTitle}}", "title");
         var pdfFile=FormatConverter.ConvertHtmlTextToPDF(htmlStr);

         return new BinaryContentResult($"aaa.pdf", "application/pdf", pdfFile);
      }


      private void CreateAndBindMedal()
      {
         var c = APDBDef.CroResource;
         var f = APDBDef.Files;

         var winlevelResources = db.CroResourceDal.ConditionQuery(c.WinLevelPKID > 0 & c.ActiveId == 2, null, null, null);
         var i = 0;
         foreach (var item in winlevelResources)
         {
            var filePath = Server.MapPath("~/Attachments/abc.html");
            var htmlStr = System.IO.File.ReadAllText(filePath).Replace("{{Auther}}", item.Author).Replace("{{ResourceTitle}}", item.Title);
            var fs = MentalConverter.ConverHtmlToImage(htmlStr, "myImage");
            using (fs)
            {
               if (fs.Length <= 0)
               {
                  Console.Write($"{item.Title} creat fail");
                  break;
               }
               var md5 = FileHelper.ConvertToMD5(fs);

               var fileIsExist = db.FilesDal.ConditionQueryCount(f.Md5 == md5)>0;
               if (fileIsExist)
                  break;

               var docFile = new UploadFile
               {
                  Stream = fs,
                  FileName = $"2018/files/{DateTime.Today.ToString("yyyyMMdd")}/{md5}{FileHelper.GifExtName}"
               };
               var docResult = FileUploader.SliceUpload(docFile);
               if (docResult.IsSuccess)
               {
                  var file = new Files { FileName = $"{item.Title}的奖章", FilePath = docResult.FileUrl, ExtName = FileHelper.GifExtName, Md5 = md5 };
                  db.FilesDal.Insert(file);
                  db.CroResourceMedalDal.Insert(new CroResourceMedal { FileId = file.FileId, CrosourceId = item.CrosourceId, ActiveId = 2, CreateDate = DateTime.Now });
               }
               else
               {
                  Console.Write($"{item.Title} nendal upload fail");
                  break;
               }
            }

            i++;
         }
      }

   }

}