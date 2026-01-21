import { UserDTO } from '../../user/routes/user/user.dto';
import { FailedEmailDelivery } from '../failed-email-delivery.entity';

export class FailedEmailDeliveryDTO extends FailedEmailDelivery {
    id?: string;
    sender: string;
    recipient: string;
    subject: string;
    body: string;
    error: string;

    createdAt?: Date;
    updatedAt?: Date;
    user?: UserDTO;
}
