import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'core' })
export class SysGroup {
    @PrimaryColumn()
    SysGroupID: number;

    @Column({
        type: 'varchar'
    })
    Name: string;

    @Column({
        type: 'varchar'
    })
    Description: string;

    /**
     * Relations
     */

    /**
     * Trackers
     */

    // @CreateDateColumn()
    // @Exclude({toPlainOnly: true})
    // createdAt?: Date | string;
    //
    // @UpdateDateColumn()
    // @Exclude({toPlainOnly: true})
    // updatedAt?: Date | string;
}
