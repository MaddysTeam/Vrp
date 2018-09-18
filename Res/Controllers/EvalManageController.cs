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
      // GET:		/EvalGroup/Search
      // POST:		/EvalGroup/Search
      //

      public ActionResult Search()
      {
         return View();
      }

      [HttpPost]
      public ActionResult Search(int current, int rowCount, string searchPhrase, FormCollection fc)
      {
         var eg = APDBDef.EvalGroup;
         var query = APQuery
             .select(eg.GroupId, eg.GroupName, eg.LevelPKID) //t.MediumTypePKID,
             .from(eg);

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(eg.GroupName.Match(searchPhrase));
         }

         query.primary(eg.GroupId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);
         var egs = db.Query(query, eg.TolerantMap).ToList();
         var list = (from c in egs
                     select new
                     {
                        id = c.GroupId,
                        GroupName = c.GroupName,
                        Level = c.Level,
                        ActiveId = CurrentActive.ActiveId,
                        ActiveName = CurrentActive.ActiveName
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
      // GET:		/EvalGroup/Edit
      // POST     /EvalGroup/Edit
      //

      public ActionResult Edit(long? id)
      {
         EvalGroup model = null;
         if (id == null)
         {
            model = new EvalGroup
            {
               ActiveId = CurrentActive.ActiveId,
               ActiveName = CurrentActive.ActiveName
            };
         }
         else
         {
            model = APBplDef.EvalGroupBpl.PrimaryGet(id.Value);
            model.ActiveId = CurrentActive.ActiveId;
            model.ActiveName = CurrentActive.ActiveName;
         }

         return PartialView(model);
      }

      [HttpPost]
      public ActionResult Edit(EvalGroup model)
      {
         if (model.GroupId == 0)
         {
            model.ActiveId = CurrentActive.ActiveId;
            APBplDef.EvalGroupBpl.Insert(model);
         }
         else
         {

         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }



      // GET: ExpManage/ExpList
      // POST-Ajax: ExpManage/ExpList

      public ActionResult ExpList(int id)
      {
         return PartialView(id);
      }

      [HttpPost]
      public ActionResult ExpList(int current, int rowCount, string searchPhrase, int id)
      {
         ThrowNotAjax();

         var u = APDBDef.ResUser;
         var ur = APDBDef.ResUserRole;
         var ege = APDBDef.EvalGroupExpert;

         var query = APQuery.select(u.UserId, u.RealName, u.UserName, ege.GroupExpertId)
             .from(
                 u,
                 ur.JoinLeft(u.UserId == ur.UserId),
                 ege.JoinLeft(u.UserId == ege.ExpertId)
                 )
             //.where(up.UserType == BzRoleNames.Teacher)
             .primary(u.UserId)
             .skip((current - 1) * rowCount)
             .take(rowCount);


         //过滤条件
         //模糊搜索用户名、实名进行

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            searchPhrase = searchPhrase.Trim();
            query.where_and(u.RealName.Match(searchPhrase));
         }


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

         var r = APDBDef.CroResource;
         var egr = APDBDef.EvalGroupResource;

         var query = APQuery.select(r.CrosourceId, r.Title, r.AuthorCompany, r.Author, r.SubjectPKID, r.GradePKID, egr.GroupResourceId)
             .from(r, egr.JoinLeft(r.CrosourceId == egr.ResourceId))
             //.where(up.UserType == BzRoleNames.Teacher)
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
               resId = r.CrosourceId.GetValue(rd),
               title = r.Title.GetValue(rd),
               company = r.AuthorCompany.GetValue(rd),
               author = r.Author.GetValue(rd),
               subject = "fuck",
               grade = "二年级",
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
      // POST:		/EvalGroup/AssignResource
      //

      [HttpPost]
      public ActionResult AssignRes(long id, long resId)
      {
         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }

      //
      // 分配参与评审组的专家
      // POST:		/EvalGroup/AssignExp
      //

      [HttpPost]
      public ActionResult AssignExp(long id, long expId)
      {
         var ege = APDBDef.EvalGroupExpert;
         var isExist = APBplDef.EvalGroupExpertBpl.ConditionQueryCount(ege.ExpertId == expId & ege.GroupId == id) > 0;
         if (!isExist)
         {
            APBplDef.EvalGroupExpertBpl.Insert(new EvalGroupExpert { ExpertId = expId, GroupId = id });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      //
      // 评审指标列表
      // POST:		/EvalGroup/IndicationList
      //

      public ActionResult IndicationList()
      {
         return View();
      }


      public ActionResult IndicationList(int current,int rowCount,string searchPhrase)
      {

         ThrowNotAjax();

         //var i = APDBDef.Indication;

         //var query = APQuery.select(i.IndicationId, i.IndicationName, i.Type, r.Author, r.SubjectPKID, r.GradePKID, egr.GroupResourceId)
         //    .from(r, egr.JoinLeft(r.CrosourceId == egr.ResourceId))
         //    //.where(up.UserType == BzRoleNames.Teacher)
         //    .primary(r.CrosourceId)
         //    .skip((current - 1) * rowCount)
         //    .take(rowCount);


         ////过滤条件
         ////模糊搜索用户名、实名进行

         //if (!string.IsNullOrEmpty(searchPhrase))
         //{
         //   searchPhrase = searchPhrase.Trim();
         //   query.where_and(r.Author.Match(searchPhrase));
         //}


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

         //var total = db.ExecuteSizeOfSelect(query);


         //查询结果集

         //var result = query.query(db, rd =>
         //{
         //   return new
         //   {
         //      //resId = r.CrosourceId.GetValue(rd),
         //      //title = r.Title.GetValue(rd),
         //      //company = r.AuthorCompany.GetValue(rd),
         //      //author = r.Author.GetValue(rd),
         //      //subject = "fuck",
         //      //grade = "二年级",
         //      //isSelect = egr.GroupResourceId.GetValue(rd) > 0
         //   };
         //}).ToList();

         return Json(new
         {
            //rows = null,
            current,
            rowCount,
            //total
         });
      }


   }

}