import { TermEventDto } from './term-event-dto';

export class ClassEventDto {
    id: string;

    azureId: string;

    title: string;

    classCode: string;

    orgId: string;

    term: TermEventDto;
}
