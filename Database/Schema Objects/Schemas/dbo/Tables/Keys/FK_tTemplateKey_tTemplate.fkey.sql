ALTER TABLE [dbo].[TemplateKey]
    ADD CONSTRAINT [FK_tTemplateKey_tTemplate] FOREIGN KEY ([template_id]) REFERENCES [dbo].[Template] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

