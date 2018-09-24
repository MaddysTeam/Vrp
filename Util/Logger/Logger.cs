//using Camoran.GodLog;
//using Camoran.GodLog.Configuration;
//using Camoran.GodLog.DB.Writer;
//using System;
//using System.Collections.Generic;

//namespace Business.Log
//{

//   public class LogFactory 
//   {

//      static string currentLogType = "Business.CamoranAdoLogger";// will get this type from config

//      public static Logger Create<Logger>() where Logger : class
//      {
//         return Activator.CreateInstance<Logger>();
//      }


//      public static ILogger Create()
//      {
//         var logger= Activator.CreateInstance(Type.GetType(currentLogType)) as ILogger;

//         if (logger == null)
//         {
//            throw new ApplicationException("logger create failure");
//         }

//         return logger;
//      }

//   }


//   public interface ILogger
//   {

//      void Log(string log);

//      void Log(LogModel logModel);

//   }


//   public class CamoranAdoLogger : ILogger
//   {
//      string sqlConnectString = "Data Source=10.4.4.22,2933;Initial Catalog=shserc;uid=sa; pwd=sa";
//      string[] patters = new string[] { CamoranLoggerPatterns.Who, CamoranLoggerPatterns.When, CamoranLoggerPatterns.Where, CamoranLoggerPatterns.DoSomthing };
//      string _insertSql;
//      List<ADODBParams> _paras;

//      public CamoranAdoLogger()
//      {
//         _insertSql = string.Format("insert into Logmodel values ({0},{1},{2},{3})",
//            patters[0], patters[1], patters[2], patters[3]);

//         _paras= new List<ADODBParams>()
//               {
//                  new ADODBParams { ParmasName = patters[0], DefaultValue = DBNull.Value, ParamsDbType = System.Data.DbType.String },
//                  new ADODBParams { ParmasName = patters[1], DefaultValue = DateTime.Now, ParamsDbType = System.Data.DbType.DateTime },
//                  new ADODBParams { ParmasName = patters[2], DefaultValue = DBNull.Value,  ParamsDbType = System.Data.DbType.String },
//                  new ADODBParams { ParmasName = patters[3], DefaultValue = DBNull.Value, ParamsDbType = System.Data.DbType.String },
//               };
//      }


//      public void Log(string log)
//      {
//         LogManager
//          .Instance
//          .SetConfiguration(new ADONetDbConfiguraton(sqlConnectString, _insertSql, _paras))
//          .AddPatterns(patters)
//          .GetLogger<AdoNetLogger>()
//          .Info(log);
//      }


//      public void Log(LogModel logModel)
//      {
//         LogManager
//            .Instance
//            .SetConfiguration(new ADONetDbConfiguraton(sqlConnectString, _insertSql, _paras))
//            .AddPatterns(patters)
//            .GetLogger<AdoNetLogger>()
//            .Log(logModel);
//      }


//      class CamoranLoggerPatterns
//      {
//         public static string Who="@Who";
//         public static string When = "@When";
//         public static string Where = "@Where";
//         public static string DoSomthing = "@DoSomthing";
//      }

//   }

//}
