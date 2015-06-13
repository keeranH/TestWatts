ALTER TABLE [dbo].[UnpublishedContent]
    ADD CONSTRAINT [FK_tUnpublishedContent_tLanguage] FOREIGN KEY ([language_id]) REFERENCES [dbo].[Language] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

