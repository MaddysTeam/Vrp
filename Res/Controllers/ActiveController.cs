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

	public class ActiveController : BaseController
	{

      static APDBDef.ActiveTableDef a = APDBDef.Active;

      //
      //	项目管理 - 首页
      // GET:		/Active/Index
      //

      public ActionResult Search()
		{
			return View();
		}

      [HttpPost]
      public ActionResult Search(int current, int rowCount, string searchPhrase)
      {
         var a = APDBDef.Active;
         var query = APQuery
             .select(a.ActiveId, a.ActiveName, a.LevelPKID, a.Company,a.Description,a.StartDate,a.EndDate) 
             .from(a);

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(a.ActiveName.Match(searchPhrase));
         }

         query.primary(a.ActiveId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);
         var actives = db.Query(query, a.TolerantMap).ToList();
         var list = (from ac in actives
                     select new
                     {
                        id = ac.ActiveId,
                        name = ac.ActiveName,
                        level = ac.Level,
                        company = ac.Company,
                        description = ac.Description,
                        start = ac.StartDate.ToString("yyyy-MM-dd"),
                        end = ac.EndDate.ToString("yyyy-MM-dd")
                     }).ToList();


         return Json(new
         {
            rows = list,
            current,
            rowCount,
            total
         });
      }


      //
      //	项目管理 - 编辑/创建
      // GET:		/Active/Edit
      // POST:		/Active/Edit
      //

      public ActionResult Edit(long? id)
		{
			if (id == null)
			{
            return PartialView();
			}
			else
			{
				var model = APBplDef.ActiveBpl.PrimaryGet(id.Value);
				return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
			}
		}

		[HttpPost]
		public ActionResult Edit(long? id, Active model, FormCollection fc)
		{
			if (id == null)
			{
				model.Insert();
			}
			else
			{
				model.Update();
			}

			return RedirectToAction("Index");
		}


      //
      //	项目管理 - 删除
      // POST:		/Active/Delete
      //

      [HttpPost]
		public ActionResult Delete(long id)
		{
			//if (Request.IsAjaxRequest())
			//{

			//	if (APBplDef.ResUserRoleBpl.ConditionQueryCount(APDBDef.ResUserRole.RoleId == id) > 0)
			//		return Json(new { cmd = "Error", msg = "不可删除含有用户的角色。" });

			//	APBplDef.ResRoleBpl.PrimaryDelete(id);
			//	return Json(new { cmd = "Deleted", msg = "角色已删除。" });
			//}

			return IsNotAjax();
		}

      //
      //	项目管理 - 公开管理/角色/列表
      // GET: Active/RoleList
      // POST-Ajax: Active/RoleList

      public ActionResult PubRoleList(int id)
      {
         return PartialView(id);
      }

      [HttpPost]
      public ActionResult PubRoleList(int current, int rowCount, string searchPhrase, int id)
      {
         ThrowNotAjax();

         var r = APDBDef.ResRole;
         var ap = APDBDef.ActivePublic;

         var query = APQuery.select(r.RoleId, r.RoleName, ap.ActivePublicId)
             .from(
                 r,
                 ap.JoinLeft(r.RoleId== ap.RoleId),
                 a.JoinLeft(a.ActiveId == ap.ActiveId & ap.RoleId == id)
                 )
             .primary(r.RoleId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            searchPhrase = searchPhrase.Trim();
            query.where_and(r.RoleName.Match(searchPhrase));
         }


         #region [sort]
         //排序条件表达式

         //if (sort != null)
         //{
         //   switch (sort.ID)
         //   {
         //      case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
         //      case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
         //      case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
         //      case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
         //      case "groupCount": query.order_by(sort.OrderBy(e.GroupCount)); break;
         //   }
         //}
         #endregion

         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            return new
            {
               id = ap.ActivePublicId.GetValue(rd),
               roleId = r.RoleId.GetValue(rd),
               roleName = r.RoleName.GetValue(rd),
               isSelect = ap.ActivePublicId.GetValue(rd) > 0
            };
         }).ToList();


         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total
         });
      }


      // GET: ExpManage/ResList
      // POST-Ajax: ExpManage/ResList

      //
      //	项目管理 -下载管理/角色/列表/
      // GET: Active/RoleList
      // POST-Ajax: Active/DwnRoleList

      public ActionResult DwnRoleList(int id)
      {
         return PartialView(id);
      }

      [HttpPost]
      public ActionResult DwnRoleList(int current, int rowCount, string searchPhrase, int id)
      {
         ThrowNotAjax();

         var r = APDBDef.ResRole;
         var ap = APDBDef.ActivePublic;

         var query = APQuery.select(r.RoleId, r.RoleName, ap.ActivePublicId)
             .from(
                 r,
                 ap.JoinLeft(r.RoleId == ap.RoleId),
                 a.JoinLeft(a.ActiveId == ap.ActiveId & ap.RoleId == id)
                 )
             .primary(r.RoleId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            searchPhrase = searchPhrase.Trim();
            query.where_and(r.RoleName.Match(searchPhrase));
         }


         #region [sort]
         //排序条件表达式

         //if (sort != null)
         //{
         //   switch (sort.ID)
         //   {
         //      case "realName": query.order_by(sort.OrderBy(up.RealName)); break;
         //      case "target": query.order_by(sort.OrderBy(d.DeclareTargetPKID)); break;
         //      case "subject": query.order_by(sort.OrderBy(d.DeclareSubjectPKID)); break;
         //      case "stage": query.order_by(sort.OrderBy(d.DeclareStagePKID)); break;
         //      case "groupCount": query.order_by(sort.OrderBy(e.GroupCount)); break;
         //   }
         //}
         #endregion

         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            return new
            {
               id = ap.ActivePublicId.GetValue(rd),
               roleId = r.RoleId.GetValue(rd),
               roleName = r.RoleName.GetValue(rd),
               isSelect = ap.ActivePublicId.GetValue(rd) > 0
            };
         }).ToList();


         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total
         });
      }



      //
      // 分配评审组可打分的微课资源
      // POST:		/EvalGroup/AssignResource
      // POST:    /EvalGroup/RemoveResource

      [HttpPost]
      public ActionResult AssignRes(long id, long resId)
      {
         var egr = APDBDef.EvalGroupResource;

         var resxist = APBplDef.CroResourceBpl.PrimaryGet(resId) != null;
         var groupExist = APBplDef.EvalGroupBpl.PrimaryGet(id) != null;

         var isExist = APBplDef.EvalGroupResourceBpl.ConditionQueryCount(egr.ResourceId == resId & egr.GroupId == id) > 0;
         if (resxist && groupExist && !isExist)
         {
            APBplDef.EvalGroupResourceBpl.Insert(new EvalGroupResource { ResourceId = resId, GroupId = id });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      [HttpPost]
      public ActionResult RemoveRes(long id)
      {
         var isExists = APBplDef.EvalGroupResourceBpl.PrimaryGet(id) != null;
         if (isExists)
         {
            APBplDef.EvalGroupResourceBpl.PrimaryDelete(id);
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      //
      // 分配参与评审组的专家
      // POST:		/EvalGroup/AssignExp
      // POST:    /EvalGroup/RemoveExp

      [HttpPost]
      public ActionResult AssignExp(long id, long expId)
      {
         var ege = APDBDef.EvalGroupExpert;

         var userExist = APBplDef.ResUserBpl.PrimaryGet(expId) != null;
         var groupExist = APBplDef.EvalGroupBpl.PrimaryGet(id) != null;

         var isExist = APBplDef.EvalGroupExpertBpl.ConditionQueryCount(ege.ExpertId == expId & ege.GroupId == id) > 0;
         if (userExist && groupExist && !isExist)
         {
            APBplDef.EvalGroupExpertBpl.Insert(new EvalGroupExpert { ExpertId = expId, GroupId = id });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      [HttpPost]
      public ActionResult RemoveExp(long id)
      {
         var isExists = APBplDef.EvalGroupExpertBpl.PrimaryGet(id) != null;
         if (isExists)
         {
            APBplDef.EvalGroupExpertBpl.PrimaryDelete(id);
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }

   }

}