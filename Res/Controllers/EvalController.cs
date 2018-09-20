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

   public class EvalController : BaseController
   {

      static APDBDef.EvalGroupTableDef eg = APDBDef.EvalGroup;
      static APDBDef.CroResourceTableDef r = APDBDef.CroResource;
      static APDBDef.EvalGroupExpertTableDef ege = APDBDef.EvalGroupExpert;
      static APDBDef.EvalGroupResourceTableDef egr = APDBDef.EvalGroupResource;
      static APDBDef.ResUserTableDef u = APDBDef.ResUser;
      static APDBDef.EvalResultTableDef er = APDBDef.EvalResult;
      static APDBDef.EvalResultItemTableDef eri = APDBDef.EvalResultItem;

      //
      //	评审 - 查询
      // GET:		/Eval/Search
      // POST:		/Eval/Search
      //

      public ActionResult Search()
      {
         return View();
      }

      [HttpPost]
      public ActionResult Search(int current, int rowCount, string searchPhrase, FormCollection fc)
      {
         var expertId = ResSettings.SettingsInSession.UserId;
         var query = APQuery.select(r.CrosourceId, r.Title, r.CourseTypePKID, r.Author, r.StagePKID, r.SubjectPKID, r.AuthorCompany, r.Keywords, eg.GroupName, eg.GroupId, er.Score)
                            .from(egr,
                                  r.JoinInner(egr.ResourceId == r.CrosourceId),
                                  eg.JoinInner(eg.GroupId == egr.GroupId),
                                  ege.JoinInner(eg.GroupId == ege.GroupId),
                                  u.JoinInner(ege.ExpertId == u.UserId & ege.ExpertId == expertId),
                                  er.JoinLeft(er.ResourceId == r.CrosourceId & er.GroupId == eg.GroupId & er.ExpertId == expertId)
                                  );

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(eg.GroupName.Match(searchPhrase));
         }

         query.primary(r.CrosourceId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);

         var results = query.query(db, rd =>
         {
            return new
            {
               id = er.ResultId.GetValue(rd),
               resId = r.CrosourceId.GetValue(rd),
               title = r.Title.GetValue(rd),
               author = r.Author.GetValue(rd),
               stage = CroResourceHelper.Stage.GetName(r.StagePKID.GetValue(rd)),
               subject = CroResourceHelper.Subject.GetName(r.SubjectPKID.GetValue(rd)),
               company = r.AuthorCompany.GetValue(rd),
               keywords = r.Keywords.GetValue(rd),
               groupName = eg.GroupName.GetValue(rd),
               groupId = eg.GroupId.GetValue(rd),
               score = er.Score.GetValue(rd),
            };
         });

         return Json(new
         {
            rows = results,
            current,
            rowCount,
            total
         });
      }


      //
      //	评审 - 评审明细
      // GET:		/Eval/Details
      //

      public ActionResult Details(long id, long resId, long groupId)
      {
         var i = APDBDef.Indication;
         var a = APDBDef.Active;

         var model = APBplDef.CroResourceBpl.PrimaryGet(resId);

         var list = APQuery.select(i.IndicationId, i.Description, i.LevelPKID, i.Score,
                                    i.TypePKID, i.ActiveId, a.ActiveName, a.ActiveId,
                                    eri.ResultId, eri.Score.As("evalScore"))
            .from(i,
                 a.JoinInner(a.ActiveId == i.ActiveId),
                 eri.JoinLeft(eri.IndicationId == i.IndicationId & eri.ResultId == id)
            ).query(db, r =>
            {
               var indication = new Indication();
               indication.ActiveId = a.ActiveId.GetValue(r);
               indication.ActiveName = a.ActiveName.GetValue(r);
               indication.IndicationId = i.IndicationId.GetValue(r);
               indication.Description = i.Description.GetValue(r);
               indication.EvalScore = eri.Score.GetValue(r, "evalScore");
               indication.Score = i.Score.GetValue(r);
               indication.LevelPKID = i.LevelPKID.GetValue(r);
               indication.TypePKID = i.TypePKID.GetValue(r);
               return indication;
            }).ToList();

         ViewBag.Indications = list;

         return View(model);
      }


      //
      //	评审 - 执行评审
      // POST:		/Eval/Execute
      //

      [HttpPost]
      public ActionResult Execute(EvalResult model)
      {
         if (model == null || model.Items == null || model.Items.Count <= 0)
         {
            return Request.IsAjaxRequest() ? (ActionResult)Json(new { msg = "系统参数异常，请联系管理员" }) : IsNotAjax();
         }

         var list = APBplDef.IndicationBpl.GetAll();
         foreach (var item in model.Items)
         {
            var maxScore = list.Find(x => x.IndicationId == item.IndicationId).Score;
            if (item.Score < maxScore || item.Score > maxScore)
               return Request.IsAjaxRequest() ? (ActionResult)Json(new {  msg = "分数设置不合理，请检查" }) : IsNotAjax();
         }

         db.BeginTrans();

         try
         {
            if (APBplDef.EvalResultBpl.PrimaryGet(model.ResultId) != null)
            {
               APBplDef.EvalResultItemBpl.ConditionDelete(eri.ResultId == model.ResultId);
               APBplDef.EvalResultBpl.PrimaryDelete(model.ResultId);
            }

            double score = 0;
            foreach (var item in model.Items)
            {
               item.ResultId = model.ResultId;
               score += item.Score;
               APBplDef.EvalResultItemBpl.Insert(item);
            }



            model.AccessDate = DateTime.Now;
            model.ExpertId = ResSettings.SettingsInSession.UserId;
            model.Score = score;
            APBplDef.EvalResultBpl.Insert(model);  
         }
         catch
         {
            return Request.IsAjaxRequest() ? (ActionResult)Json(new { msg = "操作失败，请联系管理员" }) : IsNotAjax();
         }

         return Request.IsAjaxRequest() ? (ActionResult)Json(new { msg="操作成功"}) : IsNotAjax();
      }

   }

}