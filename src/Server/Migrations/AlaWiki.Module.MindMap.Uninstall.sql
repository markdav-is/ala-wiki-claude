-- AlaWiki.Module.MindMap Uninstall Script

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AlaWikiMindMap_WikiConnection]') AND type in (N'U'))
BEGIN
    DROP TABLE [dbo].[AlaWikiMindMap_WikiConnection]
END
GO
