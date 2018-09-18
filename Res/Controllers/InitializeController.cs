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
		//	GET:	/Initialize/InitCodeByRes

		public ActionResult InitCode()
		{

         //db.ResApproveDal.Insert(new ResApprove { ApproveName="EvalResource", Description="", Scope="library"  });
         //var t = APDBDef.ResResource;

         //var model = APQuery.select(t.DomainPKID, t.MediumTypePKID, t.ResourceId)
         //	.from(t)
         //	.primary(t.ResourceId)
         //	.query(db, r =>
         //	{
         //		return new ResResource
         //		{
         //			ResourceId = t.ResourceId.GetValue(r),
         //			DomainPKID = t.DomainPKID.GetValue(r),
         //			MediumTypePKID = t.MediumTypePKID.GetValue(r),
         //		};
         //	}).ToList();

         //foreach (var item in model)
         //{
         //	var Code = ResResourceHelper.ResourceCode(item.DomainPKID, item.MediumTypePKID, item.ResourceId);
         //	APQuery.update(t)
         //		.set(t.Code, Code)
         //		.where(t.ResourceId == item.ResourceId)
         //		.execute(db);
         //}


         //db.ResPickListDal.Insert(new ResPickList { InnerKey= "PLKey_UserType",Name="用户类型",Creator=1, CreatedTime=DateTime.Now, LastModifier=1, LastModifiedTime=DateTime.Now });
         //db.ResPickListItemDal.Insert(new ResPickListItem { PickListId= 5001, Name="teacher", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });
         //db.ResPickListItemDal.Insert(new ResPickListItem { PickListId = 5001, Name = "admin", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });
         //db.ResPickListItemDal.Insert(new ResPickListItem { PickListId = 5001, Name = "expert", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });


         //db.ResPickListDal.Insert(new ResPickList { InnerKey = "PLKey_Level", Name = "评审级别", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });
         //db.ResPickListItemDal.Insert(new ResPickListItem { PickListId= 5002, Name="市级评审", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });
         //db.ResPickListItemDal.Insert(new ResPickListItem { PickListId = 5002, Name = "省级评审", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });
         //db.ResPickListItemDal.Insert(new ResPickListItem { PickListId = 5002, Name = "联合评审", Creator = 1, CreatedTime = DateTime.Now, LastModifier = 1, LastModifiedTime = DateTime.Now });

         return Content("初始化成功！");
		}

	}

}