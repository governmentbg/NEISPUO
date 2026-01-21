import {
    Entity,
    Column,
    // PrimaryGeneratedColumn,
    // ManyToMany,
    // JoinTable,
    // CreateDateColumn,
    // UpdateDateColumn,
    OneToMany,
    PrimaryColumn,
} from 'typeorm';
// import {Exclude} from 'class-transformer';
// import {Town} from '../town/town.entity';
import { Municipality } from '../municipality/municipality.entity';

@Entity({ schema: 'location', name: 'Region' })
export class Region {
    @PrimaryColumn()
    RegionID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
        length: 1024,
    })
    Code: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    Description: string;

    /**
     * Relations
     */
    @OneToMany(
        (mun) => Municipality,
        (muns) => muns.Region,
    )
    municipalities?: Municipality[];

    /**
     * Trackers
     */
}
