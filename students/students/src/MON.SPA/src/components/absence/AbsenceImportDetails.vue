<template>
  <v-card>
    <v-card-title>
      {{ $t("absence.detailsTitle") }}
    </v-card-title>

    <v-card-title>
      <v-row
        v-if="model"
        dense
      >
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.schoolYearName"
            :label="$t('absence.headers.schoolYear')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.monthName"
            :label="$t('absence.monthFor')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.totalRecords"
            :label="$t('absence.importedRecordsCount')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.importType"
            :label="$t('absence.headers.importType')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.createDate ? $moment(model.createDate).format(dateTimeFormat): ''"
            :label="$t('absence.headers.createDate')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.creatorUsername"
            :label="$t('absence.headers.creator')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.signedDate ? $moment(model.signedDate).format(dateTimeFormat): ''"
            :label="$t('absence.headers.signedDate')"
            disabled
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
        >
          <v-text-field
            :value="model.signerUsername"
            :label="$t('absence.headers.signer')"
            disabled
          />
        </v-col>
      </v-row>
    </v-card-title>
    <v-card-title>
      {{ $t("absence.importedData") }}
    </v-card-title>

    <v-data-table
      v-if="model && model.records && model.records.length > 0"
      ref="absenceImportedFileDataGrid"
      :headers="headers"
      :items="model.records"
      :search="search"
      :loading="loading"
      :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true }"
      multi-sort
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar flat>
          <GridExporter
            :items="model.records"
            :file-extensions="['xlsx', 'csv', 'txt']"
            file-name="Списък с импортирани файлове"
            :headers="headers"
          />
          <v-spacer />
          <v-text-field
            v-model="search"
            append-icon="mdi-magnify"
            :label="$t('common.search')"
            single-line
            hide-details
          />
        </v-toolbar>
      </template>
    </v-data-table>
  </v-card>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import Constants from "@/common/constants.js";

import { mapGetters } from "vuex";

export default {
  name: "AbsenceImportDetails",
  components: { GridExporter },
  props: {
    absenceImportId: {
      type: Number,
      required: true,
    }
  },
  data() {
    return {
      search: '',
      loading: false,
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      model: null,
      headers: [
        {
          text: this.$t('absence.importFile.headers.institutionCode'),
          value: 'institutionCode',
        },
        {
          text: this.$t('absence.importFile.headers.schoolYear'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('absence.importFile.headers.month'),
          value: 'month',
        },
        {
          text: this.$t('absence.importFile.headers.studentIdentificationType'),
          value: 'studentIdentificationType',
        },
        {
          text: this.$t('absence.importFile.headers.identification'),
          value: 'identification',
        },
        {
          text: this.$t('absence.importFile.headers.firstName'),
          value: 'firstName',
        },
        {
          text: this.$t('absence.importFile.headers.middleName'),
          value: 'middleName',
        },
        {
          text: this.$t('absence.importFile.headers.lastName'),
          value: 'lastName',
        },
        {
          text: this.$t('absence.importFile.headers.class'),
          value: 'class',
        },
        {
          text: this.$t('absence.importFile.headers.classCode'),
          value: 'classCode',
        },
        {
          text: this.$t('absence.importFile.headers.groupCode'),
          value: 'groupCode',
        },
        {
          text: this.$t('absence.importFile.headers.className'),
          value: 'className',
        },
        {
          text: this.$t('absence.importFile.headers.monthlyAbsencesForValidReason'),
          value: 'monthlyAbsencesForValidReason',
        },
        {
          text: this.$t('absence.importFile.headers.monthlyAbsencesForUnvalidReason'),
          value: 'monthlyAbsencesForUnvalidReason',
        },
      ],
    };
  },
  computed:{
    ...mapGetters(['gridItemsPerPageOptions']),
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.absence.getImportDetails(this.absenceImportId)
      .then((response) => {
        if(response.data) {
          this.model = response.data;
        }
      })
      .catch(error => {
          this.$notifier.error('', this.$t('errors.load'));
          console.log(error.response);
      })
      .then(() => {
        this.loading = false;
      });
    },
  },
};
</script>
