<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-tabs
      v-model="tab"
      color="primary"
    >
      <v-tab
        key="personSearch"
      >
        {{ $t('enroll.studentSearch') }}
      </v-tab>
      <v-tab
        key="personXlsxLoad"
      >
        {{ $t('enroll.personExcelLoad') }}
      </v-tab>
      <v-tabs-items
        v-model="tab"
      >
        <v-tab-item
          key="personSearch"
          eager
        >
          <student-search
            v-model="searchForm"
            :loading="searching"
            :disabled="searching"
            flat
            hide-own-institution-checkbox
            @field-enter-click="onSearch"
          >
            <template #actions>
              <v-btn
                color="primary"
                :disabled="searching"
                small
                @click.stop="onSearch"
              >
                <font-awesome-icon icon="search" />
                {{ $t("buttons.search") }}
              </v-btn>

              <v-btn
                color="error"
                :disabled="searching"
                small
                @click.stop="onReset"
              >
                <v-icon left>
                  fas fa-times
                </v-icon>
                {{ $t("buttons.clear") }}
              </v-btn>
            </template>
          </student-search>
        </v-tab-item>
        <v-tab-item
          key="personXlsxLoad"
          eager
        >
          <v-card-text>
            <v-row dense>
              <v-file-input
                ref="file"
                v-model="file"
                accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                label="Файл"
                show-size
                truncate-length="50"
                prepend-icon="mdi-microsoft-excel"
                @change="clearState"
              />
              <v-spacer />
              <button-group>
                <button-tip
                  icon-name="mdi-import"
                  tooltip="enroll.personExcelLoad"
                  text="buttons.import"
                  small
                  bottom
                  :disabled="!file"
                  @click="submitFile()"
                />
                <button-tip
                  v-if="excelData"
                  icon-name="mdi-details"
                  text="buttons.details"
                  color="secondary"
                  small
                  bottom
                  :disabled="!file"
                  @click="excelDataDialog = true"
                />
                <button-tip
                  icon-name="mdi-cloud-download"
                  tooltip="enroll.downloadSampleExcelFile"
                  text="buttons.download"
                  color="success"
                  small
                  bottom
                  :href="`${spaBaseUrl}download/EnrollmentList.xlsx`"
                />
              </button-group>
            </v-row>
            <v-alert
              v-if="loadStudentsErrorDetails"
              border="bottom"
              colored-border
              type="error"
              elevation="2"
              class="my-2"
            >
              <h4>
                {{ $t('enroll.excelStudentsLoadError') }}
              </h4>
            </v-alert>
            <v-card
              v-if="excelWithoutHeaderData && excelWithoutHeaderData.length > 0"
            >
              <v-card-text>
                <v-simple-table dense>
                  <thead>
                    <tr>
                      <th class="text-left">
                        Вид на идентификатора - 0 ЕГН, 1 ЛНЧ, 2 ИДН
                      </th>
                      <th class="text-left">
                        Идентификатор (колоната трябва да е форматирана като текст)
                      </th>
                      <th class="text-left">
                        Статус
                      </th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="(item, index) in excelWithoutHeaderData"
                      :key="index"
                    >
                      <td>
                        {{ item[0] }}
                      </td>
                      <td>
                        {{ item[1] }}
                      </td>
                      <td>
                        <v-chip
                          v-if="item.length > 2 && item[2].loaded != undefined"
                          :color=" item[2].loaded ? 'success' : 'error'"
                          small
                        >
                          {{ item[2].loaded ? 'Намерен' : 'Не е намерен' }}
                        </v-chip>
                      </td>
                    </tr>
                  </tbody>
                </v-simple-table>
              </v-card-text>
            </v-card>
          </v-card-text>
        </v-tab-item>
      </v-tabs-items>
    </v-tabs>

    <v-card-title>
      {{ $t('enroll.studentsList') }}
    </v-card-title>
    <v-data-table
      :headers="headers"
      :items="value"
      item-key="uid"
      class="elevation-1"
    >
      <template v-slot:[`item.controls`]="{ item }">
        <button-tip
          icon
          icon-name="mdi-close"
          icon-color="error"
          tooltip="buttons.remove"
          bottom
          iclass=""
          small
          @click="onRemoveClick(item.personId)"
        />
      </template>
    </v-data-table>

    <v-row
      v-if="enrollmentForm"
      class="mt-2"
    >
      <v-col
        cols="12"
        md="4"
      >
        <date-picker
          id="enrollmentDate"
          ref="enrollmentDate"
          v-model="enrollmentForm.enrollmentDate"
          :show-buttons="false"
          :scrollable="false"
          :no-title="true"
          :show-debug-data="false"
          :label="$t('documents.admissionDateLabel')"
          :rules="[$validator.required()]"
          class="required"
        />
      </v-col>
    </v-row>

    <v-dialog
      v-model="excelDataDialog"
    >
      <v-card>
        <v-toolbar
          color="info"
          outlined
        >
          <v-btn
            icon
            dark
            @click="excelDataDialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-spacer />
          <v-toolbar-items>
            <v-btn
              dark
              text
              @click="excelDataDialog = false"
            >
              {{ $t('buttons.close') }}
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text>
          <vue-json-pretty
            :data="excelData"
            show-length
            show-line
            show-icon
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            raised
            color="light"
            @click.stop="excelDataDialog = false"
          >
            <v-icon left>
              mdi-close
            </v-icon>
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-overlay :value="loadingStudents">
      <v-row
        justify="center"
      >
        <v-progress-circular
          :value="loadingProgressPercentage"
          color="primary"
          size="128"
          width="13"
        >
          <h2 class="white--text">
            {{ `${loadingProgressCount}/${excelWithoutHeaderData.length}` }}
          </h2>
        </v-progress-circular>
      </v-row>
      <div class="text-center mt-5">
        <h3>{{ loadingStudentDetails }}</h3>
      </div>
    </v-overlay>
  </v-form>
