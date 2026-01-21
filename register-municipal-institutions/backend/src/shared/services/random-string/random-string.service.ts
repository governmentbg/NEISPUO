import cryptoRandomString from 'crypto-random-string';

export class RandomStringService {
    random(size: number) {
        return cryptoRandomString({ length: size });
    }

    generate(alphabet: string, size: number) {
        return cryptoRandomString({ length: size, characters: alphabet });
    }
}
