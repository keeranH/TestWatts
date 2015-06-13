ALTER TABLE [dbo].[Section]
    ADD CONSTRAINT [FK_tSection_tSection] FOREIGN KEY ([parent_id]) REFERENCES [dbo].[Section] ([id]) ON DELETE NO ACTION ON UPDATE NO ACTION;

