ALTER TABLE [dbo].[UnpublishedContent]
    ADD CONSTRAINT [FK_tUnpublishedContent_tTemplateKey] FOREIGN KEY ([key_id]) REFERENCES [dbo].[TemplateKey] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

