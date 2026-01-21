import type { Theme } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import './style.css'

export default {
  extends: DefaultTheme,

  enhanceApp({ app, siteData }) {
    // Регистрация на custom компоненти
  }
} satisfies Theme
