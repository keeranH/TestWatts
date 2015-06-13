ALTER TABLE [dbo].[PublishedContent]
    ADD CONSTRAINT [FK_tPublishedContent_tTemplateKey] FOREIGN KEY ([key_id]) REFERENCES [dbo].[TemplateKey] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

