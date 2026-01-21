const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const CopyPlugin = require('copy-webpack-plugin');

module.exports = {
  mode: 'production',
  context: path.join(__dirname, 'src'),
  entry: {
    'api-main': './api/main.ts',
    'api-grades-view': './api/tests/grades-view.ts',
    'api-absences-view': './api/tests/absences-view.ts',
    'api-topics-view': './api/tests/topics-view.ts',
    'extapi-main': './extapi/main.ts'
  },
  output: {
    path: path.join(__dirname, 'dist'),
    libraryTarget: 'commonjs',
    filename: '[name].js'
  },
  resolve: {
    alias: {
      src: path.resolve(__dirname, 'src')
    },
    extensions: ['.ts', '.js']
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: 'babel-loader'
      }
    ]
  },
  target: 'web',
  externals: /^(k6|https?\:\/\/)(\/.*)?/,
  stats: {
    colors: true
  },
  devtool: 'source-map',
  plugins: [
    new CleanWebpackPlugin(),
    new CopyPlugin({
      patterns: [path.resolve(__dirname, 'src/extapi/data/test-data.json')]
    })
  ],
  optimization: {
    // Don't minimize, as it's not used in the browser
    minimize: false
  }
};
