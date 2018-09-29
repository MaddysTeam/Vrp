using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Symber.Web.Data;
using Res.Business;
using Res.Business.Utils;

namespace Res.Controllers
{

	public class UserController : BaseController
	{

		#region [ UserManager ]

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public UserController()
		{
		}

		public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		#endregion

		//
		//	用户 - 首页
		// GET:		/User/Index
		//

		public ActionResult Index()
		{
			return View();
		}


      //
      //	用户 - 编辑
      // GET:		/User/Edit
      // POST:		/User/Edit
      //

      public ActionResult Edit(long? id)
      {
         ViewBag.Provinces = CrosourceController.GetStrengthDict(ResSettings.SettingsInSession.AllProvince());
         ViewBag.Areas = CrosourceController.GetStrengthDict(ResSettings.SettingsInSession.AllAreas());
         ViewBag.Schools = CrosourceController.GetStrengthDict(ResSettings.SettingsInSession.AllSchools());

         if (id == null)
         {
            return Request.IsAjaxRequest() ? (ActionResult)PartialView() : View();
         }
         else
         {
            var model = APBplDef.ResUserBpl.PrimaryGet(id.Value);
            return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
         }
      }

      [HttpPost]
      public async Task<ActionResult> Edit(ResUser model)
      {
         DateTime birthday;

         if (!IDCardCheck.CheckIdentityCode(model.IDCard, out birthday))
            return Json(new
            {
               error = "IDCard",
               msg = "身份证号码无效"
            });

         var t = APDBDef.ResUser;

         int g = model.IDCard[model.IDCard.Length - 2] - '0';
         model.GenderPKID = g % 2 == 0 ? ResUserHelper.GenderFemale : ResUserHelper.GenderMale;

         if (model.UserId == 0)
         {
            if (APBplDef.ResUserBpl.ConditionQueryCount(t.UserName == model.UserName) > 0)
            {
               return Json(new
               {
                  error = "Username",
                  msg = "登录名称已经被使用"
               });
            }

            var password = "teacher";
            model.RegisterTime = DateTime.Now;
            model.LastLoginTime = DateTime.Now;
            model.Password = password;
            var result=await UserManager.CreateAsync(model, password);
            if (!result.Succeeded)
            {
               return Json(new
               {
                  error = "Signin",
                  msg = string.Join(",", result.Errors)
               });
            }
           // APBplDef.ResUserBpl.Insert(model);
         }
         else
         {
            APBplDef.ResUserBpl.UpdatePartial(model.UserId, new
            {
                model.Email,
                model.RealName,
                model.PhotoPath,
                model.CompanyId,
                model.IDCard,
                model.UserTypePKID,
                model.ProvinceId,
                model.AreaId
            });
         }

         return Json(new
         {
            error = "none",
            msg = "编辑成功"
         });
      }


      //
      //	用户 - 查询
      // GET:		/User/Search
      // POST:		/User/Search
      //

