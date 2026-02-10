import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'azure_temp', name: 'UserProfile' })
export class UserProfile {
  @PrimaryColumn()
  SysUserID: number;
  @Column()
  Username: string;
  @Column()
  PersonID: number;
  @Column()
  SysRoleID: number;
  @Column()
  SysRoleName: string;
  @Column()
  ThreeNames: string;
  @Column()
  IsRoleFromEducationalState: 0 | 1;
  @Column()
  InstitutionID: number;
  @Column()
  InstitutionName: string;
  @Column()
  PositionID: number;
  @Column()
  PositionName: string;
  @Column()
  MunicipalityID: number;
  @Column()
  MunicipalityName: string;
  @Column()
  RegionID: number;
  @Column()
  RegionName: string;
  @Column()
  BudgetingInstitutionID: number;
  @Column()
  BudgetingInstitutionName: string;
}
