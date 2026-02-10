import crypto from 'crypto';
import Hashids from 'hashids/cjs';
import md5File from 'md5-file';

export class HashesService {
    md5File(path: string) {
        return md5File(path);
    }

    md5(value: string) {
        return crypto
            .createHash('md5')
            .update(value)
            .digest('hex');
    }

    sha256(string: string) {
        return crypto
            .createHash('sha256')
            .update(string)
            .digest('hex');
    }

    hashid(uuid: string) {
        /**
         * numbers - predefined number as we will always add uuid strings and the number does not need to change
         * minimumLength - minimum outoupt lenght of the hash output
         * alphabet - include uppercase without O and digits without 0 to avoid confusion
         */
        const numbers = [1];
        const length = 5;
        const alphabet = 'ABCDEFGHIJKLMNPQRSTUVWXYZ123456789';
        const hashids = new Hashids(uuid, length, alphabet);
        return hashids.encode(numbers);
    }
}
