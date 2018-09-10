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

	public class ReportController : BaseController
	{

		public ActionResult Main()
		{
			return View();
		}


		public ActionResult Summary()
		{
			var modal = new ResReportSummary();

			var u = APDBDef.ResUser;
			var r = APDBDef.ResResource;


			modal.TotalUser = (int)APQuery.select(u.Asterisk.Count()).from(u).executeScale(db);
			modal.TotalResource = (int)APQuery.select(r.Asterisk.Count()).from(r).executeScale(db);
			modal.TotalResourceSize = (long)APQuery.select(r.FileSize.Sum()).from(r).executeScale(db);
			modal.TotalView = (int)APQuery.select(APDBDef.ResView.Asterisk.Count()).from(APDBDef.ResView).executeScale(db);
			modal.TotalFavorite = (int)APQuery.select(APDBDef.ResFavorite.Asterisk.Count()).from(APDBDef.ResFavorite).executeScale(db);
			modal.TotalStar = (int)APQuery.select(APDBDef.ResStar.Asterisk.Count()).from(APDBDef.ResStar).executeScale(db);
			modal.TotalComment = (int)APQuery.select(APDBDef.ResComment.Asterisk.Count()).from(APDBDef.ResComment).executeScale(db);
			modal.TotalDownload = (int)APQuery.select(APDBDef.ResDownload.Asterisk.Count()).from(APDBDef.ResDownload).executeScale(db);
			modal.CreateThisWeek = (int)APQuery.select(r.Asterisk.Count()).from(r).where(r.CreatedTime >
				DateTime.Today.AddDays(-(((int)DateTime.Today.DayOfWeek + 7) % 8))
				).executeScale(db);
			modal.CreateThisMonth = (int)APQuery.select(r.Asterisk.Count()).from(r).where(r.CreatedTime >
				DateTime.Today.AddDays(-(DateTime.Today.Day - 1))
				).executeScale(db);
			modal.CreateThisYear = (int)APQuery.select(r.Asterisk.Count()).from(r).where(r.CreatedTime >
				DateTime.Today.AddDays(-(DateTime.Today.DayOfYear - 1))
				).executeScale(db);

			return View(modal);
		}


		public ActionResult UserState()
		{
			string sql = @"
SELECT t2.CompanyName, COUNT(t2.CompanyName)
FROM resuser AS t1 INNER JOIN
(SELECT c1.CompanyId AS companyId, c2.CompanyId AS regionId, c2.CompanyName
FROM ResCompany AS c1 INNER JOIN ResCompany AS c2 ON c1.ParentId = c2.CompanyId) AS t2 ON t1.CompanyId = t2.companyId
GROUP BY t2.CompanyName
";
			Dictionary<string, int> dict = new Dictionary<string, int>();
			using (var reader = db.CreateSqlCommand(sql).ExecuteReader())
			{
				while (reader.Read())
				{
					dict.Add(reader.GetString(0), reader.GetInt32(1));
				}
			}

			return View(dict);
		}


		public ActionResult ResourceState()
		{
			return View();
		}
		[HttpPost]
		public ActionResult ResourceState(string type)
		{
			return Json(GetState(type).ToArray());
		}


		public IEnumerable<object> GetState(string field)
		{
			string sql = @"
SELECT t1.Name, COUNT(t.{0}), SUM(t.FileSize)
FROM ResResource AS t INNER JOIN ResPickListItem AS t1 ON t.{0} = t1.PickListItemId
WHERE t.StatePKID = 10352
GROUP BY t1.Name, t1.PickListItemId
ORDER BY t1.PickListItemId
";
			using (var reader = db.CreateSqlCommand(sql, field).ExecuteReader())
			{
				while (reader.Read())
				{
					yield return new
					{
						name = reader.GetString(0),
						count = reader.GetInt32(1),
						size = reader.GetInt64(2)
					};
				}
			}
			
		}
	}

}