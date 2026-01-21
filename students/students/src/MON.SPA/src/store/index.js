// import Cookie from 'js-cookie';
// import Crypto from 'crypto-js';
import Vue from 'vue';
import Vuex from 'vuex';
// import VuexPersistence from 'vuex-persist';
import actions from './actions';
import getters from './getters';
import mutations from './mutations';
import state from './state';
// import {uuid} from 'vue-uuid';

// const cookieName = 'neispuo-students';

// const storageKey = 'vuex-neispuo-students';

// // Get the encryption token from cookie or generate a new one.
// const encryptionToken = Cookie.get(cookieName) || uuid.v4();

// // Store the encryption token in a secure cookie.
// Cookie.set(cookieName, encryptionToken, { secure: true, expires: 180, SameSite: "Lax" });







// const vuexLocal = new VuexPersistence({
//   //storage: window.localStorage,
//   storage: {
//     getItem: () => {
//       // Get the store from local storage.
//       const store = window.localStorage.getItem(storageKey);

//       if (store) {
//         try {
//           // Decrypt the store retrieved from local storage
//           // using our encryption token stored in cookies.
//           const bytes = Crypto.AES.decrypt(store, encryptionToken);

//           return JSON.parse(bytes.toString(Crypto.enc.Utf8));
//         } catch (e) {
//           // The store will be reset if decryption fails.
//           window.localStorage.removeItem(storageKey);
//         }
//       }

//       return null;
//     },
//     setItem: (key, value) => {
//       // Encrypt the store using our encryption token stored in cookies.
//       const store = Crypto.AES.encrypt(value, encryptionToken).toString();

//       // Save the encrypted store in local storage.
//       return window.localStorage.setItem(storageKey, store);
//     },
//     removeItem: () => window.localStorage.removeItem(storageKey),
//   },
// });

Vue.use(Vuex);

export default new Vuex.Store({
  // plugins: [vuexLocal.plugin],
  state,
  mutations,
  actions,
  getters
});
