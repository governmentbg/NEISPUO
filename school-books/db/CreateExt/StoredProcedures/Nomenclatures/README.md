### How to add a new nomenclature

1. Create a `Get_Nom<Name>` stored procedure
2. Insert a service record like this one

   ```
   INSERT INTO [core].[ExtSystemServices] ([ServiceName], [ServiceParameters], [ProcedureName], [IsReturnArray], [ApiCallQuery], [IsValid], [Description])
   VALUES ('getNomReplrParticipationType', NULL, '[ext].[Get_NomReplrParticipationType]', 1, 'api/Datas?getNomReplrParticipationType', 1, N'Номенклатура - Вид на участието в РЕПЛР')
   ```

3. Insert a service map record

   ```
   INSERT INTO [core].[ExtSystemServicesMap]([ExtServiceID], [ExtSystemID])
   VALUES
   (<ExtServiceID>, 7),  -- Школо
   (<ExtServiceID>, 8),  -- АдминСофтПлюс
   (<ExtServiceID>, 9),  -- OneBook
   (<ExtServiceID>, 23), -- Тестова система (neispuo-extapi-test.crt)
   (<ExtServiceID>, 30), -- Информационно обслужване
   (<ExtServiceID>, 32)  -- EDG.BG
   ```

4. Test with

   ```
   curl -X GET -i -k \
   --cert neispuo-extapi-test.crt \
   --key neispuo-extapi-test.key \
   -H 'accept: application/json' \
   'https://test-extapi-neispuo.mon.bg/projects/api/Datas?getNomAbsenceReason'
   ```

### How to add a new ExtSystem with access to certain ExtSystemServices

```
SET IDENTITY_INSERT [core].[ExtSystem] ON
INSERT INTO [core].[ExtSystem] ([ExtSystemID], [Name], [IsValid], [SysUserID])
VALUES (
  38,
  N'(Сиела) Инструментариум за оценка на качеството на преподаване в дуалната система на обучение и за оценяване на работното място',
  1,
  NULL)
SET IDENTITY_INSERT [core].[ExtSystem] OFF

INSERT INTO [core].[ExtSystemAccess] ([ExtSystemID], [ExtSystemType], [IsValid])
VALUES (38, 3, 1)

INSERT INTO [core].[ExtSystemCertificate] ([ExtSystemID], [Thumbprint], [NotAfter], [NotBefore], [IsValid])
VALUES (
  38,
  '19A9A98D9786A6B9AC7B0C1A77CF6E9C13C2EE93',
  DATEADD(HOUR, DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), CONVERT(DATETIME2, '2023-02-17 10:50:40Z')),
  DATEADD(HOUR, DATEDIFF(HOUR, GETUTCDATE(), GETDATE()), CONVERT(DATETIME2, '2024-08-31 10:50:40Z')),
  1)

INSERT INTO [core].[ExtSystemServicesMap]([ExtServiceID], [ExtSystemID])
VALUES
  (78, 38),  -- getNomNKPDPosition
  (103, 38), -- getNomSubject
  (105, 38) -- getNomSubjectType
```

### How to add a new parameterized ExtSystemService

```
INSERT INTO [core].[ExtSystemServices] ([ServiceName], [ServiceParameters], [ProcedureName], [IsReturnArray], [ApiCallQuery], [IsValid], [Description])
VALUES (
  'getEduSurveyTeachers',
  'schoolYear|institutionId',
  '[ext].[Get_EduSurvey_Teachers]',
  1,
  'api/Datas?getEduSurveyTeachers/{schoolYear}/{institutionId}',
  1,
  'интеграция EduSurvey (Сиела) - Списък с учители за институция'
)

INSERT INTO [core].[ExtSystemServicesMap]([ExtServiceID], [ExtSystemID])
VALUES (SCOPE_IDENTITY(), 38)

INSERT INTO [core].[ExtSystemServices] ([ServiceName], [ServiceParameters], [ProcedureName], [IsReturnArray], [ApiCallQuery], [IsValid], [Description])
VALUES (
  'getEduSurveyStudents',
  'schoolYear|institutionId',
  '[ext].[Get_EduSurvey_Students]',
  1,
  'api/Datas?getEduSurveyStudents/{schoolYear}/{institutionId}',
  1,
  'интеграция EduSurvey (Сиела) - Списък с учениците за институция'
)

INSERT INTO [core].[ExtSystemServicesMap]([ExtServiceID], [ExtSystemID])
VALUES (SCOPE_IDENTITY(), 38)
```
