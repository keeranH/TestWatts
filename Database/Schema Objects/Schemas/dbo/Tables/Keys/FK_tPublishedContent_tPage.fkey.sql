ALTER TABLE [dbo].[PublishedContent]
    ADD CONSTRAINT [FK_tPublishedContent_tPage] FOREIGN KEY ([page_id]) REFERENCES [dbo].[Page] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

