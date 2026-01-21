import { IsNotEmpty, IsOptional } from 'class-validator';
import { Type } from 'class-transformer';

export class UserGuideDTO {
  @IsNotEmpty()
  name: string;

  @IsNotEmpty()
  @Type(() => Number)
  category: number;

  @IsOptional()
  filename?: string;

  @IsOptional()
  mimeType?: string;

  @IsOptional()
  fileContent?: Buffer;

  @IsOptional()
  URLOverride?: string;
}
