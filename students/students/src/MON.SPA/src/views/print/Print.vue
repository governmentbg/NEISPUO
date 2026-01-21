<template>
  <print-component
    :id="id"
    :left1-margin="left1Margin"
    :top1-margin="top1Margin"
    :left2-margin="left2Margin"
    :top2-margin="top2Margin"
    :report-name="reportName"
  />
</template>

<script>
import PrintComponent from "@/components/print/PrintComponent.vue";
export default {
  name: "Print",
  components: {
    PrintComponent,
  },
  props: {
    id: {
      type: String,
      default: "1"
    },
    reportName: {
      type: String,
      default: "Student\\Test"
    },
    left1Margin:{
      type: Number,
      default: 0
    },
    top1Margin:{
      type: Number,
      default: 0
    },
    left2Margin:{
      type: Number,
      default: 0
    },
    top2Margin:{
      type: Number,
      default: 0
    }

  },
};
</script>
