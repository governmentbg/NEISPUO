import { IsOptional, IsString } from 'class-validator';
import { ClassEventDto } from './class-event-dto';
import { DeleteClassEventDto } from './delete-class-event-dto';
import { DeleteOrgEventDto } from './delete-org-event-dto';
import { DeleteUserEventDto } from './delete-user-event-dto';
import { EnrollmentEventDto } from './enrollment-event-dto';
import { OrgEventDto } from './org-event-dto';
import { UserEventDto } from './user-event-dto';

export class EventDto {
    @IsString()
    rowID: number;

    @IsString()
    id: string;

    @IsString()
    type: string;

    @IsOptional()
    attributes:
        | OrgEventDto
        | DeleteOrgEventDto
        | ClassEventDto
        | DeleteClassEventDto
        | UserEventDto
        | DeleteUserEventDto
        | EnrollmentEventDto;
}
