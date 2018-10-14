using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;
using Util.NPOI;
using System.IO;

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


      //
      //	单位 - 导入
      // POST:		/Company/ImportPreview
      // POST:		/Company/Import
      //
      [HttpPost]
      public ActionResult ImportPreview()
      {
         if (Request.IsAuthenticated)
         {
            var file = Request.Files[0];
            if (file == null) throw new NullReferenceException();

            Stream sm = file.InputStream;
            ExecelOperator exporter = new ExecelOperator(sm);
            var result = exporter.ExcelToList<CompanyImportModel>(new string[] { "CompanyName", "Address", "Province", "Area", "Phone" });

            ResSettings.SettingsInSession.RemoveCache(result.GetType());
            ResSettings.SettingsInSession.SetCache(result, result.GetType());

            return PartialView("_preview");
         }

         return Json("error");
      }

      [HttpPost]
      public ActionResult ImporPrevieList(int current, int rowCount, string searchPhrase)
      {
         if (Request.IsAuthenticated)
         {
            var cache = ResSettings.SettingsInSession.GetCache(typeof(List<CompanyImportModel>));

            if (null != cache && cache is List<CompanyImportModel>)
            {
               var result = cache as List<CompanyImportModel>;
               if (!string.IsNullOrEmpty(searchPhrase))
               {
                  Predicate<CompanyImportModel> paras = x => x.Address.IndexOf(searchPhrase) >= 0 || x.Province.IndexOf(searchPhrase) >= 0 || x.Area.IndexOf(searchPhrase) >= 0 || x.CompanyName.IndexOf(searchPhrase) >= 0;
                  Func<CompanyImportModel, bool> paras2 = x => x.Address.IndexOf(searchPhrase) >= 0 || x.Province.IndexOf(searchPhrase) >= 0 || x.Area.IndexOf(searchPhrase) >= 0 || x.CompanyName.IndexOf(searchPhrase) >= 0;
                  if (result.Exists(paras))
                     result = result.Where(paras2).ToList();
                  else
                     result = new List<CompanyImportModel>();
               }
               var total = result.Count;
               if (total > 0)
               {
                  result = result.Skip(rowCount * (current - 1)).Take(rowCount).ToList();
               }

               return Json(new
               {
                  rows = result,
                  current,
                  rowCount,
                  total
               });
            }
         }

         return Json(new
         {
            error = "true",
            msg = "导入预览失败"
         });
      }

      [HttpPost]
      public ActionResult Import()
      {
         if (Request.IsAuthenticated)
         {
            var errorList = new List<CompanyImportModel>();
            var allprovince = ResSettings.SettingsInSession.AllProvince();
            var allArea = ResSettings.SettingsInSession.AllAreas();
            var allCompany = ResSettings.SettingsInSession.AllSchools();
            var cache = ResSettings.SettingsInSession.GetCache(typeof(List<CompanyImportModel>));
            ResCompany province, area, existCompany = null;

            if (null != cache && cache is List<CompanyImportModel> && null != allprovince && null != allArea && null != allCompany)
            {
               var result = cache as List<CompanyImportModel>;
               foreach (var item in result)
               {
                  try
                  {
                     province = allArea.Find(a => a.CompanyName == item.Area.Trim());
                     if (null == province) throw new Exception("省市不匹配");
                     area = allArea.Find(a => a.CompanyName == item.Area.Trim());
                     if (null == area) throw new Exception("地区不匹配");
                     existCompany = allCompany.Find(ac => ac.CompanyName.IndexOf(item.CompanyName.Trim()) >= 0);
                     if (null != existCompany) throw new Exception("单位已经存在");

                     var company = new ResCompany
                     {
                        Address = item.Address.Trim(),
                        CompanyName = item.CompanyName.Trim(),
                        Phone = item.Phone.Trim(),
                        ParentId = area.CompanyId,
                        Path = area.Path + String.Format("{0}\\", db.ResCompanyDal.GetNewId(APDBDef.ResCompany.CompanyId))
                     };
                     APBplDef.ResCompanyBpl.Insert(company);
                  }
                  catch (Exception e)
                  {
                     item.IsSuccess = false;
                     item.FailReason = e.Message;
                     errorList.Add(item);
                  }
               }

               //清除导入缓存

               return Json(new
               {
                  IsSuccess = true
               });
            }
         }

         //清除导入缓存
         //ResSettings.SettingsInSession.RemoveCache(typeof(List<CompanyImportModel>));

         return Json(new
         {
            msg = "您还未登录，请登录后再导入"
         });
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

         var rootList = APBplDef.ResCompanyBpl.GetTree(path, user.UserTypePKID != ResUserHelper.Admin && user.UserTypePKID != ResUserHelper.ProvinceAdmin);

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