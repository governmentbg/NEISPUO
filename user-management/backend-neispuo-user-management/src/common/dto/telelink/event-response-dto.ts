import { EventActionDto } from './event-action-dto';

export class EventResponseDto {
    id: string;

    type: string;

    actionsTriggered: EventActionDto[];
}
