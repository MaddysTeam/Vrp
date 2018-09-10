using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Res.Business;

namespace Res
{
	public class ApplicationUserManager : UserManager<ResUser, long>
	{

		public ApplicationUserManager(IUserStore<ResUser, long> store)
			: base(store)
		{
		}


		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new MyUserStore());

			manager.UserLockoutEnabledByDefault = false;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					 new DataProtectorTokenProvider<ResUser, long>(dataProtectionProvider.Create("ASP.NET Identity"));
			}

			return manager;
		}

	}


	public class ApplicationSignInManager : SignInManager<ResUser, long>
	{
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}


		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ResUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}


		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}

	}

}
