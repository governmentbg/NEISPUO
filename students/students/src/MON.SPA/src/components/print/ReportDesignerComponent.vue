<template>
  <!-- Елементът трябва да има стил за позиция, иначе не се показва. Това е причината да има id. -->
  <div
    id="webReportDesignerId"
    ref="webReportDesigner"
  >
    Печатният образец се зарежда...
  </div>
</template>

<!-- <script src="https://code.jquery.com/jquery-3.3.1.min.js"></script>
 <script src="https://kendo.cdn.telerik.com/2020.1.114/js/kendo.all.min.js"></script>
 <script src="https://localhost:44398/api/reportdesigner/resources/js/telerikReportViewer/"></script>
 <script src="https://localhost:44398/api/reportdesigner/designerresources/js/webReportDesigner/"></script> -->

<script>
    import { config } from '@/common/config.js';
    // Съществуват няколко npm пакета с различни теми за Kendo компонентите.
    // Те са бъгави по отношение на report viewer-а, но Telerik изпратиха кръпки (виж style най-долу).
    // Тези пакети съдържат sass-based теми. Това било по-модерно от less-based темите, но и двата вида се поддържат актуални от Telerik.
    // През 2021-01 махнахме less-based css-ите от index.html и включихме темата чрез следния пакет (и кръпките най-долу).
    import '@progress/kendo-theme-material';
    // Дефинира символите $ и jQuery и зарежда telerikReportViewer.kendo.min.js, за да сработи telerikReportViewer.js.
    import '@/assets/designer/initExtDepsDesigner';
    // Превод на български език на повечето текстове в Telerik report viewer-а.
    import { WebReportDesignerStringsBase } from './TelerikReportDesignerStringResources.bg.js';

    export default {
        name: "ReportDesignerComponent",
        props:{
            id: {
                type: Number,
                required: true
            }
        },
        created(){
            // let jqueryScript = document.createElement('script');
            // jqueryScript.setAttribute('src', 'https://code.jquery.com/jquery-3.3.1.min.js');
            // document.head.appendChild(jqueryScript);
            // let kendoScript = document.createElement('script');
            // kendoScript.setAttribute('src', 'https://kendo.cdn.telerik.com/2020.1.114/js/kendo.all.min.js');
            // document.head.appendChild(kendoScript);

            let telerikScript = document.createElement('script');
            telerikScript.setAttribute('src', `${config.reportDesignerBaseUrl}/resources/js/telerikReportViewer/`);
            document.head.appendChild(telerikScript);
            let reportScript = document.createElement('script');
            reportScript.setAttribute('src', `${config.reportDesignerBaseUrl}/designerresources/js/webReportDesigner/`);
            document.head.appendChild(reportScript);
        },
        mounted() {

          jQuery.ajaxPrefilter(function (options, originalOptions, jqXHR) {
              jqXHR.setRequestHeader('Authorization', 'Bearer ' + user?.access_token);
            });

            // overload the fetch method
            const fetchOverride = window.fetch;
            window.fetch = function (url, args) {
              // Get the parameter in arguments
              if (!args) {
                args = {
                  headers: {
                    Authorization: 'Bearer ' + user?.access_token
                  }
                };
              } else if (!args.headers) {
                args.headers = {
                  Authorization: 'Bearer ' + user?.access_token
                };
              } else {
                args.headers.Authorization = 'Bearer ' + user?.access_token;
              }

              // Intercept the parameter here
              return fetchOverride(url, args);
            };

            const user = this.$store.getters.user;
            var data = this;

             setTimeout(
                 function () {
                  localStorage.setItem("RestoreReports", "false");
                  localStorage.removeItem("PreviouslyOpenedReports");
                  localStorage.removeItem("LastOpenedReport");

                    $("#webReportDesignerId").telerik_WebReportDesigner({
                    toolboxArea: {
                        layout: "list" //Change to "grid" to display the contents of the Components area in a flow grid layout.
                    },
                    serviceUrl: `${config.reportDesignerBaseUrl}`,
                    report: String(data.id) + ".trdp",//String(data.id),
                    // ReportDesigner-a не поддържа authenticationToken, затова се слага prefetch event, който препопълва данните
                    // authenticationToken: user?.access_token,
                    error: (e, args) => {
                        console.log(e, args);
                    }
                })
                .data("telerik_WebDesigner");

                // Повечето текстове се подменят с български вариант. Идеята е взета от:
                 window.telerikWebDesignerResources = new WebReportDesignerStringsBase();

            }, 5000);
        }
    };
</script>

<style scoped>
#webReportDesignerId {
    position: relative;
 width:100%;
 height:800px;
 /* margin:0;
 left:0;
 right:0; */
 /*! top:0; *//*! bottom:0; */
}
</style>
