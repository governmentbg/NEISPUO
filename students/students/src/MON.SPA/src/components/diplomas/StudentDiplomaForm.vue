<template>
  <div>
    <v-alert
      v-for="message in value.messages"
      :key="message"
      border="top"
      colored-border
      type="info"
      elevation="2"
    >
      {{ message.message }}
    </v-alert>
    <v-form
      :ref="mainFormRefKey"
      :disabled="isDetailsView || disabled"
    >
      <v-tabs
        v-if="value"
        v-model="tab"
      >
        <v-tab key="basicData">
          <h5>
            {{ $t('studentTabs.basicData') }}
          </h5>
          <v-icon
            v-if="tabsValidationErrors && tabsValidationErrors[mainFormRefKey] && tabsValidationErrors[mainFormRefKey].length > 0"
            right
            color="error"
          >
            mdi-alert
          </v-icon>
        </v-tab>
        <v-tab
          v-for="part in value.diplomaData.parts"
          :key="part.id"
        >
          <h5>
            {{ part.name || part.id }}
          </h5>
          <v-icon
            v-if="tabsValidationErrors && tabsValidationErrors[`DiplomaSubjectSectionForm_${part.id}`] && tabsValidationErrors[`DiplomaSubjectSectionForm_${part.id}`].length > 0"
            right
            color="error"
          >
            mdi-alert
          </v-icon>
        </v-tab>
        <v-tab key="additionalDocuments">
          <h5>
            {{ $t('diplomas.additionalDocuments') }}
          </h5>
          <v-icon
            v-if="tabsValidationErrors && tabsValidationErrors[additionalDocumentsFormRefKey] && tabsValidationErrors[additionalDocumentsFormRefKey].length > 0"
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
          key="diplomaGeneralData"
        >
          Student details
        </v-tab>
        <v-tab
          v-if="mode !== 'prod'"
          key="modelPreview"
        >
          Model preview
        </v-tab>
      </v-tabs>
      <v-tabs-items
        v-if="value"
        v-model="tab"
      >
        <v-tab-item key="basicData">
          <validation-errors-details
            v-if="tabsValidationErrors"
            :value="tabsValidationErrors[mainFormRefKey]"
          />
          <diploma-section-generator
            v-if="value && value.schema"
            :ref="'DiplomaSectionGeneratorForm_' + _uid"
            v-model="value.diplomaData.generalDataModel"
            :schema="value.schema"
            :include-person-information-fields="true"
            :include-institution-information-fields="true"
          />
        </v-tab-item>
        <v-tab-item
          v-for="(part, index) in value.diplomaData.parts"
          :key="part.id"
          eager
        >
          <diploma-subjects-section-editor
            :ref="'DiplomaSubjectSectionForm_' + part.id"
            :value="value.diplomaData.parts[index]"
            :disabled="disabled"
          />
        </v-tab-item>
        <v-tab-item key="additionalDocuments">
          <diploma-documents-editor
            :ref="additionalDocumentsFormRefKey"
            :value="value"
            :disabled="disabled"
            :person-id="personId"
          />
        </v-tab-item>
        <v-tab-item key="commission">
          <diploma-commission-editor
            :ref="commissionFormRefKey"
            :value="value.diplomaData"
            :disabled="disabled"
          />
        </v-tab-item>
        <v-tab-item
          v-if="mode !== 'prod'"
          key="diplomaGeneralData"
        >
          <institution-profile
            v-if="userInstitutionId"
            class="my-3"
            :institution-id="userInstitutionId"
            is-preview
          />
          <person
            v-if="personId"
            :id="personId"
            :edit="false"
          />
        </v-tab-item>
        <v-tab-item
          v-if="mode !== 'prod'"
          key="modelPreview"
        >
          <vue-json-pretty
            path="res"
            :data="value.diplomaData.generalDataModel"
            show-length
          />
        </v-tab-item>
      </v-tabs-items>
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import DiplomaSectionGenerator from '@/components/tabs/diplomas/DiplomaSectionGenerator.vue';
import DiplomaSubjectsSectionEditor from '@/components/diplomas/DiplomaSubjectsSectionEditor';
import DiplomaCommissionEditor from '@/components/diplomas/DiplomaCommissionEditor';
import DiplomaDocumentsEditor from '@/components/diplomas/DiplomaDocumentsEditor';
import ValidationErrorsDetails from '@/components/common/ValidationErrorsDetails';
import Person from '@/components/person/Person.vue';
import InstitutionProfile from '@/components/institution/Profile.vue';
import { mapGetters } from 'vuex';

import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

export default {
  name: 'StudentDiplomaFormComponent',
  components: {
    DiplomaSectionGenerator,
    DiplomaSubjectsSectionEditor,
    Person,
    InstitutionProfile,
    VueJsonPretty,
    DiplomaCommissionEditor,
    DiplomaDocumentsEditor,
    ValidationErrorsDetails
  },
  props: {
    value: {
      type: Object,
      default: null
    },
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default: false
    },
    isDetailsView: {
      type: Boolean,
      default: false
    },
  },
  data() {
    return {
      tab: null,
      mainFormRefKey: `StudentDiplomaForm_${this._uid}`,
      commissionFormRefKey: `DiplomaCommissionForm_${this._uid}`,
      additionalDocumentsFormRefKey: `DiplomaAdditionalDocumentsForm_${this._uid}`,
      tabsValidationErrors: {}
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId', 'mode'])
  },
  methods: {
    validate() {
      this.tabsValidationErrors = {};

      const form = this.$refs[this.mainFormRefKey];
      let isValid = form ? form.validate() : false;
      this.tabsValidationErrors[this.mainFormRefKey] = this.$helper.getValidationErrorsDetails(form);

      const commissionEditor = this.$refs[this.commissionFormRefKey];
      if (commissionEditor) {
        // commissionEditor може да не съществува, ако таба не е кликнат, поради отложеното рендериране на v-tabs
        const commissionFormIsValid = commissionEditor.validate();
        this.tabsValidationErrors[this.commissionFormRefKey] = commissionEditor.validationErrors;
        isValid = isValid && commissionFormIsValid;
      }

      const additionalDocumentsEditor = this.$refs[this.additionalDocumentsFormRefKey];
      if (additionalDocumentsEditor) {
        // commissionEditor може да не съществува, ако таба не е кликнат, поради отложеното рендериране на v-tabs
        const additionalDocumentsFormIsValid = additionalDocumentsEditor.validate();
        this.tabsValidationErrors[this.additionalDocumentsFormRefKey] = additionalDocumentsEditor.validationErrors;
        isValid = isValid && additionalDocumentsFormIsValid;
      }

      if (!!this.value?.diplomaData?.parts === true) {
        this.value.diplomaData.parts.forEach(x => {
          const refKey = `DiplomaSubjectSectionForm_${x.id}`;
          const subjectsPart = this.$refs[refKey][0];
          if (subjectsPart) {
            const subjectsPartIsValid = subjectsPart.validate();
            if (!subjectsPartIsValid) {
              this.tabsValidationErrors[refKey] = subjectsPart.validationErrors;
            }
            isValid = isValid && subjectsPartIsValid;
          }
        });
      }
      return isValid;
    },
  }
};
</script>
