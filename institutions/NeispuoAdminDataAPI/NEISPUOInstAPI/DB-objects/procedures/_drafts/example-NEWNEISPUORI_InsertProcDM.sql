DECLARE @_json NVARCHAR(MAX) = N'{
  "procedureTypeID": 1,
  "procedureDate": "2020-12-01",
  "procedureYearDue": "2021",
  "procedureStatusTypeID": 1,
  "procedureTransformTypeID": 5,
  "procedureTransformNotes": "Тестови бележки",
  "codeNEISPUO": "9999999",
  "bulstat": "123456789",
  "abbreviation": "Тестова инст ДМ 3",
  "name": "Тестова институцияяяяяяяяяяяяяяя ДМ",
  "baseSchoolTypeID": 12,
  "detailedSchoolTypeID": 114,
  "budgetingSchoolTypeID": 1,
  "financialSchoolTypeID": 1,
  "settlementCountry": 34,
  "settlementRegion": 22,
  "settlementMunicipality": 220,
  "settlementTown": 68134,
  "settlementLocalArea": 24,
  "settlementAddress": "ул. Алеко Константинов 51",
  "settlementPostCode": "1506",
  "headFirstName": "Деян",
  "headMiddleName": "Иванов",
  "headLastName": "Марков",
  "isDataDue": true,
  "isNational": false,
  "personnelCount": "123",
  "religInstDetails": "N/A1",
  "authProgram": "N/A2",
  "primaryAddress": [
    {
      "id": "",
	  "primaryAddrCountry": 34,
	  "primaryAddrRegion": 22,
	  "primaryAddrMunicipality": 220,
	  "primaryAddrTown": 68134,
	  "primaryAddrLocalArea": 3,
	  "primaryAddrAddress": "Основен адрес на дейността - Младост 1, бл. 1234 ",
	  "primaryAddrPostCode": "1234"
    }
  ],
  "premInventory": null,
  "institutionDepartments": [
    {
      "id": "",
      "departmentName": "Подразделение 1",
      "departmentCountry": 34,
      "departmentRegion": 2,
      "departmentMunicipality": 16,
      "departmentTown": 7079,
      "departmentLocalArea": null,
      "departmentAddress": "Адрес на подразделение 1 - ул. Синеморец, N5",
      "departmentPostalCode": "5601",
      "departmentCadasterCode": "УПИ на Подразделение 1"
    },
    {
      "id": "",
      "departmentName": "Подразделение 2",
      "departmentCountry": 34,
      "departmentRegion": 3,
      "departmentMunicipality": 28,
      "departmentTown": 84,
      "departmentLocalArea": null,
      "departmentAddress": "Адрес на подразделение 2",
      "departmentPostalCode": "5605",
      "departmentCadasterCode": "УПИ на Подразделение 2"
    }
  ]
}';

INSERT INTO [neispuo].[reginst_basic].[RIProcedure]
           ([InstitutionID]
           ,[ProcedureTypeID]
           ,[TransfromTypeID]
           ,[TransformDetails]
           ,[YearDue]
           ,[StatusTypeID]
           ,[ProcedureDate]
		   ,[IsActive])
SELECT *, 0 as IsActive
FROM OPENJSON(@_json)
WITH (   
            InstitutionID int				'$.codeNEISPUO' ,  
            ProcedureTypeID int				'$.procedureTypeID',  
			TransfromTypeID int				'$.procedureTransformTypeID',
			TransformDetails nvarchar(4000)	'$.procedureTransformNotes',
			YearDue int						'$.procedureYearDue',
			StatusTypeID int				'$.procedureStatusTypeID',
			ProcedureDate datetime			'$.procedureDate'
 ); 

-- GET INSERTED ID(autoincrement)
DECLARE @procID int;
SELECT @procID = SCOPE_IDENTITY();
SELECT @procID;

