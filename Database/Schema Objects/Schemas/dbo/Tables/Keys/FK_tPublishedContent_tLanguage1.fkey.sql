ALTER TABLE [dbo].[PublishedContent]
    ADD CONSTRAINT [FK_tPublishedContent_tLanguage1] FOREIGN KEY ([language_id]) REFERENCES [dbo].[Language] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

