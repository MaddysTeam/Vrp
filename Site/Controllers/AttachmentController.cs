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
using Util.ThirdParty.WangsuCloud;

namespace Res.Controllers
{

   public class AttachmentController : BaseController
   {

      static APDBDef.FilesTableDef f = APDBDef.Files;

      //
      //	文件 - 上传其他文件到 CDN
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
            var ext = Path.GetExtension(hpf.FileName);
            var anotherName = md5 + ext;
            // upload file to CDN Server
            var uploadFile = new UploadFile { Stream = hpf.InputStream, FileName = $"2019/files/{DateTime.Today.ToString("yyyyMMdd")}/{anotherName}" };
            var result = FileUploader.SliceUpload(uploadFile);

            if (null == result || null == result.FileUrl) return Content("上传失败");

            if (ext.ToLowerInvariant() == ".doc" || ext.ToLowerInvariant() == ".docx" )
            {
               Stream docStream = null;
               try
               {
                  docStream = Util.ThirdParty.Aspose.WordConverter.ConvertoPdf(hpf.InputStream);
                  var docFile = new UploadFile
                  {
                     Stream = docStream,
                     FileName = $"2019/files/{DateTime.Today.ToString("yyyyMMdd")}/{anotherName}{FileHelper.PdfExtName}"
                  };
                  var docResult=FileUploader.SliceUpload(docFile);
                  if (null == docResult || null == docResult.FileUrl || !docResult.IsSuccess) return Content("word 转pdf失败");
               }
               catch { }
               finally
               {
                  if (docStream != null)
                  {
                     docStream.Close();
                     docStream.Dispose();
                  }
               }
            }

            file = new Files { Md5 = md5, FileName = hpf.FileName, FilePath = result.FileUrl, ExtName = ext, FileSize = hpf.ContentLength };
            db.FilesDal.Insert(file);
         }

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               fileId = file.FileId,
               name = file.FileName,
               path = file.FilePath,
               size = file.FileSize,
               ext = file.ExtName
            });
         }
         else
         {
            return Content("upload ok");
         }
      }

      //
      //	文件 - 上传视频到 CDN  只允许mp4文件
      // POST:		/Attachment/UploadResource
      //
      [HttpPost]
      public ActionResult UploadVideo()
      {
         HttpPostedFileBase hpf = Request.Files[0];
         var md5 = FileHelper.ConvertToMD5(hpf.InputStream);
         var file = Files.ConditionQuery(f.Md5 == md5, null).FirstOrDefault();
         if (file == null)
         {
            string ext = Path.GetExtension(hpf.FileName);
            if (!string.Equals(ext,".mp4",StringComparison.InvariantCultureIgnoreCase))
               return Content("上传失败");

            // upload file to CDN Server
            var uploadFile = new UploadFile { Stream = hpf.InputStream, FileName = $"2019/videos/{DateTime.Today.ToString("yyyyMMdd")}/{md5+ext}" };
            var result = FileUploader.SliceUpload(uploadFile);
            if (result.IsSuccess)
            {
               // save file record
               file = new Files { Md5 = md5, FileName = hpf.FileName, FilePath = result.FileUrl, ExtName = ext, FileSize = hpf.ContentLength };
               db.FilesDal.Insert(file);
            }
         }

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               fileId = file.FileId,
               name = file.FileName,
               path = file.FilePath,
               size = file.FileSize,
               ext = file.ExtName
            });
         }
         else
         {
            return Content("upload ok");
         }
      }

      //
      //	文件 - 上传封面到 CDN
      // POST:		/Attachment/UploadResource
      //
      [HttpPost]
      public ActionResult UploadCover()
      {
         if (Request.Files.Count != 1)
            return Content("Error");

         HttpPostedFileBase hpf = Request.Files[0];
         var md5 = FileHelper.ConvertToMD5(hpf.InputStream);
         var file = Files.ConditionQuery(f.Md5 == md5, null).FirstOrDefault();
         if (null == file)
         {
            file = CutAndUploadCover(hpf, 480, md5);
            if (null == file) return Content("upload failure");
         }

         if (Request.IsAjaxRequest())
         {
            return Json(new
            {
               fileId = file.FileId,
               name = file.FileName,
               path = file.FilePath,
               showPath = file.FilePath,
            });
         }
         else
         {
            return Content("upload ok");
         }
      }

      /// <summary>
      ///  Upload and cut image if possible
      /// </summary>
      /// <param name="hpf"></param>
      /// <param name="targetWidth"></param>
      /// <returns></returns>
      protected Files CutAndUploadCover(HttpPostedFileBase hpf, int targetWidth, string md5)
      {
         int width, height;
         Image original = Image.FromStream(hpf.InputStream);
         Stream ms = new MemoryStream();
         width = original.Width;
         height = original.Height;

         if (width > 480)
         {
            var img = new Bitmap(original, targetWidth, targetWidth * height / width);
            img.Save(ms, ImageFormat.Jpeg);
         }
         else
         {
            ms = hpf.InputStream;
         }

         var filename = md5 + Path.GetExtension(hpf.FileName);
         var file = new UploadFile { Stream = ms, FileName = $"2019/pics/{DateTime.Today.ToString("yyyyMMdd")}/{filename}" };
         var result = FileUploader.SliceUpload(file);

         Files files = null;
         if (result.IsSuccess)
         {
            files = new Files { Md5 = md5, FileName = filename, FilePath = result.FileUrl, ExtName = Path.GetExtension(filename), FileSize = hpf.ContentLength };
            db.FilesDal.Insert(files);
         }

         ms.Close();
         ms.Dispose();

         return files;
      }

      [HttpPost]
      public ActionResult ShowProgress()
      {
         return Json(new
         {
            msg = Session["process"] ?? "0"
         });
      }

   }

}