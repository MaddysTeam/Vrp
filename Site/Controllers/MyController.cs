using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Res;
using Symber.Web.Data;
using Res.Business;
using System.IO;
using System.Threading.Tasks;


namespace Res.Controllers
{
	[Authorize]
	public class MyController : BaseController
	{
		#region [ UserManager ]

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public MyController()
		{
		}

		public MyController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
		// 我的信息
		// GET:		/My/Index
		//

		public ActionResult Index(long id)
		{
			var tc = APDBDef.ResCompany;
			var userid = id;
	
			var user = db.ResUserDal.PrimaryGet(userid);
			user.FavoriteCount = db.ResFavoriteDal.ConditionQueryCount(APDBDef.ResFavorite.UserId == userid);
			user.DownCount = db.ResDownloadDal.ConditionQueryCount(APDBDef.ResDownload.UserId == userid);
			user.CommentCount = db.ResCommentDal.ConditionQueryCount(APDBDef.ResComment.UserId == userid);
			user.CompanyName = (string)APQuery.select(tc.CompanyName)
				.from(tc).where(tc.CompanyId == user.CompanyId).executeScale(db);
			user.UserId = userid;
			return View(user);
		}

		public ActionResult Resource(int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyResource(out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;

			return View();
		}

		public ActionResult Favorite(long id,int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyFavorite(id,out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}



		public ActionResult Download(long id, int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyDownload(id,out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}


		public ActionResult Comment(long id, int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyComment(id,out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}

		public ActionResult DeleteFavorite(long id)
		{
			APBplDef.ResFavoriteBpl.PrimaryDelete(id);

			return RedirectToAction("Favorite");
		}





		public ActionResult DeleteDownload(long id)
		{
			APBplDef.ResDownloadBpl.PrimaryDelete(id);

			return RedirectToAction("Download");
		}



		public ActionResult DeleteComment(long id)
		{
			APBplDef.ResCommentBpl.PrimaryDelete(id);

			return RedirectToAction("Comment");
		}



		//
		// 修改个人信息
		// GET:		/My/Edit
		// POST:		/My/Edit
		//

		public ActionResult Edit()
		{
			var model = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);

			return Request.IsAjaxRequest() ? (ActionResult)PartialView(model) : View(model);
		}

		[HttpPost]
		public ActionResult Edit(ResUser model)
		{
			APBplDef.ResUserBpl.UpdatePartial(ResSettings.SettingsInSession.UserId, new { model.RealName, model.Email });

			return Json("success");
		}


		public ActionResult ChgPwd()
		{
			return Request.IsAjaxRequest() ? (ActionResult)PartialView() : View();
		}

		[HttpPost]
		public async Task<ActionResult> ChgPwd(ChgPwd model)
		{
			var user = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);
			var result = await UserManager.ChangePasswordAsync(user.UserId, user.Password, model.Password);

			return Json("success");
		}




		//上传资源


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



		public ActionResult Upload(long? id)
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

				return RedirectToAction("Resource");
			}
		}



		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Upload(long? id, ResResource model, FormCollection fc)
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

				model.StatePKID = ResResourceHelper.StateWait;	// 是否提供管理员自动审核功能


				APBplDef.ResResourceBpl.Insert(model);
			}

			return RedirectToAction("Resource");
		}






		private string GetSafeExt(string path)
		{
			int idx = path.IndexOf('?');
			if (idx != -1)
				path = path.Substring(0, idx);
			return Path.GetExtension(path);
		}

	}

}