      public ActionResult Search()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Search(int current, int rowCount, string searchPhrase, long companyId, FormCollection fc)
		{
         var companyPath = ResSettings.SettingsInSession.CompanyPath;

			//----------------------------------------------------------
			var t = APDBDef.ResUser;
			var c = APDBDef.ResCompany;
			APSqlOrderPhrase order = null;
			APSqlWherePhrase where = t.Removed == false;

			// 取排序
			var co = GridOrder.GetSortDef(fc);
			if (co != null)
			{
				switch (co.Id)
				{
					case "UserName": order = new APSqlOrderPhrase(t.UserName, co.Order); break;
					case "RealName": order = new APSqlOrderPhrase(t.RealName, co.Order); break;
					case "CompanyName": order = new APSqlOrderPhrase(c.CompanyName, co.Order); break;
					case "RoleName": order = new APSqlOrderPhrase(APDBDef.ResRole.RoleId, co.Order); break;
					case "Gender": order = new APSqlOrderPhrase(t.GenderPKID, co.Order); break;
					case "Email": order = new APSqlOrderPhrase(t.Email, co.Order); break;
					case "RegisterTime": order = new APSqlOrderPhrase(t.RegisterTime, co.Order); break;
					case "LoginCount": order = new APSqlOrderPhrase(t.LoginCount, co.Order); break;
					case "Actived": order = new APSqlOrderPhrase(t.Actived, co.Order); break;
				}
			}

			// 按资源标题过滤
			if (searchPhrase != null)
			{
				searchPhrase = searchPhrase.Trim();
				if (searchPhrase != "")
					where &= (t.UserName.Match(searchPhrase) | t.RealName.Match(searchPhrase));
			}

         if (companyId != 0)
         {
            where &= new APSqlConditionPhrase(c.Path, APSqlConditionOperator.Like,
               APQuery.select(APSqlThroughExpr.Expr("path + '%'"))
               .from(c).where(c.CompanyId == companyId));
         }
         else
         {
            where &= c.Path.Match(companyPath);
            //if (!string.IsNullOrEmpty(companyPath))
            //   where &= new APSqlConditionPhrase(c.Path, APSqlConditionOperator.Like,
            //   APQuery.select(APSqlThroughExpr.Expr("path + '%'"))
            //   .from(c).where(c.Path.Match(companyPath)));

         }
			int total;
			var list = APBplDef.ResUserBpl.TolerantSearch(out total, current, rowCount, where, order);
			//----------------------------------------------------------

			if (Request.IsAjaxRequest())
			{
				return Json(
					new
					{
						rows = from res in list
								 select new
								 {
									 id = res.UserId,
									 res.UserName,
									 res.RealName,
									 res.CompanyName,
									 res.UserType,
									 res.Gender,
									 res.Email,
									 RegisterTime = res.RegisterTime.ToString("yyyy-MM-dd"),
									 res.LoginCount,
									 Actived = res.Actived ? "有效" : "无效"
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
		//	用户 - 有效/无效
		// POST:		/User/Actived
		//

		[HttpPost]
		public ActionResult Actived(long id, bool value)
		{
			if (Request.IsAjaxRequest())
			{
				APBplDef.ResUserBpl.UpdatePartial(id, new { Actived = value });
				return Json(new { cmd = "Processed", value = value, msg = "用户是否有效设置完成。" });
			}

			return IsNotAjax();
		}


		//
		//	用户 - 授权
		// GET:		/User/Approve
		// POST:		/User/Approve
		//

		public ActionResult Approve(long id)
		{
			if (Request.IsAjaxRequest()){
				return PartialView(id);
			}

			return IsNotAjax();
		}

		[HttpPost]
		public ActionResult Approve(long id, long roleId)
		{
			var t = APDBDef.ResUserRole;

			if (Request.IsAjaxRequest())
			{
				var item = APBplDef.ResUserRoleBpl.ConditionQuery(t.UserId == id, null).FirstOrDefault();
				if (item == null)
				{
					new ResUserRole() { UserId = id, RoleId = roleId }.Insert();
				}
				else if (item.RoleId != roleId)
				{
					item.RoleId = roleId;
					item.Update();
				}

				return Json(new
				{
					error = "none",
					msg = "权限设置成功"
				});
			}

			return IsNotAjax();
		}


		//
		//	用户 - 详情
		// GET:		/User/Info
		//

		public ActionResult Info(long? id)
		{
			if (id == null)
				id = ResSettings.SettingsInSession.UserId;

			var model = APBplDef.ResUserBpl.PrimaryGet(id.Value);
			var company = APBplDef.ResCompanyBpl.PrimaryGet(model.CompanyId);
			if (company != null)
				model.CompanyName = company.CompanyName;

			return View(model);
		}


		[HttpPost]
		public async Task<ActionResult> ResetPwd(long? id)
		{
			if (id == null)
				id = ResSettings.SettingsInSession.UserId;

			var user = APBplDef.ResUserBpl.PrimaryGet(id.Value);

			var Token = await UserManager.GeneratePasswordResetTokenAsync(id.Value);
			var result = await UserManager.ResetPasswordAsync(id.Value, Token, "111111");
		//	var result = await UserManager.ChangePasswordAsync(id.Value, user.Password, "111111");
			APBplDef.ResUserBpl.UpdatePartial(id.Value, new { Password = "111111" });

			if (result.Succeeded)
			{
				return Json("用户密码已经被重置为6个1");
			}
			else
			{
				return Json("error");
			}
		}
	}

}