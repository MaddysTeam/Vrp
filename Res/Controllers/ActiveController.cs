using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;

namespace Res.Controllers
{

	public class ActiveController : BaseController
	{

      static APDBDef.ActiveTableDef a = APDBDef.Active;

      //
      //	项目管理 - 首页
      // GET:		/Active/Index
      //

      public ActionResult Search()
		{
			return View();
		}

      [HttpPost]
      public ActionResult Search(int current, int rowCount, string searchPhrase)
      {
         return View();
      }


      //
      //	项目管理 - 编辑/创建
      // GET:		/Active/Edit
      // POST:		/Active/Edit
      //

      public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return Request.IsAjaxRequest() ? (ActionResult)PartialView() : View();
			}
			else
			{
				var model = APBplDef.ResRoleBpl.PrimaryGet(id.Value);
				return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
			}
		}

		[HttpPost]
		public ActionResult Edit(long? id, Active model, FormCollection fc)
		{
			if (id == null)
			{
				model.Insert();
			}
			else
			{
				model.Update();
			}

			return RedirectToAction("Index");
		}


      //
      //	资源 - 删除
      // POST:		/Active/Delete
      //

      [HttpPost]
		public ActionResult Delete(long id)
		{
			//if (Request.IsAjaxRequest())
			//{

			//	if (APBplDef.ResUserRoleBpl.ConditionQueryCount(APDBDef.ResUserRole.RoleId == id) > 0)
			//		return Json(new { cmd = "Error", msg = "不可删除含有用户的角色。" });

			//	APBplDef.ResRoleBpl.PrimaryDelete(id);
			//	return Json(new { cmd = "Deleted", msg = "角色已删除。" });
			//}

			return IsNotAjax();
		}

	}

}