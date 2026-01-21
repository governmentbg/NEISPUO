import {
  Entity,
  Column,
  ManyToOne,
  JoinColumn,
  PrimaryGeneratedColumn,
} from 'typeorm';
import { NeispuoCategory } from '../neispuo-category/neispuo-category.entity';

@Entity({ schema: 'portal', name: 'UserGuide' })
export class UserGuide {
  @PrimaryGeneratedColumn({ name: 'UserGuideID' })
  id: number;

  @Column({ name: 'Name' })
  name: string;

  @Column({ name: 'MimeType' })
  mimeType: string;

  @Column({ name: 'Filename' })
  filename: string;

  @ManyToOne(
    type => NeispuoCategory,
    NeispuoCategory => NeispuoCategory.userGuides,
  )
  @JoinColumn({ name: 'CategoryID' })
  category?: NeispuoCategory;

  @Column({ type: 'varbinary', nullable: true })
  fileContent: Buffer;

  @Column({ type: 'varchar', nullable: true })
  URLOverride: string;
}
