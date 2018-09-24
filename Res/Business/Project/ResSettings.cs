using Symber.Web.Data;
using Symber.Web.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;

namespace Res.Business
{

	/// <summary>
	/// 上下文设置
	/// </summary>
	public class HttpContextSettings
	{

		#region [ Constructors ]

		public HttpContextSettings()
		{

		}

		#endregion


		#region [ Properties ]


		public RequestContext RequestContext { get; set; }


		public HttpContextBase Context { get; set; }


		#endregion


		#region [ Methods ]


		public void SetParameter(string key, object value)
		{
			HttpContext.Current.Items[key] = value;
		}


		public object GetParameter(string key)
		{
			return HttpContext.Current.Items[key];
		}


		#endregion

	}


	/// <summary>
	/// 会话设置
	/// </summary>
	public class SessionSettings
	{

		#region [ Fields ]


		private ResUser _user;
		private ResRole _role;
		private long[] _approves;


		#endregion


		#region [ Constructors ]


		public SessionSettings()
		{
		}


		#endregion


		#region [ Properties ]


		public long UserId
		{
			get { CheckCurrent(); return _user != null ? _user.UserId : 0; }
		}


		public long RoleId
		{
			get { CheckCurrent(); return _role != null ? _role.RoleId : 0; }
		}


		public string UserName
		{
			get { CheckCurrent(); return _user != null ? _user.UserName : ""; }
		}


		public string RealName
		{
			get { CheckCurrent(); return _user != null ? _user.RealName : ""; }
		}


		public string RoleName
		{
			get { CheckCurrent(); return _role != null ? _role.RoleName : ""; }
		}

		public long[] Approves
		{
			get { CheckCurrent(); return _approves != null ? _approves : new long[0]; }
		}

		public bool CanbeUploadResource
		{
			get { CheckCurrent(); return Approves.Contains(1); }
		}
		public bool CanbeDownloadResource
		{
			get { CheckCurrent(); return Approves.Contains(2); }
		}
		public bool CanbeAuditResource
		{
			get { CheckCurrent(); return Approves.Contains(3); }
		}
		public bool CanbeManageComment
		{
			get { CheckCurrent(); return Approves.Contains(4); }
		}
		public bool CanbeManageUser
		{
			get { CheckCurrent(); return Approves.Contains(9); }
		}

		#endregion


		#region [ Methods ]


		private void CheckCurrent()
		{
         if (!HttpContext.Current.Request.IsAuthenticated)
            throw new NotSupportedException(
               "本系统的 SessionSettings 不支持非登录用户采样，请确保你书写的代码段不存在非登录用户访问的漏洞！");
         if (_user == null)
			{
				var u = APDBDef.ResUser;
				var r = APDBDef.ResRole;
				var ur = APDBDef.ResUserRole;
				var ra = APDBDef.ResRoleApprove;

				using (APDBDef db = new APDBDef())
				{
					_user = db.ResUserDal.ConditionQuery(u.UserName == HttpContext.Current.User.Identity.Name, null, null, null).FirstOrDefault();
					if (_user != null)
					{
						_role = APQuery.select(r.Asterisk)
							.from(r, ur.JoinInner(r.RoleId == ur.RoleId))
							.where(ur.UserId == _user.UserId)
							.query(db, r.Map).FirstOrDefault();

						if (_role != null)
						{
							_approves = APQuery.select(ra.ApproveId).from(ra).where(ra.RoleId == _role.RoleId)
								.query(db, reader => { return ra.ApproveId.GetValue(reader); }).ToArray();
						}
					}
				}
			}
		}


		public void ResetCurrent()
		{
			if (_user != null)
			{
				_user = null;
				_role = null;
				_approves = null;
			}
		}


		#endregion


		//
		// 在回话环境中存取缓存的一组函数，需要注意的是，虽然描述为在上下文环境中，但是其生存期并不依赖
		// HttpContent 的生存期，而是固定由 ThisApp.CacheMinutes 指定。对全局用户有效。此处的缓存
		// 适合保存全局快速访问的数据，例如字典信息。

