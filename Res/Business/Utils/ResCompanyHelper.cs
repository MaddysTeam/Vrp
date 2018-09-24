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

      static List<ResCompany> _all;
      public static List<ResCompany> All
      {
         get
         {
            if (_all == null)
               _all = APBplDef.ResCompanyBpl.GetAll();
            return _all;
         }
      }

      public static List<ResCompany> GetChildren(long parentId)
      {
         if (All == null) return null;
         return All.FindAll(x => x.ParentId == parentId);
      }

      public static List<ResCompany> GetChildren(List<ResCompany> parents)
      {
         if (parents == null) parents = new List<ResCompany>();
         var children = new List<ResCompany>();
         foreach(var item in parents)
         {
            children.AddRange(All.FindAll(x=>x.ParentId==item.CompanyId));
         }

         return children;
      }

      public static List<ResCompany> AllProvince()
      {
         return All.FindAll(x => x.ParentId == 0);
      }

      public static List<ResCompany> GetSchools()
      {
         return All.FindAll(x => x.Path.LastIndexOf(@"\") > 9);
      }

      public static List<ResCompany> GetAreas()
      {
         return All.FindAll(x => x.Path.Length == 10);
      }

   }

}