CREATE TABLE [dbo].[Page] (
    [id]                      INT            IDENTITY (1, 1) NOT NULL,
    [template_id]             INT            NOT NULL,
    [section_id]              INT            NOT NULL,
    [relative_path]           NVARCHAR (200) NOT NULL,
    [enabled]                 BIT            NOT NULL,
    [unpublished_template_id] INT            NULL
);

