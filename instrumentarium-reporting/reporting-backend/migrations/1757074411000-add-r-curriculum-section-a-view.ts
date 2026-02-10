import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRCurriculumSectionAView1757074411000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_CURRICULUM_SECTION_A}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_CURRICULUM_SECTION_A}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_CURRICULUM_SECTION_A}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_CURRICULUM_SECTION_A}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_CURRICULUM_SECTION_A}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_Curriculum_Section_A]
		AS
    SELECT distinct i.InstitutionID as 'Код по НЕИСПУО'
          ,i.Name as 'Наименование'
    	  ,t.Name as 'Населено място'
    	  ,m.Name as 'Община'
    	  ,r.Name as 'Област'
	  ,r.RegionID
    	  ,iif(la.Name is null,'не е приложима',la.Name) as 'Район'
          ,fst.Name as 'Вид по чл. 35-36 (според собствеността)'
          ,dst.Name as 'Вид по чл. 38 (детайлен)'
          ,bi.Name as 'Финансиращ орган'
          ,bst.Name as 'Вид по чл. 37 (общ, според вида на подготовката)'
          ,'info-' + CAST(i.InstitutionID AS VARCHAR(20)) + '@edu.mon.bg' as 'Email'
    	  ------Данни за етап на обучение
    	  ,cg.SchoolYear as 'Учебна година'
    	  , case 
    			when cg.BasicClassID<5 then 'Начален'
    			when (cg.BasicClassID>4 and cg.BasicClassID<8) then 'Прогимназиален'
    			when (cg.BasicClassID>7 and cg.BasicClassID<11) then 'Първи гимназиален'
    			when (cg.BasicClassID>10 and cg.BasicClassID<13) then 'Втори гимназиален'
    		end as 'Етап на обучение'
    	  --,cg.BasicClassID
    	   ,cg.ClassName as 'Наименование на випуск/клас'
    	 --  ,cg.ClassGroupNum
    	 ---------Данни за учебен план
    	 --,cc.CurriculumID
    		,case
    			when c.SubjectID=90 then s.SubjectName + ' (' + iif(fl.Name is null,'',fl.Name) + ')'
    			else s.SubjectName 
    		end as 'Учебен предмет'
    		,st.Name as 'Начин на изучаване'
    		,case 
    			when c.SubjectID=90 then 'по професията'
    			when (c.SubjectID>99 and c.SubjectID<144) then flst.Name
    			else 'не е чужд език'
    		end as 'Чужд език'
    		,c.WeeksFirstTerm as 'I срок УС'
    		,c.HoursWeeklyFirstTerm as 'I срок ЧС'
    		,c.WeeksSecondTerm as 'II срок УС'
    		,c.HoursWeeklySecondTerm as 'II срок ЧС'
    		,c.TotalTermHours as 'Общ бр.ч.'
    		,c.StudentsCount as 'Общ брой ученици'
    		,p.FirstName + ' ' + p.LastName as 'Преподавател'
      FROM [core].[Institution] i
    	left join noms.FinancialSchoolType fst on fst.FinancialSchoolTypeID=i.FinancialSchoolTypeID
    	left join noms.DetailedSchoolType dst on dst.DetailedSchoolTypeID=i.DetailedSchoolTypeID
    	left join noms.BaseSchoolType bst on bst.BaseSchoolTypeID=i.BaseSchoolTypeID
    	left join noms.BudgetingInstitution bi on bi.BudgetingInstitutionID=i.BudgetingSchoolTypeID
    	left join location.Town t on t.TownID=i.TownID
    	left join location.Municipality m on m.MunicipalityID=t.MunicipalityID
    	left join location.Region r on r.RegionID=m.RegionID
    	left join location.LocalArea la on la.LocalAreaID=i.LocalAreaID
    	left join inst_year.ClassGroup cg on cg.InstitutionID=i.InstitutionID 
    				and cg.IsValid=1 and cg.BasicClassID in (1,2,3,4,5,6,7,8,9,10,11,12) and cg.ParentClassID is not null and cg.ClassName<>'служебна'
        left join inst_year.CurriculumClass cc on cc.ClassID=cg.ParentClassID and cc.SchoolYear=cg.SchoolYear
    				and cc.IsValid=1
    	inner join inst_year.Curriculum c on c.CurriculumID=cc.CurriculumID and c.SchoolYear=cc.SchoolYear
    				and c.IsValid=1 and c.CurriculumPartID=1
        inner join inst_nom.Subject s on s.SubjectID=c.SubjectID 
    	left join inst_nom.SubjectType st on st.SubjectTypeID=c.SubjectTypeID
        left join inst_nom.FL fl on fl.FLID=c.FLSubjectID
    	left join inst_nom.FLStudyType flst on flst.FLStudyTypeID=cg.FLTypeID
    	left join inst_year.CurriculumTeacher ct on ct.CurriculumID=c.CurriculumID and ct.SchoolYear=c.SchoolYear
    				and ct.IsValid=1
    	left join inst_basic.StaffPosition sp on sp.StaffPositionID=ct.StaffPositionID
    	left join core.Person p on p.PersonID=sp.PersonID
        
     WHERE i.BaseSchoolTypeID in (1,2,3,7,11,12,13,14,18) and c.SchoolYear>2022
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_CURRICULUM_SECTION_A}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_Curriculum_Section_A]`);
  }
}
