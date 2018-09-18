using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

   public static class IndicationHelper
   {
      public static PickListAPRptColumn Level;
      public static PickListAPRptColumn Type;

      static IndicationHelper()
      {
         Level = new PickListAPRptColumn(APDBDef.Indication.LevelPKID, ThisApp.PLKey_Level);
         Type = new PickListAPRptColumn(APDBDef.Indication.TypePKID,ThisApp.PLKey_IndicationType);
      }

   }

}