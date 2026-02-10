import { UserEventDto } from './telelink/user-event-dto';

describe('UserEventDto', () => {
    it('should be defined', () => {
        expect(new UserEventDto()).toBeDefined();
    });
});
