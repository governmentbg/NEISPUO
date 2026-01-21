import { Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ schema: 'logs', name: 'LoginAudit' })
export class LoginAuditEntity {
    @PrimaryGeneratedColumn({ name: 'LoginAuditID' })
    LoginAuditID: number;

    @Column()
    SysUserID: number;

    @Column()
    Username: string;

    @Column()
    SysRoleID: number;

    @Column()
    SysRoleName: string;

    @Column()
    InstitutionID: number;

    @Column()
    InstitutionName: string;

    @Column()
    RegionID: number;

    @Column()
    RegionName: string;

    @Column()
    MunicipalityID: number;

    @Column()
    MunicipalityName: string;

    @Column()
    BudgetingInstitutionID: number;

    @Column()
    BudgetingInstitutionName: string;

    @Column()
    PositionID: number;

    @Column()
    IPSource: string;

    @Column()
    CreatedOn: Date;
}
