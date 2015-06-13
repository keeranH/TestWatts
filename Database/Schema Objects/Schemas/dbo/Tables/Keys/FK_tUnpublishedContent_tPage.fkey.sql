ALTER TABLE [dbo].[UnpublishedContent]
    ADD CONSTRAINT [FK_tUnpublishedContent_tPage] FOREIGN KEY ([page_id]) REFERENCES [dbo].[Page] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

