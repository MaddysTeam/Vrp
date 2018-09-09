using Res.Business;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Controllers
{

	public class AttachmentController : Controller
	{

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

			string dirForSaving = GetDirForSaving(Guid.NewGuid());
			string mappedDir = Server.MapPath("~" + dirForSaving);
			if (!Directory.Exists(mappedDir))
			{
				Directory.CreateDirectory(mappedDir);
			}

			HttpPostedFileBase hpf = Request.Files[0];
			hpf.SaveAs(Path.Combine(mappedDir, hpf.FileName));

			string url = dirForSaving + "/" + hpf.FileName;
			string ext = Path.GetExtension(hpf.FileName);
			if (ext != null && ext.StartsWith("."))
				ext = ext.Substring(1);

			if (Request.IsAjaxRequest())
			{
				return Json(new
				{
					name = hpf.FileName,
					path = url,
					size = hpf.ContentLength,
					ext = Path.GetExtension(hpf.FileName)
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

		[HttpPost]
		public ActionResult UpdateCover(long id)
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

			string url = dirForSaving + "/" + filename;
			APBplDef.ResResourceBpl.UpdatePartial(id, new { CoverPath = url });

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


	}

}