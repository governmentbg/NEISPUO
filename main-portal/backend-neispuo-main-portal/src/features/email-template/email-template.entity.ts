import { jsonTransformer } from 'src/shared/utils/json-transformer';
import {
  Column,
  CreateDateColumn,
  Entity,
  JoinColumn,
  ManyToOne,
  PrimaryGeneratedColumn,
  UpdateDateColumn,
} from 'typeorm';
import { EmailTemplateType } from '../email-template-type/email-template-type.entity';

@Entity({
  schema: 'portal',
  name: 'EmailTemplate',
})
export class EmailTemplate {
  @PrimaryGeneratedColumn({ name: 'Id' })
  id: number;

  @Column({
    length: 150,
    name: 'Title',
  })
  title: string;

  @Column({
    type: 'nvarchar',
    length: 'max',
    name: 'Content',
  })
  content: string;

  @Column({
    type: 'nvarchar',
    length: 'max',
    nullable: true,
    name: 'Recipients',
    transformer: jsonTransformer,
  })
  recipients: string[];

  @Column({
    default: true,
    name: 'IsActive',
  })
  isActive: boolean;

  @Column({
    name: 'CreatedBy',
    length: 50,
  })
  createdBy: string;

  @Column({
    name: 'UpdatedBy',
    length: 50,
  })
  updatedBy: string;

  @CreateDateColumn({ name: 'CreatedAt' })
  createdAt: Date;

  @UpdateDateColumn({ name: 'UpdatedAt' })
  updatedAt: Date;

  @Column({ name: 'EmailTemplateTypeId' })
  emailTemplateTypeId: number;

  @ManyToOne(
    () => EmailTemplateType,
    emailTemplateType => emailTemplateType.emailTemplates,
  )
  @JoinColumn({ name: 'EmailTemplateTypeId' })
  emailTemplateType: EmailTemplateType;
}
