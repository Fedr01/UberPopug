IF OBJECT_ID(N'[Outbox]') IS NULL
BEGIN
    CREATE TABLE [Outbox] (
        [Id] int NOT NULL IDENTITY,
        [Topic] nvarchar(max) NULL,
        [Body] nvarchar(max) NULL,
        CONSTRAINT [PK_Outbox] PRIMARY KEY ([Id])
);
END;


