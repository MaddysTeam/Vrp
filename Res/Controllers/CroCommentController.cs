using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Symber.Web.Data;
using Res.Business;
using System.Collections.Generic;



namespace Res.Controllers
{

	
	public class CroCommentController : BaseController
	{
		public class comments
		{
			public string Audittype { get; set; }
			public string searchPhrase { get; set; }
		}
	
		public ActionResult Index()
		{
			return View();
		}






		public ActionResult Comment(string searchPhrase, string Audittype, int page = 1)
		{


			//if (string.IsNullOrEmpty(Audittype))
			//{
			//	ViewBag.Audittype = ResCommentTypeHelper.GetSelectList();
			//	ViewBag.auds = Audittype;
			//}
			//else
			//{
			//	List<System.Web.Mvc.SelectListItem> check = ResCommentTypeHelper.GetSelectList();
			//	foreach (SelectListItem item in check)
			//	{
			//		if (item.Value == Audittype)
			//		{
			//			item.Selected = true;
			//			break;
			//		}
			//	}
			//	ViewBag.auds = Audittype;
			//	ViewBag.Audittype = check;
			//}
			ViewBag.Audittype = ResCommentTypeHelper.GetSelectList();
			int total = 0;
			ViewBag.searchPhrase = searchPhrase;
			ViewBag.ListofResource = CrolistComment(searchPhrase,Audittype, out total, 10, (page - 1) * 10);
			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			return View();
		}



		[HttpPost]
		public ActionResult Delete(long id)
		{
			if (Request.IsAjaxRequest())
			{

				APBplDef.CroCommentBpl.PrimaryDelete(id);
				return Json(new { cmd = "Deleted", msg = "评论已删除。" });
			}

			return IsNotAjax();
		}



		[HttpPost]
		public ActionResult Audit(long id, string rtype)
		{
			if (Request.IsAjaxRequest())
			{
				CroComment model = new CroComment();
				model.Audittype =Convert.ToInt64(rtype);
				model.Auditor = ResSettings.SettingsInSession.UserId;
				model.AuditedTime = DateTime.Now;
				APBplDef.CroCommentBpl.UpdatePartial(id, new { model.Audittype,model.Auditor,model.AuditedTime
				});
				return Json(new { cmd = "Auditd", msg = "评论已经审核" });
			}
			return IsNotAjax();
		}
	}

}