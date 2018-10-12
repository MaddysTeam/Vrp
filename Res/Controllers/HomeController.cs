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

		public ActionResult Index()
		{
         if (ResSettings.SettingsInSession.User.UserTypePKID == ResUserHelper.Export)
         {
            return RedirectToAction("Search", "Eval");
         }

			return View();
		}


		//
		// 仪表板
		// GET:		/Home/Dashboard
		//

		public ActionResult Dashboard()
		{
			return View();
		}

	}

}