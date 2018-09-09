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

	public class ChartController : BaseController
	{

		//
		// 周统计数据
		// GET:		/Chart/WeekState
		//

		public ActionResult WeekState(string type)
		{
			return Json(GetWeekState(type), JsonRequestBehavior.AllowGet);
		}

		//
		// 领域分布
		// GET:		/Chart/DomainState
		//

		public ActionResult DomainState()
		{
			return Json(GetDomainState(), JsonRequestBehavior.AllowGet);
		}

		//
		// 媒体类型分布
		// GET:		/Chart/MediumState
		//

		public ActionResult MediumState()
		{
			return Json(GetMediumState(), JsonRequestBehavior.AllowGet);
		}



		#region [ WeekState ]

		public object GetWeekState(string table)
		{
			var sqlWeekDay = @"
SELECT DATEADD( dd, DATEDIFF( dd, 0, occurtime ), 0 ), COUNT(DATEADD( dd, DATEDIFF( dd, 0, occurtime ), 0 ))
FROM {0}
WHERE DATEADD( dd, DATEDIFF( dd, 0, occurtime ), 0 ) > DATEADD( dd, DATEDIFF( dd, 0, getdate() ), -7 )
GROUP BY DATEADD( dd, DATEDIFF( dd, 0, occurtime ), 0 )
ORDER BY DATEADD( dd, DATEDIFF( dd, 0, occurtime ), 0 ) DESC
";
			var sqlLastWeek = @"
SELECT COUNT(occurtime)
FROM {0}
WHERE occurtime BETWEEN DATEADD( dd, DATEDIFF( dd, 0, getdate() ), -14 )
AND DATEADD( dd, DATEDIFF( dd, 0, getdate() ), -7 )
";
			int[] state = new int[7];
			int total = 0, lastTotal;

			using (var reader = db.CreateSqlCommand(sqlWeekDay, table).ExecuteReader())
			{
				while (reader.Read())
				{
					DateTime dt = reader.GetDateTime(0);
					int count = reader.GetInt32(1);

					state[6 - (DateTime.Today - dt).Days] = count;
					total += count;
				}
			}
			lastTotal = (int)db.CreateSqlCommand(sqlLastWeek, table).ExecuteScalar();


			return new
			{
				total,
				lastTotal,
				state
			};
		}

		public object GetDomainState()
		{
			var t = APDBDef.ResResource;
			return
				APQuery.select(t.DomainPKID, t.DomainPKID.Count())
				.from(t)
				.group_by(t.DomainPKID)
				.query(db, reader =>
				{
					return new
					{
						label = ResResourceHelper.Domain.GetName(reader.GetInt64(0)),
						data = reader.GetInt32(1)
					};
				}).ToArray();
		}

		public object GetMediumState()
		{
			var t = APDBDef.ResResource;
			return
				APQuery.select(t.MediumTypePKID, t.MediumTypePKID.Count())
				.from(t)
				.group_by(t.MediumTypePKID)
				.query(db, reader =>
				{
					return new
					{
						label = ResResourceHelper.MediumType.GetName(reader.GetInt64(0)),
						data = reader.GetInt32(1)
					};
				}).ToArray();
		}

		#endregion

	}

}