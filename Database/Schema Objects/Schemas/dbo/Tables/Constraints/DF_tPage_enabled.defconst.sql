ALTER TABLE [dbo].[Page]
    ADD CONSTRAINT [DF_tPage_enabled] DEFAULT ((0)) FOR [enabled];

