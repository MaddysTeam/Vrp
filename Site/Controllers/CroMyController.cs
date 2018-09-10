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
using System.Text.RegularExpressions;


namespace Res.Controllers
{
	[Authorize]
	public class CroMyController : CroBaseController
	{
		#region [ UserManager ]

		private ApplicationSignInManager _signInManager;
		private ApplicationUserManager _userManager;

		public CroMyController()
		{
		}

		public CroMyController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
			user.FavoriteCount = db.CroFavoriteDal.ConditionQueryCount(APDBDef.CroFavorite.UserId == userid);
			user.DownCount = db.CroDownloadDal.ConditionQueryCount(APDBDef.CroDownload.UserId == userid);
			user.CommentCount = db.CroCommentDal.ConditionQueryCount(APDBDef.CroComment.UserId == userid);
			user.CompanyName = (string)APQuery.select(tc.CompanyName)
				.from(tc).where(tc.CompanyId == user.CompanyId).executeScale(db);

			return View(user);
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

			return RedirectToAction("Index");
		}







		//众筹平台 公告
		public ActionResult More(long id,string type, int page = 1)
		{
			var t = APDBDef.CroBulletin;
			int total;
			ViewBag.RankingBulletin = HomeCroBulltinList(t.CreatedTime.Desc, out total, 10, (page - 1) * 10);
			ViewBag.Title = "公告列表";
			ViewBag.ParamType = type;
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
	
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



		public ActionResult Upload(long id,long? resid)
		{
			ViewBag.ResTypes = GetStrengthDict(CroResourceHelper.ResourceType.GetItems());
			ViewBag.Grades = GetStrengthDict(CroResourceHelper.Grade.GetItems());
			ResUser user = new ResUser();
			user.UserId = id;
			if (resid == null)
			{
			   return View();			 
			}
			else
			{
				var model = APBplDef.CroResourceBpl.PrimaryGet(resid.Value);
           // model.GhostFileName = model.ResourcePath;// model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);

				return View(model);
			}
		}



		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Upload(long id, long? resid, CroResource model, FormCollection fc)
		{
			//if (model.IsLink)
			//{
			//	model.FileSize = 0;
			//	model.ResourcePath = model.GhostFileName;
			//	model.FileExtName = GetSafeExt(model.ResourcePath);
			//}
			//model.MediumTypePKID = CroResourceHelper.GetMediumType(model.FileExtName);
			ResUser user = new ResUser();
			user.UserId = id;
			if (resid == null)
			{
				model.CreatedTime = model.LastModifiedTime = DateTime.Now;
				model.Creator = model.LastModifier = ResSettings.SettingsInSession.UserId;
			//	model.ImportSourcePKID = CroResourceHelper.SourceUpload;

				model.StatePKID = CroResourceHelper.StateWait;	// 是否提供管理员自动审核功能
	

				APBplDef.CroResourceBpl.Insert(model);
			}else
		
			{
				var CroResourceModel = APBplDef.CroResourceBpl.PrimaryGet(resid.Value);

				if (CroResourceModel.StatePKID == CroResourceHelper.StateDeny)
				{
					model.StatePKID = CroResourceHelper.StateWait;
				}
				else
				{
					model.StatePKID = CroResourceModel.StatePKID;
				}

				APBplDef.CroResourceBpl.UpdatePartial(resid.Value, new
				{
					model.Title,
					model.Author,
					model.Keywords,
					model.Description,
					//model.CoverPath,
					//model.ResourcePath,
					//model.FileExtName,
					//model.FileSize,
					//model.IsLink,
					model.AuthorCompany,
					model.AuthorAddress,
					model.AuthorEmail,
					model.AuthorPhone,

					//model.DeformityPKID,
					//model.DomainPKID,
					//model.LearnFromPKID,
					//model.SchoolTypePKID,
					model.StagePKID,
					model.GradePKID,
					//model.MediumTypePKID,
					model.ResourceTypePKID,
					model.SubjectPKID,
					//model.RType,
					//model.RSource,
					LastModifier = ResSettings.SettingsInSession.UserId, 
					LastModifiedTime = DateTime.Now,
					model.StatePKID
					
				});
			}

			return RedirectToAction("CroMyResource", new { id = id });
		}






		private string GetSafeExt(string path)
		{
			int idx = path.IndexOf('?');
			if (idx != -1)
				path = path.Substring(0, idx);
			string ext = Path.GetExtension(path);
			if (ext.Length >= 20)
				ext = "";
			return ext;
		}






		//我的资源


		public ActionResult CroMyResource(long id, int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyCroResource(id, out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}

		//我的收藏资源
		public ActionResult CroMyFavorite(long id, int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyCroFavorite(id, out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user); ;
		}
		//我的评价资源
		public ActionResult CroMyComment(long id, int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyCroComment(id, out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;

			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}


		//我的下载资源
		public ActionResult CroMyDownload(long id, int page = 1)
		{
			int total = 0;
			ViewBag.ListofResource = MyCroDownload(id, out total, 10, (page - 1) * 10);

			// 分页器
			ViewBag.PageSize = 10;
			ViewBag.PageNumber = page;
			ViewBag.TotalItemCount = total;

			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}



		//我的推荐资源
		//public ActionResult CroMyRecommand(long id, int page = 1)
		//{
		//	int total = 0;
		//	ViewBag.ListofResource = CroRecommandList(id, APDBDef.CroResource.StarTotal.Desc, out total, 10, (page - 1) * 10);
		
		//	// 分页器
		//	ViewBag.PageSize = 10;
		//	ViewBag.PageNumber = page;
		//	ViewBag.TotalItemCount = total;
		//	ResUser user = new ResUser();
		//	user.UserId = id;
		//	return View(user);
		//}






		//
		// 资源查看
		// GET:		/CroResource/View
		//


		public ActionResult Details(long id)
		{
	
			var model = APBplDef.CroResourceBpl.PrimaryGet(id);
        // model.GhostFileName = model.ResourcePath;// model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);
			return View(model);
		}


		//删除资源

		public ActionResult Delete(long id, long resid)
		{
			APBplDef.CroResourceBpl.PrimaryDelete(resid);

			return RedirectToAction("CroMyResource", new { id = id });
		}

		//删除收藏
		public ActionResult DeleteFavorite(long id, long resid)
		{
			APBplDef.CroFavoriteBpl.PrimaryDelete(resid);

			return RedirectToAction("CroMyFavorite", new { id = id });
		}

		//删除下载
		public ActionResult DeleteDownload(long id, long resid)
		{
			APBplDef.CroDownloadBpl.PrimaryDelete(resid);

			return RedirectToAction("CroMyDownload", new { id = id });
		}

		//删除评论
		public ActionResult DeleteComment(long id, long resid)
		{
			APBplDef.CroCommentBpl.PrimaryDelete(resid);

			return RedirectToAction("CroMyComment", new { id = id });
		}


		//删除推荐
		public ActionResult DeleteCommand(long id, long resid)
		{
			APBplDef.CroResourceBpl.PrimaryDelete(resid);

			return RedirectToAction("CroMyRecommand", new { id = id });
		}

		


		public ActionResult NewsView(long id)
		{
			var model = APBplDef.ResBulletinBpl.PrimaryGet(id);
	
			return View(model);

		}




		public ActionResult Declare(long id)
		{
			ResUser user = new ResUser();
			user.UserId = id;
			return View(user);
		}

	}

}