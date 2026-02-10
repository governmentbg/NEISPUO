import {
 Column, Entity, JoinColumn, PrimaryColumn,
} from 'typeorm';
import { Country } from '../country/country.entity';
import { LocalArea } from '../local-area/local-area.entity';

@Entity({ schema: 'core' })
export class IdentityProvider {
    @PrimaryColumn()
    IdentityProviderID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    Description: string;

    /**
     * Relations
     */

    country: Country;

    @JoinColumn({ name: 'LocalAreaID' })
    localArea: LocalArea;

    /**
     * Trackers
     */
}
