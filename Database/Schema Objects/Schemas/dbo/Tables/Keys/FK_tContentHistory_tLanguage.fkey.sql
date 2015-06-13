ALTER TABLE [dbo].[ContentHistory]
    ADD CONSTRAINT [FK_tContentHistory_tLanguage] FOREIGN KEY ([language_id]) REFERENCES [dbo].[Language] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

