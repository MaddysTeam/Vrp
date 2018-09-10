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

	public class ReUserController : BaseController
	{

		public ActionResult More(string type, int page = 1)
		{

			int total = 0;

			ViewBag.RankingOfActiveUser = HomeActiveUserList(
				 out total,  10, (page-1)*10);


			// 分页器
			ViewBag.ParamType = type;
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			return View();
		}

	}

}