CREATE TABLE neispuo.reginst_basic.RIFlexField (
    [RIFlexFieldID] int NOT NULL IDENTITY(1,1) CONSTRAINT PK_RIFlexField PRIMARY KEY CLUSTERED,
    [SysUserID] int NOT NULL CONSTRAINT FK_RIFlexField_SysUser FOREIGN KEY REFERENCES core.SysUser(SysUserID),
    [MunicipalityID] int NOT NULL CONSTRAINT FK_RIFlexField_Municipality FOREIGN KEY REFERENCES location.Municipality(MunicipalityID),
    [Data] nvarchar(max) NOT NULL,
    [Mandatory] bit,
    [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    [ValidTo] datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (
    SYSTEM_VERSIONING = ON (HISTORY_TABLE = reginst_basic.RIFlexFieldHistory)
) GO;

CREATE TABLE neispuo.reginst_basic.RIFlexFieldValue (
    [RIFlexFieldValueID] int NOT NULL IDENTITY(1,1) CONSTRAINT PK_RIFlexFieldValue PRIMARY KEY CLUSTERED,
    [RIFlexFieldID] int NOT NULL CONSTRAINT FK_RIFlexFieldValue_RIFlexField FOREIGN KEY REFERENCES reginst_basic.RIFlexField(RIFlexFieldID),
    [RIInstitutionID] int NOT NULL CONSTRAINT FK_RIFlexFieldValue_RIInstitution FOREIGN KEY REFERENCES reginst_basic.RIInstitution(RIInstitutionID),
    [Value] nvarchar(max) NOT NULL,
    [ValidFrom] datetime2 GENERATED ALWAYS AS ROW START HIDDEN,
    [ValidTo] datetime2 GENERATED ALWAYS AS ROW END HIDDEN,
    PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
    ) WITH (
    SYSTEM_VERSIONING = ON (HISTORY_TABLE = reginst_basic.RIFlexFieldValueHistory)
) GO;
