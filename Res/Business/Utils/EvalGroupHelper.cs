using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

   public static class EvalGroupHelper
   {
      public static PickListAPRptColumn Level;

      static EvalGroupHelper()
      {
         Level = new PickListAPRptColumn(APDBDef.EvalGroup.LevelPKID, ThisApp.PLKey_Level);
      }

   }

}