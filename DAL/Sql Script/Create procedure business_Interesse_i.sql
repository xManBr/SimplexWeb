/****** Object:  StoredProcedure [business].[Interesse_i]    Script Date: 04/09/2017 15:38:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


Create procedure [business].[Interesse_i]
(
@IN_Interesse nvarchar(255) = null
)
as

insert into business.Interesse ([Interesse], [CreationDate]) values(@IN_Interesse, getdate())

GO


