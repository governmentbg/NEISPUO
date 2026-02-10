<template>
  <div>
    <v-card
      v-if="!!hasEditPermission"
      class="mb-5"
    >
      <v-card-title>
        {{ $t("basicDocumentSequence.generateTitle") }}
      </v-card-title>
      <v-card-subtitle>
        <v-alert
          border="bottom"
          colored-border
          type="info"
          elevation="2"
        >
          {{ $t('basicDocumentSequence.information') }}
        </v-alert>
      </v-card-subtitle>
      <v-card-text>
        <v-row dense>
          <v-col
            cols="6"
            sm="6"
          >
            <autocomplete
              v-model="basicDocument"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('diplomas.basicDocumentTypeName')"
              :defer-options-loading="false"
              clearable
              class="required"
              :filter="{ isAppendix: false, isRuoDoc: isRuo, filterByDetailedSchoolType: true }"
            />
          </v-col>
          <v-col
            cols="2"
            sm="2"
          >
            <v-text-field
              v-model="count"
              :label="$t('basicDocumentSequence.count')"
              clearable
            />
          </v-col>
          <v-col
            cols="2"
            sm="2"
          >
            <date-picker
              v-model="registrationDate"
              :label="$t('basicDocumentSequence.registrationDate')"
            />
          </v-col>
          <v-col
            cols="2"
            sm="2"
          >
            <v-btn
              :disabled="!basicDocument"
              color="primary"
              @click="generate()"
            >
              {{ $t('basicDocumentSequence.generate') }}
            </v-btn>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card class="mb-1">
      <v-card-text>
        <v-row dense>
          <v-col
            cols="12"
            md="3"
          >
            <school-year-selector
              v-model="year"
              item-text="value"
              hide-details
              class="required"
            />
          </v-col>
          <v-col
            cols="12"
            md="9"
          >
            <autocomplete
              v-model="institutionId"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              :disabled="!!userInstitutionId"
              :defer-options-loading="false"
              clearable
              hide-details
              :filter="{
                regionId: userRegionId
              }"
              class="required"
            />
          </v-col>
        </v-row>
        <v-row dense>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="3"
          >
            <v-radio-group
              v-model="regNumType"
              row
              hide-details
            >
              <v-radio
                label="Рег. № на институции"
                :value="1"
              />
              <v-radio
                label="Рег. № на РУО"
                :value="2"
              />
            </v-radio-group>
          </v-col>
          <v-col
            cols="12"
            :md="!!userInstitutionId ? '12' : '9'"
          >
            <autocomplete
              v-model="basicDocuments"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('diplomas.basicDocumentTypeName')"
              :defer-options-loading="false"
              chips
              small-chips
              deletable-chips
              clearable
              multiple
              hide-details
              :filter="{
                filterByDetailedSchoolType: true
              }"
            />
          </v-col>
        </v-row>
        <v-alert
          v-if="!(!!year && !!institutionId)"
          class="mt-5"
          border="left"
          colored-border
          type="warning"
          elevation="2"
          md="12"
        >
          Моля, изберете учебна година и институция.
        </v-alert>
      </v-card-text>
    </v-card>
    <grid
      v-if="schoolYearLoaded && !!year && (!!institutionId || isRuo)"
      :ref="'basicDocumentSequencesGrid' + _uid"
      url="/api/basicDocument/GetBasicDocumentSequences"
      :headers="headers"
      :title="$t('basicDocumentSequence.title')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        year: year,
        basicDocuments:
          basicDocuments && basicDocuments.length > 0
            ? basicDocuments.join('|')
            : '',
        regNumType: regNumType,
        institutionId: institutionId
      }"
    >
      <template v-slot:[`item.regDate`]="{ item }">
        {{
          item.regDate ? $moment(item.regDate).format(dateFormat) : ""
        }}
      </template>
      <template v-slot:[`item.isUsed`]="{ item }">
        <v-chip
          :color="item.isUsed === true ? 'success' : 'error'"
          small
        >
          <yes-no
            :value="item.isUsed"
          />
        </v-chip>
      </template>
      <template v-slot:[`item.actions`]="{ item }">
        <button-group>
          <template>
            <button-tip
              v-if="item.personId"
              icon
              icon-name="mdi-eye"
              icon-color="primary"
              tooltip="buttons.review"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${item.personId}/details`"
            />
            <button-tip
              v-if="!item.personId"
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              iclass=""
              small
              bottom
              @click="deleteItem(item)"
            />
          </template>
        </button-group>
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
import Grid from '@/components/wrappers/grid.vue';
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import { UserRole } from '@/enums/enums';
import Constants from "@/common/constants.js";
import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";

export default {
  name: 'BasicDocumentSequencesList',
  components: {
    Grid,
    Autocomplete,
    SchoolYearSelector,
  },
  data() {
    return {
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATE_FORMAT,
      saving: false,
      count: 1,
      basicDocument: null,
      registrationDate: null,
      year: null,
      basicDocuments: null,
      regNumType: null,
      institutionId: null,
      schoolYearLoaded: false,
      customFilter: {
        nameFilter: null,
        institutionIdFilter: null,
        fullNameFilter: null,
        basicDocumentTypeFilter: null,
        schoolYearFilter: null,
      },
      headers: [
        {text: this.$t('basicDocumentSequence.headers.institution'), value: "institutionId", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.institutionName'), value: "institutionName", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.regDate'), value: "regDate", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.schoolYear'), value: "schoolYear", sortable: false},
        {text: this.$t('basicDocumentSequence.headers.regNumberTotal'), value: "regNumberTotal", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.regNumberYear'), value: "regNumberYear", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.basicDocumentName'), value: "basicDocumentName", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.isUsed'), value: "isUsed", sortable: true},
        {text: this.$t('basicDocumentSequence.headers.fullName'), value: "fullName", sortable: true},
        {text: '', value: 'actions', inFavourOfIdentifier: false, sortable: false, filterable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['permissions', 'userInstitutionId', 'userRegionId', 'isInRole']),
    hasEditPermission() {
      return this.permissions && this.permissions.includes(Permissions.PermissionNameForBasicDocumentSequenceManage);
    },
    isRuo() {
      return (this.isInRole(UserRole.Ruo) || this.isInRole(UserRole.RuoExpert)) && !!this.userRegionId;
    }
  },
  async created() {
    this.institutionId = this.userInstitutionId;
    if(this.isRuo) {
      this.regNumType = 1;
    }

    try {
      const currentYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
      if (!isNaN(currentYear)) {
        this.year = currentYear;
      } else {
        this.year = this.$helper.getYear();
      }
    } catch (error) {
      console.log(error);
      this.year = this.$helper.getYear();
    } finally {
      this.schoolYearLoaded = true;
    }
  },
  methods: {
    async generate(){
      this.saving = true;
      try {
        let response = await this.$api.basicDocument.getNextBasicDocumentSequence(this.basicDocument, this.count, this.$moment.utc(this.registrationDate || new Date()));
         if(response.data){
            let sequences = response.data.map(item => {
              return `${item.regNumberTotal}-${item.regNumberYear} от ${this.$moment(item.regDate).format(this.dateFormat)}`;
            });
            this.$notifier.modalSuccess(`${response.data[0]?.basicDocumentName}`, `Генериран(и) ${this.count} нов(и) номер(а) : ${JSON.stringify(sequences)}` );
          }
        this.gridReload();
      } finally {
        this.saving = false;
      }
    },
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.basicDocument
          .deleteBasicDocumentSequence(item.id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.gridReload();
          })
          .catch(error => {
            this.$notifier.error('', this.$t('common.deleteError'));
            console.log(error.response);
          })
          .finally(() => {
             this.saving = false;
          });
      }
    },
    gridReload() {
      const grid = this.$refs['basicDocumentSequencesGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
  },
};
</script>
