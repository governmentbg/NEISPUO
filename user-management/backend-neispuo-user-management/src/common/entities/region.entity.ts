import { Column, Entity, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { MunicipalityEntity } from './municipality.entity';
import { SysUserSysRoleEntity } from './sys-user-sys-role.entity';

@Entity({ schema: 'location', name: 'Region' })
export class RegionEntity {
    @PrimaryGeneratedColumn({ name: 'regionID' })
    regionID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'code' })
    code: string;

    @Column({ name: 'description' })
    description: string;

    @OneToMany(() => SysUserSysRoleEntity, (sysUserSysRoles) => sysUserSysRoles.region)
    sysUserSysRoles: SysUserSysRoleEntity[];

    @OneToOne((type) => MunicipalityEntity, (municipality) => municipality.region)
    municipality: MunicipalityEntity;
}
