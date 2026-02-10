import axios from 'axios';
import { config }  from '@/common/config';

axios.interceptors.request.use(async setup => {
  setup.baseURL = config.apiBaseUrl;
  return setup;
},
(err) => {
  console.error(err);
});

export default {
  get: axios.get,
  post: axios.post,
  put: axios.put,
  delete: axios.delete,
  patch: axios.patch,
  all: axios.all
};