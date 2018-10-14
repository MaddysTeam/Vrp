using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.NPOI
{

   public class ExecelOperator
   {

        public ExecelOperator() { }

       /// <summary>
       /// 文件流初始化对象
       /// </summary>
       /// <param name="stream"></param>
       public ExecelOperator(Stream stream) 
       {
           _IWorkbook = CreateWorkbook(stream);
       }

       /// <summary>
       /// 传入文件名
       /// </summary>
       /// <param name="fileName"></param>
       public ExecelOperator(string fileName) 
       {
           using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
           {
               _IWorkbook = CreateWorkbook(fileStream);
           }
       }

       /// <summary>
       /// 工作薄
       /// </summary>
       private IWorkbook _IWorkbook;

       /// <summary>
       /// 创建工作簿对象
       /// </summary>
       /// <param name="stream"></param>
       /// <returns></returns>
       private IWorkbook CreateWorkbook(Stream stream) 
       {
           try 
           {
              return new XSSFWorkbook(stream); //07
           }
           catch
           {
              return new HSSFWorkbook(stream); //03
           }
          
       }

      /// <summary>
      /// 导出到List
      /// </summary>
      /// <typeparam name="T">类型</typeparam>
      /// <param name="fields">字段数组</param>
      /// <returns>List集合</returns>
      public List<T> ExcelToList<T>(string[] fields) where T : class, new()
      {
         return ExportToList<T>(_IWorkbook.GetSheetAt(0), fields);
      }


      private List<T> ExportToList<T>(ISheet sheet, string[] fields) where T : new()
      {
         List<T> list = new List<T>();

         for (int i = sheet.FirstRowNum + 1, len = sheet.LastRowNum + 1; i < len; i++)
         {
            T t = new T();
            IRow row = sheet.GetRow(i);
            try
            {
               for (int j = 0, len2 = fields.Length; j < len2; j++)
               {
                  ICell cell = row.GetCell(j);
                  object cellValue = null;

                  switch (cell.CellType)
                  {
                     case CellType.String:
                        cellValue = cell.StringCellValue;
                        break;
                     case CellType.Numeric:
                        cellValue =cell.NumericCellValue.ToString();//Double转换为stirng
                        break;
                     case CellType.Boolean:
                        cellValue = cell.BooleanCellValue;
                        break;
                     case CellType.Blank:
                        cellValue = "";
                        break;
                     default:
                        cellValue = "ERROR";
                        break;
                  }

                  typeof(T).GetProperty(fields[j]).SetValue(t, cellValue, null);
               }

               typeof(T).GetProperty("IsSuccess").SetValue(t, true, null);
               list.Add(t);
            }
            catch(Exception e)
            {
               typeof(T).GetProperty("FailReason").SetValue(t, e.Message, null);
               typeof(T).GetProperty("IsSuccess").SetValue(t, false, null);
               list.Add(t);
            }
         }

         return list;
      }

   }

}
