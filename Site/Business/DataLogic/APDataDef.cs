﻿using Res.Business;
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

      public int FavoriteCount { get; set; }
      public int DownCount { get; set; }
      public int CommentCount { get; set; }


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


      public string Stage { get { return CroResourceHelper.Stage.GetName(StagePKID); } }

      public string Grade { get { return CroResourceHelper.Grade.GetName(GradePKID); } }

      public string ResourceType { get { return CroResourceHelper.ResourceType.GetName(ResourceTypePKID); } }

      public string Subject { get { return CroResourceHelper.Subject.GetName(SubjectPKID); } }

      public string State { get { return CroResourceHelper.State.GetName(StatePKID); } }

      public string Province { get { return GetCompanyName(ProvinceId); } }

      public string Area { get { return GetCompanyName(AreaId); } }

      public string School { get { return GetCompanyName(CompanyId); } }

      [Display(Name = "资源路径")]
      [Required]
      public string GhostFileName { get; set; }

      public List<MicroCourse> Courses { get; set; }

      #endregion


      private string GetCompanyName(long companyId)
      {
         var company = ResSettings.SettingsInSession.Companies.Find(x => x.CompanyId == companyId);
         return company != null ? company.CompanyName : string.Empty;
      }

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
      public string FitCoverPath
      {
         get
         {
            if (string.IsNullOrEmpty(CoverPath))
               return "/assets/img/cover.png";
            return CoverPath;
         }
      }
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

}