-- AlaWiki Module Database Migration v1.0.0

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlaWiki_WikiConnection]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AlaWiki_WikiConnection] (
        [WikiConnectionId] INT IDENTITY(1,1) NOT NULL,
        [ModuleId] INT NOT NULL,
        [Name] NVARCHAR(500) NOT NULL,
        [GitUrl] NVARCHAR(1000) NOT NULL,
        [DefaultBranch] NVARCHAR(100) NULL DEFAULT 'wikiMaster',
        [PersonalAccessToken] NVARCHAR(500) NULL,
        [LocalPath] NVARCHAR(1000) NULL,
        [LastSyncedOn] DATETIME NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedBy] NVARCHAR(256) NULL,
        [CreatedOn] DATETIME NULL,
        [ModifiedBy] NVARCHAR(256) NULL,
        [ModifiedOn] DATETIME NULL,
        CONSTRAINT [PK_AlaWiki_WikiConnection] PRIMARY KEY CLUSTERED ([WikiConnectionId] ASC)
    )
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AlaWiki_WikiConnection_ModuleId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AlaWiki_WikiConnection_ModuleId]
    ON [dbo].[AlaWiki_WikiConnection] ([ModuleId] ASC)
END
GO
