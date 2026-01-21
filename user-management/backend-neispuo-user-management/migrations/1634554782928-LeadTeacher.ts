import { MigrationInterface, QueryRunner } from 'typeorm';

export class LeadTeacher1634554782928 implements MigrationInterface {
    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(
            `CREATE VIEW azure_temp.LeadTeacher AS
            SELECT
                             sc.SchoolYear												AS 'SchoolYear'
                            , p.PersonID                                                            AS 'PersonID'
                            ,p.FirstName + ' ' +
                             ISNULL(p.MiddleName,'') + ' ' +
                             p.LastName													AS 'FullName'
                            ,ISNULL(
                                      (SELECT pit.Name
                                       FROM   noms.PersonalIDType pit
                                       WHERE  pit.PersonalIDTypeID = p.PersonalIDType
                                      )
                                 ,'')													AS 'IdentityNumType'
                            ,ISNULL(p.PersonalID,'')									AS 'IdentityNum'
                            ,CONVERT(VARCHAR(10),p.BirthDate,104)						AS 'BirthDate'
                            ,ISNULL(p.PublicEduNumber,'')								AS 'PublicEduNumber'
                            ,ISNULL(
                                      (SELECT c.Name
                                       FROM   location.Country c
                                       WHERE  p.NationalityID = c.CountryID
                                      )
                                 ,'')													AS 'Nationallity'
                            ,i.InstitutionID											AS 'InstitutionID'
                            ,(
                              SELECT pos.Name
                              FROM	 core.Position pos
                              WHERE	 sc.PositionId = pos.PositionID
                             )															AS 'PositionName'
                            ,CASE WHEN sc.IsCurrent = 1
                                  THEN 'Текущ клас'
                                  ELSE ISNULL(
                                                (SELECT drt.name
                                                 FROM student.DischargeReasonType drt
                                                 WHERE sc.dischargereasonid = drt.id
                                                )
                                    ,'Преместен')
                             END														AS 'DischargeReason'
                            ,(
                              SELECT bcl.Name
                              FROM	 inst_nom.BasicClass bcl
                              WHERE  cg.basicclassid = bcl.BasicClassID
                             )															AS 'Class'
                            ,cg.ClassName												AS 'ClassGroup'
                            ,ISNULL(
                                      (SELECT ct.Name
                                       FROM   inst_nom.ClassType ct
                                       WHERE  sc.ClassTypeId = ct.classtypeid
                                      )
                                 ,'')													AS 'ClassTypeName'
                            ,ISNULL(
                                      (SELECT s.Name
                                       FROM   inst_nom.SPPOOSpeciality s
                                       WHERE  sc.StudentSpecialityId = s.SPPOOSpecialityID
                                      )
                                 ,'')													AS 'StudentSpeciality'
                            ,ISNULL(
                                      (SELECT ef.Name
                                       FROM	  inst_nom.EduForm ef
                                       WHERE  sc.StudentEduFormId = ef.ClassEduFormID
                                      )
                                 ,'')															AS 'EduForm'
                            ,CASE WHEN sc.IsIndividualCurriculum = 0
                                  THEN 'Не'
                                  ELSE 'Да'
                             END														AS 'IsIndividualCurriculum'
                            ,CASE WHEN sc.IsHourlyOrganization = 0
                                  THEN 'Не'
                                  ELSE 'Да'
                             END														AS 'IsHourlyOrganization'
                            ,(
                              SELECT rr.Name
                              FROM	 student.RepeaterReason rr
                              WHERE  sc.RepeaterId = rr.Id
                             )															AS 'RepeaterReason'
                            ,ISNULL(
                                      (SELECT ct.Name
                                       FROM	  student.CommuterType ct
                                       WHERE  sc.CommuterTypeId = ct.Id
                                      )
                                ,'')													AS 'CommuterTypeName'
                            ,CASE WHEN (
                                        SELECT COUNT(id)
                                        FROM   student.ResourceSupportReport rsr
                                        WHERE  sc.SchoolYear = rsr.SchoolYear
                                               AND sc.PersonId = rsr.PersonId
                                       ) > 0
                                  THEN 'Да'
                                  ELSE 'Не'
                             END														AS 'ResourceSupportReport'
                            ,CASE WHEN (
                                        SELECT COUNT(id)
                                        FROM   student.SpecialNeedsYear sny
                                        WHERE  sc.SchoolYear = sny.SchoolYear
                                               AND sc.PersonId = sny.PersonId
                                       ) > 0
                                  THEN 'Да'
                                  ELSE 'Не'
                             END														AS 'SpecialNeeds'
                        
                        FROM
                            student.StudentClass sc
                            INNER JOIN inst_year.ClassGroup cg	ON cg.ClassID = sc.ClassID
                            INNER JOIN core.Person p			ON p.PersonID = sc.PersonId
                            INNER JOIN core.Institution i		ON i.InstitutionID = cg.InstitutionID
            `,
            undefined,
        );
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query('DROP VIEW azure_temp.LeadTeacher; ', undefined);
    }
}
