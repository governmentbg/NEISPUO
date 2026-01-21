// Конфигурационни настройки
export const config = {
  // BASE_URL трябва да има вида '/path/' или '/'. Виж също коментара на publicPath във vue.config.js.
  spaBaseUrlRelative: parse(process.env.BASE_URL, '/'),
  apiBaseUrl: parse(process.env.VUE_APP_API, 'https://localhost:44368/'),
  ownUrl: parse(process.env.VUE_APP_OWN_URL, 'https://localhost:44359'),
  version: parse(process.env.VUE_APP_VERSION, '0.0.1'),
  gitHash: parse(process.env.VUE_APP_GIT_HASH, ''),
  mode: parse(process.env.VUE_APP_MODE, 'development'),
};

function parse(value, fallback) {
  if (typeof value === 'undefined') {
    return fallback;
  }

  switch (typeof fallback) {
    case 'boolean':
      return !!JSON.parse(value);
    case 'number':
      return JSON.parse(value);
    default:
      return value;
  }
}

export default {
  config
};
