<template>
  <report-designer-component 
    :id="id" 
  />
</template>

<script>
import ReportDesignerComponent from "@/components/print/ReportDesignerComponent.vue";
export default {
  name: "ReportDesigner",
  components: {
    ReportDesignerComponent,
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
};
</script>
