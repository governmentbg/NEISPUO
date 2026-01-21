<template>
  <div>
    <grid
      v-if="schoolYearLoaded"
      :ref="'regBookListGrid' + _uid"
      v-model="selectedItems"
      url="/api/diploma/regBookList"
      :headers="headers"
      :title="title"
      :file-export-name="title"
      :file-auto-fit-columns="true"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        regBookType: regBookType,
        year: year,
        basicDocumentId: basicDocumentId,
        institutionId: selectedInstitutionId,
        personNameFilter: customFilter.personNameFilter
          ? customFilter.personNameFilter.filter
          : null,
        personNameFilterOp: customFilter.personNameFilter
          ? customFilter.personNameFilter.op
          : null,
        seriesFilter: customFilter.seriesFilter
          ? customFilter.seriesFilter.filter
          : null,
        seriesFilterOp: customFilter.seriesFilter
          ? customFilter.seriesFilter.op
          : null,
        registrationNumberTotalFilter: customFilter.registrationNumberTotalFilter
          ? customFilter.registrationNumberTotalFilter.filter
          : null,
        registrationNumberTotalFilterOp: customFilter.registrationNumberTotalFilter
          ? customFilter.registrationNumberTotalFilter.op
          : null,
        registrationNumberYearFilter: customFilter.registrationNumberYearFilter
          ? customFilter.registrationNumberYearFilter.filter
          : null,
        registrationNumberYearFilterOp: customFilter.registrationNumberYearFilter
          ? customFilter.registrationNumberYearFilter.op
          : null,

        factoryNumberFilter: customFilter.factoryNumberFilter
          ? customFilter.factoryNumberFilter.filter
          : null,
        factoryNumberFilterOp: customFilter.factoryNumberFilter
          ? customFilter.factoryNumberFilter.op
          : null,
        institutionIdFilter: customFilter.institutionIdFilter
          ? customFilter.institutionIdFilter.filter
          : null,
        institutionIdFilterOp: customFilter.institutionIdFilter
          ? customFilter.institutionIdFilter.op
          : null,
        basicDocumentTypeFilter: customFilter.basicDocumentTypeFilter
          ? customFilter.basicDocumentTypeFilter.filter
          : null,
        basicDocumentTypeFilterOp: customFilter.basicDocumentTypeFilter
          ? customFilter.basicDocumentTypeFilter.op
          : null,
        schoolYearFilter: customFilter.schoolYearFilter
          ? customFilter.schoolYearFilter.filter
          : null,
        schoolYearFilterOp: customFilter.schoolYearFilter
          ? customFilter.schoolYearFilter.op
          : null,
      }"
      :ref-key="isInStudentLayout ? null : refKey"
      :debounce="1500"
      @pagination="clearSelections"
    >
      <template #topAppend>
        <v-row
          dense
          class="pt-3 pr-4"
        >
          <v-spacer />
          <button-group>
            <v-btn
              v-if="basicDocumentId && year"
              small
              color="success"
              outlined
              @click="exportData"
            >
              <v-icon
                left
                color="success"
              >
                fas fa-file-excel
              </v-icon>
              {{ $t("buttons.export") }}
            </v-btn>
            <v-btn
              small
              color="error"
              outlined
              @click="clearFilters"
            >
              <v-icon
                left
                color="error"
              >
                fas fa-times
              </v-icon>
              {{ $t("buttons.clear") }}
            </v-btn>
          </button-group>
        </v-row>
      </template>
      <template #subtitle>
        <v-row dense>
          <v-col
            cols="12"
            sm="4"
            lg="2"
          >
            <school-year-selector
              v-model="year"
              item-text="value"
              class="required"
              hide-details
            />
          </v-col>
          <v-col
            cols="12"
            sm="8"
            lg="10"
          >
            <autocomplete
              v-model="basicDocumentId"
              api="api/diploma/getRegBookBasicDocuments"
              :label="$t('diplomas.basicDocumentTypeName')"
              :defer-options-loading="false"
              clearable
              class="required"
              :filter="{ regBookType: regBookType }"
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
          >
            <autocomplete
              v-model="selectedInstitutionId"
              :defer-options-loading="false"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              clearable
              hide-details
              class="required"
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.registrationDate`]="{ item }">
        {{
          item.registrationDate
            ? $moment(item.registrationDate).format(dateFormat)
            : ""
        }}
      </template>

      <template v-slot:[`item.canceled`]="{ item }">
        <v-chip
          :color="!item.canceled === true ? 'success' : 'error'"
          outlined
          small
        >
          <yes-no :value="item.canceled" />
        </v-chip>
      </template>

      <template v-slot:[`item.originalRegistrationDate`]="{ item }">
        {{
          item.originalRegistrationDate
            ? $moment(item.originalRegistrationDate).format(dateFormat)
            : ""
        }}
      </template>

      <template #footerPrepend>
        <v-btn
          v-if="validationError"
          color="error"
          small
          @click="errorsDialog = true"
        >
          <v-icon
            left
            color="white"
          >
            mdi-alert-circle-outline
          </v-icon>
          Виж детайли на последната грешка
        </v-btn>
      </template>

      <template v-slot:[`item.personalId`]="{ item }">
        {{ `${item.personalId} - ${item.personalIdTypeName}` }}
      </template>

      <template v-slot:[`item.tags`]="{ item }">
        <v-chip
          v-for="(tag, index) in item.tags"
          :key="index"
          :color="tag.color || 'light'"
          small
          class="ma-1"
        >
          {{ $t(tag.localizationKey) }}
        </v-chip>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="diplomas.generateApplicationFile"
            bottom
            small
            @click="generateApplicationFile(item.item)"
          />
        </button-group>
      </template>

      <template v-slot:[`header.personalId`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.pinFilter" />
      </template>
      <template v-slot:[`header.personFullName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.personNameFilter" />
      </template>
      <template v-slot:[`header.registrationNumberTotal`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.registrationNumberTotalFilter" />
      </template>
      <template v-slot:[`header.registrationNumberYear`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.registrationNumberYearFilter" />
      </template>
      <template v-slot:[`header.series`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.seriesFilter" />
      </template>
      <template v-slot:[`header.factoryNumber`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.factoryNumberFilter" />
      </template>
      <template v-slot:[`header.institutionId`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.institutionIdFilter" />
      </template>
      <template v-slot:[`header.basicDocumentType`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.basicDocumentTypeFilter" />
      </template>
      <template v-slot:[`header.schoolYearName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.schoolYearFilter" />
      </template>
    </grid>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { Permissions, RegBookType } from "@/enums/enums";
import { mapGetters } from "vuex";
import "vue-json-pretty/lib/styles.css";
import TextHeaderFilter from "@/components/wrappers/grid/TextHeaderFilter.vue";

export default {
  name: "RegBookList",
  components: {
    Grid,
    SchoolYearSelector,
    Autocomplete,
    TextHeaderFilter
  },
  props: {
    institutionId: {
      type: Number,
      required: false,
      default: null,
    },
    personId: {
      type: Number,
      default() {
        return null;
      },
    },
    regBookType: {
      type: Number,
      required: true,
      validator(value) {
        return Object.values(RegBookType).includes(value);
      },
      default() {
        return 0;
      }
    },
    isValidation: {
      type: Boolean,
      default() {
        return false;
      },
    },
    title: {
      type: String,
      default() {
        return undefined;
      },
    },
  },
  data() {
    return {
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATEPICKER_FORMAT,
      refKey: "institutionDiplomaList",
      defaultGridFilter: {
        year: null,
      },
      year: null,
      basicDocumentId: null,
      selectedInstitutionId: this.institutionId,
      setAsEditableReason: null,
      RegBookType: RegBookType,
      saving: false,
      selectedBasicDocumentId: null,
      selectedDiplomaBasicDocumentId: null,
      isDuplicate: false,
      selectedBasicClassId: null,
      selectedTemplateId: null,
      defferedLoadingHint: this.$t("common.comboSearchHint", [
        Constants.SEARCH_INPUT_MIN_LENGTH,
      ]),
      schoolYearLoaded: false,
      errorsDialog: false,
      selectedItems: [],
      selectedItemName: '',
      validationError: null,
      selectedBasicDocument: null,
      basicClassOptions: [],
      customFilter: {
        personNameFilter: null,
        seriesFilter: null,
        registrationNumberTotalFilter: null,
        registrationNumberYearFilter: null,
        factoryNumberFilter: null,
        institutionIdFilter: null,
        basicDocumentTypeFilter: null,
        schoolYearFilter: null
      },
      headers: [
        {
          text: this.$t("diplomas.basicDocumentTypeName"),
          value: "basicDocumentAbbr",
        },
        {
          text: this.$t("diplomas.schoolYear"),
          value: "schoolYearName",
        },
        {
          text: this.$t("diplomas.headers.personName"),
          value: "personFullName",
        },
        {
          text: this.$t("diplomas.headers.pin"),
          value: "personalId",
        },
        {
          text: this.$t("diplomas.series"),
          value: "series",
        },
        {
          text: this.$t("diplomas.factoryNumber"),
          value: "factoryNumber",
        },
        {
          text: this.$t("diplomas.additionalDocument.registrationNumber"),
          value: "registrationNumberTotal",
        },
        {
          text: this.$t("diplomas.additionalDocument.registrationNumberYear"),
          value: "registrationNumberYear",
        },
        {
          text: this.$t("diplomas.registrationDate"),
          value: "registrationDate",
          type: "date"
        },
        {
          text: this.$t("institution.details.headers.eduForm"),
          value: "eduForm",
        },
        {
          text: this.$t("diplomas.educationSpecialization"),
          value: "educationSpecialization",
          align: " d-none"
        },
        {
          text: this.$t("diplomas.gpa"),
          value: "gpa",
          align: " d-none"
        },
        {
          text: this.$t("diplomas.originalDiploma.series"),
          value: "originalSeries",
          type: 'text',
          reqBookType: [RegBookType.RegBookQualificationDuplicate,
          RegBookType.RegBookCertificateDuplicate]
        },
        {
          text: this.$t("diplomas.originalDiploma.factoryNumber"),
          value: "originalFactoryNumber",
          type: 'text',
          reqBookType: [RegBookType.RegBookQualificationDuplicate,
          RegBookType.RegBookCertificateDuplicate]
        },
        {
          text: this.$t("diplomas.originalDiploma.registrationNumberTotal"),
          value: "originalRegistrationNumber",
          type: 'text',
          reqBookType: [RegBookType.RegBookQualificationDuplicate,
          RegBookType.RegBookCertificateDuplicate]
        },
        {
          text: this.$t("diplomas.originalDiploma.registrationNumberYear"),
          value: "originalRegistrationNumberYear",
          type: 'text',
          reqBookType: [RegBookType.RegBookQualificationDuplicate,
          RegBookType.RegBookCertificateDuplicate]
        },
        {
          text: this.$t("diplomas.originalDiploma.registrationDate"),
          value: "originalRegistrationDate",
          type: "date",
          reqBookType: [RegBookType.RegBookQualificationDuplicate,
          RegBookType.RegBookCertificateDuplicate]
        },
        {
          text: this.$t("diplomas.canceled"),
          value: "canceled",
          type: "boolean"
        },
        {
          text: this.$t("diplomas.receiverSignature"),
          value: "spacer",
          align: " d-none"
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
    ...mapGetters(['hasPermission', 'hasStudentPermission', 'userInstitutionId']),
    isInStudentLayout() {
      return !!this.personId;
    },
    selectedItemsIds() {
      return this.selectedItems.map((x) => x.id);
    },
    hasDiplomaReadPermission() {
      return (
        this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaRead) ||
        this.hasPermission(
          Permissions.PermissionNameForInstitutionDiplomaRead
        ) ||
        this.hasPermission(Permissions.PermissionNameForAdminDiplomaRead) ||
        this.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead) ||
        this.hasPermission(
          Permissions.PermissionNameForStudentDiplomaByCreateRequestRead
        )
      );
    },
    hasDiplomaManagePermission() {
      return this.personId
        ? this.hasStudentPermission(
          Permissions.PermissionNameForStudentDiplomaManage
        ) ||
        this.hasStudentPermission(
          Permissions.PermissionNameForStudentDiplomaByCreateRequestManage
        )
        : this.hasPermission(
          Permissions.PermissionNameForInstitutionDiplomaManage
        ) ||
        this.hasPermission(
          Permissions.PermissionNameForAdminDiplomaManage
        ) ||
        this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage);
    },
    diplomaListFilter: {
      get() {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {};
        } else {
          return this.defaultGridFilter;
        }
      },
      set(value) {
        if (this.refKey in this.$store.state.gridFilters) {
          this.$store.commit("updateGridFilter", {
            options: value,
            refKey: this.refKey,
          });
        } else {
          return (this.defaultGridFilter = value);
        }
      },
    },
    filteredBasicClassOptions() {
      if (!Array.isArray(this.basicClassOptions) || this.basicClassOptions.length === 0) {
        return [];
      }

      if (this.selectedTemplate && this.selectedTemplate.basicClassId) {
        return this.basicClassOptions.filter(x => x.value === this.selectedTemplate.basicClassId);
      }

      if (this.selectedBasicDocument && Array.isArray(this.selectedBasicDocument.basicClassList)
        && this.selectedBasicDocument.basicClassList.length > 0) {
        return this.basicClassOptions.filter(x => this.selectedBasicDocument.basicClassList.includes(x.value));
      }

      return [];
    }
  },
  async created() {
    if (this.isInStudentLayout) {
      this.schoolYearLoaded = true;
    } else {
      try {
        const currentYear = Number(
          (await this.$api.institution.getCurrentYear(this.institutionId))?.data
        );
        if (!isNaN(currentYear)) {
          this.year = currentYear;
          this.refresh();
        } else {
          this.year = this.$helper.getYear();
        }
      } catch (error) {
        console.log(error);
        this.year = this.$helper.getYear();
      } finally {
        this.schoolYearLoaded = true;
      }
    }
  },
  mounted() {
    this.loadBasicClassOptions();
    switch (this.regBookType) {
      case RegBookType.RegBookQualification:
      case RegBookType.RegBookCertificate:
        return this.headers = this.headers.filter(header => !header.reqBookType);
      default: {
        return this.headers;
      }
    }
  },
  methods: {
    refresh() {
      const grid = this.$refs["regBookListGrid" + this._uid];
      if (grid) {
        grid.get();
      }
    },
    clearFilters() {
      this.diplomaListFilter = this.defaultGridFilter;
      this.year = null;
      this.basicDocumentId = null;
      this.selectedInstitutionId = null;
    },
    async exportData() {
        try {
          this.saving = true;
          await this.$api.diploma.generateRegBookExport(this.year, this.basicDocumentId, this.regBookType).then((response) => {
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `Регистрационна книга.xlsx`);
            document.body.appendChild(link);
            link.click();
          });
        } catch (error) {
          this.$notifier.error("", this.$t("common.loadError"));
          console.log(error.response);
        } finally {
          this.saving = false;
        }
    },
    clearSelections() {
      this.$helper.clearArray(this.selectedItems);
    },
    async generateApplicationFile(item) {
      await this.$api.diploma
        .generateApplicationFile(item.id)
        .then((response) => {
          const url = window.URL.createObjectURL(new Blob([response.data]));
          const link = document.createElement("a");
          link.href = url;
          link.setAttribute("download", `Рег_кн_изд_док_${item.personFullName}.docx`);
          document.body.appendChild(link);
          link.click();
        });
    },
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassOptions({ minId: 1, maxId: 13 })
        .then((result) => {
          if (result) {
            this.basicClassOptions = result.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    }
  },
};
</script>
