using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static class ResUserHelper
	{
		public static PickListAPRptColumn Gender;

		static ResUserHelper()
		{
			Gender = new PickListAPRptColumn(APDBDef.ResUser.GenderPKID, ThisApp.PLKey_Gender);
		}

		public static IEnumerable<System.Web.Mvc.SelectListItem> GetRoleSelectList(long userId)
		{
			var t = APDBDef.ResUserRole;
			var ur = APBplDef.ResUserRoleBpl.ConditionQuery(t.UserId == userId, null).FirstOrDefault();

			foreach (var item in APBplDef.ResRoleBpl.GetAll())
			{
				yield return new System.Web.Mvc.SelectListItem()
				{
					Value = item.RoleId.ToString(),
					Text = item.RoleName,
					Selected = item.RoleId == ur.RoleId
				};
			}
		}

		// 性别
		public static long GenderMale = 1001;
		public static long GenderFemale = 1002;


      //TODO: 用户类型
      public static long Teacher = 5001;
      public static long Admin = 5002;
      public static long Export = 5003;
      public static long ProvinceAdmin = 10221;
      public static long AreaAdmin  = 10222;
      public static long SchoolAdmin = 10223;

   }

}