</template>

<script>
import StudentSearch from "@/views/students/StudentSearch.vue";
import { StudentSearchModel } from "@/models/studentSearchModel.js";
import XLSX from 'xlsx';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';
import { config } from '@/common/config';


export default {
  name: "StudentClassEnrollentPersonSelectorComponent",
  components: { StudentSearch, VueJsonPretty },
  props: {
    value: {
      type: Array,
      default() {
        return [];
      },
    },
    enrollmentForm: {
      type: Object,
      default() {
        return null;
      },
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      tab: null,
      searching: false,
      searchForm: new StudentSearchModel(),
      maxSearchResultCont: 10,
      file: null,
      excelData: null,
      excelDataDialog: false,
      loadingStudents: false,
      loadingProgressCount: 0,
      loadingStudentDetails: '',
      loadStudentsErrorDetails: '',
      spaBaseUrl: config.spaBaseUrlRelative,
      headers: [
        {
          text: this.$t("student.headers.identifier"),
          value: "pin",
          sortable: true,
        },
        {
          text: this.$t("student.headers.firstName"),
          value: "firstName",
          sortable: true,
        },
        {
          text: this.$t("student.headers.middleName"),
          value: "middleName",
          sortable: true,
        },
        {
          text: this.$t("student.headers.lastName"),
          value: "lastName",
          sortable: true,
        },
        { text: this.$t("student.headers.age"), value: "age", sortable: false },
        {
          text: this.$t("student.headers.publicEduNumber"),
          value: "publicEduNumber",
          sortable: true,
        },
        {
          text: this.$t("student.headers.district"),
          value: "district",
          sortable: false,
        },
        {
          text: this.$t("student.headers.municipality"),
          value: "municipality",
          sortable: false,
        },
        {
          text: this.$t("student.headers.school"),
          value: "school",
          sortable: false,
        },
        {
          text: "",
          value: "controls",
          filterable: false,
          sortable: false,
          align: "end",
        },
      ],
    };
  },
  computed: {
    // Махат се заглавията на колоните
    excelWithoutHeaderData() {
      if(!this.excelData || !Array.isArray(this.excelData)) {
        return [];
      }

      return this.excelData.filter(x => Array.isArray(x) && x.length > 0 && !isNaN(x[0]));
    },
    loadingProgressPercentage() {
      if (!this.excelWithoutHeaderData || !Array.isArray(this.excelWithoutHeaderData) || this.excelWithoutHeaderData.length === 0) {
        return 0;
      }

      return (this.loadingProgressCount / this.excelWithoutHeaderData.length) * 100;
    }
  },
  methods: {
    async onSearch() {
      this.searching = true;

      const data = await this.loadStudents(this.searchForm);
      if (!data || data.totalCount === 0) {
        this.searching = false;
        return this.$notifier.warn('', this.$t('common.noResults'));
      }

      if (data.totalCount > this.maxSearchResultCont) {
        this.searching = false;
        return this.$notifier.warn('',this.$t('common.maxSearchResultError', [this.maxSearchResultCont]));
      }

      data.items.forEach((item) => {
        if (!this.value.some((x) => x.personId === item.personId)) {
          this.value.push(item);
        }
      });

      this.searching = false;
    },
    async loadStudents(searchForm) {
      try {
        return (await this.$api.student.getBySearch(searchForm)).data;
      } catch (error) {
        this.$notifier.error("", this.$t("errors.studentSearch"));
        console.log(error);
        return null;
      }
    },
    onRemoveClick(personId) {
      if (this.value && this.value.length > 0) {
        const itemIndex = this.value.findIndex(x => x.personId === personId);
        this.value.splice(itemIndex, 1);
      }
    },
    onReset() {
      this.searchForm = new StudentSearchModel();
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
    async submitFile() {
      this.clearState();

      this.excelData = await this.parseExcelFile(this.file);

      if(!this.excelWithoutHeaderData || this.excelWithoutHeaderData.length === 0) {
        return this.$notifier.warn('', this.$t('common.noResults'));
      }

      this.loadingProgressCount = 0;
      this.loadingStudentDetails = '';
      this.loadingStudents = true;

      for (const item of this.excelWithoutHeaderData) {
        this.loadingProgressCount += 1;
        this.loadingStudentDetails = `${item.length > 0 ? item[0] : ''} / ${item.length > 1 ? item[1] : ''}`;
        const searchModel = {
          pinType: item[0],
          pin: item[1],
          exactMatch: true,
          onlyOwnInstitution: false,
          pageIndex: 0,
          pageSize: 10
        };

        const excelDataItems = this.excelData.filter(x => x[0] === searchModel.pinType && x[1] === searchModel.pin);

        const studentData = await this.loadStudents(searchModel);
        if (!studentData || studentData.totalCount === 0) {
          excelDataItems.forEach(item => {
            item.push({
              loaded: false
            });
          });
          continue;
        }
        const studentDataItem = studentData.items[0];
        excelDataItems.forEach(item => {
          item.splice(2, 0, {
            loaded: true,
            details: studentDataItem
          });
        });
      }

      if (this.excelWithoutHeaderData.every(x => x.length > 2 && x[2].loaded != undefined && x[2].loaded === true)) {
        for (const item of this.excelWithoutHeaderData) {
          console.log(item[2]);
          if (!this.value.some((x) => x.personId === item[2].details.personId)) {
            this.value.push(item[2].details);
          }
        }
      } else {
        // Съществува ред/редове без намерен ученик
        this.loadStudentsErrorDetails = this.$t('enroll.excelStudentsLoadError');
      }

      this.loadingStudents = false;
      this.loadingProgressCount = 0;
      this.loadingStudentDetails = '';
    },
    parseExcelFile(file) {
      return new Promise((resolve, reject) => {
        if (!file) resolve(null);
        const reader = new FileReader();

        reader.onload = (e) => {
          const bstr = e.target.result;
          const wb = XLSX.read(bstr, { type: 'binary' });
          const wsname = wb.SheetNames[0];
          const ws = wb.Sheets[wsname];
          const data = XLSX.utils.sheet_to_json(ws, { header: 1 });
          resolve(data);
        };

        reader.onerror = reject;
        reader.readAsBinaryString(file);
      });
    },
    clearState() {
      this.loadStudentsErrorDetails = '';
      this.$helper.clearArray(this.excelData);
      this.$helper.clearArray(this.value);
    }
  },
};
</script>
