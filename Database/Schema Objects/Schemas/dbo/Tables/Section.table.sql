CREATE TABLE [dbo].[Section] (
    [id]        INT            IDENTITY (1, 1) NOT NULL,
    [name]      NVARCHAR (100) NOT NULL,
    [parent_id] INT            NULL
);

