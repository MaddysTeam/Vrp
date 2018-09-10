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

	public class BulletinController : BaseController
	{

		public ActionResult Index()
		{
			return View(APBplDef.ResBulletinBpl.ConditionQuery(null,APDBDef.ResBulletin.CreatedTime.Desc,5));
		}


		//
		//	公告 - morer
		// GET:		/Bulletin/More
		// POST:		/Bulletin/More
		//




		public ActionResult More(string type, int page = 1)
		{
			var t = APDBDef.ResBulletin;
			int total=0;
			ViewBag.RankingBulletin = HomeBulltinList(t.CreatedTime.Desc, out total, 10, (page - 1) * 10);
			ViewBag.Title = "公告列表";
			ViewBag.ParamType = type;
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;

			return View();
		}





		public ActionResult View(long id)
		{

			var model = APBplDef.ResBulletinBpl.PrimaryGet(id);

			return View(model);

		}


	}

}