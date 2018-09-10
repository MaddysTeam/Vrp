using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;
using Res.Business.Utils;

namespace Res.Controllers
{

	public class RealController : BaseController
	{

		//
		//	实名 - 查询
		// GET:		/Real/Search
		// POST:		/Real/Search
		//

		public ActionResult Search()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Search(int current, int rowCount, string searchPhrase, long companyId, FormCollection fc)
		{
			//----------------------------------------------------------
			var t = APDBDef.ResReal;
			var c = APDBDef.ResCompany;
			APSqlOrderPhrase order = null;
			List<APSqlWherePhrase> where = new List<APSqlWherePhrase>();

			// 取排序
			var co = GridOrder.GetSortDef(fc);
			if (co != null)
			{
				switch (co.Id)
				{
					case "RealName": order = new APSqlOrderPhrase(t.RealName, co.Order); break;
					case "CompanyName": order = new APSqlOrderPhrase(c.CompanyName, co.Order); break;
					case "IDCard": order = new APSqlOrderPhrase(t.IDCard, co.Order); break;
					case "Birthday": order = new APSqlOrderPhrase(t.Birthday, co.Order); break;
					case "CardNo": order = new APSqlOrderPhrase(t.CardNo, co.Order); break;
					case "CardPwd": order = new APSqlOrderPhrase(t.CardPwd, co.Order); break;
					case "State": order = new APSqlOrderPhrase(t.State, co.Order); break;
				}
			}

			// 按资源标题过滤
			if (searchPhrase != null)
			{
				searchPhrase = searchPhrase.Trim();
				if (searchPhrase != "")
					where.Add((t.RealName.Match(searchPhrase) | c.CompanyName.Match(searchPhrase)));
			}

			if (companyId != 0)
			{
				where.Add(new APSqlConditionPhrase(c.Path, APSqlConditionOperator.Like, 
					APQuery.select(APSqlThroughExpr.Expr("path + '%'"))
					.from(c).where(c.CompanyId == companyId)));
			}


			int total;
			var list = APBplDef.ResRealBpl.TolerantSearch(out total, current, rowCount, where.Count == 0 ? null : new APSqlConditionAndPhrase(where), order);
			//----------------------------------------------------------

			if (Request.IsAjaxRequest())
			{
				return Json(
					new
					{
						rows = from res in list
								 select new
								 {
									 id = res.RealId,
									 res.RealName,
									 res.IDCard,
									 Birthday = res.Birthday.ToString("yyyy-MM-dd"),
									 res.CompanyName,
									 res.CardNo,
									 res.CardPwd,
									 State = res.State ? "已注册" : "未注册"
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
		//	实名 - 编辑/创建
		// GET:		/Real/Edit
		// POST:		/Real/Edit
		//

		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return Request.IsAjaxRequest() ? (ActionResult)PartialView() : View();
			}
			else
			{
				var model = APBplDef.ResRealBpl.PrimaryGet(id.Value);
				return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
			}

		}

		[HttpPost]
		public ActionResult Edit(long? id, ResReal model)
		{
			DateTime birthday;

			if (!IDCardCheck.CheckIdentityCode(model.IDCard, out birthday))
				return Json(new
				{
					error = "IDCard",
					msg = "身份证号码无效"
				});

			var t = APDBDef.ResReal;
			long idv = id == null ? 0 : id.Value;
			if (APBplDef.ResRealBpl.ConditionQueryCount(t.IDCard == model.IDCard & t.RealId != idv) > 0)
				return Json(new
				{
					error = "IDCard",
					msg = "身份证号码已被使用"
				});

			model.Birthday = birthday;

			if (id == null)
			{
				int no = (int)APQuery.select(APSqlThroughExpr.Expr("MAX(CardNo)+1"))
					.from(t)
					.executeScale(db);

				model.CardNo = no.ToString();
				model.CardPwd = new Random().Next(100000, 999999).ToString();

				APBplDef.ResRealBpl.Insert(model);
				return Json(new
				{
					error = "none",
					msg = String.Format("用户创建成功，请记录实名卡号：{0}，实名卡密码：{1}", model.CardNo, model.CardPwd)
				});
			}
			else
			{
				APBplDef.ResRealBpl.UpdatePartial(id.Value, new { model.RealName, model.Birthday, model.IDCard, model.CompanyId });
				return Json(new
				{
					error = "none",
					msg = "用户修改成功"
				});
			}
		}


		//
		//	实名 - 删除
		// POST:		/Real/Delete
		//

		[HttpPost]
		public ActionResult Delete(long id)
		{
			if (Request.IsAjaxRequest())
			{
				APBplDef.ResRealBpl.PrimaryDelete(id);
				return Json(new { cmd = "Deleted", msg = "本用户已删除。" });
			}

			return IsNotAjax();
		}

	}

}