ALTER TABLE [dbo].[ContentHistory]
    ADD CONSTRAINT [FK_tContentHistory_tPage] FOREIGN KEY ([page_id]) REFERENCES [dbo].[Page] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

