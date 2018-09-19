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

         var query = APQuery.select(r.CrosourceId, r.Title, r.CourseTypePKID, r.Author, r.StagePKID, r.SubjectPKID, r.AuthorCompany, r.Keywords,eg.GroupName,eg.GroupId)
                            .from(egr,
                                  r.JoinInner(egr.ResourceId == r.CrosourceId),
                                  eg.JoinInner(eg.GroupId == egr.GroupId),
                                  ege.JoinInner(eg.GroupId == ege.GroupId),
                                  u.JoinInner(ege.ExpertId == u.UserId & ege.ExpertId == ResSettings.SettingsInSession.UserId)
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
               resId=r.CrosourceId.GetValue(rd),
               title= r.Title.GetValue(rd),
               auther=r.Author.GetValue(rd),
               stage= CroResourceHelper.Stage.GetName(r.StagePKID.GetValue(rd)),
               subject=CroResourceHelper.Subject.GetName(r.SubjectPKID.GetValue(rd)),
               company=r.AuthorCompany.GetValue(rd),
               keywords=r.Keywords.GetValue(rd),
               groupName=eg.GroupName.GetValue(rd),
               groupId=eg.GroupId.GetValue(rd)
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

   }

}