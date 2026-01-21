import { jsonTransformer } from 'src/shared/utils/json-transformer';
import { Column, Entity, OneToMany, PrimaryGeneratedColumn } from 'typeorm';
import { EmailTemplate } from '../email-template/email-template.entity';
import { VariableMapping } from 'src/shared/interfaces/variable-mapping.interface';

@Entity({
  schema: 'portal',
  name: 'EmailTemplateType',
})
export class EmailTemplateType {
  @PrimaryGeneratedColumn({ name: 'Id' })
  id: number;

  @Column({
    length: 100,
    unique: true,
    name: 'DisplayName',
  })
  displayName: string;

  @Column({
    length: 200,
    unique: true,
    name: 'ContentProvider',
  })
  contentProvider: string;

  @Column({
    type: 'nvarchar',
    length: 'max',
    name: 'Description',
  })
  description?: string;

  @Column({
    type: 'nvarchar',
    length: 'max',
    name: 'VariableMappings',
    transformer: jsonTransformer,
  })
  variableMappings: VariableMapping[];

  @OneToMany(
    () => EmailTemplate,
    emailTemplate => emailTemplate.emailTemplateType,
  )
  emailTemplates: EmailTemplate[];
}
