//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using ToolGood.Words;

//namespace Res.Business
//{
//   public class SentisiveWordHelper
//   {
//      private static List<SensitiveWords> _all;
//      static List<SensitiveWords> All
//      {
//         get
//         {
//            if (_all == null)
//               _all = SensitiveWords.GetAll();
//            return _all;
//         }
//      }

//      private static Dictionary<string, List<string>> _verbNouns; //动词-名字集合 键值对
//      static Dictionary<string, List<string>> VerbNouns
//      {
//         get
//         {
//            if (_verbNouns == null)
//               _verbNouns = BuildKeyGroups();
//            return _verbNouns;
//         }
//      }


//      public static string Replace(string text, char splitChar)
//      {
//         //过滤专属敏感词
//         var _search = new WordsSearch();
//         var originalBadWords = GetOrginalBadWords();
//         _search.SetKeywords(originalBadWords);
//         text = _search.Replace(text, splitChar);

//         //查找动词
//         var keyWords = GetVerbs();
//         _search.SetKeywords(keyWords);
//         var searchResults = _search.FindAll(text);

//         var matchedFirstVerbs = new List<string>();
//         //只获取第一次出现过的动词，如再出现则忽略
//         foreach (var item in searchResults)
//         {
//            if (!matchedFirstVerbs.Contains(item.Keyword))
//            {
//               matchedFirstVerbs.Add(item.Keyword);
//               var nouns = VerbNouns[item.Keyword];
//               _search.SetKeywords(nouns);
//               text = _search.Replace(text, splitChar);
//            }
//         }

//         return text;
//      }


//      private static Dictionary<string, List<string>> BuildKeyGroups()
//      {
//         var results = new Dictionary<string, List<string>>();

//         foreach (var item in All)
//         {
//            if (item.Type == "动词")
//            {
//               var categories = All.Where(k => k.Word == item.Word).Select(x => x.Category);
//               foreach (var category in categories)
//               {
//                  var nouns = All
//                     .Where(k => k.Category == category && k.Type == "名词")
//                     .Select(x => x.Word)
//                     .ToList();
//                  if (results.ContainsKey(item.Word))
//                     results[item.Word].AddRange(nouns);
//                  else
//                     results.Add(item.Word, nouns);
//               }

//            }

//         }

//         return results;
//      }


//      private static List<string> GetVerbs()
//      {
//         if (VerbNouns == null || VerbNouns.Count <= 0)
//            return null;

//         return VerbNouns.Select(k => k.Key).ToList();
//      }


//      private static List<string> GetOrginalBadWords()
//      {
//         if (All == null || All.Count <= 0 || !All.Any(x => x.Type == "专属词语"))
//            return null;

//         return All.Where(x => x.Type == "专属词语").Select(k => k.Word).ToList();
//      }

//   }
//}