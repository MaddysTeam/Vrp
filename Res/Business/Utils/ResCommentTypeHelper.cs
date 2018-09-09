using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static class ResCommentTypeHelper
	{

		private static List<System.Web.Mvc.SelectListItem> slilist;
		public static List<System.Web.Mvc.SelectListItem> GetSelectList()
		{
			if (slilist == null)
			{
				slilist = new List<System.Web.Mvc.SelectListItem>();
				slilist.Add(new System.Web.Mvc.SelectListItem() { Text = "未审核", Value = "0" });
				slilist.Add(new System.Web.Mvc.SelectListItem() { Text = "审核通过", Value = "1" });
				slilist.Add(new System.Web.Mvc.SelectListItem() { Text = "审核不通过", Value = "2" });

			}
			return slilist;


		}


	}

}