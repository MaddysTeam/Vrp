using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

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
      static APDBDef.IndicationTableDef idi = APDBDef.Indication;

      public EvalController()
      {
         _pdfRender = new HtmlRender();
      }

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
         var user = ResSettings.SettingsInSession.User;
         var expertId = ResSettings.SettingsInSession.UserId;
         var query = APQuery.select(r.CrosourceId, r.Title, r.CourseTypePKID, r.Author, r.SubjectPKID, r.AuthorCompany, r.Keywords, r.GradePKID,
                                   eg.GroupName, eg.GroupId.As("groupId"), er.ResultId, er.Score, er.ResultId.As("resultId"))
                            .from(egr,
                                  r.JoinInner(egr.ResourceId == r.CrosourceId),
                                  eg.JoinInner(eg.GroupId == egr.GroupId),
                                  ege.JoinInner(eg.GroupId == ege.GroupId),
                                  u.JoinInner(ege.ExpertId == u.UserId & ege.ExpertId == expertId),
                                  er.JoinLeft(er.ResourceId == r.CrosourceId & er.GroupId == eg.GroupId & er.ExpertId == expertId)
                                  );

         if (!string.IsNullOrEmpty(searchPhrase))
         {
            query.where(eg.GroupName.Match(searchPhrase)
               | r.Title.Match(searchPhrase)
               | r.AuthorCompany.Match(searchPhrase)
               | r.Author.Match(searchPhrase)
               );
         }

         query.primary(r.CrosourceId)
              .skip((current - 1) * rowCount)
              .take(rowCount);

         var total = db.ExecuteSizeOfSelect(query);

         var results = query.query(db, rd =>
         {
            return new
            {
               id = er.ResultId.GetValue(rd, "resultId"),
               resId = r.CrosourceId.GetValue(rd),
               title = r.Title.GetValue(rd),
               author = r.Author.GetValue(rd),
               grade = CroResourceHelper.Grade.GetName(r.GradePKID.GetValue(rd)),
               subject = CroResourceHelper.Subject.GetName(r.SubjectPKID.GetValue(rd)),
               company = r.AuthorCompany.GetValue(rd),
               keywords = r.Keywords.GetValue(rd),
               groupName = eg.GroupName.GetValue(rd),
               groupId = eg.GroupId.GetValue(rd, "groupId"),
               score = er.Score.GetValue(rd),
               isEval = er.ResultId.GetValue(rd) > 0
            };
         }).ToList();

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

      public ActionResult Details(long id, long resId, long? courseId, long groupId, long? expertId)
      {
         var expert = expertId == null ? ResSettings.SettingsInSession.User : APBplDef.ResUserBpl.PrimaryGet(expertId.Value);
         if (expert == null) throw new ArgumentException("expert can not be null");

         var i = APDBDef.Indication;
         var a = APDBDef.Active;
         var er = APDBDef.EvalResult;

         var model = APBplDef.CroResourceBpl.GetResource(db, resId);

         var query = APQuery.select(i.IndicationId, i.Description, i.LevelPKID, i.Score, i.IndicationName,
                                    i.TypePKID, i.ActiveId, a.ActiveName, a.ActiveId,
                                    eri.ResultId, eri.Score.As("evalScore"),
                                    er.Comment, er.ExpertId)
            .from(i,
                 a.JoinInner(a.ActiveId == i.ActiveId),
                 eri.JoinLeft(eri.IndicationId == i.IndicationId & eri.ResultId == id),
                 er.JoinLeft(er.ResultId == eri.ResultId & er.ResultId == id)
            );

         string comment = string.Empty,
                expId = string.Empty;

         // 指标列表
         var list = query.query(db, r =>
         {
            comment = er.Comment.GetValue(r);
            expId = er.ExpertId.GetValue(r).ToString();

            var indication = new Indication();
            indication.IndicationName = i.IndicationName.GetValue(r);
            indication.ActiveId = a.ActiveId.GetValue(r);
            indication.ActiveName = a.ActiveName.GetValue(r);
            indication.IndicationId = i.IndicationId.GetValue(r);
            indication.Description = i.Description.GetValue(r);
            indication.EvalScore = eri.Score.GetValue(r, "evalScore");
            indication.Score = (int)eri.Score.GetValue(r);
            indication.LevelPKID = i.LevelPKID.GetValue(r);
            indication.TypePKID = i.TypePKID.GetValue(r);
            return indication;
         }).ToList();


         ViewBag.isSlef = (string.IsNullOrEmpty(expId) ? 0 : Convert.ToInt32(expId)) == ResSettings.SettingsInSession.UserId || (id == 0 && expert.UserTypePKID == ResUserHelper.Export);

         ViewBag.Indications = list;

         ViewBag.Comment = comment;

         ViewBag.CurrentCourse = courseId == null || courseId.Value == 0 ? model.Courses[0] : model.Courses.Find(c => c.CourseId == courseId.Value);

         return View(model);
      }


      //
      //	评审 - 评审导出至pdf
      // GET:		/Eval/Export
      //
      public ActionResult Export(long id, long resId, long groupId, long? expertId)
      {
         var expert = expertId == null ? ResSettings.SettingsInSession.User : APBplDef.ResUserBpl.PrimaryGet(expertId.Value);
         if (expert == null) throw new ArgumentException("expert can not be null");

         var i = APDBDef.Indication;
         var a = APDBDef.Active;
         var er = APDBDef.EvalResult;
         var u = APDBDef.ResUser;
         var ege = APDBDef.EvalGroupExpert;
         var egr = APDBDef.EvalGroupResource;

         var model = APBplDef.CroResourceBpl.GetResource(db, resId);
         var query = APQuery.select(i.IndicationId, i.Description, i.LevelPKID, i.Score, i.IndicationName,
                                    i.TypePKID, i.ActiveId, a.ActiveName, a.ActiveId,
                                    eri.ResultId, eri.Score.As("evalScore"),
                                    er.Comment, er.ExpertId)
            .from(i,
                 a.JoinInner(a.ActiveId == i.ActiveId),
                 eri.JoinLeft(eri.IndicationId == i.IndicationId & eri.ResultId == id),
                 er.JoinLeft(er.ResultId == eri.ResultId & er.ResultId == id)
            );

         string comment = string.Empty, expId = string.Empty;

         // indication list

         var list = query.query(db, r =>
         {
            comment = er.Comment.GetValue(r);
            expId = er.ExpertId.GetValue(r).ToString();

            var indication = new Indication();
            indication.IndicationName = i.IndicationName.GetValue(r);
            indication.ActiveId = a.ActiveId.GetValue(r);
            indication.ActiveName = a.ActiveName.GetValue(r);
            indication.IndicationId = i.IndicationId.GetValue(r);
            indication.Description = i.Description.GetValue(r);
            indication.EvalScore = eri.Score.GetValue(r, "evalScore");
            indication.Score = (int)eri.Score.GetValue(r);
            indication.LevelPKID = i.LevelPKID.GetValue(r);
            indication.TypePKID = i.TypePKID.GetValue(r);
            return indication;
         }).ToList();


         var subquery = APQuery.select(egr.GroupId).from(egr).where(egr.ResourceId == resId);

         var experts = APQuery.select(u.UserId, u.RealName)
             .from(
                 u,
                 ege.JoinLeft(ege.ExpertId == u.UserId)
                 )
             .where(ege.GroupId.In(subquery))
             .group_by(u.UserId, u.RealName)
             .query(db, r => u.RealName.GetValue(r))
             .ToArray();

         // Expert html to pdf

         var htmlText = _pdfRender.RenderViewToString(this, "Export", new EvalExportViewModel { Experts = experts, Resource = model, Indications = list, EvalComment = comment });
         byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);

         return new BinaryContentResult($"【{model.Title}】评审结果.pdf", "application/pdf", pdfFile);

         //return View(new EvalExportViewModel { Experts = experts, Resource = model, Indications = list, EvalComment = comment });
      }

      //
      //	评审 - 个人全部评审信息导出至pdf
      // GET:/Eval/ExportAll
      //
      public ActionResult ExportAll()
      {
         var user = ResSettings.SettingsInSession.User;
         var expertId = user.UserId;
         var models = GetExpertScoreViewModels(expertId);
         var indicaitons = db.IndicationDal.ConditionQuery(idi.ActiveId == ThisApp.CurrentActiveId, null, null, null);

         var htmlText = _pdfRender.RenderViewToString(this, "ExportAll", new ExportAllViewModel
         {
            ScoreModels = models,
            IndicationModels = indicaitons,
            GroupName = models.Count() <= 0 ? string.Empty : models.First().GroupName
         });

         byte[] pdfFile = FormatConverter.ConvertHtmlTextToPDF(htmlText);

         return new BinaryContentResult($"专家：{user.UserName} 的评审结果.pdf", "application/pdf", pdfFile);
      }


      //
      //	评审 - 个人全部评审信息导出至excel
      // GET:/Eval/ExpoertAllToExcel
      //
      public ActionResult ExpoertAllToExcel()
      {
         var user = ResSettings.SettingsInSession.User;
         var expertId = user.UserId;
         var models = GetExpertScoreViewModels(expertId);
         var book = NPOIHelper(models);

         // 写入到客户端 
         System.IO.MemoryStream ms = new System.IO.MemoryStream();
         book.Write(ms);
         ms.Seek(0, System.IO.SeekOrigin.Begin);
         DateTime dt = DateTime.Now;
         string dateTime = dt.ToString("yyyyMMdd");
         string fileName = $"专家：{user.UserName} 的评审结果.xls";
         return File(ms, "application/vnd.ms-excel", fileName);
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
            return Request.IsAjaxRequest() ? Json(new { msg = "系统参数异常，请联系管理员" }) : IsNotAjax();
         }

         var group = APBplDef.EvalGroupBpl.PrimaryGet(model.GroupId);
         if (group.EndDate < DateTime.Now)
         {
            return Request.IsAjaxRequest() ? Json(new { msg = "考核期已结束，无法在进行考核" }) : IsNotAjax();
         }
         if (group.StartDate > DateTime.Now)
         {
            return Request.IsAjaxRequest() ? Json(new { msg = "考核期未开始，无法在进行考核" }) : IsNotAjax();
         }

         var existEvalResource = APBplDef.EvalGroupResourceBpl.ConditionQueryCount(egr.GroupId == model.GroupId & egr.ResourceId == model.ResourceId);
         if (existEvalResource <= 0)
         {
            return Request.IsAjaxRequest() ? Json(new { error = "true", msg = "该资源考核参数疑似被篡改，请联系管理员" }) : IsNotAjax();
         }

         var exitsResult = APBplDef.EvalResultBpl.ConditionQuery(
            er.ExpertId == ResSettings.SettingsInSession.UserId
            & er.GroupId == model.GroupId
            & er.ResourceId == model.ResourceId, null);
         if (exitsResult != null && exitsResult.Count > 0 && exitsResult.First().ResultId != model.ResultId)
         {
            return Request.IsAjaxRequest() ? Json(new { error = "true", msg = "该资源考核疑似被篡改，请联系管理员" }) : IsNotAjax();
         }

         // 评审打分记录
         //var er = APDBDef.EvalResult;
         var evalRecords = APBplDef.EvalResultBpl.ConditionQuery(er.ResourceId == model.ResourceId, null);

         var list = APBplDef.IndicationBpl.GetAll();
         foreach (var item in model.Items)
         {
            var maxScore = list.Find(x => x.IndicationId == item.IndicationId).Score;
            if (item.Score < 0 || item.Score > maxScore)
               return Request.IsAjaxRequest() ? Json(new { error = "true", msg = "分数设置不合理，请检查" }) : IsNotAjax();
         }

         db.BeginTrans();

         try
         {
            if (APBplDef.EvalResultBpl.PrimaryGet(model.ResultId) != null)
            {
               APBplDef.EvalResultItemBpl.ConditionDelete(eri.ResultId == model.ResultId);
               APBplDef.EvalResultBpl.PrimaryDelete(model.ResultId);

               //如果重新打分则去除原来的打分记录
               evalRecords.RemoveAll(x => x.ResultId == model.ResultId);
            }

            double score = 0;
            model.AccessDate = DateTime.Now;
            model.ExpertId = ResSettings.SettingsInSession.UserId;
            model.Score = model.Items.Sum(m => m.Score);
            APBplDef.EvalResultBpl.Insert(model);

            foreach (var item in model.Items)
            {
               item.ResultId = model.ResultId;
               score += item.Score;
               APBplDef.EvalResultItemBpl.Insert(item);
            }

            // 更新总得分（该资源平均分）
            var fullScore = score;
            if (evalRecords != null && evalRecords.Count > 0)
               fullScore = (double)(evalRecords.Sum(x => x.Score) + score) / (evalRecords.Count + 1);

            APBplDef.CroResourceBpl.UpdatePartial(model.ResourceId, new { Score = fullScore });
         }
         catch
         {
            return Request.IsAjaxRequest() ? (ActionResult)Json(new { msg = "操作失败，请联系管理员" }) : IsNotAjax();
         }

         return Request.IsAjaxRequest() ? (ActionResult)Json(new { error = "none", msg = "操作成功" }) : IsNotAjax();
      }


      private HtmlRender _pdfRender;

      private List<ExportAllScoreViewModel> GetExpertScoreViewModels(long expertId)
      {


         var result = APQuery.select(r.CrosourceId, r.Title, r.Author, r.AuthorCompany, eg.GroupName,
                            er.ResultId, er.Score, er.Comment, eri.Score.As("detailScore"), idi.IndicationName)
                   .from(
                         egr,
                         r.JoinInner(egr.ResourceId == r.CrosourceId),
                         eg.JoinInner(eg.GroupId == egr.GroupId),
                         er.JoinLeft(er.ResourceId == r.CrosourceId & er.ExpertId == expertId),
                         eri.JoinLeft(er.ResultId == eri.ResultId),
                         idi.JoinLeft(idi.IndicationId == eri.IndicationId)
                         )
                         .where(egr.GroupId.In(APQuery.select(ege.GroupId).from(ege).where(ege.ExpertId == expertId)))
                         .query(db, rd =>
                         {
                            return new
                            {
                               id = r.CrosourceId.GetValue(rd),
                               title = r.Title.GetValue(rd),
                               Author = r.Author.GetValue(rd),
                               Compnay = r.AuthorCompany.GetValue(rd),
                               Score = er.Score.GetValue(rd),
                               IndicationScore = eri.Score.GetValue(rd, "detailScore"),
                               indicationName = idi.IndicationName.GetValue(rd),
                               comment = er.Comment.GetValue(rd),
                               IsEval = er.ResultId.GetValue(rd) > 0,
                               groupName = eg.GroupName.GetValue(rd)
                            };
                         }).ToList();

         var models = new List<ExportAllScoreViewModel>();
         var index = 1;

         foreach (var item in result)
         {
            if (!models.Exists(x => x.Id == item.id))
            {

               models.Add(new ExportAllScoreViewModel
               {
                  Id = item.id,
                  ResourceName = item.title,
                  Author = item.Author,
                  AuthorCompany = item.Compnay,
                  Comment = item.comment,
                  Score = item.Score,
                  Score1 = item.IndicationScore,
                  IsEval = item.IsEval,
                  GroupName = item.groupName
               });
            }
            else
            {
               var model = models.Find(x => x.Id == item.id);
               var dynamicColumnCount = typeof(ExportAllScoreViewModel)
                  .GetProperties().ToList()
                  .FindAll(x => x.Name.IndexOf("Score") == 0)
                  .Count() - 1;
               for (int i = 2; i <= dynamicColumnCount; i++)
               {
                  if (index == i || index % dynamicColumnCount == i)
                  {
                     model.GetType().GetProperty("Score" + i).SetValue(model, item.IndicationScore);
                  }
                  if (index == dynamicColumnCount || index % dynamicColumnCount == 0)
                  {
                     model.GetType().GetProperty("Score" + dynamicColumnCount).SetValue(model, item.IndicationScore);
                  }
               }
            }

            index++;
         }

         return models;
      }

      private HSSFWorkbook NPOIHelper(List<ExportAllScoreViewModel> models)
      {
         //创建Excel文件的对象
         NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
         //添加一个sheet
         NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");

         ICellStyle cellstyle = book.CreateCellStyle();
         cellstyle.VerticalAlignment = VerticalAlignment.Center;
         cellstyle.Alignment = HorizontalAlignment.Center;
         //sheet1.SetColumnWidth(0,100);
         //sheet1.SetColumnWidth(1, 100);
         //sheet1.SetColumnWidth(2, 100);
         //sheet1.SetColumnWidth(3, 100);
         //sheet1.SetColumnWidth(4, 100);
         //sheet1.SetColumnWidth(5, 100);
         //sheet1.SetColumnWidth(6, 100);

         #region [头部设计]

         //给sheet1添加第一行的头部标题
         NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
         row1.CreateCell(0).SetCellValue("序号");
         row1.CreateCell(1).SetCellValue("作品名称");
         row1.CreateCell(2).SetCellValue("作者姓名");
         row1.CreateCell(3).SetCellValue("单位名称");
         row1.CreateCell(4).SetCellValue("评审指标");
         row1.CreateCell(8).SetCellValue("总分");
         row1.CreateCell(9).SetCellValue("评审意见");
         row1.Cells.ForEach(ce=> {
            ce.CellStyle = cellstyle;
         });

         //给sheet1添加第二行的头部标题
         NPOI.SS.UserModel.IRow row2 = sheet1.CreateRow(1);
         row2.CreateCell(4).SetCellValue("教学设计(25)");
         row2.CreateCell(5).SetCellValue("教学行为(25)");
         row2.CreateCell(6).SetCellValue("教学效果(25)");
         row2.CreateCell(7).SetCellValue("创新与实用(25)");
         row2.Cells.ForEach(ce => {
            ce.CellStyle = cellstyle;
         });

         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 0, 0));  //	序号
         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 1, 1));  //	作品名称
         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 2, 2));  
         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 3, 3));
         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 4, 7));
         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 8, 8));
         sheet1.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, 9, 9));

         #endregion

         var i = 0;
         foreach (var item in models)  
         {
            i++;
            NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
            rowtemp.CreateCell(0).SetCellValue(i);
            rowtemp.CreateCell(1).SetCellValue(item.ResourceName);
            rowtemp.CreateCell(2).SetCellValue(item.Author);
            rowtemp.CreateCell(3).SetCellValue(item.AuthorCompany);
            rowtemp.CreateCell(4).SetCellValue(item.Score1);
            rowtemp.CreateCell(5).SetCellValue(item.Score2);
            rowtemp.CreateCell(6).SetCellValue(item.Score3);
            rowtemp.CreateCell(7).SetCellValue(item.Score4);
            rowtemp.CreateCell(8).SetCellValue(item.Score);
            rowtemp.CreateCell(9).SetCellValue(item.Comment);
         }

         NPOI.SS.UserModel.IRow rowtemp2 = sheet1.CreateRow(i + 3);
         rowtemp2.CreateCell(1).SetCellValue("评审组:");
         rowtemp2.CreateCell(2).SetCellValue(models.FirstOrDefault().GroupName);
         rowtemp2.CreateCell(4).SetCellValue("评审专家:");
         rowtemp2.CreateCell(8).SetCellValue("评审时间:");

         return book;
      }

   }

}