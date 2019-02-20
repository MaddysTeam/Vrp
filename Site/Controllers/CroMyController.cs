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
         APBplDef.ResUserBpl.UpdatePartial(ResSettings.SettingsInSession.UserId, new { model.RealName, model.Email, model.PhotoPath });

         return RedirectToAction("Index", new { id = ResSettings.SettingsInSession.UserId, });
      }


      //平台 公告
      public ActionResult More(long id, string type, int page = 1)
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


      //上传微课作品

      public ActionResult Upload(long id, long? resid)
      {
         ResSettings.SettingsInSession.CleanCompanyCache();
         var user = ResSettings.SettingsInSession.User;
         var provinces = ResSettings.SettingsInSession.AllProvince();
         var areas = ResSettings.SettingsInSession.AllAreas();
         var schools = ResSettings.SettingsInSession.AllSchools();

         if (user.ProvinceId > 0)
         {
            provinces = provinces.Where(x => x.CompanyId == user.ProvinceId).ToList();
         }
         if (user.AreaId > 0)
         {
            areas = areas.Where(x => x.CompanyId == user.AreaId).ToList();
         }
         if (user.CompanyId > 0)
         {
            schools = schools.Where(x => x.CompanyId == user.CompanyId).ToList();
         }

         ViewBag.Provinces = provinces;
         ViewBag.Areas = areas;
         ViewBag.Companies = schools;

         ViewBag.Actives = APBplDef.ActiveBpl.GetAll();

         ViewBag.ProvincesDic = GetStrengthDict(areas);
         ViewBag.AreasDic = GetStrengthDict(areas);
         ViewBag.SchoolsDic = GetStrengthDict(schools);
         ViewBag.ResTypes = GetStrengthDict(CroResourceHelper.ResourceType.GetItems());
         ViewBag.Actives = APBplDef.ActiveBpl.GetAll();

         var model= resid == null ? 
                       new CroResource { ProvinceId=user.ProvinceId, AreaId=user.AreaId, CompanyId=user.CompanyId, Courses = new List<MicroCourse> { new MicroCourse() } } :
                       APBplDef.CroResourceBpl.GetResource(db, resid.Value);

         return View(model);
      }


      [HttpPost]
      [ValidateInput(false)]
      public ActionResult Upload(long id, long? resid, CroResource model, FormCollection fc)
      {
         var mc = APDBDef.MicroCourse;
         var et = APDBDef.Exercises;
         var eti = APDBDef.ExercisesItem;

         CroResource current = null;
         if (resid != null && resid.Value > 0)
            current = APBplDef.CroResourceBpl.GetResource(db, resid.Value);

         db.BeginTrans();

         try
         {
            if (current != null)
            {
               var exeIds = new List<long>();
               foreach (var item in current.Courses)
               {
                  if (item.Exercises != null && item.Exercises.Count > 0)
                     exeIds.AddRange(item.Exercises.Select(x => x.ExerciseId).ToArray());
               }

               if (exeIds.Count() > 0)
                  APBplDef.ExercisesItemBpl.ConditionDelete(eti.ExerciseId.In(exeIds.ToArray()));

               var courseIds = current.Courses.Select(x => x.CourseId).ToArray();
               APBplDef.ExercisesBpl.ConditionDelete(et.CourseId.In(courseIds));
               APBplDef.MicroCourseBpl.ConditionDelete(mc.ResourceId == resid);
               APBplDef.CroResourceBpl.PrimaryDelete(resid.Value);

               model.CourseTypePKID = model.CourseTypePKID == 0 ? CroResourceHelper.MicroClass : current.CourseTypePKID;
               model.CreatedTime = current.CreatedTime;
               model.Creator = current.Creator;
               model.LastModifier = id;
               model.LastModifiedTime = DateTime.Now;
               model.StatePKID = current.StatePKID;
               model.PublicStatePKID = current.PublicStatePKID;
               model.DownloadStatePKID = current.DownloadStatePKID;
               model.DownCount = current.DownCount;
               model.ViewCount = current.ViewCount;
               model.Score = current.Score;
               model.WinLevelPKID = current.WinLevelPKID;
               model.StatePKID = current.StatePKID;
            }
            else
            {
               model.CourseTypePKID = model.CourseTypePKID == 0 ? CroResourceHelper.MicroClass : model.CourseTypePKID;
               model.StatePKID = CroResourceHelper.StateWait;
               model.Creator = id;
               model.CreatedTime = model.LastModifiedTime = DateTime.Now;
               model.LastModifier = ResSettings.SettingsInSession.UserId;
               model.DownloadStatePKID = CroResourceHelper.AllowDownload;
               model.PublicStatePKID = CroResourceHelper.Public;
            }

            // 微课类型为微课时，微课标题为作品标题
            if (model.CourseTypePKID == CroResourceHelper.MicroClass)
               model.Courses[0].CourseTitle = model.Title;

            model.StatePKID = model.StatePKID == CroResourceHelper.StateDeny ? CroResourceHelper.StateWait : model.StatePKID;
            APBplDef.CroResourceBpl.Insert(model);

            foreach (var item in model.Courses ?? new List<MicroCourse>())
            {
               var currentCourse = current==null? model.Courses.FirstOrDefault(x => x.CourseId == item.CourseId) : 
                                                current.Courses.FirstOrDefault(x => x.CourseId == item.CourseId);
               item.ResourceId = model.CrosourceId;
               item.PlayCount = currentCourse != null ? currentCourse.PlayCount : 0;
               item.DownCount = currentCourse != null ? currentCourse.DownCount : 0;
               APBplDef.MicroCourseBpl.Insert(item);

               foreach (var exer in item.Exercises ?? new List<Exercises>())
               {
                  exer.CourseId = item.CourseId;
                  APBplDef.ExercisesBpl.Insert(exer);

                  foreach (var exerItem in exer.Items ?? new List<ExercisesItem>())
                  {
                     exerItem.ExerciseId = exer.ExerciseId;
                     APBplDef.ExercisesItemBpl.Insert(exerItem);
                  }
               }
            }

            db.Commit();
         }
         catch
         {
            db.Rollback();
         }


         return Request.IsAjaxRequest() ? Json(new
         {
            state = "ok",
            msg = "本作品审核完成。"
         }) : (ActionResult)RedirectToAction("CroMyResource", new { id = id });

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


      //我的微课作品
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

      //我的收藏
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

      //我的评价
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

      //我的下载
      public ActionResult CroMyPraise(long id, int page = 1)
      {
         int total = 0;
         ViewBag.ListofResource = MyCroPraise(id, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);
      }

      public ActionResult CroMyMedal(long id,int page= 1)
      {
         int total = 0;
         ViewBag.ListofMedals = MyMedals(id, out total, 10, (page - 1) * 10);

         // 分页器
         ViewBag.PageSize = 10;
         ViewBag.PageNumber = page;
         ViewBag.TotalItemCount = total;

         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);
      }

      //
      // 微课作品查看
      // GET:		/CroResource/View
      //


      public ActionResult Details(long id)
      {

         var model = APBplDef.CroResourceBpl.PrimaryGet(id);
         // model.GhostFileName = model.ResourcePath;// model.IsLink ? model.ResourcePath : Path.GetFileName(model.ResourcePath);
         return View(model);
      }

      //删除微课作品

      public ActionResult Delete(long id, long resid)
      {
         APBplDef.CroResourceBpl.UpdatePartial(resid, new { StatePKID = CroResourceHelper.StateDelete });

         return RedirectToAction("CroMyResource", new { id = id });
      }

      //删除收藏

      public ActionResult DeleteFavorite(long id, long resid)
      {
         APBplDef.CroFavoriteBpl.PrimaryDelete(resid);

         return RedirectToAction("CroMyFavorite", new { id = id });
      }

      //删除评论

      public ActionResult DeleteComment(long id, long resid)
      {
         APBplDef.CroCommentBpl.PrimaryDelete(resid);

         return RedirectToAction("CroMyComment", new { id = id });
      }

      //取消点赞

      public ActionResult DeletePraise(long id, long resid)
      {
         APBplDef.CroPraiseBpl.PrimaryDelete(resid);

         return RedirectToAction("CroMyPraise", new { id = id });
      }


      public ActionResult NewsView(long id)
      {
         var model = APBplDef.CroBulletinBpl.PrimaryGet(id);

         return View(model);

      }


      public ActionResult Declare(long id)
      {
         ResUser user = new ResUser();
         user.UserId = id;
         return View(user);
      }


      //
      // 修改个人密码
      // GET:		/My/ChgPwd
      // POST:		/My/ChgPwd
      //
      public ActionResult ChgPwd(long id)
      {
         return View();
      }

      [HttpPost]
      public async Task<ActionResult> ChgPwd(ChgPwd model)
      {
         var newPassword = model.Password;
         var user = APBplDef.ResUserBpl.PrimaryGet(ResSettings.SettingsInSession.UserId);
         var result = await UserManager.ChangePasswordAsync(user.UserId, user.Password, newPassword);

         if (result.Succeeded)
            APBplDef.ResUserBpl.UpdatePartial(user.UserId, new { Password = newPassword });

         return RedirectToAction("Index", new { id = ResSettings.SettingsInSession.UserId, });
      }


      //
      // 修改公开状态
      // POST:		/My/PublicSettings
      //

      [HttpPost]
      public ActionResult PublicSetting(long id, long resid)
      {
         if (id > 0 && resid > 0)
         {
            var res = APBplDef.CroResourceBpl.PrimaryGet(resid);

            res.PublicStatePKID = res.PublicStatePKID == CroResourceHelper.Public ? CroResourceHelper.Private : CroResourceHelper.Public;

            APBplDef.CroResourceBpl.UpdatePartial(resid, new { PublicStatePKID = res.PublicStatePKID });
         }

         return Json(new
         {
            msg = "设置成功"
         });
      }


      //
      // 修改下载状态
      // POST:		/My/PublicSettings
      //

      [HttpPost]
      public ActionResult DownloadSetting(long id, long resid)
      {
         if (id > 0 && resid > 0)
         {
            var res = APBplDef.CroResourceBpl.PrimaryGet(resid);

            res.DownloadStatePKID = res.DownloadStatePKID == CroResourceHelper.AllowDownload ? CroResourceHelper.DenyDownload : CroResourceHelper.AllowDownload;

            APBplDef.CroResourceBpl.UpdatePartial(resid, new { DownloadStatePKID = res.DownloadStatePKID });
         }

         return Json(new
         {
            msg = "设置成功"
         });
      }

   }

}