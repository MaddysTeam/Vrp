using Symber.Web.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public static class CroResourceHelper
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

      static CroResourceHelper()
		{
			//Domain = new PickListAPRptColumn(APDBDef.CroResource.DomainPKID, ThisApp.PLKey_ResourceDomain);
			//Deformity = new PickListAPRptColumn(APDBDef.CroResource.DeformityPKID, ThisApp.PLKey_ResourceDeformity);
			//LearnFrom = new PickListAPRptColumn(APDBDef.CroResource.LearnFromPKID, ThisApp.PLKey_ResourceLearnFrom);
			//SchoolType = new PickListAPRptColumn(APDBDef.CroResource.SchoolTypePKID, ThisApp.PLKey_ResourceSchoolType);
			Stage = new PickListAPRptColumn(APDBDef.CroResource.StagePKID, ThisApp.PLKey_ResourceStage);
			Grade = new PickListAPRptColumn(APDBDef.CroResource.GradePKID, ThisApp.PLKey_ResourceGrade);
			//ImportSource = new PickListAPRptColumn(APDBDef.CroResource.ImportSourcePKID, ThisApp.PLKey_ResourceImportSource);
			//MediumType = new PickListAPRptColumn(APDBDef.CroResource.MediumTypePKID, ThisApp.PLKey_ResourceMedium);
			ResourceType = new PickListAPRptColumn(APDBDef.CroResource.ResourceTypePKID, ThisApp.PLKey_ResourceType);
			Subject = new PickListAPRptColumn(APDBDef.CroResource.SubjectPKID, ThisApp.PLKey_ResourceSubject);
			State = new PickListAPRptColumn(APDBDef.CroResource.StatePKID, ThisApp.PLKey_ResourceState);
         CourseType = new PickListAPRptColumn(APDBDef.CroResource.CourseTypePKID, ThisApp.PLKey_CourseType);
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