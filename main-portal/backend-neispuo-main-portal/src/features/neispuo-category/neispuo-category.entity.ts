import { Entity, Column, PrimaryColumn, OneToMany, ManyToMany, ManyToOne } from 'typeorm';
import { NeispuoModule } from '../neispuo-module/neispuo-module.entity';
import { UserGuide } from '../user-guides/user-guides.entity';


@Entity({ schema: 'portal', name: 'Category' })
export class NeispuoCategory {
    @PrimaryColumn({ name: 'CategoryID' })
    id: number;

    @Column({ name: 'Name' })
    name: string;

    @Column({ name: 'Description' })
    description?: string;

    @OneToMany(
        nm => NeispuoModule,
        nm => nm.category
    )
    neispuoModules: NeispuoModule[];

    @OneToMany(
        type => UserGuide,
        userGuides => userGuides.category
    )
    userGuides: UserGuide[];
    
}