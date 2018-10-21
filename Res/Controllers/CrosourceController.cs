
using Res.Business;
using Res.Models;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Res.Controllers

{

   /// <summary>
   /// 微课作品控制器
   /// </summary>
   public class CrosourceController : BaseController
   {

      //
      //	作品 - 首页
      // GET:		/Crosource/Index
      //

      public ActionResult Index()
      {
         return View();
      }


      //
      //	作品 - 查询
      // GET:		/Crosource/Search
      // POST:		/Crosource/Search
      //

      public ActionResult Search()
      {
         InitAreaDropDownData(true);


         return View();
      }

      [HttpPost]
      public ActionResult Search(long activeId, long provinceId, long areaId, 
                                 long companyId, long subjectId,long deliveryId,
                                 long gradeId, long maxScore,long minScore, 
                                 int current, int rowCount, string searchPhrase, FormCollection fc)
      {
         var user = ResSettings.SettingsInSession.User;

         //----------------------------------------------------------
         var t = APDBDef.CroResource;
         var u = APDBDef.ResUser;
         var dr = APDBDef.DeliveryRecord;
         APSqlOrderPhrase order = null;
         APSqlWherePhrase where = t.StatePKID != CroResourceHelper.StateDelete;

         // 取排序
         var co = GridOrder.GetSortDef(fc);
         if (co != null)
         {
            switch (co.Id)
            {
               case "Title": order = new APSqlOrderPhrase(t.Title, co.Order); break;
               case "Author": order = new APSqlOrderPhrase(u.RealName, co.Order); break;
               case "CreatedTime": order = new APSqlOrderPhrase(t.CreatedTime, co.Order); break;
               case "State": order = new APSqlOrderPhrase(t.StatePKID, co.Order); break;
               case "Score": order = new APSqlOrderPhrase(t.Score, co.Order); break;
               case "WinLevel": order = new APSqlOrderPhrase(t.WinLevelPKID, co.Order); break;
            }
         }

         // 按作品标题,内容，作者等过滤
         if (searchPhrase != null)
         {
            searchPhrase = searchPhrase.Trim();
            if (searchPhrase != "")
               where &= t.Title.Match(searchPhrase) | t.Author.Match(searchPhrase) | t.Description.Match(searchPhrase) | t.AuthorCompany.Match(searchPhrase);
         }

         // 用户数据范围或搜索
         if (user.ProvinceId > 0 || provinceId > 0)
            where &= t.ProvinceId == (provinceId > 0 ? provinceId : user.ProvinceId);
         if (user.AreaId > 0 || areaId > 0)
            where &= t.AreaId == (areaId > 0 ? areaId : user.AreaId);
         if (user.CompanyId > 0 || companyId > 0)
            where &= t.CompanyId == (companyId > 0 ? companyId : user.CompanyId);

         if (maxScore > 0)
            where &= t.Score <= maxScore;
         if (minScore > 0)
            where &= t.Score >= minScore;

         // 按项目，年级，学科,报送类型数据过滤
         if (activeId > 0)
            where &= t.ActiveId == activeId;
         if (subjectId > 0)
            where &= t.SubjectPKID == subjectId;
         if (gradeId > 0)
            where &= t.GradePKID == gradeId;

         var subquery = APQuery.select(dr.ResourceId).from(dr).where(dr.DeliveryTypePKID == deliveryId);
         // 按照报送类型过滤，当deliveryId小于0时为不报送类型
         if (deliveryId < 0)
            where &= t.CrosourceId.NotIn(subquery);
         else if(deliveryId > 0)
            where &= t.CrosourceId.In(subquery);

         int total;
         var list = APBplDef.CroResourceBpl.TolerantSearch(out total, current, rowCount, where, order);


         if (Request.IsAjaxRequest())
         {
            return Json(
               new
               {
                  rows = from cro in list
                         select new
                         {
                            id = cro.CrosourceId,
                            cro.Title,
                            cro.Author,
                            Type = cro.CourseType, // 微课或微课程
                            CreatedTime = cro.CreatedTime.ToString("yyyy-MM-dd"),
                            cro.Province,
                            cro.Area,
                            cro.School,
                            cro.State,
                            cro.StatePKID,
                            cro.Score,
                            cro.WinLevel,
                            cro.WinLevelPKID,
                            cro.PublicStatePKID,
                            cro.DownloadStatePKID,
                         },
                  current = current,
                  rowCount = rowCount,
                  total = total

               });
         }
         else
         {
            return View(list);
         }
      }


      //
      //	作品 - 分类查询
      // GET:		/Crosource/Category
      // POST:		/Crosource/Category
      //

      public ActionResult Category()
      {
         if (!String.IsNullOrEmpty(Request["d"]))
            ViewData["Domain"] = Request["d"];
         if (!String.IsNullOrEmpty(Request["r"]))
            ViewData["ResourceType"] = Request["r"];
         return View();
      }

      [HttpPost]
      public ActionResult Category(int current, int rowCount, string searchPhrase, FormCollection fc)
      {
         //----------------------------------------------------------
         var t = APDBDef.CroResource;
         var u = APDBDef.ResUser;
         APSqlOrderPhrase order = null;
         List<APSqlWherePhrase> conds = new List<APSqlWherePhrase>(){
            t.StatePKID != CroResourceHelper.StateDelete
         };

         // 取排序
         var co = GridOrder.GetSortDef(fc);
         if (co != null)
         {
            switch (co.Id)
            {
               case "Title": order = new APSqlOrderPhrase(t.Title, co.Order); break;
               case "Author": order = new APSqlOrderPhrase(u.RealName, co.Order); break;
               case "CreatedTime": order = new APSqlOrderPhrase(t.CreatedTime, co.Order); break;
               case "State": order = new APSqlOrderPhrase(t.StatePKID, co.Order); break;
               case "ViewCount": order = new APSqlOrderPhrase(t.ViewCount, co.Order); break;
               case "DownCount": order = new APSqlOrderPhrase(t.DownCount, co.Order); break;
               case "FavoriteCount": order = new APSqlOrderPhrase(t.FavoriteCount, co.Order); break;
               case "CommentCount": order = new APSqlOrderPhrase(t.CommentCount, co.Order); break;
               case "cmd_elite": order = new APSqlOrderPhrase(t.EliteScore, co.Order); break;

            }
         }

         // 取过滤条件
         foreach (string cond in fc.Keys)
         {
            switch (cond)
            {
               case "ResourceType": conds.Add(t.ResourceTypePKID == Int64.Parse(fc[cond])); break;
               case "Stage": conds.Add(t.StagePKID == Int64.Parse(fc[cond])); break;
               case "Grade": conds.Add(t.GradePKID == Int64.Parse(fc[cond])); break;
               case "State": conds.Add(t.StatePKID == Int64.Parse(fc[cond])); break;
               case "Subject": conds.Add(t.SubjectPKID == Int64.Parse(fc[cond])); break;
            }
         }

         // 按关键字过滤
         if (searchPhrase != null)
         {
            searchPhrase = searchPhrase.Trim();
            if (searchPhrase != "")
               conds.Add(t.Keywords.Match(searchPhrase));
         }


         int total;
         var list = APBplDef.CroResourceBpl.TolerantSearch(out total, current, rowCount, new APSqlConditionAndPhrase(conds), order);
         //----------------------------------------------------------

         if (Request.IsAjaxRequest())
         {
            return Json(
               new
               {
                  rows = from cro in list
                         select new
                         {
                            //id = cro.CrosourceId,
                            //cro.Title,
                            //cro.Author,
                            //cro.MediumType,
                            //CreatedTime = cro.CreatedTime.ToString("yyyy-MM-dd"),
                            //cro.State,
                            //cro.StatePKID,
                            //cro.EliteScore,
                            //cro.ViewCount,
                            //cro.DownCount,
                            //cro.FavoriteCount,
                            //cro.CommentCount,
                            //cro.StarTotal
                         },
                  current = current,
                  rowCount = rowCount,
                  total = total

               });
         }
         else
         {
            return View(list);
         }
      }


      //
      //	作品 - 删除
      // POST:		/Crosource/Delete
      //

      [HttpPost]
      public ActionResult Delete(long id)
      {
         if (Request.IsAjaxRequest())
         {
            APBplDef.CroResourceBpl.UpdatePartial(id, new { StatePKID = CroResourceHelper.StateDelete });
            return Json(new { cmd = "Deleted", msg = "本作品已删除。" });
         }

         return IsNotAjax();
      }


      //
      //	作品 - 加精/取消
      // POST:		/Crosource/Elite
      //

      [HttpPost]
      public ActionResult Elite(long id, bool value)
      {
         if (Request.IsAjaxRequest())
         {
            APBplDef.CroResourceBpl.UpdatePartial(id, new { EliteScore = value ? 1 : 0 });
            return Json(new { cmd = "Processed", value = value, msg = "本作品加精设置完成。" });
         }

         return IsNotAjax();
      }


      //
      //	作品 - 编辑/创建
      // GET:		/Resource/Edit
      // POST:		/Resource/Edit
      //

      public ActionResult Edit(long? id)
      {
         InitAreaDropDownData();

         if (id == null)
         {
            return View(
               new CroResource { Courses = new List<MicroCourse> { new MicroCourse() } } // 新增时默认一个微课
               );
         }
         else
         {
            var model = APBplDef.CroResourceBpl.GetResource(db, id.Value);

            return View(model);
         }
      }

      [HttpPost]
      [ValidateInput(false)]
      public ActionResult Edit(long? resid, CroResource model, FormCollection fc)
      {
         var mc = APDBDef.MicroCourse;
         var et = APDBDef.Exercises;
         var eti = APDBDef.ExercisesItem;

         CroResource current = null;
         if (resid != null && resid.Value > 0)
            current = APBplDef.CroResourceBpl.GetResource(db, resid.Value);

         db.BeginTrans();

         try
         {
            if (current != null)
            {
               var exeIds = new List<long>();
               foreach (var item in current.Courses)
               {
                  if (item.Exercises != null && item.Exercises.Count > 0)
                     exeIds.AddRange(item.Exercises.Select(x => x.ExerciseId).ToArray());
               }

               if (exeIds.Count() > 0)
                  APBplDef.ExercisesItemBpl.ConditionDelete(eti.ExerciseId.In(exeIds.ToArray()));

               var courseIds = current.Courses.Select(x => x.CourseId).ToArray();
               APBplDef.ExercisesBpl.ConditionDelete(et.CourseId.In(courseIds));
               APBplDef.MicroCourseBpl.ConditionDelete(mc.ResourceId == resid);
               APBplDef.CroResourceBpl.PrimaryDelete(resid.Value);

               model.CourseTypePKID = model.CourseTypePKID == 0 ? CroResourceHelper.MicroClass : current.CourseTypePKID;
               model.CreatedTime = current.CreatedTime;
               model.Creator = current.Creator;
               model.LastModifier = ResSettings.SettingsInSession.UserId;
               model.LastModifiedTime = DateTime.Now;
               model.StatePKID = CroResourceHelper.StateWait;
               model.PublicStatePKID = current.PublicStatePKID;
               model.DownloadStatePKID = current.DownloadStatePKID;
               model.DownCount = current.DownCount;
               model.ViewCount = current.ViewCount;
               model.Score = current.Score;
               model.WinLevelPKID = current.WinLevelPKID;
               model.StatePKID = current.StatePKID;
              // model.DeliveryTypePKID = current.DeliveryTypePKID;
            }
            else
            {
               model.CourseTypePKID = model.CourseTypePKID == 0 ? CroResourceHelper.MicroClass : model.CourseTypePKID;
               model.StatePKID = CroResourceHelper.StateWait;
               model.Creator = ResSettings.SettingsInSession.UserId;
               model.CreatedTime = model.LastModifiedTime = DateTime.Now;
               model.LastModifier = ResSettings.SettingsInSession.UserId;
               model.DownloadStatePKID = CroResourceHelper.AllowDownload;
               model.PublicStatePKID = CroResourceHelper.Public;
            }

            model.StatePKID = model.StatePKID == CroResourceHelper.StateDeny ? CroResourceHelper.StateWait : model.StatePKID;
            APBplDef.CroResourceBpl.Insert(model);

            foreach (var item in model.Courses ?? new List<MicroCourse>())
            {
               var currentCourse = current == null ? model.Courses.FirstOrDefault(x => x.CourseId == item.CourseId) :
                                        current.Courses.FirstOrDefault(x => x.CourseId == item.CourseId);
               item.ResourceId = model.CrosourceId;
               item.PlayCount = currentCourse != null ? currentCourse.PlayCount : 0;
               item.DownCount = currentCourse != null ? currentCourse.DownCount : 0;
               APBplDef.MicroCourseBpl.Insert(item);

               foreach (var exer in item.Exercises ?? new List<Exercises>())
               {
                  exer.CourseId = item.CourseId;
                  APBplDef.ExercisesBpl.Insert(exer);

                  foreach (var exerItem in exer.Items ?? new List<ExercisesItem>())
                  {
                     exerItem.ExerciseId = exer.ExerciseId;
                     APBplDef.ExercisesItemBpl.Insert(exerItem);
                  }
               }
            }

            db.Commit();
         }
         catch
         {
            db.Rollback();
         }

         return RedirectToAction("Details", new { id = model.CrosourceId });
      }


      private string GetSafeExt(string path)
      {
         int idx = path.IndexOf('?');
         if (idx != -1)
            path = path.Substring(0, idx);
         return Path.GetExtension(path);
      }


      //
      //	作品 - 详情
      // GET:		/Crosource/Details
      //

      public ActionResult Details(long id, long? courseId)
      {
         var model = APBplDef.CroResourceBpl.GetResource(db, id);

         ViewBag.CurrentCourse = courseId == null || courseId.Value == 0 ? model.Courses[0] : model.Courses.Find(c => c.CourseId == courseId);

         return View(model);
      }


      //
      //	作品 - 审核合格/不合格
      // GET:    /Resource/Approve
      // POST:		/Resource/MultiApprove
      // POST:		/Resource/Approve
      //

      public ActionResult Approve(string ids)
      {
         var list = CroResourceHelper.DictApprove
                          .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() })
                          .ToList();

         ViewBag.ids = ids;

         return PartialView("_approve", list);
      }

      [HttpPost]
      public ActionResult MultiApprove(string ids, long state)
      {
         if (Request.IsAjaxRequest() && !string.IsNullOrEmpty(ids))
         {
            var cr = APDBDef.CroResource;
           // var state = value ? CroResourceHelper.StateAllow : CroResourceHelper.StateDeny;
            var array = ids.Split(',');
            var idArray = new long[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
               idArray[i] = Convert.ToInt64(array[i]);
            }

            APQuery.update(cr).set(cr.StatePKID, state).where(cr.CrosourceId.In(idArray)).execute(db);

            return Json(new { cmd = "Processed", value = state, msg = "批量审核完成。" });
         }

         return IsNotAjax();
      }

      [HttpPost]
      public ActionResult Approve(long id, bool value, string opinion)
      {
         if (Request.IsAjaxRequest())
         {
            APBplDef.CroResourceBpl.UpdatePartial(id, new
            {
               StatePKID = value ? CroResourceHelper.StateAllow : CroResourceHelper.StateDeny,
               Auditor = ResSettings.SettingsInSession.UserId,
               AuditedTime = DateTime.Now,
               AuditOpinion = opinion
            });
            return Json(new { cmd = "Processed", value = value, msg = "本作品审核完成。" });
         }

         return IsNotAjax();
      }


      //	作品
      //	GET:				/Crosource/Report

      //public ActionResult Report()
      //{
      //   var cro = APDBDef.CroResource;
      //   var u = APDBDef.ResUser;
      //   var cmp = APDBDef.ResCompany;
      //   var area = APDBDef.ResCompany.As("area");


      //   var model = APQuery.select(area.CompanyName.As("area"), cmp.CompanyName.As("cmp"), cro.CrosourceId.Count().As("cnt"), area.CompanyId)
      //      .from(cmp,
      //            area.JoinInner(cmp.ParentId == area.CompanyId),
      //            u.JoinLeft(cmp.CompanyId == u.CompanyId),
      //            cro.JoinLeft(cro.Creator == u.UserId & cro.StatePKID == ResResourceHelper.StateAllow))
      //      .where(area.CompanyId.NotIn(120, 118, 108, 104, 76, 71))
      //      .group_by(area.CompanyName, cmp.CompanyName, area.CompanyId)
      //      .query(db, r =>
      //      {
      //         return new CroReport()
      //         {
      //            Area = r.GetString(0),
      //            Company = r.GetString(1),
      //            Cnt = r.GetInt32(2)
      //         };
      //      }).ToList();


      //   return View(model);
      //}


      //
      // 微课视频点击
      // GET:		/Crosource/Play
      //

      [HttpPost]
      public ActionResult Play(long id, long courseId)
      {
         APBplDef.MicroCourseBpl.CountingPlay(db, id, courseId, ResSettings.SettingsInSession.UserId);

         return Json(new
         {
            state = "ok",
            msg = "恭喜您，已经播放成功！"
         });
      }


      //
      // 微课视频下载
      // GET:		/Crosource/Download
      //

      public ActionResult Download(long id, long courseId, long fileId)
      {
         if (!Request.IsAuthenticated)
         {
            return Json(new
            {
               state = "failure",
               msg = "请您先登录再下载！"
            });
         }
         else
         {
            var t = APDBDef.CroDownload;
            APBplDef.CroResourceBpl.CountingDownload(db, id, courseId, fileId, Request.IsAuthenticated ? ResSettings.SettingsInSession.UserId : 0);
            return Json(new
            {
               state = "ok",
               msg = "恭喜您，已经下载成功！"
            });

         }
      }


      //
      // 设置奖项
      // GET:		/Crosource/WinLevel
      // GET:		/Crosource/MultiWinLevel
      // POST:    /Crosource/MultiWinLevel
      // POST:    /Crosource/WinLevel

      public ActionResult WinLevel(long id)
      {
         var crosource = APBplDef.CroResourceBpl.PrimaryGet(id);

         var list = CroResourceHelper.DictWinLevel
                          .Select(x => new SelectListItem { Value = x.Key.ToString(), Selected = x.Key == crosource.WinLevelPKID, Text = x.Value })
                          .ToList();

         return PartialView("_winLevel", list);
      }


      public ActionResult MultiWinLevel(string ids)
      {
         var list = CroResourceHelper.DictWinLevel
                          .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() })
                          .ToList();

         ViewBag.ids = ids;

         return PartialView("_winLevel", list);
      }

      [HttpPost]
      public ActionResult WinLevel(long id, long levelId)
      {
         ThrowNotAjax();

         if (levelId > -1 && id > 0) // level id 为0 等于撤销奖项
            APBplDef.CroResourceBpl.UpdatePartial(id, new { WinLevelPKID = levelId });

         return Json(new { msg = "奖项设置成功" });
      }

      [HttpPost]
      public ActionResult MultiWinLevel(string ids, long levelId)
      {
         ThrowNotAjax();
         if (Request.IsAjaxRequest() && !string.IsNullOrEmpty(ids))
         {
            var cr = APDBDef.CroResource;
            var array = ids.Split(',');
            var idArray = new long[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
               idArray[i] = Convert.ToInt64(array[i]);
            }

            APQuery.update(cr).set(cr.WinLevelPKID, levelId).where(cr.CrosourceId.In(idArray)).execute(db);

            return Json(new { cmd = "Processed", msg = "批量奖项设置完成。" });
         }
          return IsNotAjax();
      }


      //
      // 修改和批量修改公开状态
      // POST:		/Crosource/PublicSetting
      // GET:		/Crosource/MultiPublicSetting
      // POST:		/Crosource/MultiPublicSetting
      //

      [HttpPost]
      public ActionResult PublicSetting(long id)
      {
         if (id > 0)
         {
            var res = APBplDef.CroResourceBpl.PrimaryGet(id);

            res.PublicStatePKID = res.PublicStatePKID == CroResourceHelper.Public ? CroResourceHelper.Private : CroResourceHelper.Public;

            APBplDef.CroResourceBpl.UpdatePartial(id, new { PublicStatePKID = res.PublicStatePKID });
         }

         return Json(new
         {
            cmd = "Updated",
            msg = "设置成功"
         });
      }

      public ActionResult MultiPublicSetting(string ids)
      {
         var list = CroResourceHelper.DictPublicSetting
                          .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() })
                          .ToList();

         ViewBag.ids = ids;

         return PartialView("_publicSetting", list);
      }

      [HttpPost]
      public ActionResult MultiPublicSetting(string ids, long settingId)
      {
         if (Request.IsAjaxRequest() && !string.IsNullOrEmpty(ids))
         {
            var cr = APDBDef.CroResource;
            var idArray = ConvertByString(ids);

            APQuery.update(cr).set(cr.PublicStatePKID, settingId).where(cr.CrosourceId.In(idArray)).execute(db);

            return Json(new { cmd = "Processed", value = settingId, msg = "批量设定完成。" });
         }

         return IsNotAjax();
      }


      //
      // 修改和批量下载状态
      // POST:		/Crosource/PublicSettings
      // GET:		/Crosource/MultiDownloadSetting
      // POST:		/Crosource/MultiDownloadSetting
      //

      [HttpPost]
      public ActionResult DownloadSetting(long id)
      {
         if (id > 0)
         {
            var res = APBplDef.CroResourceBpl.PrimaryGet(id);

            res.DownloadStatePKID = res.DownloadStatePKID == CroResourceHelper.AllowDownload ? CroResourceHelper.DenyDownload : CroResourceHelper.AllowDownload;

            APBplDef.CroResourceBpl.UpdatePartial(id, new { DownloadStatePKID = res.DownloadStatePKID });
         }

         return Json(new
         {
            cmd = "Updated",
            msg = "设置成功"
         });
      }

      public ActionResult MultiDownloadSetting(string ids)
      {
         var list = CroResourceHelper.DictDownloadSetting
                          .Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() })
                          .ToList();

         ViewBag.ids = ids;

         return PartialView("_downloadSetting", list);
      }

      [HttpPost]
      public ActionResult MultiDownloadSetting(string ids, long settingId)
      {
         if (Request.IsAjaxRequest() && !string.IsNullOrEmpty(ids))
         {
            var cr = APDBDef.CroResource;
            var idArray = ConvertByString(ids);

            APQuery.update(cr).set(cr.DownloadStatePKID, settingId).where(cr.CrosourceId.In(idArray)).execute(db);

            return Json(new { cmd = "Processed", value = settingId, msg = "批量设定完成。" });
         }

         return IsNotAjax();
      }


      //
      // 批量报送和取消报送
      // POST:		/Crosource/MultiDelivery
      // POST:		/Crosource/MultiCancelDelivery
      //

      [HttpPost]
      public ActionResult MultiDelivery(string ids)
      {
         var user = ResSettings.SettingsInSession.User;

         long deliveryType = 0;
         if (user.UserTypePKID == ResUserHelper.ProvinceAdmin)
            deliveryType = CroResourceHelper.ProviceLevelDelivery;
         else if(user.UserTypePKID == ResUserHelper.CityAdmin)
            deliveryType = CroResourceHelper.CityLevelDelivery;

         var idArray = ConvertByString(ids);
         foreach (var id in idArray)
         {
            APBplDef.DeliveryRecordBpl.Insert(new DeliveryRecord { DeliveryTypePKID = deliveryType, Recorder = user.Id, ResourceId = id, AddTime = DateTime.Now });
          }
         return Json(new { cmd = "Processed", msg = "批量报送完成" });
      }

      [HttpPost]
      public ActionResult MultiCancelDelivery(string ids)
      {
         var user = ResSettings.SettingsInSession.User;
         var dr = APDBDef.DeliveryRecord;
         var idArray = ConvertByString(ids);

         APBplDef.DeliveryRecordBpl.ConditionDelete(dr.ResourceId.In(idArray) & dr.Recorder== user.Id );

         return Json(new { cmd = "Processed", msg = "取消报送完成" });
      }

      //
      // 在线答题列表
      // POSt:		/CroResurce/ExerciesList
      //

      public ActionResult ExerciesList(long courseId)
      {
         var exe = APDBDef.Exercises;
         var exei = APDBDef.ExercisesItem;

         var list = APQuery.select(exe.Name, exe.Answer, exe.ExerciseId, exei.Item, exei.ItemId, exei.Code)
                       .from(exe, exei.JoinInner(exe.ExerciseId == exei.ExerciseId))
                       .where(exe.CourseId == courseId)
                       .query(db, r =>
                         new
                         {
                            itemId = exei.ItemId.GetValue(r),
                            item = exei.Item.GetValue(r),
                            code = exei.Code.GetValue(r),
                            answer = exe.Answer.GetValue(r),
                            exeName = exe.Name.GetValue(r),
                            exeId = exe.ExerciseId.GetValue(r)
                         }
                       ).ToList();

         var models = new List<Exercises>();

         foreach (var item in list)
         {
            var model = models.FirstOrDefault(x => x.ExerciseId == item.exeId);
            if (null == model)
            {
               model = new Exercises { ExerciseId = item.exeId, Answer = item.answer, Name = item.exeName };
               model.Items = new List<ExercisesItem>();
               models.Add(model);
            }

            model.Items.Add(new ExercisesItem { Code = item.code, Item = item.item, ItemId = item.itemId });
         }

         return PartialView("_exercies", models);
      }


      public static object GetStrengthDict(List<ResPickListItem> items)
      {
         List<object> array = new List<object>();
         foreach (var item in items)
         {
            array.Add(new
            {
               key = item.StrengthenValue,
               id = item.PickListItemId,
               name = item.Name
            });
         }
         return array;
      }

      public static object GetStrengthDict(List<ResCompany> items)
      {
         List<object> array = new List<object>();
         foreach (var item in items)
         {
            array.Add(new
            {
               key = item.ParentId,
               id = item.CompanyId,
               name = item.CompanyName
            });
         }
         return array;
      }

      private long[] ConvertByString(string ids,char splitChar=',')
      {
         var array = ids.Split(splitChar);
         var idArray = new long[array.Length];
         for (int i = 0; i < array.Length; i++)
         {
            idArray[i] = Convert.ToInt64(array[i]);
         }

         return idArray;
      }


   }

}