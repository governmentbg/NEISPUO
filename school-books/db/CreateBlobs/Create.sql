USE [$(dbName)]
GO

---------------------------------------------------------------
--Stored Procedures
---------------------------------------------------------------

:r "./CreateBlobs/StoredProcedures/spCreateIdSequence.sql"
:r "./CreateBlobs/StoredProcedures/spDropSchemaObjects.sql"

---------------------------------------------------------------
--Tables
---------------------------------------------------------------

:r "./CreateBlobs/Tables/Blobs/BlobContents.sql"
:r "./CreateBlobs/Tables/Blobs/Blobs.sql"
