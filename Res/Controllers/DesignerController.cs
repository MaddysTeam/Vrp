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
         //var filePath = Server.MapPath("~/Attachments/test.html");
         //var htmlStr = System.IO.File.ReadAllText(filePath).Replace("{{Auther}}", "test").Replace("{{ResourceTitle}}", "测试测试测试测试测试测试测试测试");
         //var pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlStr);

        // return new BinaryContentResult($"aaa.pdf", "application/pdf", pdfFile);

         CreateAndBindMedal();

         return View();
      }

      private void CreateAndBindMedal()
      {
         var c = APDBDef.CroResource;
         var f = APDBDef.Files;

         var winlevelResources = db.CroResourceDal.ConditionQuery(c.Author== "陆旻", null, null, null);
         var i = 0;
         foreach (var item in winlevelResources)
         {
            if (i > 10) break;

            var md5 = string.Empty;
            var filePath = Server.MapPath("~/Attachments/medal.html");
            var htmlStr = System.IO.File.ReadAllText(filePath).Replace("{{Auther}}",item.Author)
               .Replace("{{ResourceTitle}}", item.Title)
               .Replace("{{WinLevel}}", "二等奖")
               .Replace("{{MedalCode}}", $"2018{CodeMappings[item.AreaId]}01{item.CrosourceId}");
            var fs = MentalConverter.ConverHtmlToImage(htmlStr, item.Title, out md5);
            using (fs)
            {
               if (fs.Length <= 0)
               {
                  Console.Write($"{item.Title} creat fail");
                  break;
               }

               var fileIsExist = db.FilesDal.ConditionQueryCount(f.Md5 == md5) > 0;
               if (fileIsExist)
                  break;

               var docFile = new UploadFile
               {
                  Stream = fs,
                  FileName = $"2019/files/{DateTime.Today.ToString("yyyyMMdd")}/{md5}{FileHelper.GifExtName}"
               };
               var docResult = FileUploader.SliceUpload(docFile);
               if (docResult.IsSuccess)
               {
                  //var file = new Files { FileName = $"{item.Title}的奖章", FilePath = docResult.FileUrl, ExtName = FileHelper.GifExtName, Md5 = md5 };
                  //db.FilesDal.Insert(file);
                  //db.CroResourceMedalDal.Insert(new CroResourceMedal { FileId = file.FileId, CrosourceId = item.CrosourceId, ActiveId = 2, CreateDate = DateTime.Now });
               }
               else
               {
                  Console.Write($"{item.Title} mendal upload fail");
                  break;
               }
            }

            i++;
         }
      }

      private Dictionary<long, string> CodeMappings = new Dictionary<long, string>()
      {
         { 1163,"310101"},
         { 1164,"310104"},
         { 1165,"310105"},
         { 1166,"310106"},
         { 1167,"310107"},
         { 1169,"310109"},
         { 1170,"310110"},
         { 1171,"310112"},
         { 1172,"310113"},
         { 1173,"310114"},
         { 1174,"310115"},
         { 1175,"310116"},
         { 1176,"310117"},
         { 1177,"310118"},
         { 1178,"310120"},
         { 1180,"310151"},
      };

   }

}
