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

	public class ResourceController : BaseController
	{


		//
		// 列表
		// GET:		/Resource/More
		//

		public ActionResult More(string type, int page = 1)
		{
			var t = APDBDef.ResResource;
			int total = 0;


			if (type == "recommand")
			{
				ViewBag.ListOfMore = HomeRankingList(t.StarTotal.Desc, out total, 10, (page-1)*10);
				ViewBag.Title = "资源推荐";
			}
			else if (type == "viewcount")
			{
				ViewBag.ListOfMore = HomeRankingList(t.ViewCount.Desc, out total, 10, (page - 1) * 10);
				ViewBag.Title = "点击排名";
			}
			else if (type == "commentcount")
			{
				ViewBag.ListOfMore = HomeRankingList(t.CommentCount.Desc, out total, 10, (page - 1) * 10);
				ViewBag.Title = "评论排名";
			}
			else if (type == "downcount")
			{
				ViewBag.ListOfMore = HomeRankingList(t.DownCount.Desc, out total, 10, (page - 1) * 10);
				ViewBag.Title = "下载排名";
			}
			else if (type == "newresource")
			{
				ViewBag.ListOfMore = HomeRankingList(t.CreatedTime.Desc, out total, 10, (page - 1) * 10);
				ViewBag.Title = "最新资源";
			}


			// 分页器
			ViewBag.ParamType = type;
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;


			// 热门资源
			ViewBag.RankingOfViewCount = HomeRecommandList(t.ViewCount.Desc, out total, 15);
			// 最新评论
			ViewBag.RankingOfNewComment = HomeNewCommentList(15);

			return View();
		}


		//
		// 搜索
		// GET:		/Resource/Search
		//

		public ActionResult Search(int page = 1, string sort = "")
		{
			var t = APDBDef.ResResource;
			List<APSqlWherePhrase> where = new List<APSqlWherePhrase>();
			APSqlOrderPhrase order = null;

			#region [ Request Param ]

			string tmp = "";
			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Domain")))
			{
				where.Add(t.DomainPKID == Int64.Parse(tmp));
			}
			ViewData["Domain"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Deformity")))
			{
				where.Add(t.DeformityPKID == Int64.Parse(tmp));
			}
			ViewData["Deformity"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("ResourceType")))
			{
				where.Add(t.ResourceTypePKID == Int64.Parse(tmp));
			}
			ViewData["ResourceType"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Subject")))
			{
				where.Add(t.SubjectPKID == Int64.Parse(tmp));
			}
			ViewData["Subject"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Stage")))
			{
				where.Add(t.StagePKID == Int64.Parse(tmp));
			}
			ViewData["Stage"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Grade")))
			{
				long grade = Int64.Parse(tmp);
				if (grade == ResResourceHelper.GradeLow)
					where.Add(t.GradePKID.In(ResResourceHelper.Grade1, ResResourceHelper.Grade2, ResResourceHelper.Grade3, ResResourceHelper.GradeLow));
				else if (grade == ResResourceHelper.GradeMiddle)
					where.Add(t.GradePKID.In(ResResourceHelper.Grade4, ResResourceHelper.Grade5, ResResourceHelper.Grade6, ResResourceHelper.GradeMiddle));
				else if (grade == ResResourceHelper.GradeHigh)
					where.Add(t.GradePKID.In(ResResourceHelper.Grade7, ResResourceHelper.Grade8, ResResourceHelper.Grade9, ResResourceHelper.GradeHigh));
				else if (grade == ResResourceHelper.GradePrimary)
					where.Add(t.GradePKID.In(ResResourceHelper.Grade1, ResResourceHelper.Grade2, ResResourceHelper.Grade3, ResResourceHelper.Grade4, ResResourceHelper.Grade5, ResResourceHelper.GradePrimary, ResResourceHelper.GradeLow, ResResourceHelper.GradeMiddle));
				else if (grade == ResResourceHelper.GradeJunior)
					where.Add(t.GradePKID.In(ResResourceHelper.Grade6, ResResourceHelper.Grade7, ResResourceHelper.Grade8, ResResourceHelper.Grade9, ResResourceHelper.GradeJunior, ResResourceHelper.GradeHigh));

				else if (grade == ResResourceHelper.Grade1)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeLow, ResResourceHelper.GradePrimary));
				else if (grade == ResResourceHelper.Grade2)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeLow, ResResourceHelper.GradePrimary));
				else if (grade == ResResourceHelper.Grade3)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeLow, ResResourceHelper.GradePrimary));
				else if (grade == ResResourceHelper.Grade4)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeMiddle, ResResourceHelper.GradePrimary));
				else if (grade == ResResourceHelper.Grade5)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeMiddle, ResResourceHelper.GradePrimary));
				else if (grade == ResResourceHelper.Grade6)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeMiddle, ResResourceHelper.GradeJunior));
				else if (grade == ResResourceHelper.Grade7)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeHigh, ResResourceHelper.GradeJunior));
				else if (grade == ResResourceHelper.Grade8)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeHigh, ResResourceHelper.GradeJunior));
				else if (grade == ResResourceHelper.Grade9)
					where.Add(t.GradePKID.In(grade, ResResourceHelper.GradeHigh, ResResourceHelper.GradeJunior));
				else
					where.Add(t.GradePKID == Int64.Parse(tmp));
			}
			ViewData["Grade"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("SchoolType")))
			{
				where.Add(t.SchoolTypePKID == Int64.Parse(tmp));
			}
			ViewData["SchoolType"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("LearnFrom")))
			{
				where.Add(t.LearnFromPKID == Int64.Parse(tmp));
			}
			ViewData["LearnFrom"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("MediumType")))
			{
				where.Add(t.MediumTypePKID == Int64.Parse(tmp));
			}
			ViewData["MediumType"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Keywords")) && tmp.Trim() != "")
			{
				where.Add(t.Keywords.Match(tmp) | t.Title.Match(tmp) | (t.Code == tmp));
			}
			ViewData["Keywords"] = tmp;

			if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Sort")))
			{
				switch (tmp)
				{
					case "va": order = t.ViewCount.Asc; break;
					case "vd": order = t.ViewCount.Desc; break;
					case "da": order = t.DownCount.Asc; break;
					case "dd": order = t.DownCount.Desc; break;
					case "ca": order = t.CommentCount.Asc; break;
					case "cd": order = t.CommentCount.Desc; break;
					case "sa": order = t.StarTotal.Asc; break;
					case "sd": order = t.StarTotal.Desc; break;
				}
			}
			ViewData["Sort"] = tmp;

			#endregion

			int total = 0;

			ViewBag.ListOfMore = SearchResourceList(where.Count > 0 ? new APSqlConditionAndPhrase(where) : null, order, out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;

			return View();
		}


		//
		// 资源查看
		// GET:		/Resource/View
		//

		public ActionResult View(long id)
		{
			var model = APBplDef.ResResourceBpl.PrimaryGet(id);
			ViewBag.Title = model.Title;

			var t = APDBDef.ResResource;

			// 访问历史
			APBplDef.ResResourceBpl.CountingView(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

			ViewBag.CommentCount = APBplDef.ResCommentBpl.ConditionQueryCount(APDBDef.ResComment.ResourceId == id&APDBDef.ResComment.Audittype==1);

			// 相关资源
			ViewBag.RankingOfRelation = HomeRelationList(id, model.Keywords.Split(','), 15, model.FileExtName, APDBDef.ResResource.MediumTypePKID == model.MediumTypePKID);			
	
			int total = 0;
			// 热门资源
			ViewBag.RankingOfViewCount = HomeRecommandList(t.ViewCount.Desc, out total, 15, 0, t.MediumTypePKID == model.MediumTypePKID, model.FileExtName);


			ViewBag.RankingOfNewComment = HomeNewCommentList(15,t.MediumTypePKID == model.MediumTypePKID,model.FileExtName);
		
			model.GhostFileName = model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);





			int cou1 = 0;
			int cou2 = 0;
			int cou3 = 0;
			int cou4 = 0;
			int cou5 = 0;
			List<ResStar> resstartlist = ResstarList(id);
			foreach (ResStar cs in resstartlist)
			{
				switch (cs.Score)
				{
					case 1:
						{
							cou1 = Convert.ToInt32(cs.UserId);
							break;
						}
					case 2:
						{
							cou2 = Convert.ToInt32(cs.UserId);
							break;
						}
					case 3:
						{
							cou3 = Convert.ToInt32(cs.UserId);
							break;
						}
					case 4:
						{
							cou4 = Convert.ToInt32(cs.UserId);
							break;
						}
					case 5:
						{
							cou5 = Convert.ToInt32(cs.UserId);
							break;
						}
				}
			}

			ViewBag.count1 = cou1;
			ViewBag.count2 = cou2;
			ViewBag.count3 = cou3;
			ViewBag.count4 = cou4;
			ViewBag.count5 = cou5;
			int cou6 = cou1 + cou2 + cou3 + cou4 + cou5;
			ViewBag.count6 = cou6;

			if (cou6 == 0)
			{
			ViewBag.ct1 =0;
			ViewBag.ct2 = 0;
			ViewBag.ct3 = 0;
			ViewBag.ct4 = 0;
			ViewBag.ct5 = 0;
			}
			else { 
			ViewBag.ct1 = cou1 / cou6 * 100;
			ViewBag.ct2 = cou2 / cou6 * 100;
			ViewBag.ct3 = cou3 / cou6 * 100;
			ViewBag.ct4 = cou4 / cou6 * 100;
			ViewBag.ct5 = cou5 / cou6 * 100;
			}
			if (model.StarCount == 0) { ViewBag.avg = 0; }
			else
			{
			ViewBag.avg = model.StarTotal / model.StarCount;
				}
			return View(model);

		}

		//
		// 资源查看
		// GET:		/Resource/Favorite
		//
		public ActionResult Favorite(long id)

		{
			if (!Request.IsAuthenticated)
			{
				return Json(new
				{
					state = "failure",
					msg = "请您先登录再收藏！"
				});
			}
			else
			{
				var t = APDBDef.ResFavorite;
				if (APBplDef.ResFavoriteBpl.ConditionQueryCount(
					t.UserId == ResSettings.SettingsInSession.UserId & t.ResourceId == id) > 0)
				{
					return Json(new
					{
						state = "failure",
						msg = "您已经收藏过啦！"
					});

				}

				APBplDef.ResResourceBpl.CountingFavorite(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

				return Json(new
				{
					state = "ok",
					msg = "恭喜您，已经收藏成功！"
				});

			}
		}


		public ActionResult Download(long id)
		{
			if (!Request.IsAuthenticated)
			{
				return Json(new
				{
					state = "failure",
					msg = "请您先登录再下载！"
				});
			}
			else
			{
				var t = APDBDef.ResDownload;
				APBplDef.ResResourceBpl.CountingDownload(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

				return Json(new
				{
					state = "ok",
					//msg = "恭喜您，已经下载成功！"
				});

			}
		}


		[HttpPost]
		public ActionResult Star(long id, int value)
		{
			if (Request.IsAuthenticated && Request.IsAjaxRequest())
			{
				var t = APDBDef.ResResource;
				var t1 = APDBDef.ResStar;

				if (db.ResStarDal.ConditionQueryCount(t1.ResourceId == id & t1.UserId == ResSettings.SettingsInSession.UserId) == 0)
				{
					APBplDef.ResResourceBpl.CountingStar(db, id, value, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
				}

				return Content("allow");
			}

			return Content("deny");
		}



	}

}