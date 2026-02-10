import {
    Column,
    Entity,
    JoinColumn,
    ManyToOne,
    PrimaryGeneratedColumn,
} from 'typeorm';
import { CPLRAreaType } from '@domain/cplr-area-type/cplr-area-type.entity';
import { RIProcedure } from '../ri-procedure/ri-procedure.entity';
import { SysUser } from '../sys-user/sys-user.entity';

@Entity({ schema: 'reginst_basic', name: 'RICPLRArea' })
export class RICPLRArea {
    @PrimaryGeneratedColumn({ name: 'RICPLRAreaID' })
    RICPLRAreaID: number;

    /**
     * Relations
     */

    @ManyToOne(
        (type) => SysUser,
        (sysUser) => sysUser.RICPLRAreas,
    )
    @JoinColumn({ name: 'SysUserID' })
    SysUser?: SysUser;

    @ManyToOne(
        (type) => RIProcedure,
        (riProcedure) => riProcedure.RICPLRArea,
    )
    @JoinColumn({ name: 'RIprocedureID' })
    RIProcedure: RIProcedure;

    @ManyToOne(
        (type) => CPLRAreaType,
        (cplrAreaType) => cplrAreaType.RICPLRAreas,
        { eager: true },
    )
    @JoinColumn({ name: 'CPLRAreaTypeID' })
    CPLRAreaType: CPLRAreaType;

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
