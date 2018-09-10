using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Controllers
{

	public class InitializeController : BaseController
	{

		//	初始化ResRsouce中的Code
		//	GET:				/Initialize/InitCodeByRes

		public ActionResult InitCode()
		{
			var t = APDBDef.ResResource;

			var model = APQuery.select(t.DomainPKID, t.MediumTypePKID, t.ResourceId)
				.from(t)
				.primary(t.ResourceId)
				.query(db, r =>
				{
					return new ResResource
					{
						ResourceId = t.ResourceId.GetValue(r),
						DomainPKID = t.DomainPKID.GetValue(r),
						MediumTypePKID = t.MediumTypePKID.GetValue(r),
					};
				}).ToList();

			foreach (var item in model)
			{
				var Code = ResResourceHelper.ResourceCode(item.DomainPKID, item.MediumTypePKID, item.ResourceId);
				APQuery.update(t)
					.set(t.Code, Code)
					.where(t.ResourceId == item.ResourceId)
					.execute(db);
			}

			return Content("初始化成功！");
		}

	}

}