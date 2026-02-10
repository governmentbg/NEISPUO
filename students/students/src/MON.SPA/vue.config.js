const zlib = require('zlib');
const fs = require('fs');

// git-describe не работи в Docker
// const { gitDescribeSync } = require('git-describe');
process.env.VUE_APP_GIT_HASH = process.env.VERSION; //gitDescribeSync().hash;
process.env.VUE_APP_VERSION = require('./package.json').version;

module.exports = {
  pluginOptions: {
    i18n: {
      locale: 'bg',
      fallbackLocale: 'en',
      localeDir: 'locales',
      enableInSFC: false,
      enableLegacy: false
    },
    compression:{
      brotli: {
        filename: '[file].br[query]',
        algorithm: 'brotliCompress',
        include: /\.(js|css|html|svg|json)(\?.*)?$/i,
        compressionOptions: {
          params: {
            [zlib.constants.BROTLI_PARAM_QUALITY]: 11,
          },
        },
        minRatio: 0.8,
      },
      gzip: {
        filename: '[file].gz[query]',
        algorithm: 'gzip',
        include: /\.(js|css|html|svg|json)(\?.*)?$/i,
        minRatio: 0.8,
      }
    }
  },
    // Името BASE_URL не е случайно. Vue CLI записва стойността на publicPath в process.env.BASE_URL.
    // BASE_URL се вижда/замества в кода подобно на променливите NODE_ENV и VUE_APP_*.
    // https://cli.vuejs.org/guide/mode-and-env.html#using-env-variables-in-client-side-code
    // От друга страна, publicPath се променя за различните среди, затова трябва да се чете от .env файловете.
    // Вместо да измисляме друга променлива, в .env файловете e декларирана именно променлива BASE_URL.
    // Тук тя първо се чете, след което Vue CLI я презаписва със същата стойност. Ако следния ред го нямаше,
    // стойността на BASE_URL от .env файловете щеше да се замаже с '/', защото това е publicPath по подразбиране.
  publicPath: process.env.BASE_URL,
  devServer: {
    host: process.env.HOST || 'localhost',
    open: true,
    port: process.env.PORT || 44357,
    https: {
      key: fs.readFileSync('./ssl/localhost.key'),
      cert: fs.readFileSync('./ssl/localhost.cer')
    }
  },
  transpileDependencies: [
    "vuetify"
  ],
  pwa: {
    name: 'Деца и ученици',
    // configure the workbox plugin
    workboxPluginMode: 'GenerateSW',
 },
  configureWebpack: {
    devtool: "source-map",
    // Error: Cannot use [chunkhash] or [contenthash] for chunk in 'js/[name].[contenthash:8].js' (use [hash] instead)
    // https://github.com/gloriaJun/til/issues/3
    output: {
      filename: process.env.NODE_ENV === 'production' ? '[name].[chunkhash].js' : '[name].[hash].js',
      chunkFilename: process.env.NODE_ENV === 'production' ? '[name].[chunkhash].js' : '[name].[hash].js',
      //filename: 'js/[name].[contenthash:8].js',
      //chunkFilename: 'js/[name].[contenthash:8].js'
      hashFunction : 'sha512'
    }
  }
};
