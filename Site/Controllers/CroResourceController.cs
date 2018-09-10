﻿using System;
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
         List<APSqlWherePhrase> where = new List<APSqlWherePhrase>();
         APSqlOrderPhrase order = null;

         #region [ Request Param ]

         string tmp = "";
         //if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Domain")))
         //{
         //   where.Add(t.DomainPKID == Int64.Parse(tmp));
         //}
         //ViewData["Domain"] = tmp;

         //if (!String.IsNullOrEmpty(tmp = Request.Params.Get("Deformity")))
         //{
         //   where.Add(t.DeformityPKID == Int64.Parse(tmp));
         //}
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
            where.Add(t.GradePKID == Int64.Parse(tmp));
         }
         ViewData["Grade"] = tmp;

         //if (!String.IsNullOrEmpty(tmp = Request.Params.Get("SchoolType")))
         //{
         //   where.Add(t.SchoolTypePKID == Int64.Parse(tmp));
         //}
         //ViewData["SchoolType"] = tmp;

         //if (!String.IsNullOrEmpty(tmp = Request.Params.Get("LearnFrom")))
         //{
         //   where.Add(t.LearnFromPKID == Int64.Parse(tmp));
         //}
         ViewData["LearnFrom"] = tmp;

         //if (!String.IsNullOrEmpty(tmp = Request.Params.Get("MediumType")))
         //{
         //   where.Add(t.MediumTypePKID == Int64.Parse(tmp));
         //}
         ViewData["MediumType"] = tmp;

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

         ViewBag.ListOfMore = SearchCroResourceList(where.Count > 0 ? new APSqlConditionAndPhrase(where) : null, order, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         return View();
      }


      /// <summary>
      /// 更多资源
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
            if (type == "rmgd")
            {
               ViewBag.ListOfMore = CroHomeRankingList(t.EliteScore.Desc, null, out total, 10, (page - 1) * 10);
               ViewBag.Title = "热门资源";
            }
            else if (type == "zxgd")
            {
               ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, null, out total, 10, (page - 1) * 10);
               ViewBag.Title = "最新资源";
            }

            // 分页器
            ViewBag.ParamType = type;
            ViewBag.PageSize = 10;
            ViewBag.PageNumber = page;
            ViewBag.TotalItemCount = total;
            // 右侧热门资源
            ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5);
            // 右侧最新资源
            ViewBag.RankingROfNewCount = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc, null, out total, 5);

         }
         else
         {
            if (type == "rmgd")
            {
             //  ViewBag.ListOfMore = CroHomeRankingList(t.EliteScore.Desc, null, out total, 10, (page - 1) * 10, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);
               ViewBag.Title = "热门资源";
            }
            else if (type == "zxgd")
            {
             //  ViewBag.ListOfMore = CroHomeRankingList(t.CreatedTime.Desc, null, out total, 10, (page - 1) * 10, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);
               ViewBag.Title = "最新资源";
            }

            // 分页器
            ViewBag.ParamType = type;
            ViewBag.PageSize = 10;
            ViewBag.PageNumber = page;
            ViewBag.TotalItemCount = total;


            // 右侧热门资源
            //ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5, 0, t.MediumTypePKID == Int64.Parse(mediumtypepkid), FileExtName);
            //右侧最新资源
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
         //右侧热门资源
         ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5);

         return View();
      }


      /// <summary>
      /// 资源详情
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>


      public ActionResult ZcView(long id)
      {
         int total = 0;
         var model = APBplDef.CroResourceBpl.PrimaryGet(id);
         ViewBag.Title = model.Title;



         var t = APDBDef.CroResource;

         // 访问历史
         APBplDef.CroResourceBpl.CountingView(db, id, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);

         ViewBag.CommentCount = APBplDef.CroCommentBpl.ConditionQueryCount(APDBDef.CroComment
            .ResourceId == id & APDBDef.CroComment.Audittype == 1);

         //model.GhostFileName = Path.GetFullPath( model.ResourcePath); //model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);


         //// 相关资源
         //ViewBag.RankingOfRelation = CroHomeRelationList(id, model.Keywords.Split(','), 8, model.FileExtName, APDBDef.CroResource.MediumTypePKID == model.MediumTypePKID);

         ////右侧热门资源
         //ViewBag.RankingROfHotViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5, 0, t.MediumTypePKID == model.MediumTypePKID, model.FileExtName);

         ////右侧最新资源
         //ViewBag.RankingROfNewCount = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc, null, out total, 5, 0, t.MediumTypePKID == model.MediumTypePKID, model.FileExtName);

         //ViewBag.mediumtypepkid = model.MediumTypePKID;
         //ViewBag.FileExtName = model.FileExtName;

         int cou1 = 0;
         int cou2 = 0;
         int cou3 = 0;
         int cou4 = 0;
         int cou5 = 0;
         List<CroStar> crostartlist = CrostarList(id);
         foreach (CroStar cs in crostartlist)
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
            ViewBag.ct1 = 0;
            ViewBag.ct2 = 0;
            ViewBag.ct3 = 0;
            ViewBag.ct4 = 0;
            ViewBag.ct5 = 0;
         }
         else
         {
            ViewBag.ct1 = cou1 / cou6 * 100;
            ViewBag.ct2 = cou2 / cou6 * 100;
            ViewBag.ct3 = cou3 / cou6 * 100;
            ViewBag.ct4 = cou4 / cou6 * 100;
            ViewBag.ct5 = cou5 / cou6 * 100;
         }
         ViewBag.avg = 0;
         //if (model.StarCount == 0) { ViewBag.avg = 0; }
         //else
         //{
         //   ViewBag.avg = model.StarTotal / model.StarCount;
         //}
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
               //	msg = "恭喜您，已经下载成功！"
            });

         }
      }


      public ActionResult NewsView(long id)
      {
         int total = 0;
         var model = APBplDef.CroBulletinBpl.PrimaryGet(id);
         //右侧活跃用户
         ViewBag.RankingOfActiveUser = CroHomeActiveUserList(out total, 9);
         //右侧热门资源
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