using Res.Business;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Res.Controllers
{

   public class AttachmentController : BaseController
   {

      static APDBDef.FilesTableDef f = APDBDef.Files;

      //
      //	文件 - 上传封面图片
      // POST:		/Attachment/UploadCover
      //

      [HttpPost]
      public ActionResult UploadCover()
      {
         if (Request.Files.Count != 1)
            return Content("Error");

         string dirForSaving = GetDirForSaving(Guid.NewGuid());
         string mappedDir = Server.MapPath("~" + dirForSaving);
         if (!Directory.Exists(mappedDir))
         {
            Directory.CreateDirectory(mappedDir);
         }

         HttpPostedFileBase hpf = Request.Files[0];
         string filename = CutCover(hpf, 480, mappedDir);
         //hpf.SaveAs(Path.Combine(mappedDir, hpf.FileName));

         string url = dirForSaving + "/" + filename;

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               name = hpf.FileName,
               path = url,
               showPath = Url.Content(url)
            });
         }
         else
         {
            return Content("upload ok");
         }
      }


      //
      //	文件 - 上传文件
      // POST:		/Attachment/UploadResource
      //

      [HttpPost]
      public ActionResult UploadResource()
      {
         if (Request.Files.Count != 1)
            return Content("Error");

         HttpPostedFileBase hpf = Request.Files[0];
         var md5 = FileHelper.ConvertToMD5(hpf.InputStream);
         var file = Files.ConditionQuery(f.Md5 == md5, null).FirstOrDefault();
         if (file == null)
         {
            var dirForSaving = GetDirForSaving(Guid.NewGuid());
            var mappedDir = Server.MapPath("~" + dirForSaving);
            var url = dirForSaving + "/" + hpf.FileName;
            var ext = Path.GetExtension(hpf.FileName);
            var absPath = Path.Combine(mappedDir, hpf.FileName);

            if (!Directory.Exists(mappedDir))
            {
               Directory.CreateDirectory(mappedDir);
            }

            hpf.SaveAs(absPath);

            //if file format is word then convert to html
            //if (ext==".doc" || ext==".docx")
            //{
            //   var saveHtmlPath = absPath.Replace(".doc", ".html").Replace(".docx", ".html");
            //   Util.Office.WordConverter.ConvertToHtml(absPath, saveHtmlPath);
            //}

            if (ext != null && ext.StartsWith("."))
               ext = ext.Substring(1);

            file = new Files { Md5 = md5, FileName = hpf.FileName, FilePath = url, ExtName = ext, FileSize = hpf.ContentLength }; //TODO
            db.FilesDal.Insert(file);
            file.FileId = Files.ConditionQuery(f.Md5 == md5, null).FirstOrDefault().FileId;
         }

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               fileId = file.FileId,
               name = file.FileName,
               path = file.FilePath,
               size = file.FileSize,
               ext = Path.GetExtension(file.ExtName),
            });
         }
         else
         {
            return Content("upload ok");
         }
      }

      public string CutCover(HttpPostedFileBase hpf, int targetWidth, string path)
      {
         int width, height;
         Image original = Image.FromStream(hpf.InputStream);
         width = original.Width;
         height = original.Height;

         if (width > 480)
         {
            string filename = hpf.FileName.Substring(0, hpf.FileName.LastIndexOf('.')) + ".jpg";
            Bitmap img = new Bitmap(original, targetWidth, targetWidth * height / width);
            img.Save(Path.Combine(path, filename), ImageFormat.Jpeg);
            return filename;
         }
         else
         {
            hpf.SaveAs(Path.Combine(path, hpf.FileName));
            return hpf.FileName;
         }
      }

      public static string GetDirForSaving(Guid guid)
      {
         return "/Attachments/" + DateTime.Today.ToString("yyyyMMdd") + "/" + guid.ToString();
      }



      //
      //	文件 - 上传用户头像
      // POST:		/Attachment/UploadPhoto
      //

      [HttpPost]
      public ActionResult UploadPhoto()
      {
         if (Request.Files.Count != 1)
            return Content("Error");

         string dirForSaving = GetDirForSaving(Guid.NewGuid());
         string mappedDir = Server.MapPath("~" + dirForSaving);
         if (!Directory.Exists(mappedDir))
         {
            Directory.CreateDirectory(mappedDir);
         }

         HttpPostedFileBase hpf = Request.Files[0];
         string filename = CutPhoto(hpf, 184, 184, mappedDir);
         //hpf.SaveAs(Path.Combine(mappedDir, hpf.FileName));

         string url = dirForSaving + "/" + filename;

         if (Request.IsAuthenticated)
         {
            APBplDef.ResUserBpl.UpdatePartial(ResSettings.SettingsInSession.UserId, new { PhotoPath = url });
         }

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               name = hpf.FileName,
               path = url,
               showPath = Url.Content(url)
            });
         }
         else
         {
            return Content("upload ok");
         }
      }


      public string CutPhoto(HttpPostedFileBase hpf, int targetWidth, int targetHeight, string path)
      {
         int width, height;
         Image original = Image.FromStream(hpf.InputStream);
         width = original.Width;
         height = original.Height;

         if (width > 480)
         {
            string filename = hpf.FileName.Substring(0, hpf.FileName.LastIndexOf('.')) + ".jpg";
            Bitmap img = new Bitmap(original, targetWidth, targetHeight);
            img.Save(Path.Combine(path, filename), ImageFormat.Jpeg);
            return filename;
         }
         else
         {
            hpf.SaveAs(Path.Combine(path, hpf.FileName));
            return hpf.FileName;
         }
      }





      //
      //	文件 - 上传资源封面图片
      // POST:		/Attachment/UpdateCover
      //

      //[HttpPost]
      //public ActionResult UpdateCover(long id)
      //{
      //   if (Request.Files.Count != 1)
      //      return Content("Error");

      //   string dirForSaving = GetDirForSaving(Guid.NewGuid());
      //   string mappedDir = Server.MapPath("~" + dirForSaving);
      //   if (!Directory.Exists(mappedDir))
      //   {
      //      Directory.CreateDirectory(mappedDir);
      //   }

      //   HttpPostedFileBase hpf = Request.Files[0];
      //   string filename = CutCover(hpf, 480, mappedDir);

      //   string url = dirForSaving + "/" + filename;
      //   APBplDef.ResResourceBpl.UpdatePartial(id, new { CoverPath = url });

      //   if (Request.IsAjaxRequest())
      //   {
      //      return Json(new
      //      {
      //         name = hpf.FileName,
      //         path = url,
      //         showPath = Url.Content(url)
      //      });
      //   }
      //   else
      //   {
      //      return Content("upload ok");
      //   }
      //}


   }

}