using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Res;
using Symber.Web.Data;
using Res.Business;
using Res.Business.Utils;

namespace Res.Controllers
{

   public class DesignerController : BaseController
   {

      public ActionResult Design()
      {
         var filePath = Server.MapPath("~/Attachments/second.htm");
         MentalConverter.ConverHtmlToImage(filePath, @"D:\temp","myImage");

         return View();  
      }

   }

}