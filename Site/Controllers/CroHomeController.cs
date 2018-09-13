
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
         ViewBag.RankingOfActiveUser = new List<ResActiveUser>(); // CroHomeActiveUserList(out total, 9);
         // 首页--资源库热门资源
         ViewBag.RankingOfRMViewCount = new List<ResResourceRanking>();// HomeRecommandList(APDBDef.ResResource.ViewCount.Desc, out total, 5);
         // 首页--公告
         ViewBag.RankingOfBulletin = HomeCroBulltinList(APDBDef.CroBulletin.CreatedTime.Desc, out total, 5);
         return View();
      }

      /// <summary>
      /// 热门资源
      /// </summary>
      /// <param name="isLink"></param>
      /// <returns></returns>
      [AllowAnonymous]
      public ActionResult Hot(string RType)
      {
         int total;
         var t = APDBDef.CroResource;

         var list = CroHomeRankingList(APDBDef.CroResource.EliteScore.Desc,null, out total, 8);
         return PartialView("_Hot", list);
      }

      /// <summary>
      /// 最新资源
      /// </summary>
      /// <param name="isLink"></param>
      /// <returns></returns>
      [AllowAnonymous]
      public ActionResult New(string RType)
      {
         int total;
         var t = APDBDef.CroResource;

         var list = CroHomeRankingList(APDBDef.CroResource.CreatedTime.Desc,null, out total, 8);
         return PartialView("_New", list);
      }



   }

}