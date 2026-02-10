<template>
  <div>
    <app-loader
      v-if="loading"
    />
    <form-layout
      v-else
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        {{ $t("diplomas.newDiplomaTemplateTitle") }}
      </template>

      <template #subtitle>
        {{ model ? model.basicDocumentName : '' }}
      </template>

      <template #left-actions>
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
        <v-tabs
          v-if="model"
          v-model="tab"
        >
          <v-tab key="templateGeneralData">
            <h5>
              {{ $t('diplomas.templateDescription') }}
            </h5>
            <v-icon
              v-if="tabsValidationErrors && tabsValidationErrors[mainFormRefKey] && tabsValidationErrors[mainFormRefKey].length > 0"
              right
              color="error"
            >
              mdi-alert
            </v-icon>
          </v-tab>
          <v-tab key="basicData">
            <h5>
              {{ $t('studentTabs.basicData') }}
            </h5>
            <v-icon
              v-if="tabsValidationErrors && tabsValidationErrors[basicDataFormRefKey] && tabsValidationErrors[mainFormRefKey].length > 0"
              right
              color="error"
            >
              mdi-alert
            </v-icon>
          </v-tab>
          <v-tab
            v-for="part in model.parts"
            :key="part.id"
          >
            <h5>
              {{ part.name || part.id }}
            </h5>
            <v-icon
              v-if="tabsValidationErrors && tabsValidationErrors[`DiplomaTemplateSectionForm_${part.id}`] && tabsValidationErrors[`DiplomaSubjectSectionForm_${part.id}`].length > 0"
              right
              color="error"
            >
              mdi-alert
            </v-icon>
          </v-tab>
          <v-tab key="commission">
            <h5>
              {{ $t('diplomas.commission.title') }}
            </h5>
            <v-icon
              v-if="tabsValidationErrors && tabsValidationErrors[commissionFormRefKey] && tabsValidationErrors[commissionFormRefKey].length > 0"
              right
              color="error"
            >
              mdi-alert
            </v-icon>
          </v-tab>
          <v-tab
            v-if="mode !== 'prod'"
            key="help"
          >
            {{ $t('menu.help') }}
          </v-tab>
          <v-tab
            v-if="mode !== 'prod'"
            key="modelPreview"
          >
            Dynamic model preview
          </v-tab>
        </v-tabs>
        <v-tabs-items
          v-if="model"
          v-model="tab"
        >
          <v-tab-item key="templateGeneralData">
            <diploma-template-editor
              v-if="model"
              :ref="mainFormRefKey"
              :value="model"
              :disabled="saving"
            />
          </v-tab-item>
          <v-tab-item
            key="basicData"
          >
            <diploma-section-generator
              v-if="schema"
              :ref="basicDataFormRefKey"
              v-model="generalDataModel"
              :schema="schema"
              :include-validations="false"
              :include-person-information-fields="false"
              :include-institution-information-fields="false"
            />
          </v-tab-item>
          <v-tab-item
            v-for="(part, index) in model.parts"
            :key="part.id"
            eager
          >
            <diploma-template-section-editor
              :ref="'DiplomaTemplateSectionForm_' + part.id"
              :value="model.parts[index]"
              :disabled="saving"
            />
          </v-tab-item>
          <v-tab-item
            key="commission"
          >
            <diploma-commission-editor
              :ref="commissionFormRefKey"
              :value="model"
              :disabled="saving"
            />
          </v-tab-item>
          <v-tab-item key="help">
            <ul>
              <li>Неактивните полета са такива, които се изтеглят автоматично при създаването на документ от даден шаблон.</li>
            </ul>
          </v-tab-item>
          <v-tab-item key="modelPreview">
            <vue-json-pretty
              path="res"
              :data="generalDataModel"
              show-length
            />
          </v-tab-item>
        </v-tabs-items>
      </template>
    </form-layout>
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
import AppLoader from '@/components/wrappers/loader.vue';
import DiplomaSectionGenerator from '@/components/tabs/diplomas/DiplomaSectionGenerator.vue';
import DiplomaTemplateEditor from '@/components/diplomas/DiplomaTemplateEditor';
import DiplomaTemplateSectionEditor from '@/components/diplomas/DiplomaTemplateSectionEditor';
import DiplomaCommissionEditor from '@/components/diplomas/DiplomaCommissionEditor';
import Constants from '@/common/constants.js';
import { DynamicSectionModel } from '@/models/dynamic/dynamicSectionModel';
import isEqual from 'lodash.isequal';
import clonedeep from 'lodash.clonedeep';
import { mapGetters } from 'vuex';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';


