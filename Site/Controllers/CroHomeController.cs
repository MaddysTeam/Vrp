
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Res;
using Symber.Web.Data;
using Res.Business;
using System.IO;
using System.Threading.Tasks;


namespace Res.Controllers
{

   public class CroHomeController : CroBaseController
   {



      //
      // 首页
      // GET:		/Home/Index
      //
      [AllowAnonymous]
      public ActionResult Index(string type)
      {
         int total;
         // 首页--活跃用户
         ViewBag.RankingOfActiveUser =  CroHomeActiveUserList(out total, 9);
         // 首页--热门作品
         ViewBag.RankingOfRMViewCount = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc, null, out total, 5);
         // 首页--公告
         ViewBag.RankingOfBulletin = HomeCroBulltinList(APDBDef.CroBulletin.CreatedTime.Desc, out total, 5);
         return View();
      }

      /// <summary>
      /// 最热排行榜
      /// </summary>
      [AllowAnonymous]
      public ActionResult Hot()
      {
         int total;
         var t = APDBDef.CroResource;

         var list = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc,null, out total, 8);
         return PartialView("_Hot", list);
      }

      /// <summary>
      /// 最新微课排行榜
      /// </summary>
      [AllowAnonymous]
      public ActionResult New()
      {
         int total;
         var t = APDBDef.CroResource;

         var list = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc,null, out total, 8);
         return PartialView("_New", list);
      }


      /// <summary>
      /// 微课得票排行榜
      /// </summary>
      [AllowAnonymous]
      public ActionResult Favorite()
      {
         int total;
         var t = APDBDef.CroResource;

         var list = CroHomeRankingList(APDBDef.CroResource.FavoriteCount.Desc, null, out total, 8);
         return PartialView("_Favorite", list);
      }


      /// <summary>
      /// 各省微课排行榜
      /// </summary>
      /// <param name="RType"></param>
      /// <returns></returns>
      [AllowAnonymous]
      public ActionResult Province(string proviceId)
      {
         int total;
         var rc = APDBDef.ResCompany;

         var list = CroHomeRankingList(APDBDef.CroResource.FavoriteCount.Desc, rc.Path.Match(proviceId), out total, 8);
         return PartialView("_New", list);
      }


   }

}