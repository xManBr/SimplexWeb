use MercoplanoProd
go

select * from business.interesse
--where interesse <> '#entrada'
order by id desc


select * from business.interesse
where interesse <> '#entrada'
order by id desc

select url, count(*) from business.interesse
group by url
order by count(*) desc



--update common.translation set LabelName = 'Qual o seu interesse?' where TranslationId = 527 and languageId = 1


--update [common].[TranslationDate] set [LastModifiedDate] = getdate()

select top 10 * from common.translation
order by TranslationId desc

delete from common.translation where TranslationId = 530



select top 5 * from  common.Translation
order by TranslationId desc

  SELECT top 2

'INSERT INTO common.Translation VALUES( ' +  ltrim(rtrim(CONVERT(NCHAR(20),TranslationId))) +  ',' + ltrim(rtrim(CONVERT(NCHAR(20),LanguageId))) +
  ',N''' + IsNull(LabelName,'') + ''',' +ltrim(rtrim(CONVERT(NCHAR(20),IsNull(LengthNumber,'')))) + ',''' + IsNull(DescriptionName,'') + ''',GETDATE()' + ',''' + IsNull(ObjectTypeId,'') + ''',''' + IsNull(StatusId,'') + ''',GETDATE() )' 
 FROM common.Translation (noLock)


 INSERT INTO common.Translation VALUES( 531,1,N'Preço',0,'',GETDATE(),'DEF','ACT',GETDATE() )
 INSERT INTO common.Translation VALUES( 531,2,N'Price',0,'',GETDATE(),'DEF','ACT',GETDATE() )

