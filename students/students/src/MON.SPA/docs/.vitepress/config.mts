import { defineConfig, loadEnv } from 'vitepress'
const env = loadEnv(`${process.env.VITEPRESS_ENV}`, process.cwd(), '')

export default defineConfig({
  title: "Модул Деца и ученици",
  description: "Движение на ученици, ЛОД, Дипломи",
  base: `${env.VITE_BASE}`,
  locales: {
    root: {
      label: 'Български',
      lang: 'bg',
    }
  },
  markdown: {
    container: {
      tipLabel: 'Съвет',
      warningLabel: 'Предупреждение',
      dangerLabel: 'Внимание!',
      infoLabel: 'Информация',
      detailsLabel: 'Детайли'
    }
  },
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    logo: '/logo.png',
    i18nRouting: false,
    nav: [
      { text: 'Начало', link: '/' },
      { text: 'Деца и ученици', link: '/guide/' },
      {
        text: '🖨️',
        noIcon: true,
        link: 'javascript:window.print()',
        target: '_self'
      }
    ],
    search: {
      provider: 'local',
      options: {
        locales: {
          root: {
            translations: {
              button: {
                buttonText: 'Търсене',
                buttonAriaLabel: 'Търсене'
              },
              modal: {
                displayDetails: 'Display detailed list',
                resetButtonTitle: 'Нулирай търсене',
                backButtonTitle: 'Затвори търсене',
                noResultsText: 'Няма резултати за',
                footer: {
                  selectText: 'за избор',
                  selectKeyAriaLabel: 'enter',
                  navigateText: 'преминаване към',
                  navigateUpKeyAriaLabel: 'up arrow',
                  navigateDownKeyAriaLabel: 'down arrow',
                  closeText: 'за затваряне',
                  closeKeyAriaLabel: 'escape'
                }
              }
            }
          }
        }
      }
    },

    sidebar: [
      {
        text: 'Съдържание',
        items: [
          {
            text: 'Начало',
            link: '/guide/home/index.md'
          },
          {
            text: 'Деца и ученици',
            collapsed: false,
            link: '/guide/student/',
            items: [
              { text: 'Създаване на ново дете/ученик', link: '/guide/student/create' },
              { text: 'Търсене на дете/ученик', link: '/guide/student/search' },
              { text: 'Движение на дете/ученик', link: '/guide/student/movement' },
              { text: 'Записване в клас/група', link: '/guide/student/enrollment' },
              { text: 'Записване в неучебна група', link: '/guide/student/additionalEnrollment' },
              { text: 'Записване в неучебна група на друга институция/ЦПЛР', link: '/guide/student/fastEnrollment' },
              { text: 'Учебен план на дете/ученик', link: '/guide/student/curriculum' },
              { text: 'ЛОД', link: '/guide/student/lod' }
                   ]
          },
          {
            text: 'Документи (дипломи, свидетелства, удостоверения)',
            collapsed: false,
            link: '/guide/diploma/'
          },
          {
            text: 'Приключване на ЛОД',
            collapsed: false,
            link: '/guide/home/LODFinalization'
          },
          {
            text: 'Данни за АСП',
            collapsed: false,
            link: '/guide/absence/absence'
          },
          {
            text: 'Данни за здравно осигуряване',
            collapsed: false,
            link: '/guide/healthInsurance/'
          },
        {
            text: 'Локален сървър',
            collapsed: false,
            link: '/guide/localServer/',
            items: [
              { text: 'Разрешаване на проблеми', link: '/guide/localServer/troubleshooting' },
             ]
          }
          ]
      }
    ],
    footer: {
      message: 'МОН - НЕИСПУО',
      copyright: `© 2021-${new Date().getFullYear()}`
    },
    outline: {
      label: "На тази страница"
    },
    docFooter: {
      prev: 'Предишна страница',
      next: 'Следваща страница'
    }
  }
});
