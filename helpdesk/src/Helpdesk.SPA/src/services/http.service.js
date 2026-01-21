import axios from 'axios';
import { config }  from '@/common/config';
import i18n from '@/language/language';
import notifier from '@/plugins/notifier/notifier';
import AuthService from '@/services/auth.service.js';
import router from '@/router';

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
    if (response.data.isError) {
      notifier.modalError(i18n.t("common.error"), i18n.t(response.data.message));
    }
    else{
      return response.data;
    }
  }
  else{
    return response;
  }
}, function (error) {
  if(error.response !== undefined){
    if (401 === error.response.status) {
      //notifier.modalError('', i18n.t('errors.accessDenied'), 'error');
      router.push('/errors/AccessDenied');
      return Promise.reject(error);
    } else if(403 === error.response.status) {
      notifier.toast('', i18n.t('errors.403'), 'error');
      // router.push('/');
      return Promise.reject(error);
    } else {
      return Promise.reject(error);
    }
  }
  else{
    throw new Error("Network error.");
  }
});

export default {
  get: axios.get,
  post: axios.post,
  put: axios.put,
  delete: axios.delete,
  patch: axios.patch,
  all: axios.all
};
