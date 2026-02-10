<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ diplomaEditTitle }}</h3>
      </template>

      <template #left-actions>
        <v-btn
          v-if="validationError"
          color="red"
          outlined
          @click="dialog = true"
        >
          <v-icon
            large
            class="mr-2"
          >
            mdi-alert-circle-outline
          </v-icon>
          Виж детайли на последната грешка
        </v-btn>
        <v-spacer />
        <v-alert
          v-if="mode !== 'prod' && hasUnsavedChanges"
          dense
          outlined
          type="error"
        >
          {{ $t('common.hasUnsavedChanges') }}
        </v-alert>
      </template>

      <template #default>
        <app-loader
          v-if="loading"
        />
        <student-diploma-form
          :ref="'StudentDiplomaCreateForm_' + _uid"
          :value="model"
          :disabled="disabled"
          :person-id="model.personId"
        />
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        color="red"
        outlined
      >
        <v-btn
          icon
          dark
          @click="dialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-spacer />
        <v-toolbar-items>
          <v-btn
            dark
            text
            @click="dialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-toolbar-items>
      </v-toolbar>

      <api-error-details
        :value="validationError"
      />
    </v-dialog>
  </div>
</template>

<script>
import StudentDiplomaForm from '@/components/diplomas/StudentDiplomaForm.vue';
import ApiErrorDetails from '@/components/admin/ApiErrorDetails.vue';
import AppLoader from '@/components/wrappers/loader.vue';
import { DynamicSectionModel } from '@/models/dynamic/dynamicSectionModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';
import clonedeep from 'lodash.clonedeep';
import isEqual from 'lodash.isequal';
import Constants from '@/common/constants.js';
import { DiplomaAdditionalDocumentModel } from '@/models/diploma/diplomaAdditionalDocumentModel';

export default {
  name: 'StudentDiplomaEditView',
  components: {
    StudentDiplomaForm,
    ApiErrorDetails,
    AppLoader
  },
  props: {
    diplomaId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      validationError: null,
      dialog: false,
      model: {
        personId: null,
        templateId: this.templateId,
        basicDocumentId: this.basicDocumentId,
        basicDocumentName: null,
        institutionId: null,
        schema: null,
        messages: [],
        mainBasicDocuments: [],
        diplomaData: {
          commissionOrderNumber: null,
          commissionOrderData: null,
          generalDataModel: {},
          parts: null,
          additionalDocuments: [],
          commissionMembers: []
        }
      },
      initialModelData: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'hasPermission', 'userDetails', 'mode']),
    disabled() {
      return this.saving;
    },
    hasUnsavedChanges() {
      return !isEqual(this.initialModelData, this.model.diplomaData);
    },
    diplomaEditTitle() {
      const diplomaTitle = this.$t('diplomas.editDiplomaTitle');
      return this.model.basicDocumentName ? `${diplomaTitle}/${this.model.basicDocumentName}` : diplomaTitle;
    },
    hasStudentDiplomaManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaManage);
    },
    hasStudentDiplomaByCreateRequestManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentDiplomaByCreateRequestManage);
    },
    hasAdminDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage);
    },
    hasMonHrDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage);
    },
    hasRuoHrDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage);
    },
    hasInstitutionDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage);
    }
  },
  async mounted() {
    if (!(this.hasStudentDiplomaManagePermission || this.hasStudentDiplomaByCreateRequestManagePermission
      || this.hasAdminDiplomaManagePermission || this.hasInstitutionDiplomaManagePermission
      || this.hasMonHrDiplomaManagePermission || this.hasRuoHrDiplomaManagePermission)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.loading = true;
    await this.loadUpdateModel(this.diplomaId);
    this.loading = false;
  },
  methods: {
    async loadUpdateModel(diplomaId) {
      try {
        const data = (await this.$api.diploma.getUpdateModel(diplomaId)).data;
        if (data) {
          this.model.diplomaId = data.diplomaId;
          this.model.personId = data.personId;
          this.model.diplomaData.commissionOrderNumber = data.commissionOrderNumber;
          this.model.diplomaData.commissionOrderData = data.commissionOrderData
            ? this.$moment(data.commissionOrderData).format(Constants.DATEPICKER_FORMAT)
            : data.commissionOrderData;

          this.model.messages = data.messages;
          if (data.basicDocumentTemplate) {
            this.model.basicDocumentId = data.basicDocumentTemplate.basicDocumentId;
            this.model.basicDocumentName = data.basicDocumentTemplate.basicDocumentName;
            this.model.institutionId = data.basicDocumentTemplate.institutionId;
            this.model.mainBasicDocuments = data.basicDocumentTemplate.mainBasicDocuments || [];

            if (data.basicDocumentTemplate.parts) {
              this.model.diplomaData.parts = data.basicDocumentTemplate.parts || null;
            }

            if (data.basicDocumentTemplate.schema) {
              const json = JSON.parse(data.basicDocumentTemplate.schema);
              this.model.schema = json.map(x => new DynamicSectionModel(x));
            }

            if (data.basicDocumentTemplate.commissionMembers) {
              this.model.diplomaData.commissionMembers = data.basicDocumentTemplate.commissionMembers;
            }
          }

          if (data.contents) {
            this.model.diplomaData.generalDataModel = JSON.parse(data.contents);
          }

          if (data.additionalDocuments && Array.isArray(data.additionalDocuments)) {
            this.model.diplomaData.additionalDocuments = data.additionalDocuments.map(x => new DiplomaAdditionalDocumentModel(x));
          }

        }

        this.initialModelData = clonedeep(this.model.diplomaData);
      } catch (error) {
        this.$notifier.error('', this.$t('errors.diplomaSchemaLoad'));
        console.log(error);
      }
    },
    onSave() {
      const form = this.$refs['StudentDiplomaCreateForm_' + this._uid];
      if(!form) {
        console.log('Empty form');
        return this.$notifier.error('', this.$t('common.saveError'), 5000);
      }

      const isValid = form.validate();
      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      const payload = {
        diplomaId: this.model.diplomaId,
        templateId: this.model.templateId,
        basicDocumentId: this.model.basicDocumentId,
        personId: this.model.personId,
        institutionId: this.model.institutionId,
        commissionOrderNumber: this.model.diplomaData.commissionOrderNumber,
        commissionOrderData: this.$helper.parseDateToIso(this.model.diplomaData.commissionOrderData, ''),
        contents: JSON.stringify(this.model.diplomaData.generalDataModel),
        additionalDocuments: this.model.diplomaData.additionalDocuments,
        parts: this.model.diplomaData.parts,
        commissionMembers: this.model.diplomaData.commissionMembers,
      };

      if (payload.additionalDocuments && Array.isArray(payload.additionalDocuments)) {
        payload.additionalDocuments.forEach(x => {
          x.registrationDate = this.$helper.parseDateToIso(x.registrationDate, '');
        });
      }

      this.saving = true;
      this.validationError = null;
      this.$api.diploma
        .update(payload)
        .then((result) => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);

          const validationResult = result.data;
          if(validationResult?.hasWarnings === true){
            this.$notifier.modalWarn(this.$t('errors.diplomaCreateWarningsModalTitle'), validationResult.warnings.map(x => x.message));
          }

          this.onCancel();
        })
        .catch((error) => {
          const {message, errors} = this.$helper.parseError(error.response);
          this.validationError = { date: new Date(), ...error.response.data } ;
          this.$notifier.modalError(message, errors);
          this.$helper.logError({ action: 'DiplomaCreate', message: message}, errors, this.userDetails);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
