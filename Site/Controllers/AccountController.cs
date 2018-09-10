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

namespace Res.Controllers
{
	public class AccountController : BaseController
	{

		#region [ UserManager ]

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
		//	用户登录
		// GET:		/Account/Login
		// POST:		/Account/Login

		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// 这不会计入到为执行帐户锁定而统计的登录失败次数中
			// 若要在多次输入错误密码的情况下触发帐户锁定，请更改为 shouldLockout: true
			var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
			switch (result)
			{
				case SignInStatus.Success:
					APBplDef.ResUserBpl.SetLastLoginTime(model.UserName);
					return RedirectToLocal(returnUrl);
				case SignInStatus.LockedOut:
					return View("Lockout");
				case SignInStatus.RequiresVerification:
					return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				case SignInStatus.Failure:
				default:
					ModelState.AddModelError("", "用户名或密码不正确。");
					return View(model);
			}
		}


		//
		// 用户登出
		// GET:		/Account/LogOff
		//

		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			ResSettings.SettingsInSession.ResetCurrent();
			return RedirectToAction("Index", "Home");
		}

		public ActionResult LogOff2()
		{
			AuthenticationManager.SignOut();
			ResSettings.SettingsInSession.ResetCurrent();
			return RedirectToAction("Index", "CroHome");
		}


      [HttpPost]
      [AllowAnonymous]
      [ValidateAntiForgeryToken]
      public async Task<ActionResult> Register(Register model)
      {
         if (!ModelState.IsValid)
         {
            return View(model);
         }

         model.Username = model.Username.Trim();
         model.Password = model.Password.Trim();
         model.ConfirmPassword = model.ConfirmPassword.Trim();
         model.Email = model.Email.Trim();
         model.Question = model.Question.Trim();
         model.Answer = model.Answer.Trim();

         var t = APDBDef.ResUser;
         if (APBplDef.ResUserBpl.ConditionQueryCount(t.UserName == model.Username) > 0)
         {
            ModelState.AddModelError("Username", "登录名称已经被使用");
            return View(model);
         }

         var real = APBplDef.ResRealBpl.PrimaryGet(model.RealId);
         int g = real.IDCard[real.IDCard.Length - 2] - '0';

         var user = new ResUser
         {
            UserName = model.Username,
            Email = model.Email,
            Password = model.Password,
            Question = model.Question,
            Answer = model.Answer,
            RealName = real.RealName,
            PhotoPath = "",
            GenderPKID = g % 2 == 0 ? ResUserHelper.GenderFemale : ResUserHelper.GenderMale,
            CompanyId = real.CompanyId,
            IDCard = real.IDCard,
            Actived = true,
            Removed = false,
            RegisterTime = DateTime.Now,
            LastLoginTime = DateTime.Now,
            LoginCount = 1
         };
         var result = await UserManager.CreateAsync(user, model.Password);
         if (result.Succeeded)
         {
            APBplDef.ResRealBpl.UpdatePartial(model.RealId, new { state = true });
            APBplDef.ResUserRoleBpl.Insert(new ResUserRole() { UserId = user.UserId, RoleId = 2 });
            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

            return RedirectToAction("Index", "CroHome");
         }

         return View(model);
      }


		//
		// 修改密码
		// GET:		/Account/ChgPwd
		// POST:		/Account/ChgPwd
		//

		public ActionResult ChgPwd()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ChgPwd(LocalPasswordModel model)
		{
			var result = await UserManager.ChangePasswordAsync(ResSettings.SettingsInSession.UserId, model.OldPassword, model.NewPassword);

			if (result.Succeeded)
			{
				return RedirectToAction("Info", "User", new { id = ResSettings.SettingsInSession.UserId });
			}
			else
			{
				AddErrors(result);
				return View();
			}
		}


		#region [ Dirty ]

		////
		//// GET: /Account/Register
		//[AllowAnonymous]
		//public ActionResult Register()
		//{
		//	return View();
		//}

		////
		//// POST: /Account/Register
		//[HttpPost]
		//[AllowAnonymous]
		//[ValidateAntiForgeryToken]
		//public async Task<ActionResult> Register(RegisterViewModel model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		var user = new ResUser {
		//			UserName = model.UserName,
		//			RealName = model.UserName,
		//			RegisterTime = DateTime.Now,
		//			LastLoginTime = DateTime.Now
		//		};
		//		var result = await UserManager.CreateAsync(user, model.Password);
		//		if (result.Succeeded)
		//		{
		//			await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

		//			return RedirectToAction("Index", "Home");
		//		}
		//		AddErrors(result);
		//	}

		//	// 如果我们进行到这一步时某个地方出错，则重新显示表单
		//	return View(model);
		//}


		//		  //
		//		  // GET: /Account/ResetPassword
		//		  [AllowAnonymous]
		//		  public ActionResult ResetPassword(string code)
		//		  {
		//				return code == null ? View("Error") : View();
		//		  }

		//		  //
		//		  // POST: /Account/ResetPassword
		//		  [HttpPost]
		//		  [AllowAnonymous]
		//		  [ValidateAntiForgeryToken]
		//		  public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		//		  {
		//				if (!ModelState.IsValid)
		//				{
		//					 return View(model);
		//				}
		//				var user = await UserManager.FindByNameAsync(model.Email);
		//				if (user == null)
		//				{
		//					 // 请不要显示该用户不存在
		//					 return RedirectToAction("ResetPasswordConfirmation", "Account");
		//				}
		//				var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
		//				if (result.Succeeded)
		//				{
		//					 return RedirectToAction("ResetPasswordConfirmation", "Account");
		//				}
		//				AddErrors(result);
		//				return View();
		//		  }

		//		  //
		//		  // GET: /Account/ResetPasswordConfirmation
		//		  [AllowAnonymous]
		//		  public ActionResult ResetPasswordConfirmation()
		//		  {
		//				return View();
		//		  }

		//		  //
		//		  // POST: /Account/ExternalLogin
		//		  [HttpPost]
		//		  [AllowAnonymous]
		//		  [ValidateAntiForgeryToken]
		//		  public ActionResult ExternalLogin(string provider, string returnUrl)
		//		  {
		//				// 请求重定向到外部登录提供程序
		//				return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		//		  }

		//		  //
		//		  // GET: /Account/SendCode
		//		  [AllowAnonymous]
		//		  public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
		//		  {
		//				var userId = await SignInManager.GetVerifiedUserIdAsync();
		//				if (userId == null)
		//				{
		//					 return View("Error");
		//				}
		//				var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
		//				var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
		//				return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
		//		  }

		//		  //
		//		  // POST: /Account/SendCode
		//		  [HttpPost]
		//		  [AllowAnonymous]
		//		  [ValidateAntiForgeryToken]
		//		  public async Task<ActionResult> SendCode(SendCodeViewModel model)
		//		  {
		//				if (!ModelState.IsValid)
		//				{
		//					 return View();
		//				}

		//				// 生成令牌并发送该令牌
		//				if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
		//				{
		//					 return View("Error");
		//				}
		//				return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
		//		  }

		//		  //
		//		  // GET: /Account/ExternalLoginCallback
		//		  [AllowAnonymous]
		//		  public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		//		  {
		//				var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
		//				if (loginInfo == null)
		//				{
		//					 return RedirectToAction("Login");
		//				}

		//				// 如果用户已具有登录名，则使用此外部登录提供程序将该用户登录
		//				var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
		//				switch (result)
		//				{
		//					 case SignInStatus.Success:
		//						  return RedirectToLocal(returnUrl);
		//					 case SignInStatus.LockedOut:
		//						  return View("Lockout");
		//					 case SignInStatus.RequiresVerification:
		//						  return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
		//					 case SignInStatus.Failure:
		//					 default:
		//						  // 如果用户没有帐户，则提示该用户创建帐户
		//						  ViewBag.ReturnUrl = returnUrl;
		//						  ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
		//						  return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
		//				}
		//		  }

		//		  //
		//		  // POST: /Account/ExternalLoginConfirmation
		//		  [HttpPost]
		//		  [AllowAnonymous]
		//		  [ValidateAntiForgeryToken]
		//		  public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		//		  {
		//				if (User.Identity.IsAuthenticated)
		//				{
		//					 return RedirectToAction("Index", "Manage");
		//				}

		//				if (ModelState.IsValid)
		//				{
		//					 // 从外部登录提供程序获取有关用户的信息
		//					 var info = await AuthenticationManager.GetExternalLoginInfoAsync();
		//					 if (info == null)
		//					 {
		//						  return View("ExternalLoginFailure");
		//					 }
		//					 var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
		//					 var result = await UserManager.CreateAsync(user);
		//					 if (result.Succeeded)
		//					 {
		//						  result = await UserManager.AddLoginAsync(user.Id, info.Login);
		//						  if (result.Succeeded)
		//						  {
		//								await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
		//								return RedirectToLocal(returnUrl);
		//						  }
		//					 }
		//					 AddErrors(result);
		//				}

		//				ViewBag.ReturnUrl = returnUrl;
		//				return View(model);
		//		  }

		//		  //
		//		  // POST: /Account/LogOff
		//		  [HttpPost]
		//		  [ValidateAntiForgeryToken]
		//		  public ActionResult LogOff()
		//		  {
		//				AuthenticationManager.SignOut();
		//				return RedirectToAction("Index", "Home");
		//		  }

		//		  //
		//		  // GET: /Account/ExternalLoginFailure
		//		  [AllowAnonymous]
		//		  public ActionResult ExternalLoginFailure()
		//		  {
		//				return View();
		//		  }

		//		  protected override void Dispose(bool disposing)
		//		  {
		//				if (disposing)
		//				{
		//					 if (_userManager != null)
		//					 {
		//						  _userManager.Dispose();
		//						  _userManager = null;
		//					 }

		//					 if (_signInManager != null)
		//					 {
		//						  _signInManager.Dispose();
		//						  _signInManager = null;
		//					 }
		//				}

		//				base.Dispose(disposing);
		//		  }

		#endregion

		#region 帮助程序
		// 用于在添加外部登录名时提供 XSRF 保护
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}