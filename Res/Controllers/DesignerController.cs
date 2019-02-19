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

         CreateAndBindMedal();

         return View();
      }


      private void CreateAndBindMedal()
      {
         var c = APDBDef.CroResource;

         var winlevelResources = db.CroResourceDal.ConditionQuery(c.WinLevelPKID > 0 & c.ActiveId==2,null,null,null);
         var i = 0;
         foreach(var item in winlevelResources)
         {
            if (i > 0) break;

            var filePath = Server.MapPath("~/Attachments/second.htm");
            MentalConverter.ConverHtmlToImage(filePath, @"D:\temp", "myImage");
            var fs = System.IO.File.Open(@"D:\tempmyImage1.Gif", FileMode.Open);
            var md5 =FileHelper.ConvertToMD5(fs);
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
               db.CroResourceMedalDal.Insert(new CroResourceMedal { FileId = file.FileId, CrosourceId = item.CrosourceId });
            }
            else
            {
               //记录失败信息
            }

            fs.Close();
            fs.Dispose();
            i++;
         }
      }

   }

}