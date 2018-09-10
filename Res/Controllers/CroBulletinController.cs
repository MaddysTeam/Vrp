using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;
using System.IO;

namespace Res.Controllers
{

	public class CroBulletinController : BaseController
	{

		public ActionResult Index()
		{
			return View(APBplDef.CroBulletinBpl.ConditionQuery(null,APDBDef.CroBulletin.CreatedTime.Desc,20));
		}


		//
		//	公告 - 编辑/创建
		// GET:		/Bulletin/Edit
		// POST:		/Bulletin/Edit
		//

		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return Request.IsAjaxRequest() ? (ActionResult)PartialView() : View();
			}
			else
			{
				var model = APBplDef.CroBulletinBpl.PrimaryGet(id.Value);
				return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(long? id, CroBulletin model, FormCollection fc)
		{


			if (id == null)
			{
				model.CreatedTime = DateTime.Now;

				APBplDef.CroBulletinBpl.Insert(model);
			
			}
			else
			{
				APBplDef.CroBulletinBpl.UpdatePartial(id.Value, new
				{
					model.Title,
					model.Content,
					model.ResourcePath,
					model.FileExtName,
					model.FileSize});
				
			}
			
			return RedirectToAction("Index");

		}


		//
		//	公告 - 删除
		// POST:		/Bulletin/Delete
		//

		[HttpPost]
		public ActionResult Delete(long id)
		{
			if (Request.IsAjaxRequest())
			{

				APBplDef.CroBulletinBpl.PrimaryDelete(id);
				return Json(new { cmd = "Deleted", msg = "公告已删除。" });
			}

			return IsNotAjax();
		}

		private string GetSafeExt(string path)
		{
			int idx = path.IndexOf('?');
			if (idx != -1)
				path = path.Substring(0, idx);
			return Path.GetExtension(path);
		}

	}

}