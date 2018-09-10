using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Res.Business
{

	public partial class APDalDef
	{

		/// <summary>
		/// Initialize ResPickList and ResPickListItem
		/// </summary>
		public partial class ResPickListDal : ResPickListDalBase
		{

			public override void InitData(APDBDef db)
			{
				long key, lessthen;


				#region [ 1000 < 1010 : PLKey_Gender ]

				{
					key = 1000; lessthen = 1010;

					var pk = new ResPickList(key, ThisApp.PLKey_Gender, "性别", "对性别进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] { "男", "女" },
						null,
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}


				#endregion

				#region [ 10000 < 10030 : PLKey_ResourceDomain ]

				{
					key = 10000; lessthen = 10030;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceDomain, "资源领域", "对资源领域进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] { "政策与文献", "诊断与评估", "课程与教学", "康复与干预", "支持与服务" },
						null,
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}


				#endregion

				#region [ 10030 < 10070 : PLKey_ResourceDeformity ]

				{
					key = 10030; lessthen = 10070;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceDeformity, "残疾类型", "对残疾类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"视力残疾", "听力残疾", "智力残疾", "肢体残疾（脑瘫）", "肢体残疾（非脑瘫）",
							"言语残疾", "精神残疾", "精神残疾（自闭症）", "多重残疾", "其他残疾"
						},
						null,
						null);
					items[items.Count - 1].PickListItemId = lessthen - 1;

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10070 < 10090 : PLKey_ResourceLearnFrom ]

				{
					key = 10070; lessthen = 10090;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceLearnFrom, "安置类型", "对安置类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"特殊学校", "特教班", "送教上门", "随班就读", "其他"
						},
						null,
						null);
					items[items.Count - 1].PickListItemId = lessthen - 1;

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10090 < 10110 : PLKey_ResourceSchoolType ]

				{
					key = 10090; lessthen = 10110;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceSchoolType, "学校类型", "对学校类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"聋校", "幼儿园", "辅读学校", "普通学校", "盲校",
							"职业技术学校", "大学"
						},
						null,
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10110 < 10120 : PLKey_ResourceStage ]

				{
					key = 10110; lessthen = 10120;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceStage, "学段", "对学段进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"学前教育", "义务教育", "高中教育", "初职教育", "中职教育", "高等教育",
							"终身教育"
						},
						null,
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10120 < 10200 : PLKey_ResourceGrade ]

				{
					key = 10120; lessthen = 10200;
					long stage;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceGrade, "年级", "对年级进行选择的字典项。");
					List<ResPickListItem> items = new List<ResPickListItem>();

					key = 10121; stage = 10112;
					AddItemToList(items, stage, key, new string[] { "一年级", "二年级", "三年级", "四年级", "五年级", "六年级", "七年级", "八年级", "九年级", "低年级", "中年级", "高年级", "小学段", "初中段" }, null, null);
					key = 10141; stage = 10113;
					AddItemToList(items, stage, key, new string[] { "高一", "高二", "高三", "高四" }, null, null);
					key = 10151; stage = 10114;
					AddItemToList(items, stage, key, new string[] { "初职一", "初职二", "初职三", "初职四" }, null, null);
					key = 10161; stage = 10115;
					AddItemToList(items, stage, key, new string[] { "中职一", "中职二", "中职三", "中职四" }, null, null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10200 < 10210 : PLKey_ResourceImportSource ]

				{
					key = 10200; lessthen = 10210;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceImportSource, "来源类型", "对来源类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"系统导入", "用户上传"
						},
						null,
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10210 < 10230 : PLKey_ResourceMedium ]

				{
					key = 10210; lessthen = 10230;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceMedium, "媒体类型", "对媒体类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"文本(含OFFICE文档)", "图形（图像）", "视频", "音频", "动画", "综合多种媒体"
						},
						new string[] {
							".txt,.doc,.docx,.xls,.xlsx,.ppt,.pptx,.rtf,.log,.pdf",
							".jpg,.jpeg,.png,.gif,.bmp",
							".mpg,.mpeg,.avi,.wmv,.asf,.rm,.rmvb,.flv,.mp4",
							".mid,.wma,.asx,.mp3,.wav",
							".swf",
							".exe,.zip"
						},
						null);

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10230 < 10270 : PLKey_ResourceType ]

				{
					key = 10230; lessthen = 10270;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceType, "资源类型", "对资源类型进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"政策", "评估工具与方法", "评估报告", "课程方案与标准", "教学设计",
							"教学课件", "教学实录", "个别化教育计划", "康复训练设计", "康复训练课件",
							"康复训练实录", "残疾人支持服务项目介绍", "评估视频", "教学案例", "教学资源包",
							"校本教材", "辅助器具介绍", "教具学具介绍", "支持与服务个案报告", "文献",
							"康复训练案例", "个别化康复训练计划", "家庭教育"
						},
						null,
						null);
					long[] strengths = new long[]{
						10001, 10002, 10002, 10003, 10003,
						10003, 10003, 10003, 10004, 10004,
						10004, 10005, 10002, 10003, 10003,
						10003, 10005, 10005, 10004, 10001,
						10004, 10004, 10005
					};

					for (var i = 0; i < strengths.Length; i++)
					{
						items[i].StrengthenValue = strengths[i];
					}

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10270 < 10350 : PLKey_ResourceSubject ]

				{
					key = 10270; lessthen = 10350;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceSubject, "学科", "对学科进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"生活自理", "语文", "数学", "英语", "科学",
							"生活", "品德与社会", "社会", "信息技术", "艺术",
							"劳动技术", "职业技术", "体育与健康", "言语沟通训练", "感知运动训练",
							"行为训练", "定向行走训练", "综合康复", "拓展", "综合实践活动",
							"学前特教", "物理", "精细动作训练", "认知", "按摩职业教育",
							"个训或小组训练","其他学科"
						},
						null,
						null);
					items[items.Count - 1].PickListItemId = lessthen - 1;

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion

				#region [ 10350 < 10360 : PLKey_ResourceState ]

				{
					key = 10350; lessthen = 10360;

					var pk = new ResPickList(key, ThisApp.PLKey_ResourceState, "状态", "对状态进行选择的字典项。");
					var items = FromArray(
						0,
						new string[] {
							"未审核", "审核合格", "审核不合格", "已删除"
						},
						null,
						null);
					items[items.Count - 1].PickListItemId = lessthen - 1;

					SyncInitData(db, ThisApp.AppId, pk, items);
				}

				#endregion







			}

			#region [ Helper methods ]


			private void AddItemToList(List<ResPickListItem> list, long strengthenValue, long baseId, string[] items, string[] code, string defaultItem)
			{
				for (int i = 0, len = items.Length; i < len; i++)
				{
					ResPickListItem item = new ResPickListItem(items[i], strengthenValue) { PickListItemId = baseId++ };
					if (code != null)
						item.Code = code[i];
					if (item.Name == defaultItem)
						item.IsDefault = true;
					list.Add(item);
				}
			}


			private List<ResPickListItem> FromArray(long strengthenValue, string[] itemNames, string[] codes, string defaultItem)
			{
				List<ResPickListItem> items = new List<ResPickListItem>();
				for (int i = 0, len = itemNames.Length; i < len; i++)
				{
					ResPickListItem item = new ResPickListItem(itemNames[i], strengthenValue);
					if (codes != null)
						item.Code = codes[i];
					if (item.Name == defaultItem)
						item.IsDefault = true;
					items.Add(item);
				}

				return items;
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, string[] itemNames)
			{
				List<ResPickListItem> items = FromArray(0/* strengthValue */, itemNames, null/* codes */, null/* defaultItem */);

				SyncInitData(db, apResAppID, data, items);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, string[] itemNames, string[] codes)
			{
				List<ResPickListItem> items = FromArray(0/* strengthValue */, itemNames, codes, null/* defaultItem */);

				SyncInitData(db, apResAppID, data, items);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, string[] itemNames, string defaultItem)
			{
				List<ResPickListItem> items = FromArray(0/* strengthValue */, itemNames, null/* codes */, defaultItem);

				SyncInitData(db, apResAppID, data, items);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, List<ResPickListItem> items)
			{
				SyncInitData(db, apResAppID, data, items, 0);
			}


			private void SyncInitData(APDBDef db, long apResAppID, ResPickList data, List<ResPickListItem> items, int baseInc)
			{
				List<ResPickList> res = db.ResPickListDal.ConditionQuery(APDBDef.ResPickList.InnerKey == data.InnerKey, null, 1, null);
				if (res.Count == 0)
				{
					DateTime now = DateTime.Now;

					if (data.PickListId == 0)
						throw new Exception("Has not special PickListId！This is a Obvious Mistake");
					data.CreatedTime = data.LastModifiedTime = now;
					data.Creator = data.LastModifier = ThisApp.AppUser_Designer_Id;
					db.ResPickListDal.Insert(data);

					SyncItems(db, data.PickListId, baseInc, now, items);
				}
			}


			private void SyncItems(APDBDef db, long pickListId, int baseInc, DateTime now, List<ResPickListItem> items)
			{
				long baseId = pickListId + 1 + baseInc;
				foreach (ResPickListItem item in items)
				{
					if (item.PickListItemId == 0)
						item.PickListItemId = baseId++; //db.ResObjectCurrentIdDal.GetNewCoreAppObjId();
					item.PickListId = pickListId;
					item.CreatedTime = item.LastModifiedTime = now;
					item.Creator = item.LastModifier = ThisApp.AppUser_Designer_Id;
					db.ResPickListItemDal.Insert(item);
				}
			}


			#endregion

		}

	}

}