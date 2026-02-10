<template>
  <grid
    :url="withImported ? '/api/absence/GetInstitutionsWithImportedData' : '/api/absence/GetInstitutionsWithoutImportedData'"
    file-export-name="Списък с импортирани файлове"
    :headers="headers"
    :title="withImported ? $t('absence.institutionsWithImportedData') : $t('absence.institutionsWithoutImportedData')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    :filter="searchFilter"
  >
    <template
      v-if="withImported"
      #month="item"
    >
      {{ $helper.getMonthName(item.item.month) }}
    </template>

    <template #actions="item">
      <button-group>
        <button-tip
          v-if="withImported && item.item.absenceId"
          icon
          icon-name="mdi-eye"
          icon-color="primary"
          tooltip="buttons.details"
          bottom
          iclass=""
          small
          :to="`/absence/import/${item.item.absenceId}/details`"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>

import Grid from "@/components/wrappers/grid.vue";

export default {
  name: "InstitutionsAbsences",
  components: { Grid },
  props: {
    schoolYear: {
      type: [Number, String],
      default: undefined
    },
    month: {
      type: [Number, String],
      default: undefined
    },
    withImported: {
      type: Boolean,
      required: false,
    },
    title: {
      type: String,
      default() {
        return '';
      }
    },
    applyFilter: {
      type: Boolean,
      required: false
    }
  },
  data() {
    return {
      headers: this.withImported
        ? [
            { text: this.$t("common.institution"), value: "institutionName" },
            { text: this.$t("absence.headers.schoolYear"), value: "schoolYearName" },
            { text: this.$t("absence.headers.month"), value: "month" },
            { text: ' ', value: "controls", sortable: false, align: 'end' },
          ]
        : [
          { text: this.$t("common.institution"), value: "institutionName" },
          { text: ' ', value: "controls", sortable: false, align: 'end' },
        ]
    };
  },
  computed: {
    searchFilter() {
      return this.applyFilter ? { schoolYear: this.schoolYear, month: this.month } : {};
    }
  }
};
</script>
