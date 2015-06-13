ALTER TABLE [dbo].[PageTemplateHistory]
    ADD CONSTRAINT [FK_tPageTemplateHistory_tTemplate] FOREIGN KEY ([template_id]) REFERENCES [dbo].[Template] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

