DBCC CHECKIDENT ([common.Language], RESEED, 2)
go

INSERT INTO [common].[Language] VALUES('es','Español','DEF','ACT',GETDATE(),GETDATE(), NULL)
go
INSERT INTO [common].[Language] VALUES('fr','Française','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('de','Deutsch','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('it','Italiano','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('af','Africâner','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('jp','Japonês','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('sa','Árabe','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('cn-sim','Chinês-Sim','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('ru','Russo','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('hi','Hindi','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('kr','Coreano','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO
INSERT INTO [common].[Language] VALUES('cn-tra','Chinês-Tra','DEF','ACT',GETDATE(),GETDATE(), NULL)
GO

