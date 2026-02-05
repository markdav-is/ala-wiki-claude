-- AlaWiki Module Uninstall Script

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlaWiki_WikiConnection]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[AlaWiki_WikiConnection]
END
GO
