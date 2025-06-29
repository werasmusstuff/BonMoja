IF DB_ID('EventLoggerDB') IS NULL
BEGIN
    CREATE DATABASE EventLoggerDB;
END;
GO

USE EventLoggerDB;
GO

-- Create EventMetadata table
IF OBJECT_ID('dbo.EventMetadata', 'U') IS NULL
    CREATE TABLE dbo.EventMetadata (
      EventId UNIQUEIDENTIFIER NOT NULL,
      UserId NVARCHAR(100) NOT NULL,
      EventType NVARCHAR(100) NOT NULL,
      Timestamp DATETIME2 NOT NULL,
      CONSTRAINT PK_EventMetadata_EventId PRIMARY KEY NONCLUSTERED (EventId)
    );
GO

-- Create clustered index on UserId, Timestamp, EventId
IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_EventMetadata_UserId_Timestamp_EventId' AND object_id = OBJECT_ID('dbo.EventMetadata'))
BEGIN
  CREATE CLUSTERED INDEX 
    IX_EventMetadata_UserId_Timestamp_EventId
  ON 
    dbo.EventMetadata (UserId, Timestamp, EventId);
END;
GO

-- Create stored procedure to insert metadata
IF OBJECT_ID('dbo.sp_InsertEventMetadata', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_InsertEventMetadata;
GO

CREATE PROCEDURE dbo.sp_InsertEventMetadata
    @EventId UNIQUEIDENTIFIER,
    @UserId NVARCHAR(100),
    @EventType NVARCHAR(100),
    @Timestamp DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.EventMetadata 
    (
      EventId
      ,UserId
      ,EventType
      ,Timestamp
    )
    VALUES 
    (
      @EventId
      ,@UserId
      ,@EventType
      ,@Timestamp
    );
END;
GO