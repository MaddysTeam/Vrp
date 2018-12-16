using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{
	/// <summary>
	/// 推荐作品
	/// </summary>
	public class ResResourceRecommand
	{
		public long ResourceId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string CoverPath { get; set; }
		public int StarCount { get; set; }
		public int StarTotal { get; set; }
		public int Star { get { if (StarCount == 0)return 0; return StarTotal / StarCount; } }
		public string FitCoverPath
		{
			get
			{
				if (CoverPath == "")
					return "/assets/img/cover.png";
				return CoverPath;
			}
		}
	}

	/// <summary>
	/// 排行榜作品
	/// </summary>
	public class ResResourceRanking : ResResourceRecommand
	{
		public string AuthorCompany { get; set; }
		public DateTime CreatedTime { get; set; }
		public int ViewCount { get; set; }
		public int CommentCount { get; set; }
		public int DownCount { get; set; }
		public string FileExtName { get; set; }
		public string Description { get; set; }
	}


	public class ResMyResource
	{
		public long ResourceId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string CoverPath { get; set; }
		public string FileExtName { get; set; }
		public string Description { get; set; }
		public DateTime OccurTime { get; set; }
		public long StatePKID { get; set; }

		public long OccurId { get; set; }
		public string Content { get; set; }
		public long Audittype { get; set; }
		public long Auditor { get; set; }
		public DateTime AuditedTime { get; set; }
		public string FitCoverPath
		{
			get
			{
				if (CoverPath == "")
					return "/assets/img/cover.png";
				return CoverPath;
			}
		}
	}


	//我的作品

	public class CroMyResource
	{
		public long CrosourceId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string CoverPath { get; set; }
		public string FileExtName { get; set; }
		public string Description { get; set; }
		public DateTime OccurTime { get; set; }
		public long StatePKID { get; set; }

		public long OccurId { get; set; }
		public string Content { get; set; }

		public long Audittype { get; set; }
		public long Auditor { get; set; }
		public DateTime AuditedTime { get; set; }

		public string FitCoverPath
		{
			get
			{
				if (CoverPath == "")
					return "/assets/img/cover.png";
				return CoverPath;
			}
		}
	}


	/// <summary>
	/// 评价作品
	/// </summary>
   /// 
	public class CroResourcecommend
	{
		public long CrosourceId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string CoverPath { get; set; }
		public int StarCount { get; set; }
		public int StarTotal { get; set; }
		public int Star { get { if (StarCount == 0)return 0; return StarTotal / StarCount; } }
		public string FitCoverPath
		{
			get
			{
				if (CoverPath == "")
					return "/assets/img/cover.png";
				return CoverPath;
			}
		}
	}


	public class CroResourceRanking : CroResourcecommend
	{
		public string AuthorCompany { get; set; }
		public DateTime CreatedTime { get; set; }
		public int ViewCount { get; set; }
		public int CommentCount { get; set; }
		public int DownCount { get; set; }
		public string FileExtName { get; set; }
		public string Description { get; set; }
	}


   /// <summary>
   /// 评审进度
   /// </summary>
   public class EvalProgress
   {
      public string Expert { get; set; }
      public long ExpertId { get; set; }
      public long GorupId { get; set; }
      public string GroupName { get; set; }
      public double Percent { get; set; }
   }



	/// <summary>
	/// 活跃用户
	/// </summary>
	public class ResActiveUser
	{
		public long UserId { get; set; }
		public string RealName { get; set; }
		public string PhotoPath { get; set; }
		public long GenderPKID { get; set; }
		public int ViewCount { get; set; }
		public string FitPhotoPath
		{
			get
			{
				if (PhotoPath == "")
					return "/assets/img/gender_" + GenderPKID + ".jpg";
				return PhotoPath;
			}
		}

		public static string FitPhotoPathWithUser(ResUser user)
		{
			if (user.PhotoPath == "")
				return "/assets/img/gender_" + user.GenderPKID + ".jpg";
			return user.PhotoPath;
		}


		/// <summary>
		/// 公告
		/// </summary>
		public class BulletionRecommand
		{
			public string title { get; set; }
			public string Content { get; set; }
			public string CreatedTime { get; set; }
		}


	}

   public class ImportModel
   {
      public bool IsSuccess { get; set; }
      public string FailReason { get; set; }
   }

   /// <summary>
   /// 用户导入模型
   /// </summary>
   public class UserImportModel: ImportModel
   {
      public string UserName { get; set; }
      public string RealName { get; set; }
      public string UserType { get; set; }
      public string Province { get; set; }
      public string Area { get; set; }
      public string Company { get; set; }
   }


   /// <summary>
   /// 单位导入模型
   /// </summary>
   public class CompanyImportModel: ImportModel
   {
      public string CompanyName { get; set; }
      public string Address { get; set; }
      public string Province { get; set; }
      public string Area { get; set; }
      public string Phone { get; set; }
   }

   public class EvalExportViewModel
   {
      public CroResource Resource { get; set; }
      public List<Indication> Indications { get; set; }
      public string EvalComment { get; set; }
      public string[] Experts { get; set; }
   }

   public class ExportAllScoreViewModel
   {
      public long Id { get; set; }
      public string ResourceName { get; set; }
      public string Author { get; set; }
      public string AuthorCompany { get; set; }
      public double Score1 { get; set; }
      public double Score2 { get; set; }
      public double Score3 { get; set; }
      public double Score4 { get; set; }
      public double Score { get; set; }
      public string Comment { get; set; }
      public bool IsEval { get; set; }
      public string GroupName { get; set; }
   }

   public class ExportAllViewModel
   {
      public List<ExportAllScoreViewModel> ScoreModels { get; set; }
      public List<Indication> IndicationModels { get; set; }
      public string GroupName { get; set; }
   }

}