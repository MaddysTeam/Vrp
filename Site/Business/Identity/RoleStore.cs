using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Res.Business
{

	public class MyRoleStore : IQueryableRoleStore<ResRole, long>
	{

		#region [ IQueryableRoleStore ]


		public async virtual Task CreateAsync(ResRole role)
		{
			await Task.Run(() =>
			{
				APBplDef.ResRoleBpl.Insert(role);
			});
		}


		public async virtual Task DeleteAsync(ResRole role)
		{
			await Task.Run(() =>
			{
				APBplDef.ResRoleBpl.PrimaryDelete(role.RoleId);
			});
		}


		public async virtual Task<ResRole> FindByIdAsync(long roleId)
		{
			return await Task.Run(() =>
			{
				return APBplDef.ResRoleBpl.PrimaryGet(roleId);
			});
		}


		public async virtual Task<ResRole> FindByNameAsync(string roleName)
		{
			return await Task.Run(() =>
			{
				return APBplDef.ResRoleBpl.ConditionQuery(APDBDef.ResRole.RoleName == roleName, null, 1).FirstOrDefault();
			});
		}


		public async virtual Task UpdateAsync(ResRole role)
		{
			await Task.Run(() =>
			{
				APBplDef.ResRoleBpl.Update(role);
			});
		}


		public IQueryable<ResRole> Roles
		{
			get { return APBplDef.ResRoleBpl.GetAll().AsQueryable(); }
		}


		#endregion


		public void Dispose()
		{
			// Do nothing
		}

	}

}