DBCC CHECKIDENT ([common.Language], RESEED, 2)
go

INSERT INTO [common].[Language] VALUES('es','Espa�ol','DEF','ACT',GETDATE(),GETDATE(), NULL)
go
INSERT INTO [common].[Language] VALUES('fr','Fran�aise','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('de','Deutsch','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('it','Italiano','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('af','Afric�ner','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('jp','Japon�s','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('sa','�rabe','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('cn-sim','Chin�s-Sim','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('ru','Russo','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('hi','Hindi','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('kr','Coreano','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('cn-tra','Chin�s-Tra','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO

