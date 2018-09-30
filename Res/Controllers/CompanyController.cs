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

   public class CompanyController : BaseController
   {

      public ActionResult Index()
      {

         return View();
      }


      public ActionResult Edit(long id = 0)
      {
         // 根据ID查询单位信息

         var model = db.ResCompanyDal.PrimaryGet(id);

         if (model == null)
         {
            model = new ResCompany() { CompanyId = 0 };
         }

         return PartialView(model);
      }


      [HttpPost]
      public ActionResult Edit(ResCompany model, long SuperiorId = 0)
      {
         // 删除缓存
         ResSettings.SettingsInSession.RemoveCache(typeof(ResCompany));

         // 执行新增操作

         if (model.CompanyId == 0)
         {
            // 查重

            var t = APDBDef.ResCompany;
            var IsRepeat = db.ResCompanyDal.ConditionQuery(t.CompanyName == model.CompanyName,
                                                            null, null, null).Count;

            if (IsRepeat > 0)
            {
               return Json(new
               {
                  error = "success",
                  msg = "公司名称重复，请重试"
               });
            }
            var parent = db.ResCompanyDal.PrimaryGet(SuperiorId);
            var parentPath = parent == null ? string.Empty : parent.Path;

            var newModel = new ResCompany
            {
               ParentId = SuperiorId,
               CompanyName = model.CompanyName,
               Address = model.Address
            };
            db.ResCompanyDal.Insert(newModel);
            newModel.Path = parentPath + String.Format("{0}\\", newModel.CompanyId);
            db.ResCompanyDal.UpdatePartial(newModel.CompanyId, new
            {
               newModel.Path
            });
         }

         // 执行修改操作

         else
         {

            // 查重

            var t = APDBDef.ResCompany;
            var IsRepeat = db.ResCompanyDal.ConditionQuery(t.CompanyName == model.CompanyName &
                                                            t.CompanyId != model.CompanyId,
                                                            null, null, null).Count;

            if (IsRepeat > 0)
            {
               return Json(new
               {
                  error = "success",
                  msg = "公司名称重复，请重试"
               });
            }


            db.ResCompanyDal.UpdatePartial(model.CompanyId, new
            {
               model.CompanyName,
               model.Address
            });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }

      [HttpPost]
      public ActionResult Remove(long CompanyId = 0)
      {
         ResSettings.SettingsInSession.RemoveCache(typeof(ResCompany));

         var CompanyModel = db.ResCompanyDal.PrimaryGet(CompanyId);

         if (CompanyModel.ParentId == 0)
         {
            return Json(new
            {
               error = "success",
               msg = "父节点无法被删除"
            });
         }
         else
         {
            db.ResCompanyDal.PrimaryDelete(CompanyId);

            return Json(new
            {
               error = "none",
               msg = "删除成功"
            });
         }
      }

      // 获取树形菜单

      public ActionResult Tree()
      {
         var path = string.Empty;
         var user = ResSettings.SettingsInSession.User;
         if (user.ProvinceId > 0)
            path += user.ProvinceId + @"\";
         if (user.AreaId > 0)
            path += user.AreaId + @"\";
         if (user.CompanyId > 0)
            path += user.CompanyId + @"\";
         if (user.UserTypePKID == ResUserHelper.Admin)
            path = string.Empty;

         var rootList = APBplDef.ResCompanyBpl.GetTree(path);

         return Json(getChildren(rootList), JsonRequestBehavior.AllowGet);
      }


      // 获取只有父级节点的树形菜单

      public ActionResult ParentTree()
      {
         var rootList = APBplDef.ResCompanyBpl.GetParentTree();

         return Json(getChildren(rootList), JsonRequestBehavior.AllowGet);
      }

      private List<object> getChildren(List<ResCompany> list)
      {
         list = list ?? new List<ResCompany>();
         List<object> ret = new List<object>();
         foreach (var item in list)
         {
            object node = null;
            if (item.Children == null)
            {
               node = new { id = item.CompanyId, text = item.CompanyName };
            }
            else
            {
               node = new { id = item.CompanyId, text = item.CompanyName, children = getChildren(item.Children) };
            }
            ret.Add(node);
         }
         return ret;
      }

   }

}