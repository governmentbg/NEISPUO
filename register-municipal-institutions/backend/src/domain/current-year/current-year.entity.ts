import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'inst_basic', name: 'CurrentYear' })
export class CurrentYearType {
    @PrimaryColumn()
    CurrentYearID: number;

    @Column({
        type: 'varchar',
    })
    Name: string;

    @Column({
        type: 'varchar',
        nullable: true,
    })
    Description: string;

    @Column({
        type: 'tinyint',
        nullable: true,
    })
    IsValid: boolean;

    @Column()
    TempCurrentYearID: number;

    @Column({
        type: 'datetime',
    })
    ChangeYearFrom: Date;

    @Column({
        type: 'datetime',
    })
    ChangeYearTo: Date;
}
