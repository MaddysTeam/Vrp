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
      public int Star { get { if (StarCount == 0) return 0; return StarTotal / StarCount; } }
      public string FitCoverPath
      {
         get
         {
            if (string.IsNullOrEmpty(CoverPath))
               return "/assets/img/cover.png";
            return CoverPath;
         }
      }
   }

   /// <summary>
   /// 作品排行榜
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
      public string FitCoverPath
      {
         get
         {
            if (string.IsNullOrEmpty(CoverPath))
               return "/assets/img/cover.png";
            return CoverPath;
         }
      }
   }


   //我的作品

   public class CroMyResource
   {
      public long CrosourceId { get; set; }
      public string Active { get; set; }
      public string Title { get; set; }
      public string Author { get; set; }
      public string CoverPath { get; set; }
      public string FileExtName { get; set; }
      public string Description { get; set; }
      public DateTime OccurTime { get; set; }
      public long StatePKID { get; set; }
      public long PublicStatePKID { get; set; }
      public long DownloadStatePKID { get; set; }
      public string PublicState => PublicStatePKID == CroResourceHelper.Public ? "取消公开" : "点击公开";
      public string DownloadState => DownloadStatePKID == CroResourceHelper.AllowDownload ? "禁止下载" : "允许下载";

      public long ProvinceId { get; set; }
      public long AreaId { get; set; }
      public long CompanyId { get; set; }
      public string Province { get { return ResCompanyHelper.GetCompanyName(ProvinceId); } }
      public string Area { get { return ResCompanyHelper.GetCompanyName(AreaId); } }
      public string Company { get { return ResCompanyHelper.GetCompanyName(CompanyId); } }

      public long OccurId { get; set; }
      public string Content { get; set; }

      public long Audittype { get; set; }
      public long Auditor { get; set; }
      public DateTime AuditedTime { get; set; }
      public string AuditOpinion { get; set; }
      public string FitCoverPath
      {
         get
         {
            if (string.IsNullOrEmpty(CoverPath))
               return "/assets/img/cover.png";
            return CoverPath;
         }
      }
   }




   /// <summary>
   /// 微课程评价
   /// </summary>
   public class MicroCoursecommend
   {
      public long CourseId { get; set; } // 微课程id
      public long CrosourceId { get; set; } // 作品id
      public string ResourceTitle { get; set; } // 作品名称
      public string Title { get; set; }
      public string Author { get; set; }
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

      public long ProvinceId { get; set; }
      public long AreaId { get; set; }
      public long SchoolId { get; set; }
      public string Province { get { return ResCompanyHelper.GetCompanyName(ProvinceId); } }
      public string Area { get { return ResCompanyHelper.GetCompanyName(AreaId); } }
      public string School { get { return ResCompanyHelper.GetCompanyName(SchoolId); } }
   }




   public class MicroCourseRanking : MicroCoursecommend
   {
      public string AuthorCompany { get; set; }
      public DateTime CreatedTime { get; set; }
      public int PlayCount { get; set; }
      public int FavoriteCount { get; set; }
      public string CompanyPath { get; set; }
      public int ViewCount { get; set; }
      public int CommentCount { get; set; }
      public int DownCount { get; set; }
      public string FileExtName { get; set; }
      public string Description { get; set; }
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
      /// 作品获奖级别
      /// </summary>
      public class CroResourceLevel
      {
         public long LevelId { get; set; }
         public string Name { get; set; }
         public bool IsSelect { get; set; }
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

}