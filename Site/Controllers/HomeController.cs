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

	public class HomeController : BaseController
	{

		//
		// 首页
		// GET:		/Home/Index
		//

		public ActionResult Index()
		{
			//{
			//	var list = db.ResUserDal.ConditionQuery(null, null, null, null);
			//	var t = APDBDef.ResUser;
			//	var random = new Random();
			//	foreach (var item in list)
			//	{
			//		APQuery.update(t).set(t.PhotoPath, String.Format("/Attachments/face/{0}.jpg", random.Next(1, 24)))
			//			.where(t.UserId == item.UserId)
			//			.execute(db);
			//	}
			//}
			//{
			//	var list = db.ResResourceDal.ConditionQuery(null, null, null, null);
			//	var t = APDBDef.ResResource;
			//	var random = new Random();
			//	foreach (var item in list)
			//	{

			//		APQuery.update(t).set(t.CoverPath, String.Format("/Attachments/demo/{0}.jpg", random.Next(1, 24)))
			//			.where(t.ResourceId == item.ResourceId)
			//			.execute(db);
			//	}
			//}


			int total;

			List<ResResourceRecommand>
				page1 = new List<ResResourceRecommand>(),
				page2 = new List<ResResourceRecommand>();
			int i = 0;
			foreach(var item in HomeRecommandList(APDBDef.ResResource.EliteScore.Desc, out total, 12)){
				if (i < 6)
					page1.Add(item);
				else
					page2.Add(item);
				i++;
			}
			ViewBag.RankingOfRecommandPage1 = page1;
			ViewBag.RankingOfRecommandPage2 = page2;

			ViewBag.RankingOfViewCount = HomeRankingList(APDBDef.ResResource.ViewCount.Desc, out total, 3);
			ViewBag.RankingOfCommentCount = HomeRankingList(APDBDef.ResResource.CommentCount.Desc, out total, 3);
			ViewBag.RankingOfDownCount = HomeRankingList(APDBDef.ResResource.DownCount.Desc, out total, 3);
         ViewBag.RankingOfActiveUser = null;//HomeActiveUserList(out total, 15);


         ViewBag.RankingOfBulletin = HomeBulltinList(APDBDef.ResBulletin.CreatedTime.Desc, out total, 3);
			return View();
		}


	}

}