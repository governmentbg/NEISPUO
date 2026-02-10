import { ApiModelProperty } from '@nestjs/swagger';
import { Type } from 'class-transformer';
import { IsArray, IsOptional, IsString } from 'class-validator';
import { DTO } from '../responses/dto.interface';

//@TODO we should refactor interfaces into classes because we cannot anotatate in them, Here i cannot put a swagger anotation
export class ChildSchoolBookCode {
    schoolBookCode: string;

    personalID: string;
}

export class ParentCreateRequestDTO implements DTO {
    @ApiModelProperty()
    @IsString()
    firstName: string;

    @ApiModelProperty()
    @IsString()
    @IsOptional()
    middleName: string;

    @ApiModelProperty()
    @IsString()
    lastName: string;

    @ApiModelProperty()
    @IsString()
    password: string;

    @ApiModelProperty()
    @IsString()
    email: string;

    @ApiModelProperty({
        isArray: true,
    })
    @IsArray()
    @Type(() => ChildSchoolBookCode)
    childrenCodes: ChildSchoolBookCode[];

    personID: number;

    azureID: string;

    publicEduNumber: string;
}
