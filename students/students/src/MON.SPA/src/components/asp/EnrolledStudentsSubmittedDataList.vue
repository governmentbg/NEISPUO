<template>
  <grid
    v-if="hasReadPermission"
    :ref="'enrolledStudentSubmittedDataGrid' + _uid"
    url="/api/asp/enrolledStudentsSubmittedDataList"
    :headers="headers"
    :title="$t('menu.asp.enrolledStudentSubmittedDataHistory')"
    :filter="{ year: year, month: month, exportTypeCode: exportTypeCode, aspStatus: aspStatus }"
    multi-sort
  >
    <template #subtitle>
      <v-row dense>
        <v-col>
          <school-year-selector
            v-model="year"
            item-text="value"
            :label="$t('common.year')"
          />
        </v-col>
        <v-col>
          <custom-month-picker
            v-model="month"
          />
        </v-col>
        <v-col>
          <v-select
            v-model="exportTypeCode"
            :items="exportTypeCodeOptions"
            clearable
            label="Вид подадени данни"
          />
        </v-col>
        <v-col>
          <v-select
            v-model="aspStatus"
            :items="aspStatusOptions"
            clearable
            label="Статус АСП"
          />
        </v-col>
      </v-row>
    </template>
  </grid>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import CustomMonthPicker from "@/components/wrappers/CustomMonthPicker";
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'EnrolledStudentsSubmittedDataList',
  components: { Grid, CustomMonthPicker, SchoolYearSelector },
  data() {
    return {
      year: null,
      month: null,
      exportTypeCode: null,
      aspStatus: null,
      headers: [
        {
          text: this.$t('common.year'),
          value: "year",
        },
        {
          text: this.$t('common.month'),
          value: "monthName",
        },
        {
          text: ' ',
          value: "exportTypeCode",
        },
        {
          text: this.$t('asp.headers.institutionCode'),
          value: "institutionCode",
        },
        {
          text: this.$t('absence.importFile.headers.identification'),
          value: "personalId",
        },
        {
          text: this.$t('absence.importFile.headers.studentIdentificationType'),
          value: "personalIdType",
        },
        {
          text: this.$t('absence.importFile.headers.firstName'),
          value: "firstName",
        },
        {
          text: this.$t('absence.importFile.headers.middleName'),
          value: "middleName",
        },
        {
          text: this.$t('absence.importFile.headers.lastName'),
          value: "lastName",
        },
        {
          text: this.$t('absence.importFile.headers.educationForm'),
          value: "studentEduFormId",
        },
        {
          text: this.$t('absence.importFile.headers.class'),
          value: "basicClassId",
        },
        {
          text: this.$t('absence.importFile.headers.aspStatus'),
          value: "status",
        },
      ],
      exportTypeCodeOptions: [
        { value: 'MONZP', text: "MONZP"},
        { value: 'MONZPC', text: "MONZPC"}
      ],
      aspStatusOptions: [
        { value: 1, text: "1"},
        { value: 2, text: "2"},
        { value: 3, text: "3"}
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userDetails']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForASPEnrolledStudentsExport);
    },
  },
  methods: {
    gridReload() {
    const grid = this.$refs['enrolledStudentSubmittedDataGrid' + this._uid];
    if(grid) {
      grid.get();
    }
  },
  }
};
</script>
