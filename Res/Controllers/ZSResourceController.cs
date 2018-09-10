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

	public class ZSResourceController : BaseController
	{

		//	GET:			/ZSResource/Index

		public ActionResult Index()
		{
			return View();
		}


		//	GET:					/ZSResource/Search
		// POST-AJAX			/ZSResource/Search

		public ActionResult Search()
		{
			if (!String.IsNullOrEmpty(Request["r"]))
				ViewData["TowLevel"] = Request["r"];

			return View();
		}

		[HttpPost]
		public ActionResult Search(int current, int rowCount, string searchPhrase, FormCollection fc)
		{
			ThrowNotAjax();


			var t = APDBDef.ZSResource;
			var u = APDBDef.ResUser;
			

			var query = APQuery.select(t.ResourceId, t.Title, u.RealName, t.CreatedTime)
				.from(t, u.JoinInner(t.Creator == u.UserId))
				.primary(t.ResourceId)
				.skip((current - 1) * rowCount)
				.take(rowCount);


			// 取过滤条件

			foreach (string cond in fc.Keys)
			{
				switch (cond)
				{
					case "TowLevel": query.where_and(t.TowLevel == fc[cond]); break;
				}
			}


			// 按关键字过滤

			if (searchPhrase != null)
			{
				searchPhrase = searchPhrase.Trim();
				if (searchPhrase != "")
					query.where_and(t.Title.Match(searchPhrase));
			}


			// 取排序

			var co = GridOrder.GetSortDef(fc);
			if (co != null)
			{
				switch (co.Id)
				{
					case "title": query.order_by(new APSqlOrderPhrase(t.Title, co.Order)); break;
					case "creator": query.order_by(new APSqlOrderPhrase(u.RealName, co.Order)); break;
					case "createTime": query.order_by(new APSqlOrderPhrase(t.CreatedTime, co.Order)); break;
				}
			}


			//	获得查询的总数量

			var total = db.ExecuteSizeOfSelect(query);


			//	查询结果集

			var result = query.query(db, r =>
			{
				var recTime = r.GetValue(r.GetOrdinal(t.CreatedTime.ColumnName));
				DateTime? retRecTime = recTime == DBNull.Value ? null : (DateTime?)recTime;

				return new
				{
					id = t.ResourceId.GetValue(r),
					title = t.Title.GetValue(r),
					creator = u.RealName.GetValue(r),
					createTime = t.CreatedTime.GetValue(r).ToString("yyyy-MM-dd")
				};
			});


			return Json(new
			{
				rows = result,
				current,
				rowCount,
				total
			});
		}


		//	GET:		/ZSResource/Details

		public ActionResult Details(long id)
		{
			var model = db.ZSResourceDal.PrimaryGet(id);

			return View(model);
		}


		//	GET:				/ZSResource/Edit
		// POST-AJAX		/ZSResource/Edit

		public ActionResult Edit(long id)
		{
			ViewBag.ResTypes = GetStrengthDict(ResResourceHelper.ResourceType.GetItems());
			ViewBag.Grades = GetStrengthDict(ResResourceHelper.Grade.GetItems());

			var model = db.ZSResourceDal.PrimaryGet(id);
			model.GhostFileName = model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);

			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(long id, ZSResource model)
		{


			if (model.IsLink)
			{
				model.FileSize = 0;
				model.ResourcePath = model.GhostFileName;
				model.FileExtName = GetSafeExt(model.ResourcePath);
			}

			APBplDef.ZSResourceBpl.UpdatePartial(id, new
			{
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
				LastModifier = ResSettings.SettingsInSession.UserId,
				LastModifiedTime = DateTime.Now
			});

			return RedirectToAction("Details", new { id = model.ResourceId });
		}


		private string GetSafeExt(string path)
		{
			int idx = path.IndexOf('?');
			if (idx != -1)
				path = path.Substring(0, idx);
			return Path.GetExtension(path);
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

	}

}