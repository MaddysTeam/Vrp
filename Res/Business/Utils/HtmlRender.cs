using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Res.Business
{

   public class HtmlRender
   {

      public string RenderViewToString(Controller controller,string viewName,object viewData)
      {
         var builder = new StringBuilder();
         using (var responseWriter=new StringWriter(builder))
         {
            var fakeResponse = new HttpResponse(responseWriter);
            var fakeContext = new HttpContext(HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(new HttpContextWrapper(fakeContext), controller.ControllerContext.RouteData, controller.ControllerContext.Controller);

            //var oldContext = HttpContext.Current;
            //HttpContext.Current = fakeContext;

            using (var viewPage = new ViewPage())
            {
               var html = new HtmlHelper(CreateViewContext(responseWriter, fakeControllerContext), viewPage);
               html.RenderPartial(viewName, viewData);
              // HttpContext.Current = oldContext;
            }
         }

         return builder.ToString();
      }


      public static ViewContext CreateViewContext(TextWriter writer,ControllerContext controllerContext)
      {
         return new ViewContext(controllerContext, new TempView(), new ViewDataDictionary(), new TempDataDictionary(), writer);
      }
          
   }


   public class TempView : IView
   {

      public void Render(ViewContext viewContext, TextWriter writer)
      {
         throw new NotImplementedException();
      }

   }

}