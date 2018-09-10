using Microsoft.AspNet.Identity;
using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Res.Business
{

	public class MyUserStore :
		IQueryableUserStore<ResUser, long>,
		IUserRoleStore<ResUser, long>,
		IUserPasswordStore<ResUser, long>,
		IUserSecurityStampStore<ResUser, long>,
		IUserLockoutStore<ResUser, long>,
		IUserTwoFactorStore<ResUser, long>
	{

		#region [ IUserTwoFactorStore ]


		public virtual Task<bool> GetTwoFactorEnabledAsync(ResUser user)
		{
			return Task.FromResult(false);
		}


		public virtual Task SetTwoFactorEnabledAsync(ResUser user, bool enabled)
		{
			return Task.FromResult(0);
		}


		#endregion


		#region [ IUserSecurityStampStore ]


		public virtual Task<int> GetAccessFailedCountAsync(ResUser user)
		{
			return Task.FromResult(0);

		}


		public virtual Task<bool> GetLockoutEnabledAsync(ResUser user)
		{
			return Task.FromResult(false);

		}


		public virtual Task<DateTimeOffset> GetLockoutEndDateAsync(ResUser user)
		{
			return Task.FromResult(DateTimeOffset.MaxValue);

		}


		public virtual Task<int> IncrementAccessFailedCountAsync(ResUser user)
		{
			return Task.FromResult(0);

		}


		public virtual Task ResetAccessFailedCountAsync(ResUser user)
		{
			return Task.FromResult(0);

		}
		public virtual Task SetLockoutEnabledAsync(ResUser user, bool enabled)
		{
			return Task.FromResult(0);

		}


		public virtual Task SetLockoutEndDateAsync(ResUser user, DateTimeOffset lockoutEnd)
		{
			return Task.FromResult(0);

		}


		#endregion


		#region [ IUserSecurityStampStore ]


		public virtual Task<string> GetSecurityStampAsync(ResUser user)
		{
			return Task.FromResult(user.SecurityStamp);
		}


		public virtual Task SetSecurityStampAsync(ResUser user, string stamp)
		{
			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}


		#endregion


		# region [ IUserPasswordStore ]


		public virtual Task<string> GetPasswordHashAsync(ResUser user)
		{
			return Task.FromResult(user.PasswordHash);
		}


		public virtual Task<bool> HasPasswordAsync(ResUser user)
		{
			return Task.FromResult(!String.IsNullOrEmpty(user.PasswordHash));
		}


		public virtual Task SetPasswordHashAsync(ResUser user, string passwordHash)
		{
			user.PasswordHash = passwordHash;
			return Task.FromResult(0);
		}


		#endregion


		#region [ IUserRoleStore ]


		public async virtual Task AddToRoleAsync(ResUser user, string roleName)
		{
			await Task.Run(() =>
			{
				var t = APDBDef.ResUserRole;
				var r = APDBDef.ResRole;

				using (APDBDef db = new APDBDef())
				{
					var role = db.ResRoleDal.ConditionQuery(r.RoleName == roleName, null, 1, 0).FirstOrDefault();
					if (role != null)
						db.ResUserRoleDal.Insert(new ResUserRole() { UserId = user.UserId, RoleId = role.RoleId });
				}
			});
		}


		public virtual async Task<IList<string>> GetRolesAsync(ResUser user)
		{
			return await Task.Run(() =>
			{
				var t = APDBDef.ResUserRole;
				var r = APDBDef.ResRole;

				using (APDBDef db = new APDBDef())
				{
					return APQuery.select(r.RoleName)
						.from(r, t.JoinLeft(r.RoleId == t.RoleId))
						.where(t.UserId == user.UserId)
						.query(db, reader =>
						{
							return reader.GetString(0);
						}).ToList();
				}
			});
		}


		public virtual async Task<bool> IsInRoleAsync(ResUser user, string roleName)
		{
			return await Task.Run(() =>
			{
				var t = APDBDef.ResUserRole;
				var r = APDBDef.ResRole;

				using (APDBDef db = new APDBDef())
				{
					return (int)APQuery.select(APSqlAsteriskExpr.Expr.Count())
						  .from(t, r.JoinInner(t.RoleId == r.RoleId))
						  .where(t.UserId == user.UserId, r.RoleName == roleName)
						  .executeScale(db) > 0;
				}
			});
		}


		public async virtual Task RemoveFromRoleAsync(ResUser user, string roleName)
		{
			await Task.Run(() =>
			{
				var t = APDBDef.ResUserRole;
				var r = APDBDef.ResRole;

				using (APDBDef db = new APDBDef())
				{
					APQuery.delete(t)
						.from(t, r.JoinInner(t.RoleId == r.RoleId))
						.where(r.RoleName == roleName)
						.execute(db);
				}
			});
		}


		#endregion


		#region [ IQueryableUserStore ]


		public async virtual Task CreateAsync(ResUser user)
		{
			await Task.Run(() =>
			{
				APBplDef.ResUserBpl.Insert(user);
			});
		}


		public async virtual Task DeleteAsync(ResUser user)
		{
			await Task.Run(() =>
			{
				APBplDef.ResUserBpl.PrimaryDelete(user.UserId);
			});
		}


		public async virtual Task<ResUser> FindByIdAsync(long userId)
		{
			return await Task.Run(() =>
			{
				return APBplDef.ResUserBpl.PrimaryGet(userId);
			});
		}


		public async virtual Task<ResUser> FindByNameAsync(string userName)
		{
			return await Task.Run(() =>
			{
				return APBplDef.ResUserBpl.ConditionQuery(APDBDef.ResUser.UserName == userName, null, 1).FirstOrDefault();
			});
		}


		public async virtual Task UpdateAsync(ResUser user)
		{
			await Task.Run(() =>
			{
				APBplDef.ResUserBpl.Update(user);
			});
		}


		public IQueryable<ResUser> Users
		{
			get { return APBplDef.ResUserBpl.GetAll().AsQueryable(); }
		}


		#endregion


		public void Dispose()
		{
			// Do nothing
		}

	}

}