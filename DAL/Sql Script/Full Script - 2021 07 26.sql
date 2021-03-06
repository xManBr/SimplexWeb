USE [Simplex]
GO
/****** Object:  Schema [business]    Script Date: 26/07/2021 14:28:16 ******/
CREATE SCHEMA [business]
GO
/****** Object:  Schema [common]    Script Date: 26/07/2021 14:28:16 ******/
CREATE SCHEMA [common]
GO
/****** Object:  Table [business].[Interesse]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [business].[Interesse](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Interesse] [nvarchar](255) NULL,
	[CreationDate] [datetime] NOT NULL,
	[url] [nvarchar](255) NULL,
	[urlanterior] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [common].[Language]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [common].[Language](
	[LanguageId] [smallint] IDENTITY(1,1) NOT NULL,
	[LanguageCode] [nvarchar](10) NOT NULL,
	[LanguageName] [nvarchar](255) NOT NULL,
	[ObjectTypeId] [nvarchar](5) NOT NULL,
	[StatusId] [nvarchar](5) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[LastModifiedDate] [datetime] NOT NULL DEFAULT (getdate()),
	[TranslationId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [common].[Translation]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [common].[Translation](
	[TranslationId] [int] NOT NULL,
	[LanguageId] [smallint] NOT NULL,
	[LabelName] [nvarchar](255) NOT NULL,
	[LengthNumber] [smallint] NULL,
	[DescriptionName] [nvarchar](255) NULL,
	[CreationDate] [datetime] NOT NULL,
	[ObjectTypeId] [nvarchar](5) NOT NULL,
	[StatusId] [nvarchar](5) NOT NULL,
	[LastModifiedDate] [datetime] NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED 
(
	[TranslationId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [common].[TranslationDate]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [common].[TranslationDate](
	[LastModifiedDate] [datetime] NOT NULL
) ON [PRIMARY]

GO
ALTER TABLE [common].[Translation]  WITH CHECK ADD FOREIGN KEY([LanguageId])
REFERENCES [common].[Language] ([LanguageId])
GO
ALTER TABLE [common].[Translation]  WITH CHECK ADD FOREIGN KEY([LanguageId])
REFERENCES [common].[Language] ([LanguageId])
GO
/****** Object:  StoredProcedure [business].[Interesse_i]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [business].[Interesse_i]
(
@IN_Interesse nvarchar(255) = null,
@IN_ur nvarchar(255) = null,
@IN_urlanterior nvarchar(255) = null
)
as

insert into business.Interesse ([Interesse], [CreationDate], url, urlanterior) values(@IN_Interesse, getdate(), @IN_ur, @IN_urlanterior)



GO
/****** Object:  StoredProcedure [business].[p_Interesse_s]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [business].[p_Interesse_s]
as

SELECT distinct top 100 [Id]
      ,[Interesse]
      ,[CreationDate]
  FROM [business].[Interesse]
  where interesse <> '#entrada' and interesse is not null and interesse <> ''
  order by id desc




GO
/****** Object:  StoredProcedure [common].[p_Language_s]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



/*

SELECT * FROM [common].[Language]

*/

CREATE procedure [common].[p_Language_s]
(
@IN_LanguageId smallint = null
)
as

SELECT [LanguageId]
      ,[LanguageCode]
      ,[LanguageName]
      ,[CreationDate]
      ,[ObjectTypeId]
      ,[StatusId]
      ,[LastModifiedDate]
  FROM [common].[Language] (noLock)
  where (@IN_LanguageId = 0 or @IN_LanguageId is null or  (LanguageId = @IN_LanguageId ) )
  and [StatusId] = 'ACT'








GO
/****** Object:  StoredProcedure [common].[p_Translation_Ajust]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




/*

Author: Laerte A do Nascimento
Date: 2015/12/08
Subject: Inserir registro inicial na tabela de traduções
OBS: O ajuste no nome, onde se coloca os espaços deve ser feito manualmente 

*/

CREATE procedure [common].[p_Translation_Ajust]
(
@IN_LabelName nvarchar(255)
)
as

declare @IN_TranslationIdx int
if not exists (select 1 from [common].[Translation] (NoLock) where Upper(Replace(LabelName,' ','')) = Upper(Replace(@IN_LabelName,' ','')) )
begin

set @IN_TranslationIdx = (select IsNull(MAX(TranslationId),0) +1 from common.Translation)

INSERT INTO [common].[Translation]
           ([TranslationId]
           ,[LanguageId]
           ,[LabelName]
           ,[LengthNumber]
           ,[DescriptionName]
           ,[CreationDate]
           ,[ObjectTypeId]
           ,[StatusId]
           ,[LastModifiedDate])
     VALUES
           (
		    @IN_TranslationIdx
           ,2
           ,@IN_LabelName
           ,null
           ,null
           ,getdate()
           ,'DEF'
           ,'ACT'
           ,GETDATE()
		   )
end





GO
/****** Object:  StoredProcedure [common].[p_Translation_s]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE procedure [common].[p_Translation_s]
(
@IN_TranslationId int = null
)

as

SELECT t.[TranslationId]
      ,t.[LanguageId]
      ,t.[LabelName]
      ,t.[LengthNumber]
      ,t.[DescriptionName]
      ,t.[CreationDate]
      ,t.[ObjectTypeId]
      ,t.[StatusId]
      ,t.[LastModifiedDate]
      ,l.LanguageCode
	  ,'' as LabelCode
  FROM common.[Translation] t (noLock)
  inner join common.Language l (NoLock)
  on t.LanguageId = l.LanguageId
where ( @IN_TranslationId is null or @IN_TranslationId = 0 or t.[TranslationId] = @IN_TranslationId )











GO
/****** Object:  StoredProcedure [common].[p_TranslationDate_s]    Script Date: 26/07/2021 14:28:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




/*
Author: Laerte do Nascimento
Date: 2014/04/29
Subject: Recuperar a data da última atualização da tradução no banco de dados
*/

CREATE procedure [common].[p_TranslationDate_s]
as
SELECT top 1 Convert(datetime,[LastModifiedDate],121) as LastModifiedDate
  FROM [common].[TranslationDate]







GO
