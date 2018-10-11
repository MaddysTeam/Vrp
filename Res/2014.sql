
	-- 插入course
	delete from MicroCourse
	insert into [dbo].[MicroCourse]
	(
	 [ResourceId]
	,[CourseTitle]
	,[VideoId]
	,StarCount
	,CoverId
	,SummaryId
	,DesignId
	,DownCount
	,PlayCount
	,CoursewareId
	,AttachmentId
	,CharpterSortId
	,class
	)
	select f.VideoId,cr.title,f.FileId,0,0,0,0,0,0,0,0,0,f.class  from files f
	join CroResource cr on cr.CrosourceId=f.VideoId  where f.Code='video'

	-- update microcouse
	update mc set mc.VideoId=f.FileId 
	 select *
	 from [MicroCourse] mc join [Files] f on mc.class=f.class and f.Code='fm'

	 update mc set mc.coverId=f2.fileId
	 from MicroCourse mc join (
	 select f.FileId, f.FileName,f.class as class from Files f where f.Code='jxfs' group by f.FileId, f.FileName, f.class
	 ) f2 on mc.class=f2.class

	  update mc set mc.SummaryId=f2.fileId
	 from MicroCourse mc join (
	 select f.FileId, f.FileName,f.class as class from Files f where f.Code='jxfs' group by f.FileId, f.FileName, f.class
	 ) f2 on mc.class=f2.class

 
	  update mc set mc.CoursewareId=f2.fileId
	 from MicroCourse mc join (
	 select f.FileId, f.FileName,f.class as class from Files f where f.Code='kejian' group by f.FileId, f.FileName, f.class
	 ) f2 on mc.class=f2.class

	  update mc set mc.AttachmentId=f2.fileId
	 from MicroCourse mc join (
	 select f.FileId, f.FileName,f.class as class from Files f where f.Code='jxotherfj' group by f.FileId, f.FileName, f.class
	 ) f2 on mc.class=f2.class

	   update mc set mc.DesignId=f2.fileId
	 from MicroCourse mc join (
	 select f.FileId, f.FileName,f.class as class from Files f where f.Code='jxsj' group by f.FileId, f.FileName, f.class
	 ) f2 on mc.class=f2.class


	-- 插入files
	delete from [dbo].[Files]
	INSERT INTO [dbo].[Files]
			   (
				[FileName]
			   ,[ExtName]
			   ,[FilePath]
			   ,[FileSize]
			   ,[Md5]
			   ,[VideoId]
			   ,[Code]
			   ,[Class])
		SELECT 
		  REPLACE(REPLACE([ file_name],'''',''),' ','')
		  ,replace(REPLACE([ file_format],'''',''),' ','')
		  ,case replace(REPLACE([ transed],'''',''),' ','') when '1' then  replace(REPLACE([ flv_name],'''',''),' ','')  else  Replace(REPLACE( [ save_file_name],'''',''),' ','') end
		  ,cast (REPLACE([ file_length],'''','') as bigint)
		  ,''
		  ,cast(REPLACE([ video_id],'''','') as bigint)
		  ,replace(REPLACE([ code],'''',''),' ','')
		  ,cast (REPLACE([ class_id],'''','')as bigint)
	  FROM [vrp].[dbo].[t_a] 


    -- 更新filepath
    update Files set FilePath= 'http://cdncsj.sser.shdjg.net/2014/videos/videos/'+FilePath 

	-- 更新Resource
    update [dbo].[CroResource] set DownloadStatePKID=10453 where DownloadStatePKID=4014
	update [dbo].[CroResource] set PublicStatePKID=10451 where PublicStatePKID=4009
	update [dbo].[CroResource] set PublicStatePKID=10452 where PublicStatePKID=4010
	update [dbo].[CroResource] set StatePKID=10352 where StatePKID=4006
	update [dbo].[CroResource] set StatePKID=10353 where StatePKID=4007

	-- 同步状态
	update e set e.StatePKID= cast(REPLACE(REPLACE(t.[ audit_status],'''',''),' ','') as bigint)
	 from CroResource e
	join [dbo].[t_video2] t on e.CrosourceId = REPLACE(REPLACE(t.[video_id],'''',''),' ','')

	-- 公开
	update e set e.PublicStatePKID= cast(REPLACE(REPLACE(t.[ public_scope],'''',''),' ','') as bigint)
	 from CroResource e
	join [dbo].[t_video2] t on e.CrosourceId = REPLACE(REPLACE(t.[video_id],'''',''),' ','')
	
	-- 审核状态
	select [ audit_status] from t_video2 group by [ audit_status]
	update t_video2 set [ audit_status] = '4007' where  [ audit_status] like '%NULL%'

	-- 作品类型修改
   update e set e.CourseTypePKID=5010
   from CroResource e join
     (select  ResourceId as resourceId,count(ResourceId) c from MicroCourse group by ResourceId) e2
      on e2.resourceId=e.CrosourceId
      where e2.c=1

	  select DownloadStatePKID from CroResource
	--更新地区
	update e set e.Areaid=c.ParentId
	from [dbo].[CroResource] e join
	ResCompany c on e.CompanyId =c.CompanyId 

	--更新省份
    update e set e.ProvinceId=c.ParentId
	from [dbo].[CroResource] e join
	ResCompany c on e.AreaId =c.CompanyId 

	--更新获奖等级
	 update e set e.WinLevelPKID= case e.WinLevelPKID when 1 then 205  when 2 then 206 when 3 then 207 else 0 end
	from [dbo].[CroResource] e

	update CroResource set PublicStatePKID=10450 where  PublicStatePKID=10451
	update CroResource set DownloadStatePKID=10452 where PublicStatePKID=10452

	-- 更新项目默认为2014年
    update CroResource set ActiveId=1

