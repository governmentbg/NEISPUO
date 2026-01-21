<template>
  <!-- Елементът трябва да има стил за позиция, иначе не се показва. Това е причината да има id. -->
  <div
    id="reportViewerElementId"
    ref="reportViewerElement"
  >
    Печатният образец се зарежда...
  </div>
</template>

<script>
    import { config } from '@/common/config.js';
    // Съществуват няколко npm пакета с различни теми за Kendo компонентите.
    // Те са бъгави по отношение на report viewer-а, но Telerik изпратиха кръпки (виж style най-долу).
    // Тези пакети съдържат sass-based теми. Това било по-модерно от less-based темите, но и двата вида се поддържат актуални от Telerik.
    // През 2021-01 махнахме less-based css-ите от index.html и включихме темата чрез следния пакет (и кръпките най-долу).
    import '@progress/kendo-theme-material';
    // Дефинира символите $ и jQuery и зарежда telerikReportViewer.kendo.min.js, за да сработи telerikReportViewer.js.
    import '@progress/telerik-angular-report-viewer/dist/dependencies/initExtDeps';
    // В проекта е добавен npm пакет с Angular компонента, която опакова jQuery-based report viewer-а.
    // Следният ред зарежда директно базовия telerikReportViewer.js без да ползва Angular компонентата.
    // Това е обсолютно същият скрипт, който би се ползвал с jQuery.
    import {
        // TelerikReportViewerOptions,
        ReportViewer
    } from '@progress/telerik-angular-report-viewer/dist/dependencies/telerikReportViewer';
    // Превод на български език на повечето текстове в Telerik report viewer-а.
    import { StringResources } from './TelerikReportViewerStringResources';

    export default {
        name: "PrintComponent",
        props:{
            id: {
                type: String,
                default() {
                    return null;
                }
            },
            reportName: {
                type: String,
                default() {
                    return 'Student\\ThirdAndFifthClass\\ThirdAndFifthClassRelocationDocument';
                }
            },
            top1Margin: {
              type: Number,
              default(){
                return 0;
              }
            },
            left1Margin: {
              type: Number,
              default(){
                return 0;
              }
            },
            top2Margin: {
              type: Number,
              default(){
                return 0;
              }
            },
            left2Margin: {
              type: Number,
              default(){
                return 0;
              }
            },
            isDuplicate: {
              type: Boolean,
              default(){
                return false;
              }
            }
        },
        mounted() {
          const user = this.$store.getters.user;

          const options =  {
              serviceUrl: config.reportBaseUrl,
              reportSource: {
                  report: this.reportName,
                  parameters: { id: this.id, left1Margin: this.left1Margin, top1Margin: this.top1Margin,
                    left2Margin: this.left2Margin, top2Margin: this.top2Margin, isDuplicate: this.isDuplicate }
              },
              authenticationToken: user?.access_token,
              scaleMode: 'SPECIFIC', // По подразбиране е FIT_PAGE.
              sendEmail: { enabled: true },
              // viewMode: "PRINT_PREVIEW",
              error: (e, args) => {
                  console.log(e, args);
              }
          };

          const reportViewer = new ReportViewer(this.$refs.reportViewerElement, options);

          // Повечето текстове се подменят с български вариант. Идеята е взета от:
          // https://docs.telerik.com/reporting/angular-report-viewer-localization
          const sr = reportViewer.stringResources;
          Object.assign(sr, StringResources.bulgarian);

          // ✅ Force Monday as first day in all Kendo calendars
          if (window.kendo && window.kendo.culture()) {
            const culture = window.kendo.culture();
            if (culture.calendar) {
              culture.calendar.firstDay = 1; // 0=Sunday, 1=Monday
            }
          }
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
    /* Задават максимална височина 800px, но за момента не се преоразмеряват*/
    #reportViewerElementId {
        position: relative;
        width:100%;
        height:80vh;
    }
    /* Ако не се укажат всички тези стилови параметри, report viewer-ът изобщо не се показва. */
/*    #reportViewerElementId {
        position: absolute;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
      }*/
</style>

<!--
    Следните кръпки поправят визуални дефекти в report viewer-а при използване на material sass темата от пакета @progress/kendo-theme-material.
    Секцията е глобална, защото няма ефект ако е scoped.
    Кръпките са изпратени от Telerik по наш сигнал: https://www.telerik.com/forums/telerik-report-toolbar-missing-with-kendo-sass-themes
-->
<style>
    /*
        align values
        Клас .k-splitter по подразбиране задава line-height: 2; Това измества данните в report viewer-а много надолу.
        Предложението от Telerik беше класът да се override-не така: .k-splitter { line-height: inherit; }
        Това върши работа в изолиран проект, но в този проект Vuetify задава централно .v-application { line-height: 1.5; }.
        Това също измества данните, макар и по-малко. Крайното решение е line-height да се върне на default-ното за browser-а.
    */
    .k-splitter {
        line-height: normal;
    }

    /* aligns icons */
    .k-menu-link {
        display: inherit;
    }

    /* fixes the dropdown popup of the export button */
    #trv-main-menu-export-command {
        position: relative;
    }

    /* loading message background */
    .trv-pages-area > .trv-error-pane > .trv-centered {
        background-color: white;
        border: 1px solid lightgrey;
    }
</style>
