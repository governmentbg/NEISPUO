PRINT 'Create other schemas'
GO

USE [$(dbName)]
GO

---------------------------------------------------------------
--OtherSchemas
---------------------------------------------------------------

PRINT 'Create core, noms, location, family'
GO
create schema core;
GO
create schema noms;
GO
create schema location;
GO
create schema family;
GO
:r "./CreateOtherSchemas/Schemas/core_noms_location_family.sql"

PRINT 'Create inst_nom'
GO
CREATE SCHEMA inst_nom;
GO
:r "./CreateOtherSchemas/Schemas/inst_nom.sql"

PRINT 'Create inst_basic'
GO
CREATE SCHEMA inst_basic;
GO
:r "./CreateOtherSchemas/Schemas/inst_basic.sql"

PRINT 'Create inst_year'
GO
CREATE SCHEMA inst_year;
GO
:r "./CreateOtherSchemas/Schemas/inst_year.sql"

PRINT 'Create document'
GO
create schema document;
GO
:r "./CreateOtherSchemas/Schemas/document.sql"

PRINT 'Create student'
GO
create schema student;
GO
:r "./CreateOtherSchemas/Schemas/student.sql"

PRINT 'Create functions'
:r "./CreateOtherSchemas/Functions/fn_check_unique_person.sql"

PRINT 'Create core, noms, location, family FKs'
:r "./CreateOtherSchemas/Schemas/core_noms_location_family.fks.sql"

PRINT 'Create inst_nom FKs'
:r "./CreateOtherSchemas/Schemas/inst_nom.fks.sql"

PRINT 'Create inst_basic FKs'
:r "./CreateOtherSchemas/Schemas/inst_basic.fks.sql"

PRINT 'Create inst_year FKs'
:r "./CreateOtherSchemas/Schemas/inst_year.fks.sql"

PRINT 'Create document FKs'
:r "./CreateOtherSchemas/Schemas/document.fks.sql"

PRINT 'Create student FKs'
:r "./CreateOtherSchemas/Schemas/student.fks.sql"

PRINT 'Create inst_year Views'
:r "./CreateOtherSchemas/Schemas/inst_year.views.sql"
