using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
    
namespace Res.Controllers
{

	public class CroBaseController : BaseController
	{

		//以下为众筹资源

		#region [ 众筹资源查询 ]

		public List<MicroCourseRanking> SearchCroResourceList(APSqlWherePhrase where, APSqlOrderPhrase order, out int total, int take, int skip = -1)
		{
			var cr = APDBDef.CroResource;
         var mc = APDBDef.MicroCourse;
         var cf = APDBDef.Files;
         var rc = APDBDef.ResCompany;

         var query = APQuery.select(cr.CrosourceId, cr.Title, cr.Author, cr.FavoriteCount,cr.ProvinceId,cr.AreaId,cr.CompanyId,
				cr.AuthorCompany, cr.Description, cr.CreatedTime,rc.Path, //cr.ViewCount, cr.CommentCount, cr.DownCount //cr.FileExtName
            mc.CourseId, mc.CourseTitle, mc.PlayCount, cf.FilePath
            )
				.from(cr,
                  mc.JoinInner(mc.ResourceId==cr.CrosourceId),
                  rc.JoinInner(rc.CompanyId == cr.CompanyId),
                  cf.JoinLeft(cf.FileId==mc.CoverId)
                  )    
				.where(cr.StatePKID == CroResourceHelper.StateAllow)
				.order_by(cr.CreatedTime.Desc, cr.CrosourceId.Asc)
				.primary(cr.CrosourceId)
				.take(take);

			if (where != null)
				query.where_and(where);

			if (order != null)
				query.order_by(order).order_by_add(cr.CrosourceId.Asc);
			else
				query.order_by(cr.CrosourceId.Asc);

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
				var des = cr.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);
				return new MicroCourseRanking()
				{
               CourseId = mc.CourseId.GetValue(reader),
               CrosourceId = cr.CrosourceId.GetValue(reader),
               ResourceTitle=cr.Title.GetValue(reader),
               Title = mc.CourseTitle.GetValue(reader),
               Author = cr.Author.GetValue(reader),
               CoverPath = cf.FilePath.GetValue(reader),
               AuthorCompany = cr.AuthorCompany.GetValue(reader),
               CreatedTime = cr.CreatedTime.GetValue(reader),
               FavoriteCount = cr.FavoriteCount.GetValue(reader),
               CompanyPath = rc.Path.GetValue(reader),
               ProvinceId= cr.ProvinceId.GetValue(reader),
               AreaId=cr.AreaId.GetValue(reader),
               SchoolId=cr.CompanyId.GetValue(reader),
               //ViewCount = cr.ViewCouncr.GetValue(reader),
               //CommentCount = cr.CommentCouncr.GetValue(reader),
               //DownCount = cr.DownCouncr.GetValue(reader),
               //FileExtName = cr.FileExtName.GetValue(reader),
               Description = des,
            };
			}).ToList();
		}

		#endregion

		#region [我的众筹资源]

		public List<CroMyResource> MyCroResource(long id, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroResource;
			var userid = id;
			var query = APQuery.select(t.CrosourceId, t.Title, t.Author, //t.CoverPath, t.FileExtName, 
             t.Description, t.CreatedTime, t.AuditOpinion, t.StatePKID)
				.from(t)
				.where(t.Creator == userid)

				.order_by(t.CreatedTime.Desc)
				.primary(t.CrosourceId)
				.take(take)
				.skip(skip);

			total = db.ExecuteSizeOfSelect(query);

			return db.Query(query, reader =>
			{
				var des = t.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);

				return new CroMyResource()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
					OccurTime = t.CreatedTime.GetValue(reader),
					StatePKID = t.StatePKID.GetValue(reader),
					AuditOpinion = t.AuditOpinion.GetValue(reader)
				};
			}).ToList();
		}
		#endregion

		#region [我的众筹收藏资源]

		public List<CroMyResource> MyCroFavorite(long id, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroResource;
			var t1 = APDBDef.CroFavorite;
			var userid = id;
			var query = APQuery.select(t.CrosourceId, t.Title, t.Author
            //t.CoverPath, t.FileExtName, 
            ,t.Description, t1.OccurTime, t1.OccurId)
				.from(t, t1.JoinInner(t.CrosourceId == t1.ResourceId))
				.where(t1.UserId == userid)
				.order_by(t1.OccurTime.Desc)
				.primary(t.CrosourceId)
				.take(take)
				.skip(skip);

			total = db.ExecuteSizeOfSelect(query);

			return db.Query(query, reader =>
			{
				var des = t.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);
				return new CroMyResource()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
					OccurTime = t1.OccurTime.GetValue(reader),
					OccurId = t1.OccurId.GetValue(reader),
				};
			}).ToList();
		}
		#endregion

		#region [我的众筹评价资源]

		public List<CroMyResource> MyCroComment(long id, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroResource;
			var t1 = APDBDef.CroComment;
			var userid = id;
			var query = APQuery.select(t.CrosourceId, t.Title, t.Author, 
            //t.CoverPath, t.FileExtName, 
            t.Description, t1.OccurTime, t1.OccurId, t1.Content)
				.from(t, t1.JoinInner(t.CrosourceId == t1.ResourceId))
				.where(t1.UserId == userid)
				.order_by(t1.OccurTime.Desc)
				.primary(t1.OccurId)
				.take(take)
				.skip(skip);

			total = db.ExecuteSizeOfSelect(query);

			return db.Query(query, reader =>
			{
				var des = t.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);
				return new CroMyResource()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
					OccurTime = t1.OccurTime.GetValue(reader),
					OccurId = t1.OccurId.GetValue(reader),
					Content = t1.Content.GetValue(reader),
				};
			}).ToList();
		}

		#endregion

		#region [我的众筹推荐资源]
		//我的推荐


		public List<CroMyResource> CroRecommandList(long id, APSqlOrderPhrase order, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroResource;
			var userid = id;
			var query = APQuery.select(t.CrosourceId, t.Title, t.Author,
           // t.CoverPath, t.FileExtName, 
            t.Description, t.CreatedTime, t.StatePKID)
				.from(t)
				.where(t.Creator == userid)
				.order_by(t.CreatedTime.Desc)
				.primary(t.CrosourceId)
				.take(take)
				.skip(skip);

			total = db.ExecuteSizeOfSelect(query);

			return db.Query(query, reader =>
			{
				var des = t.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);

				return new CroMyResource()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
					OccurTime = t.CreatedTime.GetValue(reader),
					StatePKID = t.StatePKID.GetValue(reader)
				};
			}).ToList();

		}

		#endregion

		#region [我的众筹下载资源]

		public List<CroMyResource> MyCroDownload(long id, out int total, int take, int skip = 0)
		{
			var t = APDBDef.CroResource;
			var t1 = APDBDef.CroDownload;
			var userid = id;
			var query = APQuery.select(t.CrosourceId, t.Title, t.Author,
            //t.CoverPath, t.FileExtName,
            t.Description, t1.OccurTime, t1.OccurId)
				.from(t, t1.JoinInner(t.CrosourceId == t1.ResourceId))
				.where(t1.UserId == userid)
				.order_by(t1.OccurTime.Desc)
				.primary(t.CrosourceId)
				.take(take)
				.skip(skip);

			total = db.ExecuteSizeOfSelect(query);

			return db.Query(query, reader =>
			{
				var des = t.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);
				return new CroMyResource()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
					OccurTime = t1.OccurTime.GetValue(reader),
					OccurId = t1.OccurId.GetValue(reader),
				};
			}).ToList();
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="order"></param>
		/// <param name="rtype">1是原创，2是网络推荐</param>
		/// <param name="total"></param>
		/// <param name="take"></param>
		/// <param name="skip"></param>
		/// <returns></returns>
		public List<MicroCourseRanking> CroHomeRankingList(APSqlOrderPhrase order, APSqlWherePhrase where, out int total, int take, int skip = -1, APSqlWherePhrase moreWhere = null, string FileExtName = null)
		{
			var cr = APDBDef.CroResource;
         var mc = APDBDef.MicroCourse;
         var cf = APDBDef.Files;
         var rc = APDBDef.ResCompany;

         var query = APQuery.select(cr.CrosourceId, cr.Title, cr.Author, cr.FavoriteCount, //t.CoverPath,
            cr.AuthorCompany, cr.CreatedTime,rc.Path, //cr.ViewCount, cr.CommentCount, cr.DownCount, //t.FileExtName, 
            cr.Description,mc.CourseId,mc.CourseTitle,mc.PlayCount,cf.FilePath)
				.from(mc,
                  cr.JoinLeft(mc.ResourceId==cr.CrosourceId),
                  cf.JoinLeft(cf.FileId==mc.CoverId),
                  rc.JoinInner(rc.CompanyId == cr.CompanyId)
                  )
            .where(cr.StatePKID == CroResourceHelper.StateAllow)
            .order_by(order, cr.CrosourceId.Asc)
				.primary(mc.CourseId)
				.take(take);

			if (where != null)
				query.where_and(where);

			if (moreWhere != null)
			{
				query.where_and(moreWhere);
			}

			if (FileExtName != null)
			{
				if (FileExtName == ".pdf")
				{
					//query.where_and(t.FileExtName == ".pdf");
				}
				else
				{
					//query.where_and(t.FileExtName != ".pdf");
				}

			}

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
				var des = cr.Description.GetValue(reader);
				if (des.Length > 100)
					des = des.Substring(0, 100);
            return new MicroCourseRanking()
            {
               CourseId = mc.CourseId.GetValue(reader),
               CrosourceId = cr.CrosourceId.GetValue(reader),
               Title = mc.CourseTitle.GetValue(reader),
               Author = cr.Author.GetValue(reader),
               CoverPath = cf.FilePath.GetValue(reader),
               AuthorCompany = cr.AuthorCompany.GetValue(reader),
               CreatedTime = cr.CreatedTime.GetValue(reader),
               FavoriteCount = cr.FavoriteCount.GetValue(reader),
               CompanyPath = rc.Path.GetValue(reader),
					//ViewCount = t.ViewCount.GetValue(reader),
					//CommentCount = t.CommentCount.GetValue(reader),
					//DownCount = t.DownCount.GetValue(reader),
					//FileExtName = t.FileExtName.GetValue(reader),
					Description = des,
				};
			}).ToList();
		}

		/// <summary>
		/// 
		/// 众筹相关资源
		/// </summary>
		/// <param name="selfId"></param>
		/// <param name="keywords"></param>
		/// <param name="take"></param>
		/// <param name="FileExtName"></param>
		/// <param name="moreWhere"></param>
		/// <returns></returns>

		public List<MicroCourseRanking> CroHomeRelationList(long selfId, string[] keywords, int take, string FileExtName, APSqlWherePhrase moreWhere = null)
		{
			var t = APDBDef.CroResource;
			var query = APQuery.select(t.CrosourceId, t.Title, t.Author //t.CoverPath
           // t.StarCount, t.StarTotal
            )
				.from(t)
				.where(t.StatePKID == CroResourceHelper.StateAllow & t.CrosourceId != selfId)
				.order_by(t.CreatedTime.Desc)
				.primary(t.CrosourceId)
				.take(take);

			if (moreWhere != null)
				query.where_and(moreWhere);

			//if (FileExtName == ".pdf")
			//{
			//	query.where_and(t.FileExtName == ".pdf");
			//}
			//else
			//{
			//	query.where_and(t.FileExtName != ".pdf");
			//}
			List<APSqlWherePhrase> like = new List<APSqlWherePhrase>();
			foreach (var key in keywords)
			{
				if (key != "")
					like.Add(t.Keywords.Match(key));
			}
			if (like.Count > 0)
			{
				query.where_and(new APSqlConditionOrPhrase(like));
			}

			return db.Query(query, reader =>
			{
				return new MicroCourseRanking()
				{
					CrosourceId = t.CrosourceId.GetValue(reader),
					Title = t.Title.GetValue(reader),
					Author = t.Author.GetValue(reader),
					//CoverPath = t.CoverPath.GetValue(reader),
					//StarCount =0, //t.StarCount.GetValue(reader),
					//StarTotal =0, //t.StarTotal.GetValue(reader),
				};
			}).ToList();
		}


		//众筹活跃用户

		#region [ 用户短查询 ]

		public List<ResActiveUser> CroHomeActiveUserList(out int total, int take, int skip = 0)
		{
			var t = APDBDef.ResUser;
			var t1 = APDBDef.CroFavorite;

			var query = APQuery.select(t.UserId, t.RealName, t.GenderPKID, t.PhotoPath, t1.UserId.Count().As("ViewCount"))
					.from(t, t1.JoinInner(t.UserId == t1.UserId))
					.where(t.Actived == true)
					.group_by(t.UserId, t.RealName, t.GenderPKID, t.PhotoPath)
					.order_by(new APSqlOrderPhrase(t1.UserId.Count(), APSqlOrderAccording.Desc))
					.primary(t.UserId)
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
				return new ResActiveUser()
				{
					UserId = t.UserId.GetValue(reader),
					RealName = t.RealName.GetValue(reader),
					GenderPKID = t.GenderPKID.GetValue(reader),
					PhotoPath = t.PhotoPath.GetValue(reader),
					ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount")),
				};
			}).ToList();

		}


		//public List<ResActiveUser> HomeCroActiveUserList(int count)
		//{
		//	var t = APDBDef.ResUser;
		//	var t1 = APDBDef.CroFavorite;
		//	return
		//	APQuery.select(t.UserId, t.RealName, t.GenderPKID, t.PhotoPath, t1.UserId.Count().As("ViewCount"))
		//		.from(t, t1.JoinInner(t.UserId == t1.UserId))
		//		.where(t.Actived == true)
		//		.group_by(t.UserId, t.RealName, t.GenderPKID, t.PhotoPath)
		//		.order_by(new APSqlOrderPhrase(t1.UserId.Count(), APSqlOrderAccording.Desc))
		//		.take(count)
		//		.query(db, reader =>
		//		{
		//			return new ResActiveUser()
		//			{
		//				UserId = t.UserId.GetValue(reader),
		//				RealName = t.RealName.GetValue(reader),
		//				GenderPKID = t.GenderPKID.GetValue(reader),
		//				PhotoPath = t.PhotoPath.GetValue(reader),
		//				ViewCount = reader.GetInt32(reader.GetOrdinal("ViewCount")),
		//			};
		//		}).ToList();

		//}

		#endregion

	}

}