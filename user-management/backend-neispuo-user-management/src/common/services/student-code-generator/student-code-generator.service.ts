import { customAlphabet } from 'nanoid';
import { CONSTANTS } from 'src/common/constants/constants';

export class StudentCodeGeneratorService {
    static getCode() {
        const nanoid = customAlphabet(CONSTANTS.NANOID_CUSTOM_ALPHABET, 8);
        return nanoid();
    }
}
