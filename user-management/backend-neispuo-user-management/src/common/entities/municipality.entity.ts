import { Column, Entity, JoinColumn, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { RegionEntity } from './region.entity';
import { SysUserSysRoleEntity } from './sys-user-sys-role.entity';
import { TownEntity } from './town.entity';

@Entity({ schema: 'location', name: 'Municipality' })
export class MunicipalityEntity {
    @PrimaryGeneratedColumn({ name: 'municipalityID' })
    municipalityID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'code' })
    code: string;

    @Column({ name: 'regionID' })
    regionID: number;

    @Column({ name: 'description' })
    description: number;

    @OneToMany(() => SysUserSysRoleEntity, (sysUserSysRoles) => sysUserSysRoles.municipality)
    sysUserSysRoles: SysUserSysRoleEntity[];

    @OneToOne((type) => TownEntity, (town) => town.municipality)
    town: TownEntity;

    @OneToOne(() => RegionEntity, (region) => region.regionID)
    @JoinColumn({ name: 'regionID', referencedColumnName: 'regionID' })
    region: RegionEntity;
}
