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

   public static class FileHelper
   {

      /// <summary>
      /// Convert stream to MD5 string 
      /// </summary>
      /// <param name="stream">stream</param>
      /// <returns>md5 string</returns>
      public static string ConvertToMD5(Stream stream)
      {
         if (stream == null)
            throw new ArgumentNullException("stream cannot be null");

         using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
         {
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
         }
      }

      public static string HtmlExtName=".HTML";

   }

}