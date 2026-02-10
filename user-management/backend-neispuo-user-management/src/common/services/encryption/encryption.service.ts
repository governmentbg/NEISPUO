const NodeRSA = require('node-rsa');
const crypto = require('crypto');

export class EncryptionService {
    static encryptString(stringToEncrypt: string) {
        const key = new NodeRSA();

        const keyData = `-----BEGIN PUBLIC KEY-----\n${process.env.TELELINK_PUBLIC_KEY.trim()}\n-----END PUBLIC KEY-----`;

        // setOptions ecryptionScheme is default to pkcs1_oaep by setting this to pkcs1. I could able to solve my problem

        key.setOptions({
            encryptionScheme: 'pkcs1',
        });

        key.importKey(keyData, 'pkcs8-public');
        const encrypted = key.encrypt(stringToEncrypt, 'base64');
        return encrypted;
    }
}
