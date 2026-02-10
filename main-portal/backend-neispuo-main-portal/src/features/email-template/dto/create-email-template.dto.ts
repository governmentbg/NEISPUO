import { IsString, IsBoolean, IsNotEmpty, IsInt, IsOptional, IsArray } from 'class-validator';

export class CreateEmailTemplateDto {
  @IsString()
  @IsNotEmpty()
  title: string;

  @IsString()
  @IsNotEmpty()
  content: string;

  @IsInt()
  @IsNotEmpty()
  emailTemplateTypeId: number;

  @IsBoolean()
  isActive: boolean;

  @IsOptional()
  @IsArray()
  @IsString({ each: true })
  recipients?: string[];
}
