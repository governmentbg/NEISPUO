<template>
  <absence-import-details :absence-import-id="absenceImportId" />
</template>

<script>
import AbsenceImportDetails from "@/components/absence/AbsenceImportDetails";

export default {
  name: "AbsenceImportDetailsView",
  components: { AbsenceImportDetails },
  props: {
    absenceImportId: {
      type: Number,
      required: true,
    },
  },
};
</script>
