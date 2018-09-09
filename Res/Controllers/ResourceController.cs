using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Res.Controllers
{

	public class ResourceController : BaseController
	{

		//
		//	资源 - 首页
		// GET:		/Resource/Index
		//

		public ActionResult Index()
		{
			return View();
		}


		//
		//	资源 - 查询
		// GET:		/Resource/Search
		// POST:		/Resource/Search
		//

		public ActionResult Search()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Search(int current, int rowCount, string searchPhrase, FormCollection fc)
		{
			//----------------------------------------------------------
			var t = APDBDef.ResResource;
			var u = APDBDef.ResUser;
			APSqlOrderPhrase order = null;
			APSqlWherePhrase where = t.StatePKID != ResResourceHelper.StateDelete;

			// 取排序
			var co = GridOrder.GetSortDef(fc);
			if (co != null)
			{
				switch (co.Id)
				{
					case "Title": order = new APSqlOrderPhrase(t.Title, co.Order); break;
					case "Author": order = new APSqlOrderPhrase(u.RealName, co.Order); break;
					case "MediumType": order = new APSqlOrderPhrase(t.MediumTypePKID, co.Order); break;
					case "CreatedTime": order = new APSqlOrderPhrase(t.CreatedTime, co.Order); break;

					case "ViewCount": order = new APSqlOrderPhrase(t.ViewCount, co.Order); break;
					case "DownCount": order = new APSqlOrderPhrase(t.DownCount, co.Order); break;
					case "FavoriteCount": order = new APSqlOrderPhrase(t.FavoriteCount, co.Order); break;
					case "CommentCount": order = new APSqlOrderPhrase(t.CommentCount, co.Order); break;
					case "StarTotal": order = new APSqlOrderPhrase(t.StarTotal, co.Order); break;

					case "State": order = new APSqlOrderPhrase(t.StatePKID, co.Order); break;
					case "cmd_elite": order = new APSqlOrderPhrase(t.EliteScore, co.Order); break;
				}
			}

			// 按资源标题过滤
			if (searchPhrase != null)
			{
				searchPhrase = searchPhrase.Trim();
				if (searchPhrase != "")
					where &= t.Title.Match(searchPhrase);
			}


			int total;
			var list = APBplDef.ResResourceBpl.TolerantSearch(out total, current, rowCount, where, order);
			//----------------------------------------------------------
	
			if (Request.IsAjaxRequest())
			{
				return Json(
					new
					{
						rows = from res in list
								 select new
								 {
									 id = res.ResourceId,
									 res.Title,
									 res.Author,
									 res.MediumType,
									 CreatedTime = res.CreatedTime.ToString("yyyy-MM-dd"),
									 res.State,
									 res.StatePKID,
									 res.EliteScore,
									 res.ViewCount,
									 res.DownCount,
									 res.FavoriteCount,
									 res.CommentCount,
									 res.StarTotal
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
		//	资源 - 分类查询
		// GET:		/Resource/Category
		// POST:		/Resource/Category
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
			var t = APDBDef.ResResource;
			var u = APDBDef.ResUser;
			APSqlOrderPhrase order = null;
			List<APSqlWherePhrase> conds = new List<APSqlWherePhrase>(){
				t.StatePKID != ResResourceHelper.StateDelete
			};

			// 取排序
			var co = GridOrder.GetSortDef(fc);
			if (co != null)
			{
				switch (co.Id)
				{
					case "Title": order = new APSqlOrderPhrase(t.Title, co.Order); break;
					case "Author": order = new APSqlOrderPhrase(u.RealName, co.Order); break;
					case "MediumType": order = new APSqlOrderPhrase(t.MediumTypePKID, co.Order); break;
					case "CreatedTime": order = new APSqlOrderPhrase(t.CreatedTime, co.Order); break;

					case "ViewCount": order = new APSqlOrderPhrase(t.ViewCount, co.Order); break;
					case "DownCount": order = new APSqlOrderPhrase(t.DownCount, co.Order); break;
					case "FavoriteCount": order = new APSqlOrderPhrase(t.FavoriteCount, co.Order); break;
					case "CommentCount": order = new APSqlOrderPhrase(t.CommentCount, co.Order); break;
					case "StarTotal": order = new APSqlOrderPhrase(t.StarTotal, co.Order); break;

					case "State": order = new APSqlOrderPhrase(t.StatePKID, co.Order); break;
					case "cmd_elite": order = new APSqlOrderPhrase(t.EliteScore, co.Order); break;
				}
			}

			// 取过滤条件
			foreach (string cond in fc.Keys)
			{
				switch (cond)
				{
					case "Domain": conds.Add(t.DomainPKID == Int64.Parse(fc[cond])); break;
					case "ResourceType": conds.Add(t.ResourceTypePKID == Int64.Parse(fc[cond])); break;
					case "MediumType": conds.Add(t.MediumTypePKID == Int64.Parse(fc[cond])); break;
					case "SchoolType": conds.Add(t.SchoolTypePKID == Int64.Parse(fc[cond])); break;
					case "Deformity": conds.Add(t.DeformityPKID == Int64.Parse(fc[cond])); break;
					case "LearnFrom": conds.Add(t.LearnFromPKID == Int64.Parse(fc[cond])); break;
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
			var list = APBplDef.ResResourceBpl.TolerantSearch(out total, current, rowCount, new APSqlConditionAndPhrase(conds), order);
			//----------------------------------------------------------

			if (Request.IsAjaxRequest())
			{
				return Json(
					new
					{
						rows = from res in list
								 select new
								 {
									 id = res.ResourceId,
									 res.Title,
									 res.Author,
									 res.MediumType,
									 CreatedTime = res.CreatedTime.ToString("yyyy-MM-dd"),
									 res.State,
									 res.StatePKID,
									 res.EliteScore,
									 res.ViewCount,
									 res.DownCount,
									 res.FavoriteCount,
									 res.CommentCount,
									 res.StarTotal
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
		//	资源 - 删除
		// POST:		/Resource/Delete
		//

		[HttpPost]
		public ActionResult Delete(long id)
		{
			if (Request.IsAjaxRequest())
			{
				APBplDef.ResResourceBpl.UpdatePartial(id, new { StatePKID = ResResourceHelper.StateDelete });
				return Json(new { cmd = "Deleted", msg = "本资源已删除。" });
			}

			return IsNotAjax();
		}


		//
		//	资源 - 加精/取消
		// POST:		/Resource/Elite
		//

		[HttpPost]
		public ActionResult Elite(long id, bool value)
		{
			if (Request.IsAjaxRequest())
			{
				APBplDef.ResResourceBpl.UpdatePartial(id, new { EliteScore = value ? (int)(DateTime.Now - new DateTime(2000, 1, 1)).TotalMinutes : 0 });
				return Json(new { cmd = "Processed", value = value, msg = "本资源加精设置完成。" });
			}

			return IsNotAjax();
		}


		//
		//	资源 - 审核合格/不合格
		// POST:		/Resource/Approve
		//

		[HttpPost]
		public ActionResult Approve(long id, bool value, string opinion)
		{
			if (Request.IsAjaxRequest())
			{
				APBplDef.ResResourceBpl.UpdatePartial(id, new
				{
					StatePKID = value ? ResResourceHelper.StateAllow : ResResourceHelper.StateDeny,
					Auditor = ResSettings.SettingsInSession.UserId,
					AuditedTime = DateTime.Now,
					AuditOpinion = opinion
				});
				return Json(new { cmd = "Processed", value = value, msg = "本资源审核完成。" });
			}

			return IsNotAjax();
		}


		//
		//	资源 - 编辑/创建
		// GET:		/Resource/Edit
		// POST:		/Resource/Edit
		//

		public ActionResult Edit(long? id)
		{
			ViewBag.ResTypes = GetStrengthDict(ResResourceHelper.ResourceType.GetItems());
			ViewBag.Grades = GetStrengthDict(ResResourceHelper.Grade.GetItems());
			if (id == null)
			{
				return View();
			}
			else
			{
				var model = APBplDef.ResResourceBpl.PrimaryGet(id.Value);
				
				model.GhostFileName = model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);
				return View(model);
			}
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(long? id, ResResource model, FormCollection fc)
		{
			if (model.IsLink)
			{
				model.FileSize = 0;
				model.ResourcePath = model.GhostFileName;
				model.FileExtName = GetSafeExt(model.ResourcePath);
			}
			model.MediumTypePKID = ResResourceHelper.GetMediumType(model.FileExtName);
			if (id == null)
			{
				model.CreatedTime = model.LastModifiedTime = DateTime.Now;
				model.Creator = model.LastModifier = ResSettings.SettingsInSession.UserId;
				model.ImportSourcePKID = ResResourceHelper.SourceUpload;

				model.StatePKID = ResResourceHelper.StateWait;  // 是否提供管理员自动审核功能
				APBplDef.ResResourceBpl.Insert(model);
				model.Code = ResResourceHelper.ResourceCode(model.DomainPKID, model.MediumTypePKID, model.ResourceId);
				APBplDef.ResResourceBpl.UpdatePartial(model.ResourceId, new
				{
					model.Code
				});
			}
			else
			{
				model.Code = ResResourceHelper.ResourceCode(model.DomainPKID, model.MediumTypePKID, model.ResourceId);
				APBplDef.ResResourceBpl.UpdatePartial(id.Value, new {
					model.Title,
					model.Author,
					model.Keywords,
					model.Description,
					model.CoverPath,
					model.ResourcePath,
					model.FileExtName,
					model.FileSize,
					model.IsLink,
					model.AuthorCompany,
					model.AuthorAddress,
					model.AuthorEmail,
					model.AuthorPhone,

					model.DeformityPKID,
					model.DomainPKID,
					model.LearnFromPKID,
					model.SchoolTypePKID,
					model.StagePKID,
					model.GradePKID,
					model.MediumTypePKID,
					model.ResourceTypePKID,
					model.SubjectPKID,
					model.Code,
					LastModifier = ResSettings.SettingsInSession.UserId,
					LastModifiedTime = DateTime.Now
				});
			}

			return RedirectToAction("Details", new { id = model.ResourceId });
		}


		private string GetSafeExt(string path)
		{
			int idx = path.IndexOf('?');
			if (idx != -1)
				path = path.Substring(0, idx);
			return Path.GetExtension(path);
		}


		//
		//	资源 - 详情
		// GET:		/Resource/Details
		//

		public ActionResult Details(long id)
		{
			var model = APBplDef.ResResourceBpl.PrimaryGet(id);
			model.GhostFileName = model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);
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

	}

}