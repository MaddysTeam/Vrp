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

   public class EvalGroupController : BaseController
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
             .select(eg.GroupName, eg.LevelPKID) //t.MediumTypePKID,
             .from(eg);

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(eg.GroupName.Match(searchPhrase));
         }

         query.primary(eg.GroupId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);

         var result = query.query(db, r => {

            return new { };
         });


         return Json(new { });
      }


      //
      // 编辑
      // GET:		/EvalGroup/Edit
      // POST     /EvalGroup/Edit
      //

      public ActionResult Edit(long id)
      {
         return View();
      }

      [HttpPost]
      public ActionResult Edit(EvalGroup model)
      {
         return View();
      }

      //
      // 添加/删除评审组可打分的微课资源
      // GET:		/EvalGroup/Edit
      //

      [HttpPost]
      public ActionResult AddOrRemoveResource(long id, long resId)
      {
         return Json(new { });
      }

      //
      // 添加/删除参与评审组的专家
      // GET:		/EvalGroup/Edit
      //

      [HttpPost]
      public ActionResult AddOrRemoveExpert(long id, long expId)
      {
         return Json(new { });
      }

   }

}