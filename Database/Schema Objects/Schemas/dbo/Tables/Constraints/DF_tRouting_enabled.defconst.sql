ALTER TABLE [dbo].[Routing]
    ADD CONSTRAINT [DF_tRouting_enabled] DEFAULT ((0)) FOR [enabled];

