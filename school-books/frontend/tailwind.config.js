const tailwindColors = require('tailwindcss/colors');

const palette = {
  red: {
    50: '#fcd2cf',
    100: '#f45532',
    500: '#df320c',
    DEFAULT: '#df320c',
    700: '#c61a0b',
    900: '#ae0a0a'
  },
  blue: {
    50: '#eaf1fb',
    100: '#6fa1ec',
    500: '#2f73da',
    DEFAULT: '#2f73da',
    700: '#165ecc',
    900: '#002966'
  },
  green: {
    50: '#ddfded',
    100: '#0ac295',
    500: '#09a57f',
    DEFAULT: '#09a57f',
    700: '#078364',
    900: '#027357'
  },
  yellow: {
    50: '#fef7b9',
    100: '#ffda6c',
    500: '#ffb400',
    DEFAULT: '#ffb400',
    700: '#e07c02',
    900: '#c33e01'
  }
};

module.exports = {
  content: ['./projects/**/*.{html,ts}'],
  theme: {
    colors: {
      transparent: 'transparent',
      current: 'currentColor',
      black: tailwindColors.black,
      white: tailwindColors.white,
      gray: tailwindColors.gray,
      green: palette.green,
      red: palette.red,
      blue: palette.blue,
      yellow: palette.yellow,
      sky: tailwindColors.sky,

      primary: palette.blue.DEFAULT,
      success: palette.green.DEFAULT,
      warning: palette.yellow.DEFAULT,
      error: palette.red.DEFAULT
    },
    screens: {
      // breakpoints are mobile first. everything below 768px is considered a phone
      sm: '768px', // tablets in portrait and up
      md: '992px', // tablets in landscape, small desktops and up
      lg: '1200px' // large desktops and up
    },
    fontFamily: {
      sans: ['Roboto', 'Helvetica\\ Neue', 'sans-serif']
    }
  },
  plugins: [],
  important: '#twroot'
};
