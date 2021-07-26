--USE [MercoplanoProd]
--GO

USE SIMPLEX
GO

  SELECT

'INSERT INTO common.Translation VALUES( ' +  ltrim(rtrim(CONVERT(NCHAR(20),TranslationId))) +  ',' + ltrim(rtrim(CONVERT(NCHAR(20),LanguageId))) +
  ',N''' + IsNull(LabelName,'') + ''',' +ltrim(rtrim(CONVERT(NCHAR(20),IsNull(LengthNumber,'')))) + ',''' + IsNull(DescriptionName,'') + ''',GETDATE()' + ',''' + IsNull(ObjectTypeId,'') + ''',''' + IsNull(StatusId,'') + ''',GETDATE() )' 
 FROM common.Translation (noLock)




