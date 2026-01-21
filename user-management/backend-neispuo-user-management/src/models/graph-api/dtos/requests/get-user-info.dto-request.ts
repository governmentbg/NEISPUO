import { IsOptional, IsString } from 'class-validator';

export class GetUserInfoRequestDto {
    @IsOptional()
    @IsString()
    publicEduNumber?: string;

    @IsOptional()
    @IsString()
    azureId?: string;
}
