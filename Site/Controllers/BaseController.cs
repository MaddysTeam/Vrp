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
      //#region [ 资源短查询 ]

      //public List<ResResourceRanking> HomeRecommandList(APSqlOrderPhrase order, out int total, int take, int skip = 0, APSqlWherePhrase moreWhere = null, string FileExtName = null)
      //{
      //	var t = APDBDef.ResResource;

      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.StarCount, t.StarTotal, t.ViewCount)
      //		.from(t)
      //		//.where(t.StatePKID == ResResourceHelper.StateAllow)
      //		.order_by(order, t.ResourceId.Asc)
      //		.primary(t.ResourceId)
      //		.take(take);


      //	if (moreWhere != null)
      //	{
      //		query.where_and(moreWhere);
      //	}
      //	if (FileExtName != null)
      //	{
      //		if (FileExtName == ".pdf")
      //		{
      //			query.where_and(t.FileExtName == ".pdf");
      //		}
      //		else
      //		{
      //			query.where_and(t.FileExtName != ".pdf");
      //		}

      //	}

      //	if (skip != 0)
      //	{
      //		query.skip(skip);
      //		total = db.ExecuteSizeOfSelect(query);
      //	}
      //	else
      //	{
      //		total = 0;
      //	}

      //	return db.Query(query, reader =>
      //	{
      //		return new ResResourceRanking()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			StarCount = t.StarCount.GetValue(reader),
      //			StarTotal = t.StarTotal.GetValue(reader),
      //			ViewCount = t.ViewCount.GetValue(reader),
      //		};
      //	}).ToList();
      //}

      //public List<ResResourceRanking> HomeRankingList(APSqlOrderPhrase order, out int total, int take, int skip = -1)
      //{
      //	var t = APDBDef.ResResource;

      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.StarCount, t.StarTotal,
      //		t.AuthorCompany, t.CreatedTime, t.ViewCount, t.CommentCount, t.DownCount, t.FileExtName, t.Description)
      //		.from(t)
      //		.where(t.StatePKID == ResResourceHelper.StateAllow)
      //		.order_by(order, t.ResourceId.Asc)
      //		.primary(t.ResourceId)
      //		.take(take);


      //	if (skip != -1)
      //	{
      //		query.skip(skip);
      //		total = db.ExecuteSizeOfSelect(query);
      //	}
      //	else
      //	{
      //		total = 0;
      //	}

      //	return db.Query(query, reader =>
      //	{
      //		var des = t.Description.GetValue(reader);
      //		if (des.Length > 60)
      //			des = des.Substring(0, 60) + " ...";
      //		return new ResResourceRanking()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			StarCount = t.StarCount.GetValue(reader),
      //			StarTotal = t.StarTotal.GetValue(reader),

      //			AuthorCompany = t.AuthorCompany.GetValue(reader),
      //			CreatedTime = t.CreatedTime.GetValue(reader),
      //			ViewCount = t.ViewCount.GetValue(reader),
      //			CommentCount = t.CommentCount.GetValue(reader),
      //			DownCount = t.DownCount.GetValue(reader),
      //			FileExtName = t.FileExtName.GetValue(reader),
      //			Description = des,
      //		};
      //	}).ToList();
      //}

      //public List<ResResourceRecommand> HomeNewCommentList(int take, APSqlWherePhrase moreWhere = null, string FileExtName = null)
      //{
      //	var t = APDBDef.ResResource;
      //	var t1 = APDBDef.ResComment;

      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.StarCount, t.StarTotal)
      //		.from(t, t1.JoinInner(t.ResourceId == t1.ResourceId))
      //		.where(t.StatePKID == ResResourceHelper.StateAllow)
      //		.order_by(t1.OccurTime.Desc, t.ResourceId.Asc)
      //		.primary(t.ResourceId)
      //		.take(take);

      //	if (moreWhere != null)
      //	{
      //		query.where_and(moreWhere);
      //	}
      //	if (FileExtName != null)
      //	{
      //		if (FileExtName == ".pdf")
      //		{
      //			query.where_and(t.FileExtName == ".pdf");
      //		}
      //		else
      //		{
      //			query.where_and(t.FileExtName != ".pdf");
      //		}

      //	}

      //	return db.Query(query, reader =>
      //	{
      //		return new ResResourceRecommand()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			StarCount = t.StarCount.GetValue(reader),
      //			StarTotal = t.StarTotal.GetValue(reader),
      //		};
      //	}).ToList();
      //}

      //public List<ResResourceRecommand> HomeRelationList(long selfId, string[] keywords, int take, string FileExtName, APSqlWherePhrase moreWhere = null)
      //{
      //	var t = APDBDef.ResResource;
      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.StarCount, t.StarTotal)
      //		.from(t)
      //		.where(t.StatePKID == ResResourceHelper.StateAllow & t.ResourceId != selfId)
      //		.order_by(t.CreatedTime.Desc)
      //		.primary(t.ResourceId)
      //		.take(take);

      //	if (moreWhere != null)
      //		query.where_and(moreWhere);

      //	if (FileExtName == ".pdf")
      //	{
      //		query.where_and(t.FileExtName == ".pdf");
      //	}
      //	else
      //	{
      //		query.where_and(t.FileExtName != ".pdf");
      //	}
      //	List<APSqlWherePhrase> like = new List<APSqlWherePhrase>();
      //	foreach (var key in keywords)
      //	{
      //		if (key != "")
      //			like.Add(t.Keywords.Match(key));
      //	}
      //	if (like.Count > 0)
      //	{
      //		query.where_and(new APSqlConditionOrPhrase(like));
      //	}

      //	return db.Query(query, reader =>
      //	{
      //		return new ResResourceRecommand()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			StarCount = t.StarCount.GetValue(reader),
      //			StarTotal = t.StarTotal.GetValue(reader),
      //		};
      //	}).ToList();
      //}

      //#endregion

      //#region [ 资源查询 ]

      //public List<ResResourceRanking> SearchResourceList(APSqlWherePhrase where, APSqlOrderPhrase order, out int total, int take, int skip = -1)
      //{
      //	var t = APDBDef.ResResource;

      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.StarCount, t.StarTotal,
      //		t.AuthorCompany, t.CreatedTime, t.ViewCount, t.CommentCount, t.DownCount, t.FileExtName, t.Description)
      //		.from(t)
      //		.where(t.StatePKID == ResResourceHelper.StateAllow)
      //		//.order_by(t.ResourceId.Asc)
      //		.primary(t.ResourceId)
      //		.take(take);

      //	if (where != null)
      //		query.where_and(where);

      //	if (order != null)
      //		query.order_by(order).order_by_add(t.ResourceId.Asc);
      //	else
      //		query.order_by(t.ResourceId.Asc);


      //	if (skip != -1)
      //	{
      //		query.skip(skip);
      //		total = db.ExecuteSizeOfSelect(query);
      //	}
      //	else
      //	{
      //		total = 0;
      //	}

      //	return db.Query(query, reader =>
      //	{
      //		var des = t.Description.GetValue(reader);
      //		if (des.Length > 100)
      //			des = des.Substring(0, 100);
      //		return new ResResourceRanking()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			StarCount = t.StarCount.GetValue(reader),
      //			StarTotal = t.StarTotal.GetValue(reader),

      //			AuthorCompany = t.AuthorCompany.GetValue(reader),
      //			CreatedTime = t.CreatedTime.GetValue(reader),
      //			ViewCount = t.ViewCount.GetValue(reader),
      //			CommentCount = t.CommentCount.GetValue(reader),
      //			DownCount = t.DownCount.GetValue(reader),
      //			FileExtName = t.FileExtName.GetValue(reader),
      //			Description = des,
      //		};
      //	}).ToList();
      //}

      //#endregion

      //#region [ 我的 ]

      //public List<ResMyResource> MyResource(out int total, int take, int skip = 0)
      //{
      //	var t = APDBDef.ResResource;

      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.FileExtName, t.Description, t.CreatedTime, t.StatePKID)
      //		.from(t)
      //		.where(t.Creator == ResSettings.SettingsInSession.UserId)
      //		.order_by(t.CreatedTime.Desc)
      //		.primary(t.ResourceId)
      //		.take(take)
      //		.skip(skip);

      //	total = db.ExecuteSizeOfSelect(query);

      //	return db.Query(query, reader =>
      //	{
      //		var des = t.Description.GetValue(reader);
      //		if (des.Length > 100)
      //			des = des.Substring(0, 100);

      //		return new ResMyResource()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			FileExtName = t.FileExtName.GetValue(reader),
      //			Description = des,
      //			OccurTime = t.CreatedTime.GetValue(reader),
      //			StatePKID = t.StatePKID.GetValue(reader)
      //		};
      //	}).ToList();
      //}

      //public List<ResMyResource> MyFavorite(long id, out int total, int take, int skip = 0)
      //{
      //	var t = APDBDef.ResResource;
      //	var t1 = APDBDef.ResFavorite;
      //	var userid = id;

      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.FileExtName, t.Description, t1.OccurTime, t1.OccurId)
      //		.from(t, t1.JoinInner(t.ResourceId == t1.ResourceId))
      //		.where(t1.UserId == userid)
      //		.order_by(t1.OccurTime.Desc)
      //		.primary(t.ResourceId)
      //		.take(take)
      //		.skip(skip);

      //	total = db.ExecuteSizeOfSelect(query);

      //	return db.Query(query, reader =>
      //	{
      //		var des = t.Description.GetValue(reader);
      //		if (des.Length > 100)
      //			des = des.Substring(0, 100);
      //		return new ResMyResource()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			FileExtName = t.FileExtName.GetValue(reader),
      //			Description = des,
      //			OccurTime = t1.OccurTime.GetValue(reader),
      //			OccurId = t1.OccurId.GetValue(reader),
      //		};
      //	}).ToList();
      //}





      //public List<ResMyResource> MyDownload(long id, out int total, int take, int skip = 0)
      //{
      //	var t = APDBDef.ResResource;
      //	var t1 = APDBDef.ResDownload;
      //	var userid = id;
      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.FileExtName, t.Description, t1.OccurTime, t1.OccurId)
      //		.from(t, t1.JoinInner(t.ResourceId == t1.ResourceId))
      //		.where(t1.UserId == userid)
      //		.order_by(t1.OccurTime.Desc)
      //		.primary(t.ResourceId)
      //		.take(take)
      //		.skip(skip);

      //	total = db.ExecuteSizeOfSelect(query);

      //	return db.Query(query, reader =>
      //	{
      //		var des = t.Description.GetValue(reader);
      //		if (des.Length > 100)
      //			des = des.Substring(0, 100);
      //		return new ResMyResource()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			FileExtName = t.FileExtName.GetValue(reader),
      //			Description = des,
      //			OccurTime = t1.OccurTime.GetValue(reader),
      //			OccurId = t1.OccurId.GetValue(reader),
      //		};
      //	}).ToList();
      //}


      //public List<ResMyResource> MyComment(long id, out int total, int take, int skip = 0)
      //{
      //	var t = APDBDef.ResResource;
      //	var t1 = APDBDef.ResComment;
      //	var userid = id;
      //	var query = APQuery.select(t.ResourceId, t.Title, t.Author, t.CoverPath, t.FileExtName, t.Description, t1.OccurTime, t1.OccurId, t1.Content)
      //		.from(t, t1.JoinInner(t.ResourceId == t1.ResourceId))
      //		.where(t1.UserId == userid)
      //		.order_by(t1.OccurTime.Desc)
      //		.primary(t.ResourceId)
      //		.take(take)
      //		.skip(skip);

      //	total = db.ExecuteSizeOfSelect(query);

      //	return db.Query(query, reader =>
      //	{
      //		var des = t.Description.GetValue(reader);
      //		if (des.Length > 100)
      //			des = des.Substring(0, 100);
      //		return new ResMyResource()
      //		{
      //			ResourceId = t.ResourceId.GetValue(reader),
      //			Title = t.Title.GetValue(reader),
      //			Author = t.Author.GetValue(reader),
      //			CoverPath = t.CoverPath.GetValue(reader),
      //			FileExtName = t.FileExtName.GetValue(reader),
      //			Description = des,
      //			OccurTime = t1.OccurTime.GetValue(reader),
      //			OccurId = t1.OccurId.GetValue(reader),
      //			Content = t1.Content.GetValue(reader),
      //		};
      //	}).ToList();
      //}

      //#endregion

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


      #region [ 众筹公告短查询 ]

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

		#region [ 众筹评分列表 ]

		public List<CroStar> CrostarList(long id)
		{

			var t = APDBDef.CroStar;
			var query = APQuery.select(t.Score.Count().As("UserId"), t.ResourceId, t.Score)
				.from(t)
				.where(t.ResourceId == id)
				.group_by(t.Score, t.ResourceId);


			return db.Query(query, reader =>
			{
				return new CroStar()
				{
					UserId = t.UserId.GetValue(reader),
					ResourceId = t.ResourceId.GetValue(reader),
					Score = t.Score.GetValue(reader),

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