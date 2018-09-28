using Res.Business;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

   public partial class APBplDef
   {

      #region [ ResPickListBpl & ResPickListItemBpl ]


      /// <summary>
      /// Partial implementation of ResPickListBpl
      /// </summary>
      public partial class ResPickListBpl : ResPickListBplBase
      {

         #region [ Cache ]


         public class ItemCache
         {
            private long _pickListId;
            private List<ResPickListItem> _items;
            private ResPickListItem _defaultItem;
            private Dictionary<long, ResPickListItem> _idItemDict;
            private Dictionary<string, long> _nameIdDict;

            public ItemCache(List<ResPickListItem> list)
            {
               if (list.Count > 0)
                  _pickListId = list[0].PickListId;
               _items = list;
               _idItemDict = new Dictionary<long, ResPickListItem>(list.Count);
               _nameIdDict = new Dictionary<string, long>(list.Count);

               // 临时变量 ss 和 s 无实际用处，可能在预定义字典的时候冲突时，可以在调试状态下检测
               // 冲突的提示。
               List<string> ss = new List<string>();
               foreach (ResPickListItem item in list)
               {
                  try
                  {
                     if (item.IsDefault)
                        _defaultItem = item;
                     _idItemDict.Add(item.PickListItemId, item);
                     _nameIdDict.Add(item.Name, item.PickListItemId);
                  }
                  catch // (Exception ex)
                  {
                     ss.Add(item.Name);
                  }
               }
               string s = String.Join(",", ss.ToArray());
            }
            public List<ResPickListItem> Items { get { return _items; } }
            public ResPickListItem DefaultItem { get { return _defaultItem; } }
            public Dictionary<long, ResPickListItem> IdItemDict { get { return _idItemDict; } }
            public long PKID { get { return _pickListId; } }
            public string ItemName(long pickListItemId) { return _idItemDict.ContainsKey(pickListItemId) ? _idItemDict[pickListItemId].Name : ""; }
            public long ItemId(string name) { return _nameIdDict[name]; }
         }


         public static ItemCache Cached(string innerKey)
         {
            Dictionary<string, ItemCache> cache = ResSettings.GetCache(typeof(ResPickList)) as Dictionary<string, ItemCache>;


            if (cache == null)
               ResSettings.SetCache(cache = new Dictionary<string, ItemCache>(), typeof(ResPickList));


            if (cache.ContainsKey(innerKey))
               return cache[innerKey];


            ItemCache itemCache = new ItemCache(APBplDef.ResPickListItemBpl.GetByPickListInnerKey(innerKey));
            cache[innerKey] = itemCache;


            return itemCache;
         }


         public static ItemCache Cached(long pickListId)
         {
            Dictionary<string, ItemCache> cache = ResSettings.GetCache(typeof(ResPickList)) as Dictionary<string, ItemCache>;


            if (cache == null)
               ResSettings.SetCache(cache = new Dictionary<string, ItemCache>(), typeof(ResPickList));


            foreach (var p in cache)
            {
               if (p.Value.PKID == pickListId)
                  return p.Value;
            }

            return Cached(APBplDef.ResPickListBpl.PrimaryGet(pickListId).InnerKey);
         }


         /// <summary>
         /// 清除缓存
         /// </summary>
         public static void ClearCache()
         {
            ResSettings.RemoveCache(typeof(ResPickList));
         }


         /// <summary>
         /// 移除缓存
         /// </summary>
         /// <param name="innerKey"></param>
         public static void RemoveCache(string innerKey)
         {
            Dictionary<string, ItemCache> cache = ResSettings.GetCache(typeof(ResPickList)) as Dictionary<string, ItemCache>;
            if (cache != null && cache.ContainsKey(innerKey))
               cache.Remove(innerKey);
         }


         #endregion

      }


      /// <summary>
      /// Partial implementation of ResPickListBpl
      /// </summary>
      public partial class ResPickListItemBpl : ResPickListItemBplBase
      {

         /// <summary>
         /// 根据 PickList 的 InnerKey 获得所有子项
         /// </summary>
         /// <param name="innerKey"></param>
         /// <returns></returns>
         public static List<ResPickListItem> GetByPickListInnerKey(string innerKey)
         {
            var query = APQuery
               .select(APDBDef.ResPickListItem.Asterisk)
               .from(
                  APDBDef.ResPickListItem,
                  APDBDef.ResPickList.Join(APSqlJoinType.Inner, APDBDef.Res_PickList_Item)
                  )
               .where(APDBDef.ResPickList.InnerKey == innerKey);


            using (APDBDef db = new APDBDef())
            {
               return APDBDef.ResPickListItem.MapList(db.ExecuteReader(query));
            }
         }

      }


      #endregion


      #region [ ResUserBpl ]


      public partial class ResUserBpl : ResUserBplBase
      {

         /// <summary>
         /// Return a list for admin UI list. 
         /// </summary>
         /// <param name="total"></param>
         /// <param name="current"></param>
         /// <param name="rowCount"></param>
         /// <param name="where"></param>
         /// <param name="order"></param>
         /// <returns></returns>
         public static List<ResUser> TolerantSearch(out int total, int current, int rowCount, APSqlWherePhrase where, APSqlOrderPhrase order)
         {
            var t = APDBDef.ResUser;
            var c = APDBDef.ResCompany;
            var r = APDBDef.ResRole;
            var ur = APDBDef.ResUserRole;

            var query = APQuery
               .select(t.UserId, t.UserName, t.RealName, t.GenderPKID, t.Email, t.RegisterTime, t.LoginCount, t.Actived, c.CompanyName, r.RoleName)
               .from(t,
                  c.JoinInner(t.CompanyId == c.CompanyId),
                  ur.JoinInner(t.UserId == ur.UserId),
                  r.JoinInner(r.RoleId == ur.RoleId))
               .where(where)
               .primary(t.UserId)
               .skip((current - 1) * rowCount)
               .take(rowCount);

            if (order != null)
               query.order_by(order);

            using (APDBDef db = new APDBDef())
            {
               total = db.ExecuteSizeOfSelect(query);
               return db.Query(query, reader =>
               {
                  return new ResUser()
                  {
                     UserId = t.UserId.GetValue(reader),
                     UserName = t.UserName.GetValue(reader),
                     RealName = t.RealName.GetValue(reader),
                     GenderPKID = t.GenderPKID.GetValue(reader),
                     Email = t.Email.GetValue(reader),
                     RegisterTime = t.RegisterTime.GetValue(reader),
                     LoginCount = t.LoginCount.GetValue(reader),
                     Actived = t.Actived.GetValue(reader),
                     CompanyName = c.CompanyName.GetValue(reader),
                     //RoleName = r.RoleName.GetValue(reader),
                  };
               }).ToList();
            }
         }


         public static void SetLastLoginTime(string username)
         {
            var t = APDBDef.ResUser;
            var query = APQuery.update(t)
               .set(t.LastLoginTime, DateTime.Now)
               .set(t.LoginCount, APSqlThroughExpr.Expr("LoginCount + 1"))
               .where(t.UserName == username);
            using (var db = new APDBDef())
            {
               db.ExecuteNonQuery(query);
            }
         }

      }


      #endregion


      #region [ ResRoleApproveBpl ]


      public partial class ResRoleApproveBpl : ResRoleApproveBplBase
      {

         public static void Sync(long roleId, List<long> approveIds)
         {
            var t = APDBDef.ResRoleApprove;

            using (APDBDef db = new APDBDef())
            {

               var existIds = APQuery.select(t.ApproveId)
                  .from(t)
                  .where(t.RoleId == roleId).query(db, reader =>
                  {
                     return reader.GetInt64(0);
                  }).ToList();

               db.BeginTrans();
               try
               {
                  foreach (var id in approveIds)
                  {
                     if (existIds.Contains(id))
                     {
                        existIds.Remove(id);
                     }
                     else
                     {
                        db.ResRoleApproveDal.Insert(new ResRoleApprove(0, roleId, id));
                     }
                  }
                  if (existIds.Count > 0)
                     db.ResRoleApproveDal.ConditionDelete(t.RoleId == roleId & t.ApproveId.In(existIds.ToArray()));

                  db.Commit();
               }
               catch
               {
                  db.Rollback();
               }
            }
         }

      }


      #endregion


      #region [ ResCompanyBpl ]


      public partial class ResCompanyBpl : ResCompanyBplBase
      {

      }


      #endregion


      #region [ CroResource ]


      public partial class CroResourceBpl : CroResourceBplBase
      {

         /// <summary>
         /// Return a list for admin UI list. 
         /// </summary>
         /// <param name="total"></param>
         /// <param name="current"></param>
         /// <param name="rowCount"></param>
         /// <param name="where"></param>
         /// <param name="order"></param>
         /// <returns></returns>
         public static List<CroResource> TolerantSearch(out int total, int current, int rowCount, APSqlWherePhrase where, APSqlOrderPhrase order)
         {
            var t = APDBDef.CroResource;
            var u = APDBDef.ResUser;

            var query = APQuery
               .select(t.CrosourceId, t.Title, u.RealName.As("Author") //TODO:t.MediumTypePKID
               , t.CreatedTime, t.StatePKID, t.EliteScore)
               .from(t, u.JoinInner(t.Creator == u.UserId))
               .where(where)
               .primary(t.CrosourceId)
               .skip((current - 1) * rowCount)
               .take(rowCount);

            if (order != null)
               query.order_by(order);

            using (APDBDef db = new APDBDef())
            {
               total = db.ExecuteSizeOfSelect(query);
               return db.Query(query, t.TolerantMap).ToList();
            }
         }


         public static void CountingView(APDBDef db, long resourceId, long userId)
         {
            var t = APDBDef.CroResource;

            APQuery.update(t)
               .set(t.ViewCount, APSqlThroughExpr.Expr("ViewCount+1"))
               .where(t.CrosourceId == resourceId)
               .execute(db);

            if (userId != 0)
            {
               db.CroViewDal.Insert(new CroView()
               {
                  UserId = userId,
                  ResourceId = resourceId,
                  OccurTime = DateTime.Now
               });
            }
         }

         public static void CountingFavorite(APDBDef db, long resourceId, long userId)
         {
            var t = APDBDef.CroResource;

            APQuery.update(t)
               .set(t.FavoriteCount, APSqlThroughExpr.Expr("FavoriteCount+1"))
               .where(t.CrosourceId == resourceId)
               .execute(db);
            if (userId != 0)
               db.CroFavoriteDal.Insert(new CroFavorite()
               {
                  UserId = userId,
                  ResourceId = resourceId,
                  OccurTime = DateTime.Now
               });
         }

         public static void CountingComment(APDBDef db, long resourceId, string content, long userId)
         {
            var t = APDBDef.CroResource;

            APQuery.update(t)
               .set(t.CommentCount, APSqlThroughExpr.Expr("CommentCount+1"))
               .where(t.CrosourceId == resourceId)
               .execute(db);
            if (userId != 0)
               db.CroCommentDal.Insert(new CroComment()
               {
                  UserId = userId,
                  ResourceId = resourceId,
                  Content = content,
                  OccurTime = DateTime.Now,
                  Audittype = 1
               });
         }

         public static void CountingDownload(APDBDef db, long resourceId, long userId)
         {
            var t = APDBDef.CroResource;

            APQuery.update(t)
               .set(t.DownCount, APSqlThroughExpr.Expr("DownCount+1"))
               .where(t.CrosourceId == resourceId)
               .execute(db);
            if (userId != 0)
               db.CroDownloadDal.Insert(new CroDownload()
               {
                  UserId = userId,
                  ResourceId = resourceId,
                  OccurTime = DateTime.Now
               });
         }


         public static void CountingPraise(APDBDef db, long resourceId,long userId)
         {
            var t = APDBDef.CroResource;

            APQuery.update(t)
             .set(t.PraiseCount, APSqlThroughExpr.Expr("PraiseCount+1"))
             .where(t.CrosourceId == resourceId)
             .execute(db);
            if (userId != 0)
               db.CroPraiseDal.Insert(new CroPraise()
               {
                  UserId = userId,
                  ResourceId = resourceId,
                  OccurTime = DateTime.Now
               });
         }


         static APDBDef.CroResourceTableDef cr = APDBDef.CroResource;
         static APDBDef.MicroCourseTableDef mc = APDBDef.MicroCourse;
         static APDBDef.ExercisesTableDef et = APDBDef.Exercises;
         static APDBDef.ExercisesItemTableDef eti = APDBDef.ExercisesItem;
         static APDBDef.FilesTableDef vf = APDBDef.Files;
         static APDBDef.FilesTableDef cf = APDBDef.Files.As("CoverFile");
         static APDBDef.FilesTableDef df = APDBDef.Files.As("DesignFile");
         static APDBDef.FilesTableDef sf = APDBDef.Files.As("SummaryFile");

         /// <summary>
         ///  get complex resource object
         /// </summary>
         /// <param name="db">db</param>
         /// <param name="resourceId">resourceId</param>
         /// <returns>CroResource</returns>
         public static CroResource GetResource(APDBDef db, long resourceId)
         {
            var query = APQuery.select(cr.Asterisk, mc.Asterisk, et.Asterisk,eti.Asterisk,
                                      vf.FileName.As("VideoName"), vf.FilePath.As("VideoPath"),
                                      cf.FileName.As("CoverName"), cf.FilePath.As("CoverPath"),
                                      df.FileName.As("DesignName"), df.FilePath.As("DesignPath"),
                                      sf.FileName.As("SummaryName"), sf.FilePath.As("SummaryPath")
                                     )
                               .from(cr,
                                     mc.JoinLeft(cr.CrosourceId == mc.ResourceId),
                                     et.JoinLeft(et.CourseId == mc.CourseId),
                                     eti.JoinLeft(eti.ExerciseId==et.ExerciseId),
                                     vf.JoinLeft(vf.FileId == mc.VideoId),
                                     cf.JoinLeft(cf.FileId == mc.CoverId),
                                     df.JoinLeft(df.FileId == mc.DesignId),
                                     sf.JoinLeft(sf.FileId == mc.SummaryId)
                                     )
                                .where(cr.CrosourceId == resourceId);

            CroResource model = null;
            var result = query.query(db, r =>
            {
               if (model == null)
               {
                  model = new CroResource();
                  model.Courses = new List<MicroCourse>();
                  cr.Fullup(r, model, false);
               }

               var course = new MicroCourse();
               course.Exercises = new List<Exercises>();
               mc.Fullup(r, course, false);
               course.CoverPath = cf.FilePath.GetValue(r, "CoverPath");
               course.VideoPath = vf.FilePath.GetValue(r, "VideoPath");
               course.DesignPath = df.FilePath.GetValue(r, "DesignPath");
               course.SummaryPath = sf.FilePath.GetValue(r, "SummaryPath");
               course.VideoName = vf.FileName.GetValue(r, "VideoName");
               course.CoverName = cf.FileName.GetValue(r, "CoverName");
               course.DesignName = df.FileName.GetValue(r, "DesignName");
               course.SummaryName = sf.FileName.GetValue(r, "SummaryName");

               var exe = new Exercises();
               et.Fullup(r, exe, false);
               exe.Items = new List<ExercisesItem>();

               var item = new ExercisesItem();
               eti.Fullup(r, item, false);

               if (course.CourseId > 0)
                  if (!model.Courses.Exists(x => x.CourseId == course.CourseId))
                     model.Courses.Add(course);
                  else
                     course = model.Courses.Find(x => x.CourseId == course.CourseId);

               if (exe.ExerciseId > 0)
                  if (!course.Exercises.Exists(e => e.ExerciseId == exe.ExerciseId))
                     course.Exercises.Add(exe);
                  else
                     exe = course.Exercises.Find(e => e.ExerciseId == exe.ExerciseId);

               if (item.ItemId > 0)
                  exe.Items.Add(item);

               return model;
            }).ToList();

            return model;
         }

      }




      #endregion


      #region [ MIcroCourse ]

      public partial class MicroCourseBpl : MicroCourseBplBase
      {

         public static void CountingPlay(APDBDef db, long courseId)
         {
            var c = APDBDef.MicroCourse;
            APQuery.update(c)
               .set(c.PlayCount, APSqlThroughExpr.Expr("PlayCount+1"))
               .where(c.CourseId == courseId)
               .execute(db);
         }

      }

      #endregion

   }

}

