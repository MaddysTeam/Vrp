using Symber.Web.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Res.Business
{

   public static class ResCompanyHelper
   {

      public static long ShangHai = 1161;

      public static List<ResCompany> GetChildren(List<ResCompany> all,long parentId)
      {
         if (all == null) return null;
         return all.FindAll(x => x.ParentId == parentId);
      }

      public static List<ResCompany> GetChildren( List<ResCompany> all,List<ResCompany> parents)
      {
         if (parents == null) parents = new List<ResCompany>();
         var children = new List<ResCompany>();
         foreach(var item in parents)
         {
            children.AddRange(all.FindAll(x=>x.ParentId==item.CompanyId));
         }

         return children;
      }


      public static string GetCompanyName(long companyId)
      {
         var company = ResSettings.SettingsInSession.Companies.Find(x => x.CompanyId == companyId);
         return company != null ? company.CompanyName : string.Empty;
      }

   }

}