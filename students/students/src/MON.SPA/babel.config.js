const path = require('path');

module.exports = {
  presets: [
    '@vue/cli-plugin-babel/preset'
  ],

 // Exclude from transpilation
//  exclude: [
//   path.resolve(__dirname, './src/assets/designer/kendo.all.min.js')
// ],
ignore: [
  path.resolve(__dirname, './src/assets/designer/kendo.all.min.js')
],
};
