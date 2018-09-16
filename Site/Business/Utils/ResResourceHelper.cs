using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static class ResResourceHelper
	{
		public static PickListAPRptColumn Domain;
		public static PickListAPRptColumn Deformity;
		public static PickListAPRptColumn LearnFrom;
		public static PickListAPRptColumn SchoolType;
		public static PickListAPRptColumn Stage;
		public static PickListAPRptColumn Grade;
		public static PickListAPRptColumn ImportSource;
		public static PickListAPRptColumn MediumType;
		public static PickListAPRptColumn ResourceType;
		public static PickListAPRptColumn Subject;
		public static PickListAPRptColumn State;
      public static PickListAPRptColumn CourseType;

		static ResResourceHelper()
		{
			//Domain = new PickListAPRptColumn(APDBDef.ResResource.DomainPKID, ThisApp.PLKey_ResourceDomain);
			//Deformity = new PickListAPRptColumn(APDBDef.ResResource.DeformityPKID, ThisApp.PLKey_ResourceDeformity);
			//LearnFrom = new PickListAPRptColumn(APDBDef.ResResource.LearnFromPKID, ThisApp.PLKey_ResourceLearnFrom);
			//SchoolType = new PickListAPRptColumn(APDBDef.ResResource.SchoolTypePKID, ThisApp.PLKey_ResourceSchoolType);
			//Stage = new PickListAPRptColumn(APDBDef.ResResource.StagePKID, ThisApp.PLKey_ResourceStage);
			//Grade = new PickListAPRptColumn(APDBDef.ResResource.GradePKID, ThisApp.PLKey_ResourceGrade);
			//ImportSource = new PickListAPRptColumn(APDBDef.ResResource.ImportSourcePKID, ThisApp.PLKey_ResourceImportSource);
			//MediumType = new PickListAPRptColumn(APDBDef.ResResource.MediumTypePKID, ThisApp.PLKey_ResourceMedium);
			//ResourceType = new PickListAPRptColumn(APDBDef.ResResource.ResourceTypePKID, ThisApp.PLKey_ResourceType);
			//Subject = new PickListAPRptColumn(APDBDef.ResResource.SubjectPKID, ThisApp.PLKey_ResourceSubject);
			//State = new PickListAPRptColumn(APDBDef.ResResource.StatePKID, ThisApp.PLKey_ResourceState);
        // CourseType = new PickListAPRptColumn(APDBDef.ResResource.cour, ThisApp.PLKey_CourseType);
      }


		// 资源状态
		public static long StateWait = 10351;
		public static long StateAllow = 10352;
		public static long StateDeny = 10353;
		public static long StateDelete = 10359;

		// 资源来源类型
		public static long SourceImport = 10201;
		public static long SourceUpload = 10202;

		// 资源类型
		public static long MediumText = 10211;
		public static long MediumImage = 10212;
		public static long MediumVideo = 10213;
		public static long MediumAudio = 10214;
		public static long MediumAnimation = 10215;
		public static long MediumMix = 10216;

		// 小学年级
		public static long Grade1 = 10121;
		public static long Grade2 = 10122;
		public static long Grade3 = 10123;
		public static long Grade4 = 10124;
		public static long Grade5 = 10125;
		public static long Grade6 = 10126;
		public static long Grade7 = 10127;
		public static long Grade8 = 10128;
		public static long Grade9 = 10129;

		public static long GradeLow = 10130;
		public static long GradeMiddle = 10131;
		public static long GradeHigh = 10132;

		public static long GradePrimary = 10133;
		public static long GradeJunior = 10134;


		private static Dictionary<string, long> dictMediumType;
		public static long GetMediumType(string ext)
		{
			if (dictMediumType == null)
			{
				dictMediumType = new Dictionary<string, long>(StringComparer.CurrentCultureIgnoreCase);
				foreach (var item in MediumType.GetItems())
				{
					foreach (var s in item.Code.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
					{
						dictMediumType[s] = item.PickListItemId;
					}
				}
			}

			if (dictMediumType.ContainsKey(ext))
				return dictMediumType[ext];
			return 10216;
		}
	}

}