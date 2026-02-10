// Конфигурационни настройки
export const config = {
    // BASE_URL трябва да има вида '/path/' или '/'. Виж също коментара на publicPath във vue.config.js.
    spaBaseUrlRelative: parse(process.env.BASE_URL, '/'),
    //spaBaseUrlAbsolute: `${window.location.origin}${this.spaBaseUrlRelative}`,
    apiBaseUrl: parse(process.env.VUE_APP_API, 'http://localhost:44398'),
    reportBaseUrl: parse(process.env.VUE_APP_API + "Report", 'http://localhost:44398/Report'),
    stsDomain: parse(process.env.VUE_APP_STS_DOMAIN, 'http://localhost:44356'),
    ownUrl: parse(process.env.VUE_APP_OWN_URL, 'http://localhost:44357'),
    version: parse(process.env.VUE_APP_VERSION, '0.0.1'),
    portal: parse(process.env.VUE_APP_PORTAL, 'https://neispuo.mon.bg'),
    remoteSupport: parse(process.env.VUE_APP_REMOTE_SUPPORT, 'https://get.teamviewer.com/neispuo'),
    gitHash: parse(process.env.VUE_APP_GIT_HASH, ''),
    egnAnonymization: parse(process.env.VUE_APP_EGN_ANONYMIZATION, false),
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

