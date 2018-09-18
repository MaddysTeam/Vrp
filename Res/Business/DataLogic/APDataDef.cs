using Res.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Res.Business
{

	#region [ ResPickList ]


	public partial class ResPickList : ResPickListBase
	{

		#region [ Constructors ]


		public ResPickList(long pickListId, string innerKey, string name, string description)
			: base(pickListId, innerKey, name, false, false, description, 0, DateTime.MinValue, 0, DateTime.MinValue)
		{
		}


		#endregion

	}


	#endregion


	#region [ ResPickListItem ]


	public partial class ResPickListItem : ResPickListItemBase
	{

		#region [ Constructors ]


		public ResPickListItem(string name)
			: base(0, 0, name, 0, "", false, 0, DateTime.MinValue, 0, DateTime.MinValue)
		{
		}


		public ResPickListItem(string name, bool isDefault)
			: base(0, 0, name, 0, "", isDefault, 0, DateTime.MinValue, 0, DateTime.MinValue)
		{
		}


		public ResPickListItem(string name, long strengthenValue)
			: base(0, 0, name, strengthenValue, "", false, 0, DateTime.MinValue, 0, DateTime.MinValue)
		{
		}


		public ResPickListItem(string name, long strengthenValue, bool isDefault)
			: base(0, 0, name, strengthenValue, "", isDefault, 0, DateTime.MinValue, 0, DateTime.MinValue)
		{
		}


		#endregion

	}


	#endregion


	#region [ ResUser ]


	public partial class ResUser : ResUserBase
	{

		#region [ Properties ]


		public string Gender { get { return ResUserHelper.Gender.GetName(GenderPKID); } }


		public string CompanyName { get; set; }
		public string RoleName { get; set; }

		public int FavoriteCount { get; set; }
		public int DownCount { get; set; }
		public int CommentCount { get; set; }


		#endregion


	}


	#endregion


	#region [ ResReal ]


	public partial class ResReal : ResRealBase
	{

		#region [ Properties ]


		public string CompanyName { get; set; }


		#endregion


	}


	#endregion


	#region [ ResCompany ]


	public partial class ResCompany : ResCompanyBase
	{

		#region [ Properties ]

		
		public List<ResCompany> Children { get; set; }


		#endregion
	
	}


	#endregion


	#region [ ResReportSummary ]


	public class ResReportSummary
	{

		public int TotalResource { get; set; }
		public long TotalResourceSize { get; set; }
		public int CreateThisWeek { get; set; }
		public int CreateThisMonth { get; set; }
		public int CreateThisYear { get; set; }

		public int TotalUser { get; set; }
		public int TotalComment { get; set; }
		public int TotalView { get; set; }
		public int TotalDownload { get; set; }
		public int TotalFavorite { get; set; }
		public int TotalStar { get; set; }

	}




   #endregion


   #region [ CroResource ]


   public partial class CroResource : CroResourceBase
   {

      #region [ Properties ]


      //Dictionary<string, String> mapping;
      //TODO:will delete later

      //public string Deformity { get { return ResResourceHelper.Deformity.GetName(DeformityPKID); } }

      //public string Domain { get { return ResResourceHelper.Domain.GetName(DomainPKID); } }

      //public string LearnFrom { get { return ResResourceHelper.LearnFrom.GetName(LearnFromPKID); } }

      //public string SchoolType { get { return ResResourceHelper.SchoolType.GetName(SchoolTypePKID); } }

      public string Stage { get { return CroResourceHelper.Stage.GetName(StagePKID); } }

      public string Grade { get { return CroResourceHelper.Grade.GetName(GradePKID); } }

      //public string ImportSource { get { return ResResourceHelper.ImportSource.GetName(ImportSourcePKID); } }

      //public string MediumType { get { return ResResourceHelper.MediumType.GetName(MediumTypePKID); } }

      public string ResourceType { get { return CroResourceHelper.ResourceType.GetName(ResourceTypePKID); } }

      public string Subject { get { return CroResourceHelper.Subject.GetName(SubjectPKID); } }

      public string State { get { return CroResourceHelper.State.GetName(StatePKID); } }

      [Display(Name = "资源路径")]
      [Required]
      public string GhostFileName { get; set; }

      // public string FileType { get { if (!String.IsNullOrEmpty(FileExtName)) return FileExtName.Substring(1); return ""; } }

      // TODO: delete later public int Star { get { if (StarCount == 0)return 0; return StarTotal / StarCount; } }

      public List<MicroCourse> Courses { get; set; }

      #endregion


   }


   #endregion


   #region [MicroCourse]

   public partial class MicroCourse : MicroCourseBase
   {
      public string VideoName { get; set; }
      public string VideoPath { get; set; }
      public string CoverName { get; set; }
      public string DesignName { get; set; }
      public string SummaryName { get; set; }
      public string CoverPath { get; set; }
      public string DesignPath { get; set; }
      public string SummaryPath { get; set; }
      public List<Exercises> Exercises { get; set; }

   }

   #endregion


   #region [Exercises]

   public partial class Exercises : ExercisesBase
   {
      public List<ExercisesItem> Items { get; set; }
   }

   #endregion


   #region [EvalGroup]


   public partial class EvalGroup : EvalGroupBase
   {
      public string Level { get { return EvalGroupHelper.Level.GetName(LevelPKID); } }

      public string ActiveName { get; set; }
   }

   #endregion


   #region [ CroBulletin ]


   public partial class CroBulletin : CroBulletinBase
	{

		#region [ Properties ]


		[Display(Name = "文件路径")]
		[Required]
		public string GhostFileName { get; set; }


		#endregion


	}


	#endregion


	#region [ ResBulletin ]


	//public partial class ResBulletin : ResBulletinBase
	//{

	//	#region [ Properties ]


	//	[Display(Name = "文件路径")]
	//	[Required]
	//	public string GhostFileName { get; set; }


	//	#endregion


	//}


	#endregion


	#region [ ZSResource]

	public partial class ZSResource : ZSResourceBase
	{

		#region [ Properties ]


		public string Deformity { get { return ResResourceHelper.Deformity.GetName(DeformityPKID); } }

		public string Domain { get { return ResResourceHelper.Domain.GetName(DomainPKID); } }

		public string LearnFrom { get { return ResResourceHelper.LearnFrom.GetName(LearnFromPKID); } }

		public string SchoolType { get { return ResResourceHelper.SchoolType.GetName(SchoolTypePKID); } }

		public string Stage { get { return ResResourceHelper.Stage.GetName(StagePKID); } }

		public string Grade { get { return ResResourceHelper.Grade.GetName(GradePKID); } }

		public string ImportSource { get { return ResResourceHelper.ImportSource.GetName(ImportSourcePKID); } }

		public string MediumType { get { return ResResourceHelper.MediumType.GetName(MediumTypePKID); } }

		public string ResourceType { get { return ResResourceHelper.ResourceType.GetName(ResourceTypePKID); } }

		public string Subject { get { return ResResourceHelper.Subject.GetName(SubjectPKID); } }

		public string State { get { return ResResourceHelper.State.GetName(StatePKID); } }

		[Display(Name = "资源路径")]
		[Required]
		public string GhostFileName { get; set; }

		public string FileType { get { if (FileExtName != null) return FileExtName.Substring(1); return ""; } }


		#endregion


	}

	#endregion




}