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

   public class CroResourceController : CroBaseController
   {

      //
      // 搜索
      // GET:		/Resource/Search
      //

      public ActionResult Search(int page = 1, string sort = "")
      {
         var t = APDBDef.CroResource;
         var rc = APDBDef.ResCompany;
         List<APSqlWherePhrase> where = new List<APSqlWherePhrase>();
         APSqlOrderPhrase order = null;

         #region [ Request Param ]

         string tmp = "";
         List<ResCompany> schools = null, areas = null, provinces = null;

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
            where.Add(t.GradePKID == Int64.Parse(tmp));
         }
         ViewData["Grade"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Province")))
         {
            where.Add(rc.Path.Match(tmp));

            areas = ResCompanyHelper.GetChildren(Int64.Parse(tmp));
            schools = ResCompanyHelper.GetChildren(areas);
         }
         ViewData["Province"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Area")))
         {
            where.Add(rc.Path.Match(tmp));

            schools = ResCompanyHelper.GetChildren(Int64.Parse(tmp));
         }
         ViewData["Area"] = tmp;
         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("School")))
         {
            where.Add(t.CompanyId == Int64.Parse(tmp));
         }
         ViewData["School"] = tmp;

         //ViewData["Province"] = tmp;
         //if (!String.IsNullOrEmpty(tmp = Request.Params.Get("MediumType")))
         //{
         //   where.Add(t.MediumTypePKID == Int64.Parse(tmp));
         //}
         //ViewData["MediumType"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Keywords")) && tmp.Trim() != "")
         {
            where.Add(t.Keywords.Match(tmp) | t.Title.Match(tmp));
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
                  //case "sa": order = t.StarTotal.Asc; break;
                  //case "sd": order = t.StarTotal.Desc; break;
            }
         }
         ViewData["Sort"] = tmp;

         #endregion

         int total = 0;

         ViewBag.ListOfMore = SearchCroResourceList(where.Count > 0 ? new APSqlConditionAndPhrase(where) : null, order, out total, 5, (page - 1) * 5);

         // 分页器
         ViewBag.PageSize = 5;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         // 省市区学校
         ViewBag.Areas = areas ?? ResCompanyHelper.GetAreas();
         ViewBag.Schools = schools ?? ResCompanyHelper.GetSchools();
         ViewBag.Provinces = provinces ?? ResCompanyHelper.AllProvince();

         return View();
      }


      /// <summary>
      /// 更多作品
      /// </summary>
      /// <param name="type"></param>
      /// <param name="page"></param>
      /// <returns></returns>
      public ActionResult More(string type, int page = 1, string mediumtypepkid = null, string FileExtName = null)
      {
         var t = APDBDef.CroResource;
         int total = 0;

         if (mediumtypepkid == "null")
         {
            if (type == CroResourceHelper.Hot)
            {
               ViewBag.ListOfMore = CroHomeRankingList(t.EliteScore.Desc, null, out total, 10, (page - 1) * 10);
               ViewBag.Title = "热门作品";
            }
            else if (type == CroResourceHelper.Latest)
            {
               ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, null, out total, 10, (page - 1) * 10);
               ViewBag.Title = "最新作品";
            }

            // 分页器
            ViewBag.ParamType = type;
            ViewBag.PageSize = 10;
            ViewBag.PageNumber = page;
            ViewBag.TotalItemCount = total;
            // 右侧热门作品
            ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5);
            // 右侧最新作品
            ViewBag.RankingROfNewCount = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc, null, out total, 5);

         }
         else
         {
            if (type == "rmgd")
            {
               //  ViewBag.ListOfMore = CroHomeRankingList(t.EliteScore.Desc, null, out total, 10, (page - 1) * 10, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);
               ViewBag.Title = "热门作品";
            }
            else if (type == "zxgd")
            {
               //  ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, null, out total, 10, (page - 1) * 10, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);
               ViewBag.Title = "最新作品";
            }

            // 分页器
            ViewBag.ParamType = type;
            ViewBag.PageSize = 10;
            ViewBag.PageNumber = page;
            ViewBag.TotalItemCount = total;


            // 右侧热门作品
            //ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5, 0, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);
            // 右侧最新作品
            //ViewBag.RankingROfNewCount = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc, null, out total, 5, 0, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);

         }

         ViewBag.mediumtypepkid = mediumtypepkid;
         ViewBag.FileExtName = FileExtName;

         return View();
      }


      /// <summary>
      /// 更多公告
      /// </summary>
      /// <param name="type"></param>
      /// <param name="page"></param>
      /// <returns></returns>

      public ActionResult NewsMore(string type, int page = 1)
      {
         var t = APDBDef.CroBulletin;
         int total = 0;
         ViewBag.NewsListMore = CroBulltinList(t.CreatedTime.Desc, out total, 10, (page - 1) * 10);
         ViewBag.Title = "公告列表";
         // 分页器
         ViewBag.ParamType = type;
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         //右侧活跃用户
         ViewBag.RankingOfActiveUser = CroHomeActiveUserList(out total, 9);
         //右侧热门作品
         ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5);

         return View();
      }


      /// <summary>
      /// 作品详情
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public ActionResult ZcView(long id, long? courseId)
      {
         var model = APBplDef.CroResourceBpl.GetResource(db, id);
         ViewBag.Title = model.Title;

         // 访问历史
         APBplDef.CroResourceBpl.CountingView(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

         ViewBag.CurrentCourse = courseId == null || courseId.Value == 0 ? model.Courses[0] : model.Courses.Find(c => c.CourseId == courseId);

         ViewBag.CommentCount = APBplDef.CroCommentBpl.ConditionQueryCount(APDBDef.CroComment
            .ResourceId == id & APDBDef.CroComment.Audittype == 1);

         //var t = APDBDef.CroResource;

         //model.GhostFileName = Path.GetFullPath( model.ResourcePath); //model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);

         //// 相关作品
         //ViewBag.RankingOfRelation = CroHomeRelationList(id, model.Keywords.Split(','), 8, model.FileExtName, APDBDef.CroResource.MediumTypePKID == model.MediumTypePKID);

         ////右侧热门作品
         //ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5, 0, t.MediumTypePKID == model.MediumTypePKID, model.FileExtName);

         ////右侧最新作品
         //ViewBag.RankingROfNewCount = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc, null, out total, 5, 0, t.MediumTypePKID == model.MediumTypePKID, model.FileExtName);

         //ViewBag.mediumtypepkid = model.MediumTypePKID;
         //ViewBag.FileExtName = model.FileExtName;


         return View(model);

      }


      //
      // 作品查看
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
            var t = APDBDef.CroFavorite;
            if (APBplDef.CroFavoriteBpl.ConditionQueryCount(
               t.UserId == ResSettings.SettingsInSession.UserId & t.ResourceId == id) > 0)
            {
               return Json(new
               {
                  state = "failure",
                  msg = "您已经收藏过啦！"
               });

            }

            APBplDef.CroResourceBpl.CountingFavorite(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);


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
            var t = APDBDef.CroDownload;
            APBplDef.CroResourceBpl.CountingDownload(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
            return Json(new
            {
               state = "ok",
               msg = "恭喜您，已经下载成功！"
            });

         }
      }


      public ActionResult NewsView(long id)
      {
         int total = 0;
         var model = APBplDef.CroBulletinBpl.PrimaryGet(id);
         //右侧活跃用户
         ViewBag.RankingOfActiveUser = CroHomeActiveUserList(out total, 9);
         //右侧热门作品
         ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5);

         return View(model);

      }


      //[HttpPost]  TODO: will delete later
      //public ActionResult Star(long id, int value)
      //{
      //   if (Request.IsAuthenticated && Request.IsAjaxRequest())
      //   {
      //      var t = APDBDef.CroResource;
      //      var t1 = APDBDef.CroStar;

      //      if (db.CroStarDal.ConditionQueryCount(t1.ResourceId == id & t1.UserId == ResSettings.SettingsInSession.UserId) == 0)
      //      {
      //         APBplDef.CroResourceBpl.CountingStar(db, id, value, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
      //      }

      //      return Content("allow");
      //   }

      //   return Content("deny");
      //}

   }

}