using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Wangsu.WcsLib.Core;
using Wangsu.WcsLib.Utility;

namespace Util.ThirdParty.WangsuCloud
{

   public class FileUploader
   {

      public const string ACCESS_KEY = "57b89577283edae1d74e261b7b637f7055e29c08";
      public const string SECURITY_KEY = "a5834d0469180f9c30d4b761aac90975cfc83d10";
      public const string UPLOAD_DOMAIN = "tejiao.up28.v1.wcsapi.com";
      public const string MANAGER_DOMAIN = "tejiao.mgr28.v1.wcsapi.com";
      public const string SCOPE = "csj-zyk";
      public const string DEADLINE = "1546300800000";
      public const string DOMAIN = "cdncsj.sser.shdjg.net";

      public static SimpleUpload SimpleUploader
      {
         get
         {
            if (_simpleUploader == null)
            {
               _simpleUploader = new SimpleUpload(
               new Mac(ACCESS_KEY, SECURITY_KEY),
               new Config(UPLOAD_DOMAIN, MANAGER_DOMAIN)
               );
            }

            return _simpleUploader;
         }
      }


      /// <summary>
      /// 上传文件，通过流的方式上传
      /// </summary>
      /// <param name="file"></param>
      /// <param name="isOverwirte"></param>
      /// <returns></returns>
      public static UploadResult Upload(UploadFile file, bool isOverwirte = true)
      {
         if (null == file && null == file.Stream)
            throw new ArgumentNullException();

         var fileName = file.FileName;
         var result = new UploadResult();
         var policy = new PutPolicy { scope = SCOPE, deadline = DEADLINE, overwrite = isOverwirte ? "1" : "0" };
         var policyJson = JsonConvert.SerializeObject(policy);

         try
         {
            file.Stream.Seek(0, SeekOrigin.Begin); //需要明确的长度

            SimpleUploader.UploadStream(file.Stream, policyJson, fileName);

            result.IsSuccess = true;
            result.FileUrl = $"http://{FileUploader.DOMAIN}/{fileName}";
         }
         catch (Exception e)
         {
            result.IsSuccess = false;
            result.Message = $"exception:{e.Message}";
         }

         return result;
      }

      private static SimpleUpload _simpleUploader;

   }

   public static class Certificate
   {

      public static string ToBase64hmac(this string strText, string strKey, Encoding encode = null)
      {
         encode = encode ?? Encoding.UTF8;
         HMACSHA1 myHMACSHA1 = new HMACSHA1(encode.GetBytes(strKey));
         byte[] byteText = myHMACSHA1.ComputeHash(encode.GetBytes(strText));

         return Convert.ToBase64String(byteText);
      }

      public static string ToBase64(this string source, Encoding encode = null)
      {
         encode = encode ?? Encoding.UTF8;
         byte[] bytes = encode.GetBytes(source);
         var str = string.Empty; ;

         try
         {
            str = Convert.ToBase64String(bytes);
         }
         catch
         {
            str = source;
         }

         return str;
      }

   }

   public class UploadFile
   {
      public string FileName { get; set; }
      public Stream Stream { get; set; }
   }

   public class UploadResult
   {
      public bool IsSuccess { get; set; }
      public string Message { get; set; }
      public string FileUrl { get; set; }
   }

   public class PutPolicy
   {
      public string scope { get; set; }
      public string deadline { get; set; }
      public string saveKey { get; set; }
      public string overwrite { get; set; }
   }

}