export default {
  name: 'DiplomaTemplateEditComponent',
  components: {
    AppLoader,
    DiplomaSectionGenerator,
    DiplomaTemplateEditor,
    DiplomaTemplateSectionEditor,
    DiplomaCommissionEditor,
    VueJsonPretty
  },
  props: {
    id: {
      type: Number,
      required : true
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      schema: null,
      generalDataModel: {},
      model: null,
      initialGeneralData: null,
      initialModelData: null,
      tab: null,
      mainFormRefKey: `DiplomaTemplateForm_${this._uid}`,
      commissionFormRefKey: `DiplomaTemplateCommissionForm_${this._uid}`,
      basicDataFormRefKey: `DiplomaSectionGeneratorForm_${this._uid}`,
      tabsValidationErrors: {}
    };
  },
  computed: {
    ...mapGetters(['mode']),
    hasUnsavedChanges() {
      return !isEqual(this.initialGeneralData, this.generalDataModel)
        || !isEqual(this.initialModelData, this.model);
    },
  },
  mounted() {
    this.loadDocumentSchema();
    this.load();
  },
  methods: {
    loadDocumentSchema() {
      this.loading = true;
      this.$api.basicDocument.loadSchemaByTemplateId(this.id)
      .then(response => {
        if(response.data) {
          const json = JSON.parse(response.data);
          this.schema = json.map(x => new DynamicSectionModel(x));
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.diplomaSchemaLoad'));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    load() {
      this.$api.diplomaTemplate.getById(this.id)
      .then(response => {
        if(response.data) {
          this.model = response.data;
          if (response.data.commissionOrderData) {
            this.model.commissionOrderData = this.$moment(response.data.commissionOrderData).format(Constants.DATEPICKER_FORMAT);
          }
          this.initialModelData = clonedeep(this.model);

          if (response.data.dynamicContent) {
            this.generalDataModel = JSON.parse(response.data.dynamicContent);
          }

          // Запазва първоначалното състояние на generalDataModel с цел да следи за незапаметени промени.
          this.initialGeneralData = clonedeep(this.generalDataModel);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('common.loadError'));
        console.log(error);
      });
    },
    async onSave() {
      this.tabsValidationErrors = {};

      const form = this.$refs[this.mainFormRefKey];
      let isValid = form ? form.validate() : false;
      this.tabsValidationErrors[this.mainFormRefKey] = this.$helper.getValidationErrorsDetails(form);

      if (this.model && this.model.parts) {
        this.model.parts.forEach(x => {
          const refKey = `DiplomaTemplateSectionForm_${x.id}`;
          const subjectsPart = this.$refs[refKey][0];
          if (subjectsPart) {
            const subjectsPartIsValid = subjectsPart.validate();
            if(!subjectsPartIsValid) {
              this.tabsValidationErrors[refKey] = subjectsPart.validationErrors;
            }
            isValid = isValid && subjectsPartIsValid;
          }
        });
      }

      const commissionEditor = this.$refs[this.commissionFormRefKey];
      if (commissionEditor) {
        // commissionEditor може да не съществува, ако таба не е кликнат, поради отложеното рендериране на v-tabs
        const commissionFormIsValid = commissionEditor.validate();
        this.tabsValidationErrors[this.commissionFormRefKey] = commissionEditor.validationErrors;
        isValid = isValid && commissionFormIsValid;
      }

      if(!isValid) {
        const errorMessages = document.getElementsByClassName('v-messages__message');
        if(errorMessages && errorMessages.length > 0) {
          errorMessages[0].scrollIntoView();
        }

        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {
        if (this.generalDataModel) {
          this.model.dynamicContent = JSON.stringify(this.generalDataModel);
        }

        this.saving = true;

        this.model.commissionOrderData = this.$helper.parseDateToIso(this.model.commissionOrderData, ''),

        this.$api.diplomaTemplate.update(this.model)
          .then(() => {
            this.$notifier.success('', this.$t('common.saveSuccess'));
            this.onCancel();
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('common.saveError'));
            console.log(error);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    onCancel() {
      this.goBack();
    },
    goBack() {
      this.$router.go(-1);
    },
  }
};
</script>

