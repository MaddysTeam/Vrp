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
      // GET:		/CroResurce/Search
      //

      public ActionResult Search(int page = 1, string sort = "")
      {
         var t = APDBDef.CroResource;
         var rc = APDBDef.ResCompany;
         var mc = APDBDef.MicroCourse;
         List<APSqlWherePhrase> where = new List<APSqlWherePhrase>();
         APSqlOrderPhrase order = null;

         #region [ Request Param ]

         string tmp = "";
         List<ResCompany> schools = null, areas = null, provinces = null;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Active")))
         {
            where.Add(t.ActiveId == Int64.Parse(tmp));
         }
         ViewData["Active"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("CourseType")))
         {
            where.Add(t.CourseTypePKID == Int64.Parse(tmp));
         }
         ViewData["CourseType"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Subject")))
         {
            where.Add(t.SubjectPKID == Int64.Parse(tmp));
         }
         ViewData["Subject"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Grade")))
         {
            where.Add(t.GradePKID == Int64.Parse(tmp));
         }
         ViewData["Grade"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Province")))
         {
            where.Add(rc.Path.Match(tmp));
            ResSettings.SettingsInSession.CleanCompanyCache();
            var companies = ResSettings.SettingsInSession.Companies;
            areas = ResCompanyHelper.GetChildren(companies, Int64.Parse(tmp));
            schools = ResCompanyHelper.GetChildren(companies, areas);
         }
         ViewData["Province"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Area")))
         {
            where.Add(rc.Path.Match(tmp));

            schools = ResCompanyHelper.GetChildren(ResSettings.SettingsInSession.Companies, Int64.Parse(tmp));
         }
         ViewData["Area"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("School")))
         {
            where.Add(t.CompanyId == Int64.Parse(tmp));
         }
         ViewData["School"] = tmp;

         if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Keywords")) && tmp.Trim() != "")
         {
            where.Add(t.Keywords.Match(tmp) | t.Title.Match(tmp) | t.Author.Match(tmp) | t.Description.Match(tmp) | mc.CourseTitle.Match(tmp));
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

         ViewBag.ListOfMore = SearchCroResourceList(where.Count > 0 ? new APSqlConditionAndPhrase(where) : null, order, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         // 省市区学校
         ViewBag.Areas = areas ?? ResSettings.SettingsInSession.AllAreas();
         ViewBag.Schools = schools ?? ResSettings.SettingsInSession.AllSchools();
         ViewBag.Provinces = provinces ?? ResSettings.SettingsInSession.AllProvince();

         return View();
      }


      /// <summary>
      /// 更多作品
      /// </summary>
      /// <param name="type"></param>
      /// <param name="page"></param>
      /// <returns></returns>
      public ActionResult More(string type, int page = 1)
      {
         var t = APDBDef.CroResource;
         int total = 0;

         if (type == CroResourceHelper.Hot)
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.ViewCount.Desc, null, out total, 10, (page - 1) * 10);
            ViewBag.Title = "热门微课";
         }
         else if (type == CroResourceHelper.Praise)
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.PraiseCount.Desc, null, out total, 10, (page - 1) * 10);
            ViewBag.Title = "得票微课";
         }
         else if (type == CroResourceHelper.Latest)
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, null, out total, 10, (page - 1) * 10);
            ViewBag.Title = "最新微课";
         }
         else if (type == CroResourceHelper.Jiangsu.ToString())
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, t.ProvinceId == CroResourceHelper.Jiangsu, out total, 10, (page - 1) * 10);
            ViewBag.Title = "江苏省";
         }
         else if (type == CroResourceHelper.Zhejiang.ToString())
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, t.ProvinceId == CroResourceHelper.Zhejiang, out total, 10, (page - 1) * 10);
            ViewBag.Title = "浙江省";
         }
         else if (type == CroResourceHelper.Shanghai.ToString())
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, t.ProvinceId == CroResourceHelper.Shanghai, out total, 10, (page - 1) * 10);
            ViewBag.Title = "上海市";
         }
         else if (type == CroResourceHelper.Anhui.ToString())
         {
            ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, t.ProvinceId == CroResourceHelper.Anhui, out total, 10, (page - 1) * 10);
            ViewBag.Title = "安徽省";
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

         var currentCourse = courseId == null || courseId.Value == 0 ? model.Courses[0] : model.Courses.Find(c => c.CourseId == courseId);

         // 访问历史
         APBplDef.CroResourceBpl.CountingView(db, id, currentCourse.CourseId, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

         //当前微课
         ViewBag.CurrentCourse = currentCourse;

         //评论数量
         ViewBag.CommentCount = APBplDef.CroCommentBpl.ConditionQueryCount(APDBDef.CroComment
            .ResourceId == id & APDBDef.CroComment.Audittype == 1);

         int total = 10;
         //右侧热门作品
         ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5, 0);

         //右侧最新作品
         ViewBag.RankingROfNewCount = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc, null, out total, 5, 0);


         return View(model);
      }


      //
      // 作品查看
      // GET:		/CroResurce/Favorite
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


      public ActionResult Download(long id, long courseId, long fileId)
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
            APBplDef.CroResourceBpl.CountingDownload(db, id, courseId, fileId, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
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


      //
      // 作品点赞
      // POST:		/CroResurce/Praise
      //

      [HttpPost]
      public ActionResult Praise(long id)
      {
         if (!Request.IsAuthenticated)
         {
            return Json(new
            {
               state = "failure",
               msg = "请您先登录再点赞！"
            });
         }
         else
         {
            var cp = APDBDef.CroPraise;
            if (APBplDef.CroPraiseBpl.ConditionQueryCount(
               cp.UserId == ResSettings.SettingsInSession.UserId & cp.ResourceId == id) > 0)
            {
               return Json(new
               {
                  state = "failure",
                  msg = "无法重复点赞！"
               });

            }
            APBplDef.CroResourceBpl.CountingPraise(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
            return Json(new
            {
               state = "ok",
               msg = "恭喜您，已经点赞成功！"
            });

         }
      }


      //
      // 微课视频点击
      // POSt:		/CroResurce/Play
      //

      [HttpPost]
      public ActionResult Play(long id, long courseId)
      {
         var userId = Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0;
         APBplDef.MicroCourseBpl.CountingPlay(db, userId, id, courseId);

         return Json(new
         {
            state = "ok",
            msg = "恭喜您，已经播放成功！"
         });
      }


      [HttpPost]
      public ActionResult Star(long id, long courseId, int value)
      {
         if (Request.IsAuthenticated && Request.IsAjaxRequest())
         {
            var t = APDBDef.CroResource;
            var t1 = APDBDef.CroStar;

           // if (db.CroStarDal.ConditionQueryCount(t1.ResourceId == id & t1.UserId == ResSettings.SettingsInSession.UserId) == 0)
          //  {
               APBplDef.CroResourceBpl.CountingStar(db, value, id, courseId, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
           // }

            return Content("allow");
         }

         return Content("deny");
      }


      //
      // 在线答题列表
      // POSt:		/CroResurce/ExerciesList
      //

      public ActionResult ExerciesList(long courseId)
      {
         var exe = APDBDef.Exercises;
         var exei = APDBDef.ExercisesItem;

         var list = APQuery.select(exe.Name, exe.Answer, exe.ExerciseId, exei.Item, exei.ItemId, exei.Code)
                       .from(exe, exei.JoinInner(exe.ExerciseId == exei.ExerciseId))
                       .where(exe.CourseId == courseId)
                       .query(db, r =>
                         new
                         {
                            itemId = exei.ItemId.GetValue(r),
                            item = exei.Item.GetValue(r),
                            code = exei.Code.GetValue(r),
                            answer = exe.Answer.GetValue(r),
                            exeName = exe.Name.GetValue(r),
                            exeId = exe.ExerciseId.GetValue(r)
                         }
                       ).ToList();

         var models = new List<Exercises>();

         foreach (var item in list)
         {
            var model = models.FirstOrDefault(x => x.ExerciseId == item.exeId);
            if (null == model)
            {
               model = new Exercises { ExerciseId = item.exeId, Answer = item.answer, Name = item.exeName };
               model.Items = new List<ExercisesItem>();
               models.Add(model);
            }

            model.Items.Add(new ExercisesItem { Code = item.code, Item = item.item, ItemId = item.itemId });
         }

         return PartialView("_exercies", models);
      }

   }

}