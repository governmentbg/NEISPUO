<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ diplomaCreateTitle }}</h3>
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
        <student-diploma-form
          :ref="`${formName}` + _uid"
          :value="model"
          :disabled="disabled"
          :person-id="personId"
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
import Constants from '@/common/constants.js';
import { DynamicSectionModel } from '@/models/dynamic/dynamicSectionModel';
import { mapGetters } from 'vuex';
import clonedeep from 'lodash.clonedeep';
import isEqual from 'lodash.isequal';
import { DiplomaAdditionalDocumentModel } from '@/models/diploma/diplomaAdditionalDocumentModel';

export default {
  name: 'StudentDiplomaCreateView',
  components: {
    StudentDiplomaForm,
    ApiErrorDetails
  },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    templateId: {
      type: Number,
      default() {
        return null;
      }
    },
    basicDocumentId: {
      type: Number,
      default() {
        return null;
      }
    },
    actionName: {
      type: String,
      default() {
        return undefined;
      }
    },
    formName: {
      type: String,
      default() {
        return undefined;
      }
    },
    basicClassId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      validationError: null,
      dialog: false,
      model: {
        templateId: this.templateId,
        basicDocumentId: this.basicDocumentId,
        basicClassId: this.basicClassId,
        basicDocumentName: null,
        institutionId: null,
        schema: null,
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
    ...mapGetters(['userDetails', 'hasStudentPermission', 'hasPermission', 'mode']),
    disabled() {
      return this.saving;
    },
    hasUnsavedChanges() {
      return !isEqual(this.initialModelData, this.model.diplomaData);
    },
    diplomaCreateTitle() {
      return this.model.basicDocumentName ? `${this.$t('diplomas.newDiplomaTitle')}/${this.model.basicDocumentName}` : this.$t('diplomas.newDiplomaTitle');
    },
    hasStudentDiplomaManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaManage);
    },
    hasAdminDiplomaManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage);
    },
  },
  async mounted() {
    if (!this.basicDocumentId && !this.templateId) {
      this.$notifier.error('', this.$t('diplomas.template.missingPropsOnCreate'));
      return this.onCancel();
    }

    this.loading = true;
    await this.loadCreateModel(this.personId, this.templateId, this.basicDocumentId, this.basicClassId);
    this.loading = false;
  },
  methods: {
    async loadCreateModel(personId, templateId, basicDocumentId, basicClassId) {
      try {
        const data = (await this.$api.diploma.getCreateModel(personId, templateId, basicDocumentId, basicClassId)).data;
        if (data) {
          if (data.basicDocumentTemplate) {
            this.model.basicDocumentId = data.basicDocumentTemplate.basicDocumentId;
            this.model.basicDocumentName = data.basicDocumentTemplate.basicDocumentName;
            this.model.institutionId = data.basicDocumentTemplate.institutionId;
            this.model.diplomaData.commissionOrderNumber = data.basicDocumentTemplate.commissionOrderNumber;
            this.model.mainBasicDocuments = data.basicDocumentTemplate.mainBasicDocuments || [];
            this.model.diplomaData.commissionOrderData = data.commissionOrderData
              ? this.$moment(data.commissionOrderData).format(Constants.DATEPICKER_FORMAT)
              : data.commissionOrderData;

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
      const form = this.$refs[this.formName + this._uid];
      if(!form) {
        console.log('Empty form');
        return this.$notifier.error('', this.$t('common.saveError'), 5000);
      }

      const isValid = form.validate();
      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      const payload = {
        templateId: this.model.templateId,
        basicDocumentId: this.model.basicDocumentId,
        personId: this.personId,
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
        .create(payload)
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
          this.$helper.logError({ action: this.actionName, message: message}, errors, this.userDetails);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
