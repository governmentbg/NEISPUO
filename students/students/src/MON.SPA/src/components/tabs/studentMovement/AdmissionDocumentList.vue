<template>
  <v-card>
    <v-card-title>
      {{ getTranslation('admissionDocumentListTitle') }}
    </v-card-title>
    <v-card-text class="pa-0">
      <v-alert
        v-if="isStudentInCurrentInstitution === true"
        outlined
        type="info"
        prominent
        dense
      >
        <h4>{{ $t('admissionDocument.isStudentInInstitutionError') }}</h4>
      </v-alert>
      <v-data-table
        ref="admissionDocumentListTable"
        :headers="headers"
        :items="admissionDocuments"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar
            flat
          >
            <v-toolbar-title>
              <GridExporter
                :items="admissionDocuments"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="$t('documents.admissionDocuments')"
                :headers="headers"
              />
            </v-toolbar-title>
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

        <template v-slot:[`item.id`]="{ item }">
          <doc-downloader
            v-for="doc in item.documents"
            :key="doc.id"
            :value="doc"
            small
          />
        </template>

        <template v-slot:[`item.noteDate`]="{ item }">
          {{ item.noteDate ? $moment(item.noteDate).format(dateFormat) : '' }}
        </template>

        <template v-slot:[`item.admissionDate`]="{ item }">
          {{ item.admissionDate ? $moment(item.admissionDate).format(dateFormat) : '' }}
        </template>

        <template v-slot:[`item.dischargeDate`]="{ item }">
          {{ item.dischargeDate ? $moment(item.dischargeDate).format(dateFormat) : '' }}
        </template>

        <template v-slot:[`item.statusName`]="{ item }">
          <v-chip
            :class="item.status === 2 ? 'success': 'light'"
            small
          >
            {{ item.statusName }}
          </v-chip>
        </template>

        <!-- <template v-slot:[`item.classEnrolledIn`]="{ item }">
          <v-chip
            v-if="item.classEnrolledIn"
            small
            color="default"
          >
            {{ item.classEnrolledIn }}
          </v-chip>
        </template> -->
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <button-tip
              v-if="hasReadPermission"
              icon
              icon-name="mdi-eye"
              icon-color="primary"
              tooltip="buttons.review"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${personId}/admissionDocument/${item.id}/details`"
            />
            <button-tip
              v-if="(hasUpdatePermission || hasReadPermission) && userInstitutionId && item.institutionId === userInstitutionId"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${personId}/admissionDocument/${item.id}/edit`"
            />
            <button-tip
              v-if="hasUpdatePermission && item.status != 2 && item.canBeModified"
              icon
              icon-name="mdi-check"
              icon-color="success"
              tooltip="buttons.confirm"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="documentConfirm(item)"
            />
            <button-tip
              v-if="hasDeletePermission && item.status !== 2 && item.canBeModified"
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="deleteItem(item)"
            />
            <button-tip
              v-if="hasStudentToClassEnrollPermission
                && item.status === 2 && item.canBeEnrolled"
              icon
              icon-name="fas fa-long-arrow-alt-right"
              icon-color="primary"
              tooltip="common.enroll"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="enrollInClass(item)"
            />
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasCreatePermission"
              small
              color="primary"
              :to="`/student/${personId}/admissionDocument/create`"
            >
              {{ $t('buttons.newRecord') }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click="load"
            >
              {{ $t('buttons.reload') }}
            </v-btn>
          </button-group>
        </template>
      </v-data-table>
      <confirm-dlg ref="confirm" />
      <confirm-dlg ref="noCancelBtnDialog" />
    </v-card-text>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import DocDownloader from '@/components/common/DocDownloader.vue';
import Constants from "@/common/constants.js";
import { Permissions, InstType, Positions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "AdmissionDocumentList",
  components: {
    GridExporter,
    DocDownloader
  },
  props: {
    personId: {
      type: Number,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      search: '',
      loading: false,
      saving: false,
      isStudentInCurrentInstitution: undefined,
      admissionDocuments: [],
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers:[
        {
          text: this.getTranslation('noteNumberLabel'),
          value: 'noteNumber'
        },
        {
          text: this.getTranslation('datePickerLabel'),
          value: 'noteDate'
        },
        {
          text: this.getTranslation('admissionDateLabel'),
          value: 'admissionDate'
        },
        {
          text: this.getTranslation('dischargeDateLabel'),
          value: 'dischargeDate'
        },
        {
          text: this.getTranslation('statusLabel'),
          value: 'statusName'
        },
        {
          text: this.$t('admissionDocument.schoolYear'),
          value: 'schoolYearName'
        },
        {
          text: this.getTranslation('institutionDropdownLabel'),
          value: 'institutionName'
        },
        {
          text: this.$t('admissionDocument.institutionCode'),
          value: 'institutionId'
        },
        {
          text: this.getTranslation('admissionReasonTypeLabel'),
          value: 'admissionReasonTypeName'
        },
        // {
        //   text: this.getTranslation('className'),
        //   value: 'classEnrolledIn'
        // },
        {
          text: this.getTranslation('filesListLabel'),
          value: 'id',
          sortable: false,
          filterable: false,
        },
        {
          text: this.getTranslation('actionsHeader'),
          value: 'actions',
          sortable: false,
          align: 'end'
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions', 'isInstType', 'userInstitutionId']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentRead);
    },
    hasCreatePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentCreate);
    },
    hasUpdatePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentUpdate);
    },
    hasDeletePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentDelete);
    },
    hasStudentToClassEnrollPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment);
    },
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    }
  },
  mounted() {
    this.load();
    this.isStudentInCurrentInstitutionCheck(this.personId);
  },
  methods: {
    getTranslation(text, params) {
      return this.$t(`documents.${text}`, params);
    },
    load() {
      this.loading = true;
      this.$api.admissionDocument
        .getByPersonId(this.personId)
        .then((response) => {
          if(response.data) {
            this.admissionDocuments = response.data;
          }
        })
        .catch(error => {
          this.$notifier.error('', this.getTranslation('admissionDocumentsFilesLoadErrorMessage'));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.admissionDocument
          .delete(item.id)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.load();
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch(error => {
            this.$notifier.error('', this.getTranslation('deleteAdmissionDocumentErrorMessage'));
            console.log(error.response.data.message);
          })
          .finally(() => {
             this.saving = false;
          });
      }
    },
    async documentConfirm(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.admissionDocument.confirm(item)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.load();
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch(error => {
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    },
    enrollInClass(item) {
      this.isCplrInstitution
       ? this.$router.push({ path: `/student/${this.personId}/class/initialCplrEnrollment?admissionDocumentId=${item.id}` })
       : item.position === Positions.StudentPersonalSupport
          ? this.$router.push({ path: `/student/${this.personId}/class/enroll` })
          : this.$router.push({ path: `/student/${this.personId}/class/initialEnrollment?admissionDocumentId=${item.id}` });
    },
    isStudentInCurrentInstitutionCheck(personId) {
      this.$api.student.isStudentInCurrentInstitution(personId)
      .then((response) => {
        if(response) {
          this.isStudentInCurrentInstitution = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    }
  }
};
</script>
