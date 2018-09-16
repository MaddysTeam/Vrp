
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
			return View();
		}

		[HttpPost]
		public ActionResult Search(int current, int rowCount, string searchPhrase, FormCollection fc)
		{
			//----------------------------------------------------------
			var t = APDBDef.CroResource;
			var u = APDBDef.ResUser;
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
					//case "MediumType": order = new APSqlOrderPhrase(t.MediumTypePKID, co.Order); break;
					case "CreatedTime": order = new APSqlOrderPhrase(t.CreatedTime, co.Order); break;
					case "State": order = new APSqlOrderPhrase(t.StatePKID, co.Order); break;
				}
			}

			// 按作品标题过滤
			if (searchPhrase != null)
			{
				searchPhrase = searchPhrase.Trim();
				if (searchPhrase != "")
					where &= t.Title.Match(searchPhrase);
			}


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
                                     Type="微课程", // 微课或微课程
                                     //cro.MediumType,
                                     CreatedTime = cro.CreatedTime.ToString("yyyy-MM-dd"),
                                     cro.State,
                                     cro.StatePKID,
                                     cro.EliteScore
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
					//case "MediumType": order = new APSqlOrderPhrase(t.MediumTypePKID, co.Order); break;
					case "CreatedTime": order = new APSqlOrderPhrase(t.CreatedTime, co.Order); break;
					case "State": order = new APSqlOrderPhrase(t.StatePKID, co.Order); break;

					case "ViewCount": order = new APSqlOrderPhrase(t.ViewCount, co.Order); break;
					case "DownCount": order = new APSqlOrderPhrase(t.DownCount, co.Order); break;
					case "FavoriteCount": order = new APSqlOrderPhrase(t.FavoriteCount, co.Order); break;
					case "CommentCount": order = new APSqlOrderPhrase(t.CommentCount, co.Order); break;
					//case "StarTotal": order = new APSqlOrderPhrase(t.StarTotal, co.Order); break;

				
					case "cmd_elite": order = new APSqlOrderPhrase(t.EliteScore, co.Order); break;

				}
			}

			// 取过滤条件
			foreach (string cond in fc.Keys)
			{
				switch (cond)
				{
					//case "Domain": conds.Add(t.DomainPKID == Int64.Parse(fc[cond])); break;
					case "ResourceType": conds.Add(t.ResourceTypePKID == Int64.Parse(fc[cond])); break;
					//case "MediumType": conds.Add(t.MediumTypePKID == Int64.Parse(fc[cond])); break;
					//case "SchoolType": conds.Add(t.SchoolTypePKID == Int64.Parse(fc[cond])); break;
					//case "Deformity": conds.Add(t.DeformityPKID == Int64.Parse(fc[cond])); break;
					//case "LearnFrom": conds.Add(t.LearnFromPKID == Int64.Parse(fc[cond])); break;
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
			ViewBag.ResTypes = GetStrengthDict(CroResourceHelper.ResourceType.GetItems());
			ViewBag.Grades = GetStrengthDict(CroResourceHelper.Grade.GetItems());

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

         CroResource current = null;
         if (resid != null && resid.Value > 0)
            current = APBplDef.CroResourceBpl.PrimaryGet(resid.Value);

         db.BeginTrans();

         try
         {
            if (current != null)
            {
               foreach (var c in model.Courses)
               {
                  APBplDef.ExercisesBpl.ConditionDelete(et.CourseId == c.CourseId);
               }
               APBplDef.MicroCourseBpl.ConditionDelete(mc.ResourceId == resid);
               APBplDef.CroResourceBpl.PrimaryDelete(resid.Value);
               model.CreatedTime = current.CreatedTime;
               model.Creator = current.Creator;
               model.LastModifier = ResSettings.SettingsInSession.UserId;
               model.LastModifiedTime = DateTime.Now;
            }
            else
            {
               model.CreatedTime = model.LastModifiedTime = DateTime.Now;
               model.LastModifier = ResSettings.SettingsInSession.UserId;
            }

            model.StatePKID = model.StatePKID == CroResourceHelper.StateDeny ? CroResourceHelper.StateWait : model.StatePKID;
            APBplDef.CroResourceBpl.Insert(model);

            foreach (var item in model.Courses ?? new List<MicroCourse>())
            {
               item.ResourceId = model.CrosourceId;
               APBplDef.MicroCourseBpl.Insert(item);

               foreach (var exer in item.Exercises ?? new List<Exercises>())
               {
                  exer.CourseId = item.CourseId;
                  APBplDef.ExercisesBpl.Insert(exer);
               }

            }

            db.Commit();
         }
         catch (Exception e)
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

		public ActionResult Details(long id)
		{
			var model = APBplDef.CroResourceBpl.PrimaryGet(id);
			//model.GhostFileName = model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);
			return View(model);
		}


		public static object GetStrengthDict(List<ResPickListItem> items)
		{
			List<object> array = new List<object>();
			foreach (var item in items)
			{
				array.Add(new {
					key = item.StrengthenValue,
					id = item.PickListItemId,
					name = item.Name
				});
			}
			return array;
		}



		//
		//	作品 - 审核合格/不合格
		// POST:		/Resource/Approve
		//

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


		//	众筹作品
		//	GET:				/Crosource/Report

		public ActionResult Report()
		{
			var cro = APDBDef.CroResource;
			var u = APDBDef.ResUser;
			var cmp = APDBDef.ResCompany;
			var area = APDBDef.ResCompany.As("area");


			var model = APQuery.select(area.CompanyName.As("area"), cmp.CompanyName.As("cmp"), cro.CrosourceId.Count().As("cnt"), area.CompanyId)
				.from(cmp,
						area.JoinInner(cmp.ParentId == area.CompanyId),
						u.JoinLeft(cmp.CompanyId == u.CompanyId),
						cro.JoinLeft(cro.Creator == u.UserId & cro.StatePKID == ResResourceHelper.StateAllow))
				.where(area.CompanyId.NotIn(120, 118, 108, 104, 76, 71))
				.group_by(area.CompanyName, cmp.CompanyName, area.CompanyId)
				.query(db, r => 
				{
					return new CroReport()
					{
						Area = r.GetString(0),
						Company = r.GetString(1),
						Cnt =	r.GetInt32(2)
					};
				}).ToList();


			return View(model);
		}

	}

}