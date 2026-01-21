DECLARE @_json NVARCHAR(MAX)
SET @_json = N'{
  "codeNEISPUO": 2999991,
  "bulstat": "1234567890",
  "abbreviation": "ТУ \"Алеко Константинов\"",
  "name": "Тестово училище \"Алеко Константинов\"",
  "baseSchoolTypeID": 12,
  "detailedSchoolTypeID": 124,
  "budgetingSchoolTypeID": 1,
  "financialSchoolTypeID": 1,
  "isProfSchool": true,
  "isNational": null,
  "isDelegateBudget": true,
  "schoolShiftTypeID": 4,
  "isProtected": null,
  "isCentral": null,
  "isStateFunding": null,
  "isInnovative": true,
  "innovations": [
    {
      "id": "1",
      "innovationType": 1,
      "notes": "тестова иновация"
    }
  ],
  "country": 34,
  "region": 22,
  "municipality": 220,
  "town": 68134,
  "localArea": 38,
  "address": "бул. Арсенал 123",
  "postCode": 1618,
  "email": "m.petkova@adminsoft.bg",
  "website": "www.tu.com",
  "institutionPhones": [
    {
      "id": "4",
      "institutionPhoneTypeID": 1,
      "institutionPhoneCode": "02",
      "institutionPhone": "123456789",
      "institutionContactKind": "директор",
      "isMain": true
    },
    {
      "id": "10",
      "institutionPhoneTypeID": 1,
      "institutionPhoneCode": "032",
      "institutionPhone": "31232131",
      "institutionContactKind": "тестов контакт",
      "isMain": null
    }
  ],
  "pedagogStaffCount": 94.5,
  "pedagogStaffSalary": 1050,
  "pedagogNonStaffCount": 29,
  "pedagogNonStaffSalary": 906,
  "yearlyBudget": 1352968,
  "bankBIC": "FINVBGSF",
  "bankIBAN": "BG92FINV91503117319092",
  "bankName": "Първа инвестиционна банка АД",
  "bankAccountHolder": "Тестово училище",
  "established": "1990",
  "incActFirst": "Заповед № РД85-850/05.12.1990г. на министъра на  народната просвета",
  "incActLast": "Заповед 345 от 2016 г."
}'

-- INSERT INTO RIProcedures
-- blah blah

-- GET INSERTED ID(autoincrement)
DECLARE @procID int;
SELECT @procID = SCOPE_IDENTITY();
SELECT @procID;

-- INSERT INTO RIInstitution WITH @procID blah blah
SELECT *
FROM OPENJSON(@_json)
WITH (   
              RIinstitutionID   int			 '$.codeNEISPUO' ,  
              Bulstat nvarchar(13)			 '$.bulstat',  
              [Name] nvarchar(1024)          '$.name',
			  [Abbreviation] nvarchar(255)	 '$.abbreviation'
			  -- and so on, for RIInstituion
 ) 

-- INSERT INTO Phones WITH @procID blah blah

SELECT *
FROM OPENJSON(@_json, '$.institutionPhones')
WITH 
(
				ID int '$.id',
				Phone nvarchar(20) '$.institutionPhone'
				--- and so on, for RIPhones
)