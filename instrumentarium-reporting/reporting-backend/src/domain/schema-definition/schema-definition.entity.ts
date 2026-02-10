import { Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ schema: 'reporting', name: 'SchemaDefinition' })
export class SchemaDefinition {
  @PrimaryGeneratedColumn({ name: 'SchemaDefinitionID' })
  SchemaDefinitionID: number;

  @Column({
    type: 'varchar',
    unique: true,
  })
  Name: string;

  @Column({
    type: 'simple-json',
  })
  Definition: any;
}
