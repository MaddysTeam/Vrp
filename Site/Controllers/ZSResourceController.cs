using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Controllers
{

	public class ZSResourceController : BaseController
	{

		// 资源展示首页
		//	GET:				/ZSResource/Index

		[AllowAnonymous]
		public ActionResult Index()
		{
			var t = APDBDef.ZSResource;

			var model = db.ZSResourceDal.ConditionQuery(null, t.EliteScore.Desc, 8, null);

			ViewBag.Hot = db.ZSResourceDal.ConditionQuery(null, t.ViewCount.Desc, 5, null);

			return View(model.ToList());
		}


		// 资源展示平台
		//	GET:			/ZSResource/Search

		[AllowAnonymous]
		public ActionResult Search(string Level, string Level1 = "")
		{
			var t = APDBDef.ZSResource;

			var model = db.ZSResourceDal.ConditionQuery((Level1 == "" ? t.OneLevel == Level : (t.TowLevel == Level1)), null, null, null);

			ViewBag.Hot = db.ZSResourceDal.ConditionQuery(null, t.ViewCount.Desc, 5, null);

			ViewBag.Banner = db.ZSResourceDal.ConditionQuery(t.OneLevel == Level, null, null, null);

			return View(model.ToList());
		}


		// 资源展示详情
		//	GET:			/ZSResource/Detail

		[AllowAnonymous]
		public ActionResult Detail(long id)
		{
			var t = APDBDef.ZSResource;

			APQuery.update(t)
					.set(t.ViewCount, APSqlThroughExpr.Expr("ViewCount+1"))
					.where(t.ResourceId == id)
					.execute(db);

			var model = db.ZSResourceDal.PrimaryGet(id);

			return View(model);
		}

	}

}