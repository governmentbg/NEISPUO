import {
    Entity,
    Column,
    ManyToOne,
    JoinColumn,
    PrimaryGeneratedColumn,
} from 'typeorm';
// import {Exclude} from 'class-transformer';
import { RIFlexField } from '@domain/ri-flex-field/ri-flex-field.entity';
import { Town } from '../town/town.entity';
import { Region } from '../region/region.entity';

@Entity({ schema: 'location', name: 'Municipality' })
export class Municipality {
    @PrimaryGeneratedColumn({ name: 'MunicipalityID' })
    MunicipalityID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
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
    Towns?: Town[];

    @ManyToOne(
        (type) => Region,
        (region) => region.municipalities,
        {
            eager: true,
        },
    )
    @JoinColumn({ name: 'RegionID' })
    Region: Region;

    RIFlexFields?: RIFlexField[];

    /**
     * Trackers
     */
}
