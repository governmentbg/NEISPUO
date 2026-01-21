import { Column, Entity, PrimaryColumn } from 'typeorm';

@Entity({ schema: 'azure_temp', name: 'EventStatus' })
export class EventStatusEntity {
  @PrimaryColumn({ name: 'RowID', type: 'int' })
  rowID: number;

  @Column({
    name: 'Name',
    type: 'varchar',
    length: 100,
    collation: 'Cyrillic_General_CI_AS',
  })
  name: string;
}
