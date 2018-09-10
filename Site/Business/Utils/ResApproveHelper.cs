using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static class ResApproveHelper
	{
		static List<ResApprove> approves;

		public static IEnumerable<System.Web.Mvc.SelectListItem> GetSelectList(long roleId = 0)
		{
			if (approves == null)
				approves = APBplDef.ResApproveBpl.GetAll();

			Dictionary<long, ResRoleApprove> dict = null;

			if (roleId != 0)
				dict = APBplDef.ResRoleApproveBpl.ConditionQuery(APDBDef.ResRoleApprove.RoleId == roleId, null).ToDictionary(m => m.ApproveId);

			foreach (var item in approves)
			{
				yield return new System.Web.Mvc.SelectListItem()
				{
					Value = item.ApproveId.ToString(),
					Text = item.Description,
					Selected = (dict != null && dict.ContainsKey(item.ApproveId))
				};
			}
		}
	}

}