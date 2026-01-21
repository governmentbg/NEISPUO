import { Entity, Column, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ name: 'SystemUserMessage', schema: 'core' })
export class SystemUserMessage {
  @PrimaryGeneratedColumn({ name: 'SystemUserMessageID' })
  id: number;

  // this is actually varchar in the DB, not specified since it messes up cyrillic encoding
  @Column({ type: 'nvarchar', name: 'Title', nullable: true })
  title: string;

  @Column({ type: 'nvarchar', length: 'max', name: 'Content', nullable: true })
  content: string;

  @Column({ type: 'datetime2', name: 'StartDate' })
  startDate: Date;

  @Column({ type: 'datetime2', name: 'EndDate' })
  endDate: Date;

  @Column({ type: 'nvarchar', length: 'max', name: 'Roles' })
  roles: string; // comma separated numbers - e.g. "1,2,3"
}
