CREATE TABLE [dbo].[PageTemplateHistory] (
    [id]           INT  IDENTITY (1, 1) NOT NULL,
    [page_id]      INT  NOT NULL,
    [template_id]  INT  NOT NULL,
    [history_date] DATE NOT NULL
);

