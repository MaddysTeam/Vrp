using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

   public static class ActiveHelper
   {

      public static PickListAPRptColumn Level;

      static ActiveHelper()
      {
         Level = new PickListAPRptColumn(APDBDef.Active.LevelPKID, ThisApp.PLKey_Level);
      }

      /// <summary>
      /// 获取当前活动id
      /// </summary>
      /// <returns></returns>
      public static Active GetCurrentActive => APBplDef.ActiveBpl.GetAll().OrderByDescending(x => x.ActiveId).First();

   }

}