ALTER TABLE [dbo].[Routing]
    ADD CONSTRAINT [FK_tRouting_tLanguage] FOREIGN KEY ([language_id]) REFERENCES [dbo].[Language] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

