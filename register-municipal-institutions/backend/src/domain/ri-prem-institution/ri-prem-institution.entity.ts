import {
 Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn,
} from 'typeorm';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'reginst_basic', name: 'RIPremInstitution' })
export class RIPremInstitution {
    @PrimaryGeneratedColumn({ name: 'RIPremInstitutionID' })
    RIPremInstitutionID: number;

    @Column({
        type: 'int',
        nullable: true,
    })
    PremInstitutionID: number;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    PremStudents: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    PremDocs: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    PremInventory: string;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => RIProcedure,
        // eslint-disable-next-line @typescript-eslint/no-shadow
        (RIProcedure) => RIProcedure.RIPremInstitution,
    )
    @JoinColumn({ name: 'RIProcedureID' })
    RIProcedure: RIProcedure;

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.RIPremInstitution,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser[];

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
