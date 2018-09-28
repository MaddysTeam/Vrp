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
             .select(a.ActiveId, a.ActiveName, a.LevelPKID, a.Company, a.Description, a.StartDate, a.EndDate)
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
         ThrowNotAjax();

         if (id == null)
         {
            model.Insert();
         }
         else
         {
            model.Update();
         }

         return Json(new
         {
            status = "success",
            msg = "编辑成功！"
         });
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
         var ad = APDBDef.ActiveDownload;

         var query = APQuery.select(r.RoleId, r.RoleName, ad.ActiveDownloadId)
             .from(
                 r,
                 ad.JoinLeft(r.RoleId == ad.RoleId),
                 a.JoinLeft(a.ActiveId == ad.ActiveId & ad.RoleId == id)
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
               id = ad.ActiveDownloadId.GetValue(rd),
               roleId = r.RoleId.GetValue(rd),
               roleName = r.RoleName.GetValue(rd),
               isSelect = ad.ActiveDownloadId.GetValue(rd) > 0
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
      // 分配/删除 允许访问的角色
      // POST:		/Active/AssignPubRole
      // POST:    /Active/RemovePubRole

      [HttpPost]
      public ActionResult AssignPubRole(long id, long roleId)
      {
         var ap = APDBDef.ActivePublic;
         var isExist = APBplDef.ActivePublicBpl.ConditionQueryCount(ap.RoleId == roleId & ap.ActiveId == id) > 0;
         if (!isExist)
         {
            APBplDef.ActivePublicBpl.Insert(new ActivePublic { ActiveId=id,RoleId=roleId});
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      [HttpPost]
      public ActionResult RemovePubRole(long id)
      {
         var isExists = APBplDef.ActivePublicBpl.PrimaryGet(id) != null;
         if (isExists)
         {
            APBplDef.ActivePublicBpl.PrimaryDelete(id);
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      //
      // 分配/删除 允许下载的角色
      // POST:		/EvalGroup/AssignDwnRole
      // POST:    /EvalGroup/AssignDwnRole

      [HttpPost]
      public ActionResult AssignDwnRole(long id, long roleId)
      {
         var ad = APDBDef.ActiveDownload;
         var isExist = APBplDef.ActiveDownloadBpl.ConditionQueryCount(ad.RoleId == roleId & ad.ActiveId == id) > 0;
         if (!isExist)
         {
            APBplDef.ActiveDownloadBpl.Insert(new ActiveDownload { ActiveId = id, RoleId = roleId });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      [HttpPost]
      public ActionResult RemoveDwnRole(long id)
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