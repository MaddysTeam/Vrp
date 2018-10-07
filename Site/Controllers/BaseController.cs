using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/**/
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

/**/


namespace Res.Controllers
{

	public class BaseController : Controller
	{

      #region [ 用户短查询 ]

      //public List<ResActiveUser> HomeActiveUserList(out int total, int take, int skip = 0)
      //{
      //   var t = APDBDef.ResUser;
      //   var t1 = APDBDef.ResFavorite;

      //   var query = APQuery.select(t.UserId, t.RealName, t.GenderPKID, t.PhotoPath, t1.UserId.Count().As("ViewCount"))
      //         .from(t, t1.JoinInner(t.UserId == t1.UserId))
      //         .where(t.Actived == true)
      //         .group_by(t.UserId, t.RealName, t.GenderPKID, t.PhotoPath)
      //         .order_by(new APSqlOrderPhrase(t1.UserId.Count(), APSqlOrderAccording.Desc))
      //         .primary(t.UserId)
      //         .take(take);
      //   if (skip != -1)
      //   {
      //      query.skip(skip);
      //      total = db.ExecuteSizeOfSelect(query);
      //   }
      //   else
      //   {
      //      total = 0;
      //   }
      //   return db.Query(query, reader =>
      //      {
      //         return new ResActiveUser()
      //         {
      //            UserId = t.UserId.GetValue(reader),
      //            RealName = t.RealName.GetValue(reader),
      //            GenderPKID = t.GenderPKID.GetValue(reader),
      //            PhotoPath = t.PhotoPath.GetValue(reader),
      //            ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount")),
      //         };
      //      }).ToList();

      //}

      #endregion

      #region [ 活动公告短查询 ]

      public List<CroBulletin> HomeCroBulltinList(APSqlOrderPhrase order, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroBulletin;
			var query = APQuery.select(t.BulletinId, t.Title, t.Content, t.CreatedTime)
				.from(t)
				.order_by(order)
				.primary(t.BulletinId)
				.take(take);

			if (skip != -1)
			{
				query.skip(skip);
				total = db.ExecuteSizeOfSelect(query);
			}
			else
			{
				total = 0;
			}

			return db.Query(query, reader =>
			{
				return new CroBulletin()
				{
					BulletinId = t.BulletinId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Content = t.Content.GetValue(reader),
					CreatedTime = t.CreatedTime.GetValue(reader),
				};
			}).ToList();

		}

		#endregion

		#region [ 公告 ]

		public List<CroBulletin> CroBulltinList(APSqlOrderPhrase order, out int total, int take, int skip = -1)
		{

			var t = APDBDef.CroBulletin;
			var query = APQuery.select(t.BulletinId, t.Title, t.Content, t.CreatedTime)
				.from(t)
				.order_by(new APSqlOrderPhrase(t.CreatedTime, APSqlOrderAccording.Desc))
				.primary(t.BulletinId)
				.take(take);
			if (skip != -1)
			{
				query.skip(skip);
				total = db.ExecuteSizeOfSelect(query);
			}
			else
			{
				total = 0;
			}


			return db.Query(query, reader =>
			{
				var des = t.Title.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);

				return new CroBulletin()
				{
					BulletinId = t.BulletinId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Content = t.Content.GetValue(reader),
					CreatedTime = t.CreatedTime.GetValue(reader),
				};
			}).ToList();
		}

		#endregion

		#region [ Base ]

		public BaseController()
		{
			db = new APDBDef();
		}

		protected APDBDef db { get; set; }

		protected override void Dispose(bool disposing)
		{
			if (db != null)
				db.Dispose();
			base.Dispose(disposing);
		}

		/// <summary>
		/// Not Ajax call.
		/// </summary>
		/// <returns></returns>
		protected ActionResult IsNotAjax()
		{
			return Content("Is Not Ajax.");
		}

		protected override void OnException(ExceptionContext filterContext)
		{
			// 标记异常已处理
			filterContext.ExceptionHandled = true;
			// 跳转到错误页
			filterContext.Result = View("Error", filterContext.Exception); //RedirectToAction("Error //RedirectResult(Url.Action("Error", "Shared"));
		}

		#endregion

	}

}