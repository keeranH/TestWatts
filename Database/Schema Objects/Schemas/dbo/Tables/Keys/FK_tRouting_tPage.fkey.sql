ALTER TABLE [dbo].[Routing]
    ADD CONSTRAINT [FK_tRouting_tPage] FOREIGN KEY ([page_id]) REFERENCES [dbo].[Page] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

