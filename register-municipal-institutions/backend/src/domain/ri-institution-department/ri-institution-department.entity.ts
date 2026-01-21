import {
 Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn,
} from 'typeorm';
import { Town } from '@domain/town/town.entity';
import { LocalArea } from '@domain/local-area/local-area.entity';
import { Country } from '../country/country.entity';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'reginst_basic', name: 'RIInstitutionDepartment' })
export class RIInstitutionDepartment {
    @PrimaryGeneratedColumn({ name: 'RIInstitutionDepartmentID' })
    RIInstitutionDepartmentID: number;

    @Column()
    Name: string;

    @Column()
    Notes: string;

    @ManyToOne(
        (type) => Country,
        // eslint-disable-next-line @typescript-eslint/no-shadow
        (Country) => Country.RIInstitutionDepartments,
    )
    @JoinColumn({ name: 'CountryID' })
    Country: Country;

    @ManyToOne(
        (type) => LocalArea,
        (localArea) => localArea.RIInstitutionDepartments,
    )
    @JoinColumn({ name: 'LocalAreaID' })
    LocalArea?: LocalArea;

    @ManyToOne(
        (type) => Town,
        // eslint-disable-next-line @typescript-eslint/no-shadow
        (Town) => Town.RIInstitutionDepartments,
    )
    @JoinColumn({ name: 'TownID' })
    Town: Town;

    @Column()
    Address: string;

    @Column()
    PostCode: number;

    @Column()
    CadasterCode: string;

    @Column()
    IsMain: boolean;

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.RIInstitutionDepartments,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => RIProcedure,
        (riProcedure) => riProcedure.RIInstitutionDepartments,
    )
    @JoinColumn({ name: 'RIprocedureID' })
    RIProcedure: RIProcedure;
    /**
     * Trackers
     */

    @Column({
        type: 'datetime2',
    })
    ValidFrom: Date;

    @Column({
        type: 'datetime2',
    })
    ValidTo: Date;
}
