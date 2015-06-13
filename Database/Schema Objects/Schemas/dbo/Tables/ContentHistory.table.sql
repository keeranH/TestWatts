CREATE TABLE [dbo].[ContentHistory] (
    [id]           INT   IDENTITY (1, 1) NOT NULL,
    [page_id]      INT   NOT NULL,
    [key_id]       INT   NOT NULL,
    [language_id]  INT   NOT NULL,
    [value]        NTEXT NULL,
    [history_date] DATE  NOT NULL
);

