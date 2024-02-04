USE [SyndykDB]
GO

CREATE TABLE [dbo].[Advertisements] (
    [ID]          INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [Title]       NVARCHAR (MAX)  NULL,
    [Link]        NVARCHAR (MAX)  NULL,
    [Price]       DECIMAL (19, 2) NULL,
    [Description] NVARCHAR (MAX)  NULL
);


