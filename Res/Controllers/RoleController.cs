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

	public class RoleController : BaseController
	{

		//
		//	角色 - 首页
		// GET:		/Fole/Index
		//

		public ActionResult Index()
		{
			return View(APBplDef.ResRoleBpl.GetAll());
		}


		//
		//	角色 - 编辑/创建
		// GET:		/Role/Edit
		// POST:		/Role/Edit
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
		public ActionResult Edit(long? id, ResRole model, FormCollection fc)
		{
			if (id == null)
			{
				model.Insert();
			}
			else
			{
				model.Update();
			}

			// 设置权限
			List<long> ids = new List<long>();
			if (fc["Approve"] != null)
			{
				string[] ss = fc["Approve"].Split(',');
				foreach (var s in ss)
				{
					ids.Add(long.Parse(s));
				}
			}
			APBplDef.ResRoleApproveBpl.Sync(model.RoleId, ids);

			return RedirectToAction("Index");
		}


      //
      //	作品 - 删除
      // POST:		/Role/Delete
      //

      [HttpPost]
		public ActionResult Delete(long id)
		{
			if (Request.IsAjaxRequest())
			{

				if (APBplDef.ResUserRoleBpl.ConditionQueryCount(APDBDef.ResUserRole.RoleId == id) > 0)
					return Json(new { cmd = "Error", msg = "不可删除含有用户的角色。" });

				APBplDef.ResRoleBpl.PrimaryDelete(id);
				return Json(new { cmd = "Deleted", msg = "角色已删除。" });
			}

			return IsNotAjax();
		}

	}

}