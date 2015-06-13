ALTER TABLE [dbo].[PageTemplateHistory]
    ADD CONSTRAINT [FK_tPageTemplateHistory_tPage] FOREIGN KEY ([page_id]) REFERENCES [dbo].[Page] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

