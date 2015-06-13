ALTER TABLE [dbo].[Page]
    ADD CONSTRAINT [FK_tPage_tTemplate] FOREIGN KEY ([template_id]) REFERENCES [dbo].[Template] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

