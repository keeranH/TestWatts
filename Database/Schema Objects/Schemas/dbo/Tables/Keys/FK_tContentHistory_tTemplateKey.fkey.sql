ALTER TABLE [dbo].[ContentHistory]
    ADD CONSTRAINT [FK_tContentHistory_tTemplateKey] FOREIGN KEY ([key_id]) REFERENCES [dbo].[TemplateKey] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

