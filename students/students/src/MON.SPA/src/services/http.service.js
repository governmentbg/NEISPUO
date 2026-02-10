import AuthService from '@/services/auth.service.js';
import axios from 'axios';
import { config }  from '@/common/config';
import i18n from '@/language/language';
import notifier from '@/plugins/notifier/notifier';
import store from '@/store';

axios.interceptors.request.use(async setup => {
  setup.baseURL = config.apiBaseUrl;
  let authService = new AuthService();
  const user = await authService.getUser();

  if (user) {
    const authToken = user.access_token;
    if (authToken) {
      setup.headers.Authorization = `Bearer ${authToken}`;
    }
  }
  return setup;
},
(err) => {
    console.error(err);
});

  // Add a 401,403 response interceptor
axios.interceptors.response.use(response => {
  // eslint-disable-next-line no-prototype-builtins
  if (response.hasOwnProperty('data') && response.data.hasOwnProperty('isError'))
  {
    if (response.data.isError === true) {
      apiErrorHandler(response.data);
      return response.data;
    }
    else{
      return response.data;
    }
  }
  else{
    return response;
  }
}, function (error) {
  console.log(JSON.stringify(error));
  if(error.response !== undefined){
    if (401 === error.response.status) {
      notifier.toast('', i18n.t('errors.401'), 'error');
      //router.push('/erros/ServiceUnavailable');
      return Promise.reject(error);
    } else if(403 === error.response.status) {
      notifier.toast('', i18n.t('errors.403'), 'error');
      // router.push('/');
      return Promise.reject(error);
    } else if(503 === error.response.status) {
      console.error(JSON.stringify(error.response));
      notifier.modalError(i18n.t('errors.503'), error.response.data);
      //window.location.href = '/errors/ServiceUnavailable';
      return Promise.reject(error);
    // eslint-disable-next-line no-prototype-builtins
    } else if (error.response.hasOwnProperty('isError') && error.response.isError === true) {
      apiErrorHandler(error.response);
      return Promise.reject(error);
    // eslint-disable-next-line no-prototype-builtins
    } else if (error.response.hasOwnProperty('data') && error.response.data.hasOwnProperty('isError') && error.response.data.isError === true) {
      apiErrorHandler(error.response.data);
      return Promise.reject(error);
    } else {
      return Promise.reject(error);
    }
  }
  else{
    throw new Error("Network error.");
  }
});

var apiErrorHandler = (error) => {
  store.commit('addApiError', { date: new Date(), ...error });

  const errors = [];
  if(Array.isArray(error.errors)) {
    if(error.errors.length < 5) {
      // Показваме грешките, ако са по-малко от 5
      error.errors.forEach(e => {
        if (e.message) {
          errors.push(e.message);
        }
      });
    } else {
      // Ако са повече от 5 ги логваме
      errors.push('За повече детайли виж грешкте в профила.');
    }
  }

  notifier.toast(error.message ?? i18n.t("common.error"), errors.join('\r\n'), error.clientNotificationLevel || 'error');
};

export default {
  get: axios.get,
  post: axios.post,
  put: axios.put,
  delete: axios.delete,
  patch: axios.patch,
  all: axios.all
};
