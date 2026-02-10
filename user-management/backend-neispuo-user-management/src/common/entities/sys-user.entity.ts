import { Column, Entity, JoinColumn, ManyToOne, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { PersonEntity } from './person.entity';
import { SysUserSysRoleEntity } from './sys-user-sys-role.entity';
import { IdentityProviderEntity } from './identity-provider.entity';

@Entity({ schema: 'core', name: 'SysUser' })
export class SysUserEntity {
    @PrimaryGeneratedColumn({ name: 'sysUserID' })
    sysUserID: number;

    @Column({ name: 'username' })
    username: string;

    @Column({ name: 'password' })
    password: string;

    @Column({ name: 'initialPassword' })
    initialPassword: string;

    @Column({ name: 'isAzureUser' })
    isAzureUser: boolean;

    @Column({ name: 'isAzureSynced' })
    isAzureSynced: boolean;

    @Column({ name: 'personID' })
    personID: number;

    @Column({ name: 'deletedOn', type: 'datetime2', nullable: true })
    deletedOn: Date;

    @Column({ name: 'IdentityProviderID', nullable: true })
    identityProviderID: number;

    @Column({ name: 'IdentityProviderUserID', nullable: true })
    identityProviderUserID: string;

    @OneToOne(() => PersonEntity, (person) => person.personID)
    @JoinColumn({ name: 'personID', referencedColumnName: 'personID' })
    person: PersonEntity;

    @OneToMany((type) => SysUserSysRoleEntity, (sysUserSysRoles) => sysUserSysRoles.user)
    sysUserSysRoles: SysUserSysRoleEntity[];

    @ManyToOne((type) => IdentityProviderEntity, (identityProviderEntity) => identityProviderEntity.sysUsers)
    @JoinColumn({ name: 'IdentityProviderID', referencedColumnName: 'identityProviderID' })
    identityProvider: IdentityProviderEntity;
}
