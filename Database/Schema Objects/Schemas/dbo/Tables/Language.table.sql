CREATE TABLE [dbo].[Language] (
    [id]      INT           IDENTITY (1, 1) NOT NULL,
    [name]    NVARCHAR (50) NOT NULL,
    [code]    NCHAR (3)     NOT NULL,
    [culture] NVARCHAR (10) NOT NULL
);

