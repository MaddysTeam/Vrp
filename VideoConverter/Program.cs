using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace VideoConverter
{

   class Program
   {

      static void Main(string[] args)
      {
         var videos = new List<string>();
         string path;

         var fs = File.OpenRead("d:\\temp\\temp.txt");
         StreamReader sr = new StreamReader(fs);

         string str = "";
         int i = 0;
         while ((str = sr.ReadLine()) != null)
         {
            Console.WriteLine(str);
            videos.Add(str);
            i++;
         }

         foreach (var video in videos)
         {
            var file = video.Substring(video.LastIndexOf("/")+1);
            //path = Path.Combine(FileUrl, video);
            DownloadAsync("http://cdncsj.sser.shdjg.net/"+video, "d:\\temp\\"+file, (o, e) =>
             {
                //var token = e.UserState as UserToken;
                //var file = Convert(token.File);
                //UploadToCDN(file);
             });
         }



         // Convert("aa.mp4", "temp.mp4");

         Console.Read();
      }

      static void DownloadAsync(string url, string file, AsyncCompletedEventHandler handler)
      {
         var client = new WebClient();
         client.DownloadFileCompleted += handler;
         client.DownloadFileAsync(new Uri(url), file, new UserToken { File = file });
      }

      static string Convert(string input, string output)
      {
         string ffmpeg = AppDomain.CurrentDomain.BaseDirectory + @"ffmpeg.exe";
         string convertCommand = $"-i {input} -vcodec h264  {output}";

         StartProcess(ffmpeg, convertCommand);

         return string.Empty;
      }

      static bool UploadToCDN(string file)
      {
         return true;
      }

      private static void StartProcess(string fileName, string args)
      {
         Process process = Process.Start(new ProcessStartInfo
         {
            FileName = fileName,

            Arguments = args,

            UseShellExecute = false,

            RedirectStandardInput = false,

            RedirectStandardOutput = false,

            CreateNoWindow = true,

            RedirectStandardError = true,
         });

         process.ErrorDataReceived += Process_ErrorDataReceived;

         process.BeginErrorReadLine();

         process.WaitForExit();

         process.Close();
      }

      private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
      {
         Console.WriteLine(e.Data);
      }

      private static string FileUrl => "http://cdncsj.sser.shdjg.net/2018/videos/20181126/"; //2018/files/" + DateTime.Today.ToString("yyyyMMdd") + "/";


   }

   /// <summary>
   ///  将文件名称传入异步方法DownloadFileAsync 中,触发DownloadFileCompleted时使用
   /// </summary>
   class UserToken
   {
      public string File { get; set; }
   }

}
