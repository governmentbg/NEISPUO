import Crypto from 'crypto-js';
import Cookie from 'js-cookie';
//import {uuid} from 'vue-uuid';

const cookieName = 'auth-neispuo-students';
// Get the encryption token from cookie or generate a new one.
//const encryptionToken = Cookie.get(cookieName) || uuid.v4();
const encryptionToken = '0d010149-3fb1-4223-a7b5-092cc7bd32a2';

// Store the encryption token in a secure cookie.
Cookie.set(cookieName, encryptionToken, { secure: true, expires: 180 });

export class SecureStorageStateStore {
    constructor({prefix = "oidc.", store = window.localStorage} = {}) {
        this._store = store;
        this._prefix = prefix;
    }

    set(key, value) {
        key = this._prefix + key;
        // Encrypt the store using our encryption token stored in cookies.
        const store = Crypto.AES.encrypt(value, encryptionToken).toString();
        console.log(store);

        this._store.setItem(key, store);
        return Promise.resolve();
    }

    get(key) {
        key = this._prefix + key;
        let item = this._store.getItem(key);
        if (item) {
          try {
            // Decrypt the store retrieved from local storage
            // using our encryption token stored in cookies.
            const bytes = Crypto.AES.decrypt(item, encryptionToken);
            return Promise.resolve(bytes.toString(Crypto.enc.Utf8));
          } catch (e) {
            // The store will be reset if decryption fails.
            window.localStorage.removeItem(key);
          }
        }
        else{
          return Promise.resolve();
        }
    }

    remove(key) {
        key = this._prefix + key;
        let item = this._store.getItem(key);
        this._store.removeItem(key);
        return Promise.resolve(item);
    }

    getAllKeys() {
        var keys = [];
        for (let index = 0; index < this._store.length; index++) {
            let key = this._store.key(index);

            if (key.indexOf(this._prefix) === 0) {
                keys.push(key.substr(this._prefix.length));
            }
        }

        return Promise.resolve(keys);
    }
}
