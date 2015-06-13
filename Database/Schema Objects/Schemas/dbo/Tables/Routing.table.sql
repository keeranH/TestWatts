CREATE TABLE [dbo].[Routing] (
    [id]               INT            IDENTITY (1, 1) NOT NULL,
    [page_id]          INT            NOT NULL,
    [language_id]      INT            NOT NULL,
    [route_value]      NVARCHAR (200) NOT NULL,
    [translated_value] NVARCHAR (200) NOT NULL,
    [enabled]          BIT            NOT NULL
);