		#region [ Cache ]


		private Dictionary<string, object> _cache;


		public object GetCache(Type contentType, string ext = null)
		{
			string key = ResSettings.GetCacheKey(contentType, ext);
			if (_cache != null && _cache.ContainsKey(key))
				return _cache[key];

			return null;
		}


		public void SetCache(object content, Type contentType, string ext = null)
		{
			if (_cache == null)
				_cache = new Dictionary<string, object>();
			_cache[ResSettings.GetCacheKey(contentType, ext)] = content;
		}


		public void SetCache(object content)
		{
			SetCache(content, content.GetType(), null);
		}


		public void RemoveCache(Type contentType, string ext = null)
		{
			if (_cache != null)
				_cache.Remove(ResSettings.GetCacheKey(contentType, ext));
		}


		#endregion

	}


	/// <summary>
	/// 全局可访问设置
	/// </summary>
	public partial class ResSettings
	{

		private static string settingsInSession = "Res_SETTINGS_IN_SESSION";
		private static readonly object settingsInHttpContext = new object();


		public static HttpContextSettings SettingsInHttpContext
		{
			get
			{
				return HttpContext.Current.Items[settingsInHttpContext] as HttpContextSettings;
			}
			set
			{
				HttpContext.Current.Items[settingsInHttpContext] = value;
			}
		}


		public static SessionSettings SettingsInSession
		{
			get
			{
				if (HttpContext.Current.Session[settingsInSession] == null)
					HttpContext.Current.Session[settingsInSession] = new SessionSettings();
				return HttpContext.Current.Session[settingsInSession] as SessionSettings;
			}
		}


		public static void CleanSession()
		{
			HttpContext.Current.Session.Remove(settingsInSession);
		}


		//
		// 在上下文环境中存取缓存的一组函数，需要注意的是，虽然描述为在上下文环境中，但是其生存期并不依赖
		// HttpContent 的生存期，而是固定由 ThisApp.CacheMinutes 指定。对全局用户有效。此处的缓存
		// 适合保存全局快速访问的数据，例如字典信息。
		//
		// 存取数据时，使用数据类型进行，有意这样设计，意味着你在开发过程中不应该缓存零碎的数据，应该将
		// 数据有效的分类打包来使用。
		// 例如：
		//   SetCache(myName ,type(string));
		//   SetCache(youName, type(string));
		//   数据会被覆盖，我期望你如下的使用
		//   public class NamesData {
		//     public string myName;
		//     public string youName;
		//   }
		//   var data = new NamesData();
		//   SetCache(data, data.GetType());
		//
		// 如果在不同场景下，确实需要存取同一数据类型的多个彼此不相干的实例时，可以用 ext 名字来扩展使用
		// SetCache(oldNames, typeof(NamesData), "thisEnv");
		// SetCache(futureNames, typeof(NamesData), "thatEnv");
		//

		#region [ Static Cache ]

		
		public static string GetCacheKey(Type contentType, string ext)
		{
			if (ext == null)
				return contentType.ToString();
			return contentType.ToString() + ext;
		}


		public static object GetCache(Type contentType, string ext = null)
		{
			return HttpContext.Current.Cache[GetCacheKey(contentType, ext)];
		}


		public static void SetCache(object content, Type contentType, string ext = null)
		{
			HttpContext.Current.Cache.Insert(
				GetCacheKey(contentType, ext),
				content,
				null,
				DateTime.Now.AddMinutes(ThisApp.CacheMinutes),
				TimeSpan.Zero,
				CacheItemPriority.High,
				null);
		}


		public static void SetCache(object content)
		{
			if (content != null)
				SetCache(content, content.GetType(), null);
		}


		public static void RemoveCache(Type contentType, string ext = null)
		{
			HttpContext.Current.Cache.Remove(GetCacheKey(contentType, ext));
		}


		#endregion
	}

}