<template>
  <div class="hello">
    <div
      id="reportViewerElementId"
      ref="reportViewerElement"
    >
      loading...
    </div>
  </div>
</template>


<script>
//import '../../assets/ReportViewer/js/telerikReportViewer-14.1.20.721.min.js';
// import { StringResources } from "./TelerikReportViewerStringResources";

// import {
//   ReportViewer,
//   TelerikReportViewerOptions,
// } from "@progress/telerik-angular-report-viewer/dist/dependencies/telerikReportViewer";

export default {
  name: "PrintComponentVue",
  data() {
    return {
      id: Number,
      reportName: String,
    };
  },
  mounted() {
    this.$nextTick(function () {
      $("#reportViewerElement").telerik_ReportViewer({
        serviceUrl: "https://localhost:44398/api/reports/",
        reportSource: {
          report: "Test",
          parameters: { id: 1 },
        },
        scaleMode: 'SPECIFIC',
        scale: 1.0,
        sendEmail: { enabled: true },
      });
    });
  },
};
</script>


<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
/* Ако не се укажат всички тези стилови параметри, report viewer-ът изобщо не се показва. */
#reportViewerElementId {
  position: absolute;
  top: 0;
  bottom: 0;
  left: 0;
  right: 0;
  /*overflow: hidden;
        clear: both;*/
}
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
