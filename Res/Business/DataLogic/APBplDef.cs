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


		#region [ ResResource ]


		//public partial class ResResourceBpl : ResResourceBplBase
		//{

		//	/// <summary>
		//	/// Return a list for admin UI list. 
		//	/// </summary>
		//	/// <param name="total"></param>
		//	/// <param name="current"></param>
		//	/// <param name="rowCount"></param>
		//	/// <param name="where"></param>
		//	/// <param name="order"></param>
		//	/// <returns></returns>
		//	public static List<ResResource> TolerantSearch(out int total, int current, int rowCount, APSqlWherePhrase where, APSqlOrderPhrase order)
		//	{
		//		var t = APDBDef.ResResource;
		//		var u = APDBDef.ResUser;

		//		var query = APQuery
		//			.select(t.ResourceId, t.Title, u.RealName.As("Author"), t.MediumTypePKID, t.CreatedTime, t.StatePKID, t.EliteScore, t.ViewCount, t.DownCount, t.FavoriteCount, t.CommentCount, t.StarTotal, t.StarCount)
		//			.from(t, u.JoinInner(t.Creator == u.UserId))
		//			.where(where)
		//			.primary(t.ResourceId)
		//			.skip((current - 1) * rowCount)
		//			.take(rowCount);

		//		if (order != null)
		//			query.order_by(order);

		//		using (APDBDef db = new APDBDef())
		//		{
		//			total = db.ExecuteSizeOfSelect(query);
		//			return db.Query(query, t.TolerantMap).ToList();
		//		}
		//	}

		//}


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

			public static List<ResCompany> GetTree()
			{
				var t = APDBDef.ResCompany;
				var list = APQuery.select(t.CompanyId, t.ParentId, t.CompanyName, t.Path)
					.from(t)
					.order_by(t.ParentId.Asc)
					.query(new APDBDef(), reader =>
					{
						return new ResCompany()
						{
							CompanyId = reader.GetInt64(0),
							ParentId = reader.GetInt64(1),
							CompanyName = reader.GetString(2),
							Path = reader.GetString(3),
						};
					}).ToList();

				ResCompany root = new ResCompany() { CompanyId = 0, ParentId = 0, CompanyName = "上海市", Path = "" };
				Dictionary<long, ResCompany> dict = new Dictionary<long, ResCompany>(){
					{0, root}
				};

				foreach (var item in list)
				{
					if (dict.ContainsKey(item.ParentId))
					{
						var node = dict[item.ParentId];
						if (node.Children == null)
							node.Children = new List<ResCompany>();
						node.Children.Add(item);
					}

					dict[item.CompanyId] = item;
				}

				return root.Children;
			}

			public static List<ResCompany> GetParentTree()
			{
				var t = APDBDef.ResCompany;
				var list = APQuery.select(t.CompanyId, t.ParentId, t.CompanyName, t.Path)
					.from(t)
					.where(t.ParentId == 0)
					.order_by(t.ParentId.Asc)
					.query(new APDBDef(), reader =>
					{
						return new ResCompany()
						{
							CompanyId = reader.GetInt64(0),
							ParentId = reader.GetInt64(1),
							CompanyName = reader.GetString(2),
							Path = reader.GetString(3),
						};
					}).ToList();

				

				return list;
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
					.select(t.UserId, t.UserName, t.RealName, t.GenderPKID, t.Email, t.RegisterTime, t.LoginCount, t.Actived, c.CompanyName
               //TODO：r.RoleName
               )
					.from(t, 
						c.JoinInner(t.CompanyId == c.CompanyId)
						//ur.JoinInner(t.UserId == ur.UserId),
						//r.JoinInner(r.RoleId == ur.RoleId)
                  )
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


		#region [ ResRealBpl ]


		public partial class ResRealBpl : ResRealBplBase
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
			public static List<ResReal> TolerantSearch(out int total, int current, int rowCount, APSqlWherePhrase where, APSqlOrderPhrase order)
			{
				var t = APDBDef.ResReal;
				var c = APDBDef.ResCompany;

				var query = APQuery
					.select(t.Asterisk, c.CompanyName)
					.from(t, c.JoinInner(t.CompanyId == c.CompanyId))
					.where(where)
					.primary(t.RealId)
					.skip((current - 1) * rowCount)
					.take(rowCount);

				if (order != null)
					query.order_by(order);

				using (APDBDef db = new APDBDef())
				{
					total = db.ExecuteSizeOfSelect(query);
					return db.Query(query, reader =>
					{
						var real = t.Map(reader);
						real.CompanyName = c.CompanyName.GetValue(reader);
						return real;
					}).ToList();
				}
			}

		}


		#endregion


		#region [ ResResource ]


		public partial class CroResourceBpl : CroResourceBplBase
		{
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
                    .select(t.CrosourceId, t.Title, u.RealName.As("Author"), t.CreatedTime, t.StatePKID, t.EliteScore,t.CourseTypePKID) //t.MediumTypePKID,
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


            static APDBDef.CroResourceTableDef cr = APDBDef.CroResource;
            static APDBDef.MicroCourseTableDef mc = APDBDef.MicroCourse;
            static APDBDef.ExercisesTableDef et = APDBDef.Exercises;
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
                var query = APQuery.select(cr.Asterisk, mc.Asterisk, et.Asterisk,
                                          vf.FileName.As("VideoName"), vf.FilePath.As("VideoPath"),
                                          cf.FileName.As("CoverName"), cf.FilePath.As("CoverPath"),
                                          df.FileName.As("DesignName"),
                                          sf.FileName.As("SummaryName")
                                         )
                                   .from(cr,
                                         mc.JoinLeft(cr.CrosourceId == mc.ResourceId),
                                         et.JoinLeft(et.CourseId == mc.CourseId),
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
                    course.VideoName = vf.FileName.GetValue(r, "VideoName");
                    course.CoverName = cf.FileName.GetValue(r, "CoverName");
                    course.DesignName = df.FileName.GetValue(r, "DesignName");
                    course.SummaryName = sf.FileName.GetValue(r, "SummaryName");

                    var exe = new Exercises();
                    et.Fullup(r, exe, false);

                    if (exe.ExerciseId > 0)
                        course.Exercises.Add(exe);

                    if (!model.Courses.Exists(x => x.CourseId == course.CourseId))
                    {
                        model.Courses.Add(course);
                    }
                    else
                    {
                        var c = model.Courses.FirstOrDefault(x => x.CourseId == course.CourseId);
                        if (!c.Exercises.Exists(x => x.ExerciseId == exe.ExerciseId))
                        {
                            c.Exercises.Add(exe);
                        }
                    }

                    return model;
                }).ToList();

                return model;
            }

        }

		#endregion


		#region [ ZSResource ]

		public partial class ZSResourceBpl : ZSResourceBplBase
		{


			public static List<ZSResource> GetLevel()
			{

				var t = APDBDef.ZSResource;

				var query = APQuery
					.select(t.OneLevel, t.TowLevel)
					.from(t)
					.primary(t.ResourceId);
					

				using (APDBDef db = new APDBDef())
				{
					return db.Query(query, r =>
					{
						return new ZSResource
						{
							OneLevel = t.OneLevel.GetValue(r),
							TowLevel = t.TowLevel.GetValue(r)
						};
					}).ToList();
				}

			}

		}

		#endregion

	}

}