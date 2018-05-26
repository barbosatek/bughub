CREATE TABLE [dbo].[Bugs]
(
  [Id] BIGINT NOT NULL PRIMARY KEY IDENTITY (1,1),
  [Title] NVARCHAR(1024) NOT NULL,
  [Description] NVARCHAR(MAX) NOT NULL,
  [CreationDate] DATETIME2 NOT NULL,
  [LastModificationDate] DATETIME2 NOT NULL
)
GO;
