<template>
  <v-card>
    <v-card-title>
      {{ $t('asp.enrolledStudentsExportListTitle') }}
      <v-spacer />
      <v-text-field
        v-model="search"
        append-icon="mdi-magnify"
        :label="$t('common.search')"
        single-line
        hide-details
      />
    </v-card-title>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="exportedFiles"
        item-key="id"
        class="elevation-1"
        :loading="loading"
        :search="search"
        :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions }"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="exportedFiles"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :v-for="$t('asp.enrolledStudentsExportTableGridExporterFileName')"
              :headers="headers"
            />
            <school-year-selector
              v-model="selectedSchoolYear"
            /> 
            <v-spacer />
          </v-toolbar>
        </template>

        <template v-slot:[`item.createdDate`]="{ item }">
          {{ item.createdDate ? $moment(item.createdDate).format(dateFormat) : "" }}
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <v-tooltip bottom>
              <template v-slot:activator="{ on: tooltip }">
                <doc-downloader
                  :value="item"
                  show-icon
                  x-small
                  :show-file-name="false"
                  v-on="{ ...tooltip }"
                />
              </template>
              <span>{{ $t('asp.exportedFileDownloadTitle') }}</span>
            </v-tooltip>
          </button-group>
        </template>

        <template slot="body.append">
          <tr>
            <th
              class="title text-right"
              colspan="3"
            >
              Общо {{ exportedFiles.length }} файла
            </th>
          </tr>
        </template>
      </v-data-table>
    </v-card-text>
  </v-card>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import DocDownloader from '@/components/common/DocDownloader.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Constants from "@/common/constants.js";
import { mapGetters } from "vuex";

export default {
  name: "EnrolledStudentsExportList",
  components: {
    GridExporter,
    DocDownloader,
    SchoolYearSelector
  },
  data() {
    return {
      search: "",
      exportedFiles: [],
      loading: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      selectedSchoolYear: null,
      headers: [
        {
          text: this.$t('asp.headers.schoolYear'),
          value: "schoolYear",
        },
        {
          text: this.$t('asp.headers.month'),
          value: "month",
        },
        {
          text: this.$t('asp.enrolledStudentsExportHeaders.createdDate'),
          value: "createdDate",
        },
        {
          text: this.$t('asp.headers.recordsCount'),
          value: "recordsCount",
        },
        { text: "", value: "actions", sortable: false },
      ],
    };
  },
  computed: {
    ...mapGetters(["gridItemsPerPageOptions"]),
  },
  watch: {
    selectedSchoolYear() {
      if(this.selectedSchoolYear !== undefined) {
        this.loadData();
      }
    },
  },
  mounted() {
    this.loadData();
  },
  methods: {
    loadData() {
      this.loading = true;

      this.$api.asp.getEnrolledStudentsExportFiles(this.selectedSchoolYear)
        .then((response) => {
          this.exportedFiles = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.enrolledStudentsExportDetailsLoad'));
          console.log(error);
        })
        .finally(() => {
          this.loading = false;
        });
    }
  }
};
</script>