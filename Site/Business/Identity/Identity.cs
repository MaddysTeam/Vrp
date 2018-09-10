using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Res.Business
{

	public partial class ResRole : ResRoleBase, IRole<long>
	{

		public long Id
		{
			get { return RoleId; }
		}


		public string Name
		{
			get { return RoleName; }
			set { RoleName = value; }
		}

	}


	public partial class ResUser : ResUserBase, IUser<long>
	{

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ResUser, long> manager)
		{
			// 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// 在此处添加自定义用户声明
			return userIdentity;
		}


		public long Id
		{
			get { return UserId; }
		}

	}


	public class LoginViewModel
	{
		[Required]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		[Display(Name = "记住我?")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "用户名")]
		public string UserName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "密码")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "确认密码")]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }
	}

	public class LocalPasswordModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "当前密码")]
		public string OldPassword { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "新密码")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "确认新密码")]
		[Compare("NewPassword", ErrorMessage = "新密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }
	}


	public class Register
	{
		public long RealId { get; set; }

		[Required]
		[Display(Name = "登录名称")]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		public string Username { get; set; }

		[Required]
		[Display(Name = "登录密码")]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		public string Password { get; set; }

		[Required]
		[Display(Name = "确认密码")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }

		[Required]
		[Display(Name = "电子邮箱")]
		public string Email { get; set; }

		[Required]
		[Display(Name = "密码问题")]
		public string Question { get; set; }

		[Required]
		[Display(Name = "密码答案")]
		public string Answer { get; set; }
	}

	public class ChgPwd
	{

		[Required]
		[Display(Name = "新密码")]
		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
		public string Password { get; set; }

		[Required]
		[Display(Name = "确认新密码")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
		public string ConfirmPassword { get; set; }


	}
}