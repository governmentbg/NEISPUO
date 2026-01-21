import { Column, Entity, OneToMany, PrimaryColumn } from 'typeorm';
import { SysUserEntity } from './sys-user.entity';

@Entity({ schema: 'core', name: 'IdentityProvider' })
export class IdentityProviderEntity {
    @PrimaryColumn({ name: 'IdentityProviderID' })
    identityProviderID: number;

    @Column({ name: 'Name', unique: true })
    name: string;

    @OneToMany((type) => SysUserEntity, (sysUserEntity) => sysUserEntity.identityProvider)
    sysUsers: SysUserEntity[];
}
