import { SysRoleEnum } from 'src/shared/enums/role.enum';
import { SysSchemaEnum } from 'src/shared/enums/schemas.enum';
import { MigrationInterface, QueryRunner } from 'typeorm';

export class AddRPersonalStaffView1757074414000
  implements MigrationInterface
{
  public async up(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
        INSERT INTO [reporting].[SchemaRoleAccess]("SchemaName", "AllowedSysRole")
        VALUES
        ('${SysSchemaEnum.R_PERSONAL_STAFF}', ${SysRoleEnum.MON_ADMIN}),
        ('${SysSchemaEnum.R_PERSONAL_STAFF}', ${SysRoleEnum.MON_EXPERT}),
        ('${SysSchemaEnum.R_PERSONAL_STAFF}', ${SysRoleEnum.RUO_EXPERT}),
        ('${SysSchemaEnum.R_PERSONAL_STAFF}', ${SysRoleEnum.RUO}),
        ('${SysSchemaEnum.R_PERSONAL_STAFF}', ${SysRoleEnum.INSTITUTION});`);

    await queryRunner.query(`
    CREATE VIEW [reporting].[R_Personal_Staff]
		AS
    SELECT i.InstitutionID as 'Код по НЕИСПУО'
          ,i.Name as 'Наименование'
    	  ,t.Name as 'Населено място'
    	  ,m.Name as 'Община'
    	  ,r.Name as 'Област'
	  ,r.RegionID
    	 -- ,iif(la.Name is null,'не е приложима',la.Name) as 'Район'
          ,fst.Name as 'Вид по чл. 35-36 (според собствеността)'
          ,dst.Name as 'Вид по чл. 38 (детайлен)'
          ,bi.Name as 'Финансиращ орган'
        --  ,bst.Name as 'Вид по чл. 37 (общ, според вида на подготовката)'
          ,'info-' + CAST(i.InstitutionID AS VARCHAR(20)) + '@edu.mon.bg' as 'Email'
    	  -----Данни за служителя
    	  ,p.FirstName as 'Име'
    	  ,p.MiddleName as 'Презиме'
    	  ,p.LastName as 'Фамилия'
    	  ,p.PublicEduNumber as 'ЛОН'
    	  ,iif(p.Gender=1,'жена','мъж') as 'Пол'
    	  ,p.BirthDate as 'Дата на раждане'
    	  ,c.Name as 'Месторождение(държава)'
    	  ,tt.Name as 'Месторождение(град)'
    	  ,cc.Name as 'Гражданство'
    	  ,mp.Name as 'Постоянен адрес – Община'
    	  ,rp.Name as 'Постоянен адрес – Област'
    	  ,tp.Name as 'Постоянен адрес – Населено място'
    	  ,p.PermanentAddress as 'Постоянен адрес'
    	  ,mc.Name as 'Настоящ адрес  – Община'
    	  ,rc.Name as 'Настоящ адрес – Област'
    	  ,tc.Name as 'Настоящ адрес – Населено място'
    	  ,p.CurrentAddress as 'Настоящ адрес'
    	  ,pd.PhoneNumber as 'Телефон'
    	  ,CAST(p.PublicEduNumber AS VARCHAR(20)) + '@edu.mon.bg' as 'Електронна поща'
    	  ,iif(pd.IsPensioneer=1,'да','не') as 'Работещ пенсионер'
    	  ,iif(pd.IsExtendStudent=1,'да','не') as 'Продължава образованието си'
    	  ,STRING_AGG(nkpd.Name, '; ') AS 'Активни длъжности'
    	  ,STRING_AGG(cst.Name, '; ') AS 'Категория персонал'
    	--  ,count(sp.NKPDPositionID)
      FROM [core].[Institution] i
    	left join noms.FinancialSchoolType fst on fst.FinancialSchoolTypeID=i.FinancialSchoolTypeID
    	left join noms.DetailedSchoolType dst on dst.DetailedSchoolTypeID=i.DetailedSchoolTypeID
    	left join noms.BaseSchoolType bst on bst.BaseSchoolTypeID=i.BaseSchoolTypeID
    	left join noms.BudgetingInstitution bi on bi.BudgetingInstitutionID=i.BudgetingSchoolTypeID
    	left join location.Town t on t.TownID=i.TownID
    	left join location.Municipality m on m.MunicipalityID=t.MunicipalityID
    	left join location.Region r on r.RegionID=m.RegionID
    	left join location.LocalArea la on la.LocalAreaID=i.LocalAreaID
    	left join inst_basic.StaffPosition sp on sp.InstitutionID=i.InstitutionID
    	left join core.Person p on p.PersonID=sp.PersonID
    	left join location.Country c on c.CountryID=p.BirthPlaceCountry
    	left join location.Town tt on tt.TownID=p.BirthPlaceTownID
        left join location.Country cc on cc.CountryID=p.NationalityID
    	left join location.Town tp on tp.TownID=p.PermanentTownID
    	left join location.Municipality mp on mp.MunicipalityID=tp.MunicipalityID
    	left join location.Region rp on rp.RegionID=mp.RegionID
    	left join location.Town tc on tc.TownID=p.CurrentTownID
    	left join location.Municipality mc on mc.MunicipalityID=tc.MunicipalityID
    	left join location.Region rc on rc.RegionID=mc.RegionID
    	left join inst_basic.PersonDetail pd on pd.PersonID=sp.PersonID
    	left join inst_nom.NKPDPosition nkpd on nkpd.NKPDPositionID=sp.NKPDPositionID
    	left join inst_nom.CategoryStaffType cst on cst.CategoryStaffTypeID=sp.CategoryStaffTypeID
        
     WHERE sp.IsValid=1
     GROUP BY
    	i.InstitutionID 
          ,i.Name 
    	  ,t.Name 
    	  ,m.Name 
    	  ,r.Name 
		  ,r.RegionID
          ,fst.Name 
          ,dst.Name 
          ,bi.Name 
    	  ,p.FirstName 
    	  ,p.MiddleName 
    	  ,p.LastName 
    	  ,p.PublicEduNumber 
    	  ,p.Gender
    	  ,p.BirthDate
    	  ,c.Name 
    	  ,tt.Name 
    	  ,cc.Name 
    	  ,mp.Name
    	  ,rp.Name 
    	  ,tp.Name 
    	  ,p.PermanentAddress 
    	  ,mc.Name 
    	  ,rc.Name 
    	  ,tc.Name 
    	  ,p.CurrentAddress 
    	  ,pd.PhoneNumber
    	  ,pd.IsPensioneer
    	  ,pd.IsExtendStudent;
    `);
  }

  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.query(`
       DELETE FROM [reporting].[SchemaRoleAccess]
       WHERE SchemaName='${SysSchemaEnum.R_PERSONAL_STAFF}'`);

    await queryRunner.query(`
       DROP VIEW [reporting].[R_Personal_Staff]`);
  }
}
