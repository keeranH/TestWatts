ALTER TABLE [dbo].[Page]
    ADD CONSTRAINT [FK_tPage_tSection] FOREIGN KEY ([section_id]) REFERENCES [dbo].[Section] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

