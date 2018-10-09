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

   public class EvalManageController : BaseController
   {

      //
      //	评审组 - 查询
      // GET:		/EvalManage/Search
      // POST:		/EvalManage/Search
      //

      public ActionResult Search()
      {
         return View();
      }

      [HttpPost]
      public ActionResult Search(int current, int rowCount, string searchPhrase, FormCollection fc)
      {
         var user = ResSettings.SettingsInSession.User;
         var eg = APDBDef.EvalGroup;
         var a = APDBDef.Active;
         var query = APQuery
             .select(eg.GroupId, eg.GroupName, eg.LevelPKID, eg.StartDate, eg.EndDate, a.ActiveId, a.ActiveName) //t.MediumTypePKID,
             .from(eg, a.JoinInner(eg.ActiveId == a.ActiveId));

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(eg.GroupName.Match(searchPhrase));
         }

         if (user.ProvinceId > 0)
            query.where_and(eg.ProvinceId == user.ProvinceId);

         if (user.AreaId > 0)
            query.where_and(eg.AreaId == user.AreaId);

         if (user.CompanyId > 0)
            query.where_and(eg.CompanyId == user.CompanyId);

         query.primary(eg.GroupId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);
         var egs = query.query(db, r =>
                                 new EvalGroup
                                 {
                                    GroupId = eg.GroupId.GetValue(r),
                                    GroupName = eg.GroupName.GetValue(r),
                                    LevelPKID = eg.LevelPKID.GetValue(r),
                                    ActiveId = a.ActiveId.GetValue(r),
                                    ActiveName = a.ActiveName.GetValue(r),
                                    StartDate = eg.StartDate.GetValue(r),
                                    EndDate = eg.EndDate.GetValue(r)
                                 }).ToList();
         var list = (from c in egs
                     select new
                     {
                        id = c.GroupId,
                        gropupName = c.GroupName,
                        level = c.Level,
                        activeId = c.ActiveId,
                        activeName = c.ActiveName,
                        start = c.StartDate.ToString("yyyy-MM-dd"),
                        end = c.EndDate.ToString("yyyy-MM-dd")
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
      // 编辑评审组
      // GET:		/EvalManage/Edit
      // POST     /EvalManage/Edit
      //

      public ActionResult Edit(long? id)
      {
         EvalGroup model = null;
         if (id == null)
         {
            model = new EvalGroup
            {
               StartDate = DateTime.Now,
               EndDate = DateTime.Now.AddMonths(2)
            };
         }
         else
         {
            model = APBplDef.EvalGroupBpl.PrimaryGet(id.Value);
         }

         ViewBag.Actives = APBplDef.ActiveBpl.GetAll();

         return PartialView(model);
      }

      [HttpPost]
      public ActionResult Edit(EvalGroup model)
      {
         var user = ResSettings.SettingsInSession.User;
         model.LevelPKID = EvalGroupHelper.UnionLevel;

         if (user.ProvinceId > 0)
         {
            model.ProvinceId = user.ProvinceId;
            model.LevelPKID = EvalGroupHelper.ProvinceLevel;
         }

         if (user.AreaId > 0)
         {
            model.LevelPKID = EvalGroupHelper.CityLevel;
            model.AreaId = user.AreaId;
         }

         if (user.CompanyId > 0)
         {
            model.LevelPKID = EvalGroupHelper.CityLevel;
            model.CompanyId = user.CompanyId;
         }

         if (model.GroupId == 0)
         {
            APBplDef.EvalGroupBpl.Insert(model);
         }
         else
         {
            APBplDef.EvalGroupBpl.UpdatePartial(model.GroupId, new
            {
               Name = model.GroupName,
               LevelPKID = model.LevelPKID,
               StartDate = model.StartDate,
               EndDate = model.EndDate
            });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }



      // GET: EvalManage/ExpList
      // POST-Ajax: EvalManage/ExpList

      public ActionResult ExpList(int id)
      {
         return PartialView(id);
      }

      [HttpPost]
      public ActionResult ExpList(int current, int rowCount, string searchPhrase, int id)
      {
         ThrowNotAjax();

         var user = ResSettings.SettingsInSession.User;

         var u = APDBDef.ResUser;
         var ege = APDBDef.EvalGroupExpert;

         var query = APQuery.select(u.UserId, u.RealName, u.UserName, ege.GroupExpertId)
             .from(
                 u,
                 ege.JoinLeft(u.UserId == ege.ExpertId & ege.GroupId == id)
                 )
             .where(u.UserTypePKID == ResUserHelper.Export)
             .primary(u.UserId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            searchPhrase = searchPhrase.Trim();
            query.where_and(u.RealName.Match(searchPhrase) | u.UserName.Match(searchPhrase));
         }

         //TODO: 角色数据范围过滤

         //if (user.ProvinceId > 0)
         //   query.where_and(u.ProvinceId == user.ProvinceId);
         //if (user.AreaId > 0 && user.ProvinceId != ResCompanyHelper.Shanghai) //如果非上海，其他市区id都是areaId
         //   query.where_and(u.AreaId == user.AreaId);


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


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            return new
            {
               id = ege.GroupExpertId.GetValue(rd),
               expId = u.UserId.GetValue(rd),
               realName = u.RealName.GetValue(rd),
               userName = u.UserName.GetValue(rd),
               isSelect = ege.GroupExpertId.GetValue(rd) > 0
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

      public ActionResult ResList(int id)
      {
         return PartialView(id);
      }

      [HttpPost]
      public ActionResult ResList(int current, int rowCount, string searchPhrase, int id)
      {
         ThrowNotAjax();

         var user = ResSettings.SettingsInSession.User;

         var r = APDBDef.CroResource;
         var egr = APDBDef.EvalGroupResource;

         var query = APQuery.select(r.CrosourceId, r.Title, r.AuthorCompany, r.Author, r.SubjectPKID, r.GradePKID, egr.GroupResourceId)
             .from(r, egr.JoinLeft(r.CrosourceId == egr.ResourceId & egr.GroupId == id))
             .primary(r.CrosourceId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            searchPhrase = searchPhrase.Trim();
            query.where_and(r.Author.Match(searchPhrase));
         }

         //TODO:角色数据范围过滤

         //if (user.ProvinceId > 0)
         //   query.where_and(r.ProvinceId == user.ProvinceId);
         //if (user.AreaId > 0)
         //   query.where_and(r.AreaId == user.AreaId);
         //if (user.CompanyId > 0)
         //   query.where_and(r.CompanyId == user.CompanyId);

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


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, rd =>
         {
            return new
            {
               id = egr.GroupResourceId.GetValue(rd),
               resId = r.CrosourceId.GetValue(rd),
               title = r.Title.GetValue(rd),
               company = r.AuthorCompany.GetValue(rd),
               author = r.Author.GetValue(rd),
               isSelect = egr.GroupResourceId.GetValue(rd) > 0
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
      // POST:		/EvalManage/AssignResource
      // POST:    /EvalManage/RemoveResource

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
      // POST:		/EvalManage/AssignExp
      // POST:    /EvalManage/RemoveExp

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


      //
      // 评审指标列表
      // GET:		/EvalManage/IndicationList
      // POST:		/EvalManage/IndicationList

      public ActionResult IndicationList()
      {
         return View();
      }

      [HttpPost]
      public ActionResult IndicationList(int current, int rowCount, string searchPhrase)
      {

         ThrowNotAjax();

         var i = APDBDef.Indication;
         var a = APDBDef.Active;

         var query = APQuery.select(i.IndicationId, i.IndicationName, i.Description, i.Score,
            i.LevelPKID, i.TypePKID, i.ActiveId, a.ActiveName
            )
            .from(i, a.JoinInner(a.ActiveId == i.ActiveId))
            .primary(i.IndicationId)
            .skip((current - 1) * rowCount)
            .take(rowCount);
         //.where(i.IndicationStatus==IndicationKeys.EnabelStatus);


         //过滤条件
         //模糊搜索

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            searchPhrase = searchPhrase.Trim();
            query.where_and(i.IndicationName.Match(searchPhrase));
         }


         //排序条件表达式

         //if (sort != null)
         //{
         //   switch (sort.ID)
         //   {
         //      //case "userName": query.order_by(sort.OrderBy(u.UserName)); break;
         //      //case "realName": query.order_by(sort.OrderBy(u.RealName)); break;
         //      //case "userType": query.order_by(sort.OrderBy(u.UserType)); break;
         //   }
         //}


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         var result = query.query(db, r =>
         {
            return new
            {
               id = i.IndicationId.GetValue(r),
               name = i.IndicationName.GetValue(r),
               description = i.Description.GetValue(r),
               level = IndicationHelper.Level.GetName(i.LevelPKID.GetValue(r)),
               type = IndicationHelper.Type.GetName(i.TypePKID.GetValue(r)),
               activeName = a.ActiveName.GetValue(r),
               activeId = i.ActiveId.GetValue(r),
               score = i.Score.GetValue(r)
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
      // 评审指标编辑
      // GET:		/EvalManage/IndicationEdit
      // POST:		/EvalManage/IndicationEdit

      public ActionResult IndicationEdit(long? id)
      {
         Indication model = null;
         if (id == null)
         {
            model = new Indication();
         }
         else
         {
            model = APBplDef.IndicationBpl.PrimaryGet(id.Value);
         }

         ViewBag.Actives = APBplDef.ActiveBpl.GetAll();

         return PartialView(model);
      }


      [HttpPost]
      public ActionResult IndicationEdit(Indication model)
      {
         if (model.IndicationId == 0)
         {
            APBplDef.IndicationBpl.Insert(model);
         }
         else
         {
            APBplDef.IndicationBpl.UpdatePartial(model.IndicationId, new
            {
               IndicationName = model.IndicationName,
               LevelPKID = model.LevelPKID,
               TypePKID = model.TypePKID,
               Description = model.Description,
               Score = model.Score,
               ActiveId = model.ActiveId
            });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      //
      // 评审结果列表
      // GET:		/EvalManage/EvalResultList
      // POST:		/EvalManage/EvalResultList

      public ActionResult EvalResultList(long? expertId, long? groupId)
      {
         return View();
      }

      [HttpPost]
      public ActionResult EvalResultList(long activeId, long groupid, long expertId, int current, int rowCount, string searchPhrase)
      {
         var user = ResSettings.SettingsInSession.User;

         var a = APDBDef.Active;
         var g = APDBDef.EvalGroup;
         var u = APDBDef.ResUser;
         var cr = APDBDef.CroResource;
         var er = APDBDef.EvalResult;

         var query = APQuery.select(er.ResultId, er.ExpertId, er.GroupId, er.AccessDate, er.Score,er.Comment,
                                    cr.CrosourceId, cr.Title, cr.Score.As("AverageScore"), u.UserName, u.UserId,
                                    g.GroupName, g.GroupId, a.ActiveName, a.ActiveId)
                          .from(er,
                                cr.JoinInner(cr.CrosourceId == er.ResourceId),
                                u.JoinInner(u.UserId == er.ExpertId),
                                g.JoinInner(er.GroupId == g.GroupId),
                                a.JoinInner(a.ActiveId == cr.ActiveId)
                                );
         if (activeId > 0)
            query = query.where_and(cr.ActiveId == activeId);
         if (groupid > 0)
            query = query.where_and(er.GroupId == groupid);
         if (expertId > 0)
            query = query.where_and(er.ExpertId == expertId);
         if (user.ProvinceId > 0)
            query.where_and(cr.ProvinceId == user.ProvinceId);
         if (user.AreaId > 0)
            query.where_and(cr.AreaId == user.AreaId);
         if (user.CompanyId > 0)
            query.where_and(cr.CompanyId == user.CompanyId);

         //if (resourceId > 0)
         //   query = query.where_and(cr.CrosourceId == resourceId);


         //过滤条件
         //模糊搜索

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            //query=query.
         }


         //获得查询的总数量

         var total = db.ExecuteSizeOfSelect(query);

         //查询结果集

         var result = query.query(db, r =>
         {
            return new
            {
               id = er.ResultId.GetValue(r),
               sourceId = cr.CrosourceId.GetValue(r),
               title = cr.Title.GetValue(r),
               date = er.AccessDate.GetValue(r),
               expert = u.UserName.GetValue(r),
               expertId = u.UserId.GetValue(r),
               averageScore = cr.Score.GetValue(r, "AverageScore"),
               score = er.Score.GetValue(r),
               group = g.GroupName.GetValue(r),
               groupId = g.GroupId.GetValue(r),
               active = a.ActiveName.GetValue(r),
               activeId = a.ActiveId.GetValue(r),
               comment=er.Comment.GetValue(r),
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
      // 评审进度列表
      // GET:		/EvalManage/GroupEvalProgress
      // POST:		/EvalManage/GroupEvalProgress

      public ActionResult EvalProgress(long id)
      {
         return PartialView();
      }

      [HttpPost]
      public ActionResult EvalProgress(long id, int current, int rowCount, string searchPhrase)
      {
         var ege = APDBDef.EvalGroupExpert;
         var egr = APDBDef.EvalGroupResource;
         var g = APDBDef.EvalGroup;
         var u = APDBDef.ResUser;
         var er = APDBDef.EvalResult;

         var groupResourceCount = APBplDef.EvalGroupResourceBpl.ConditionQueryCount(egr.GroupId == id);
         var result = APQuery.select(er.ExpertId, u.UserName, u.RealName, g.GroupId, g.GroupName, er.ResultId.Count().As("evalCount"))
                            .from(g,
                                  ege.JoinInner(ege.GroupId == g.GroupId),
                                  u.JoinInner(ege.ExpertId == u.UserId),
                                  er.JoinLeft(er.GroupId == g.GroupId & er.ExpertId == u.UserId))
                            .group_by(er.ExpertId, u.UserName, u.RealName, g.GroupId, g.GroupName)
                            .where(g.GroupId == id)
                            .skip(rowCount * (current - 1))
                            .take(rowCount)
                            .query(db, r => new EvalProgress
                            {
                               ExpertId = er.ExpertId.GetValue(r),
                               Expert = u.RealName.GetValue(r),
                               GorupId = g.GroupId.GetValue(r),
                               GroupName = g.GroupName.GetValue(r),
                               Percent = ((double)er.ResultId.GetValue(r, "evalCount") / (groupResourceCount == 0 ? 1 : groupResourceCount)) * 100
                            }).ToList();


         return Json(new
         {
            rows = result,
            current,
            rowCount,
            total = result.Count
         });
      }

   }

}