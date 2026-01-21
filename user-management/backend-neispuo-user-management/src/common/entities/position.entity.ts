import { Column, Entity, JoinColumn, OneToMany, OneToOne, PrimaryGeneratedColumn } from 'typeorm';
import { EducationalStateEntity } from './educational-state.entity';
import { SysRoleEntity } from './sys-role.entity';

@Entity({ schema: 'core', name: 'Position' })
export class PositionEntity {
    @PrimaryGeneratedColumn({ name: 'positionID' })
    positionID: number;

    @Column({ name: 'name' })
    name: string;

    @Column({ name: 'description' })
    description: string;

    @Column({ name: 'sysRoleID' })
    sysRoleID: number;

    @OneToMany((type) => EducationalStateEntity, (educationalStates) => educationalStates.position)
    educationalStates: EducationalStateEntity[];

    @OneToOne(() => SysRoleEntity, (sysrole) => sysrole.position)
    @JoinColumn({ name: 'sysRoleID', referencedColumnName: 'sysRoleID' })
    role: SysRoleEntity;
}
