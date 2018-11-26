using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoConverter
{

   class Program
   {

      static void Main(string[] args)
      {
         string[] videos = { "", "" };
         string path;
         foreach (var video in videos)
         {
            path =Path.Combine(FileUrl, video);
            DownloadAsync(path, video , (o, e) => {
               var token = e.UserState as UserToken;
               var file=Convert(token.File);
               UploadToCDN(file);
            });
         }

         Console.Read();
      }

      static void DownloadAsync(string url, string file, AsyncCompletedEventHandler handler)
      {
         var client = new WebClient();
         client.DownloadFileCompleted += handler;
         client.DownloadFileAsync(new Uri(url), file, new UserToken { File=file });
      }

      static string Convert(string file)
      {
         return string.Empty;
      }

      static bool UploadToCDN(string file)
      {
         return true;
      }


      private static string FileUrl => "2018/files/" + DateTime.Today.ToString("yyyyMMdd")+"/";

   }

   /// <summary>
   ///  将文件名称传入异步方法DownloadFileAsync 中,触发DownloadFileCompleted时使用
   /// </summary>
   class UserToken
   {
      public string File { get; set; }
   }

}
