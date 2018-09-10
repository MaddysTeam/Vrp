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

	public class CroCommentController : BaseController
	{


		public ActionResult CroCommentList(long resid, int begin)
		{
			var t = APDBDef.CroComment;
			var t1 = APDBDef.ResUser;

			var list = APQuery.select(t.OccurId,t.Content, t.OccurTime, t1.RealName, t1.PhotoPath, t1.GenderPKID)
				.from(t, t1.JoinInner(t.UserId == t1.UserId))
				.where_and(t.ResourceId == resid & t.Audittype == 1)
				.order_by(t.OccurId.Desc)
				.primary(t.OccurId)
				.take(10)
				.skip(begin)
				.query(db, reader =>
				{
					var user = new ResActiveUser()
					{
						PhotoPath = t1.PhotoPath.GetValue(reader),
						GenderPKID = t1.GenderPKID.GetValue(reader)
					};
					return new
					{
						id = t.OccurId.GetValue(reader),
						name = t1.RealName.GetValue(reader),
						photoPath = user.FitPhotoPath,
						time = t.OccurTime.GetValue(reader).ToShortDateString(),
						content = t.Content.GetValue(reader)
					};
				}).ToList();


			return Json(list, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public ActionResult Submit(long resid, string content)
		{
			content = content.Trim();
			var t = APDBDef.CroResource;
			var t1 = APDBDef.ResUser;

			if (Request.IsAuthenticated && content != "")
			{

				APBplDef.CroResourceBpl.CountingComment(db, resid, content, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

				var user = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);
				var act = new ResActiveUser()
				{
					PhotoPath = user.PhotoPath,
					GenderPKID = user.GenderPKID
				};
				return Json(new
				{
					name = user.RealName,
					photoPath = act.FitPhotoPath,
					time = DateTime.Now.ToShortDateString(),
					content = SentisiveWordHelper.Replace(content,'*'),
					canDelete = true,
				});
			}

			return Content("未登录");
		}

		[HttpPost]
		public ActionResult Delete(long id)
		{
			APBplDef.CroCommentBpl.PrimaryDelete(id);
			return Content("");
		}
	}

}