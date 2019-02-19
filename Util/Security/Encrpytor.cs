using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Util.Security
{

   public sealed class Encrpytor
   {

      public const string KEY = "#Bugfor1";

      /// <summary>
      /// 进行DES加密。
      /// </summary>
      /// <param name="pToEncrypt">要加密的字符串。</param>
      /// <param name="sKey">密钥，且必须为8位。</param>
      /// <returns>以Base64格式返回的加密字符串。</returns>
      public static string DESEncrypt(string key, string val)
      {
         using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
         {
            byte[] inputByteArray = Encoding.UTF8.GetBytes(val);
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();


            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
            {
               cs.Write(inputByteArray, 0, inputByteArray.Length);
               cs.FlushFinalBlock();
               cs.Close();
            }
            string str = Convert.ToBase64String(ms.ToArray());
            ms.Close();
            return str;
         }
      }


      /// <summary>
      ///  DES Decrypt
      /// </summary>
      /// <param name="pToDecrypt">要解密的以Base64</param>
      /// <param name="key">密钥，且必须为8位。</param>
      /// <returns>已解密的字符串。</returns>
      public static string DESDecrypt(string key, string val)
      {
         try {
            byte[] inputByteArray = Convert.FromBase64String(val);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
               des.Key = ASCIIEncoding.ASCII.GetBytes(key);
               des.IV = ASCIIEncoding.ASCII.GetBytes(key);
               MemoryStream ms = new MemoryStream();
               using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
               {
                  cs.Write(inputByteArray, 0, inputByteArray.Length);
                  cs.FlushFinalBlock();
                  cs.Close();
               }
               string str = Encoding.UTF8.GetString(ms.ToArray());
               ms.Close();
               return str;
            }
         }
         catch {
            return string.Empty;
         }

      }
   }

}