-- INSERT INTO RIInstitution WITH @procID blah blah
INSERT INTO [neispuo].[reginst_basic].[RIInstitution]
           ([RIprocedureID]
           ,[InstitutionID]
           ,[Name]
           ,[Abbreviation]
           ,[BaseSchoolTypeID]
           ,[DetailedSchoolTypeID]
           ,[FinancialSchoolTypeID]
           ,[BudgetingSchoolTypeID]
           ,[Bulstat]
           ,[TRCountryID]
           ,[TRTownID]
           ,[TRLocalAreaID]
           ,[TRAddress]
           ,[TRPostCode]
           ,[ReligInstDetails]
           ,[HeadFirstName]
           ,[HeadMiddleName]
           ,[HeadLastName]
           ,[IsNational]
           ,[PersonnelCount]
           ,[AuthProgram]
           ,[IsDataDue]
           ,[PremInventory])
SELECT @procID,*
FROM OPENJSON(@_json)
WITH (   
           InstitutionID int				'$.codeNEISPUO' ,
           Name nvarchar(1024)				'$.name',
           Abbreviation nvarchar(255)		'$.abbreviation',
           BaseSchoolTypeID int				'$.baseSchoolTypeID',
           DetailedSchoolTypeID int			'$.detailedSchoolTypeID',
           FinancialSchoolTypeID int		'$.financialSchoolTypeID',
           BudgetingSchoolTypeID int		'$.budgetingSchoolTypeID',
           Bulstat nvarchar(13)				'$.bulstat',
           TRCountryID int					'$.settlementCountry',
           TRTownID int						'$.settlementTown',
           TRLocalAreaID int				'$.settlementLocalArea',
           TRAddress nvarchar(255)			'$.settlementAddress',
           TRPostCode int					'$.settlementPostCode',
           ReligInstDetails nvarchar(max)	'$.religInstDetails',
           HeadFirstName nvarchar(255)		'$.headFirstName',
           HeadMiddleName nvarchar(255)		'$.headMiddleName',
           HeadLastName nvarchar(255)		'$.headLastName',
           IsNational bit					'$.isNational',
           PersonnelCount int				'$.personnelCount',
           AuthProgram nvarchar(max)		'$.authProgram',
           IsDataDue bit					'$.isDataDue',
           PremInventory nvarchar(max)		'$.premInventory'
 ) 

-- INSERT INTO RIInstitutionDepartment WITH @procID blah blah
INSERT INTO [neispuo].[reginst_basic].[RIInstiutionDepartment]
           ([RIprocedureID]
           ,[CountryID]
           ,[TownID]
           ,[LocalAreaID]
           ,[Address]
           ,[PostCode]
           ,[IsMain])
		   
SELECT @procID,*,1
FROM OPENJSON(@_json, '$.primaryAddress')
WITH 
(
           CountryID int				'$.primaryAddrCountry',
           TownID int					'$.primaryAddrTown',
           LocalAreaID int				'$.primaryAddrLocalArea',
           Address nvarchar(255)		'$.primaryAddrAddress',
           PostCode int					'$.primaryAddrPostCode'
)

INSERT INTO [neispuo].[reginst_basic].[RIInstiutionDepartment]
           ([RIprocedureID]
		   ,[Name]
           ,[CountryID]
           ,[TownID]
           ,[LocalAreaID]
           ,[Address]
           ,[PostCode]
		   ,[CadasterCode]
           ,[IsMain])
SELECT @procID,*,0
FROM OPENJSON(@_json, '$.institutionDepartments')
WITH 
(
           Name nvarchar(255)			'$.departmentName',
           CountryID int				'$.departmentCountry',
           TownID int					'$.departmentTown',
           LocalAreaID int				'$.departmentLocalArea',
           Address nvarchar(255)		'$.departmentAddress',
           PostCode int					'$.departmentPostalCode',
           CadasterCode nvarchar(255)	'$.departmentCadasterCode'
)