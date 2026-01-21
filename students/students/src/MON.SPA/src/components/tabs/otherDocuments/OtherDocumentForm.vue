<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row dense>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.schoolYear"
          >
            <school-year-selector
              v-model="model.schoolYear"
              :label="$t('otherDocuments.schoolYear')"
              :min-year="minSchoolYear"
              :max-year="maxSchoolYear"
              :rules="schoolYearRequired
                ? [$validator.required()]
                : []"
              :class="schoolYearRequired ? 'required' :''"
              show-current-school-year-button
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.series"
          >
            <v-text-field
              v-model="model.series"
              :label="$t('otherDocuments.series')"
              :rules="seriesRequired
                ? [$validator.maxLength(50), $validator.required()]
                : [$validator.maxLength(50)]"
              :class="seriesRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.factoryNumber"
          >
            <v-text-field
              v-model="model.factoryNumber"
              :label="$t('otherDocuments.factoryNumber')"
              :rules="factoryNumberRequired
                ? [$validator.maxLength(50), $validator.required()]
                : [$validator.maxLength(50)]"
              :class="factoryNumberRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.regNumberTotal"
          >
            <v-text-field
              v-model="model.regNumberTotal"
              :label="$t('otherDocuments.regNumberTotal')"
              :rules="regNumberTotalRequired
                ? [$validator.maxLength(400), $validator.required()]
                : [$validator.maxLength(400)]"
              :class="regNumberTotalRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.regNumber"
          >
            <v-text-field
              v-model="model.regNumber"
              :label="$t('otherDocuments.regNumber')"
              :rules="registrationNumberRequired
                ? [$validator.maxLength(400), $validator.required()]
                : [$validator.maxLength(400)]"
              :class="registrationNumberRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.issueDate"
          >
            <date-picker
              id="issueDate"
              ref="issueDate"
              v-model="model.issueDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('otherDocuments.issueDate')"
              :rules="issueDateRequired
                ? [$validator.required()]
                : []"
              :class="issueDateRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="3"
        >
          <c-info
            uid="otherDocuments.deliveryDate"
          >
            <date-picker
              id="deliveryDate"
              ref="deliveryDate"
              v-model="model.deliveryDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('otherDocuments.deliveryDate')"
              :rules="deliveryDateRequired
                ? [$validator.required()]
                : []"
              :class="deliveryDateRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="6"
        >
          <c-info
            uid="otherDocuments.institution"
          >
            <custom-autocomplete
              id="institutionDropdown"
              ref="institutionDropdown"
              v-model="model.institutionId"
              api="/api/lookups/GetInstitutionOptions"
              :label="$t('otherDocuments.institution')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :disabled="disabled || disabledInstitution"
              :rules="institutionRequired
                ? [$validator.required()]
                : []"
              :class="institutionRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="6"
        >
          <c-info
            uid="otherDocuments.documentType"
          >
            <custom-autocomplete
              id="basicDocumentTypes"
              ref="basicDocumentType"
              v-model="model.documentTypeId"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('otherDocuments.documentType')"
              :placeholder="$t('buttons.search')"
              clearable
              hide-no-data
              hide-selected
              :defer-options-loading="false"
              :disabled="disabled"
              :rules="documentTypeRequired
                ? [$validator.required()]
                : []"
              :class="documentTypeRequired ? 'required' :''"
              :filter="{ isIncludedInRegister: false }"
            />
          </c-info>
        </v-col>
      </v-row>

      <v-row dense>
        <v-col>
          <c-info
            uid="otherDocuments.description"
          >
            <v-textarea
              id="description"
              v-model="model.description"
              outlined
              prepend-icon="mdi-comment"
              :label="$t('otherDocuments.description')"
              autocomplete="description"
              name="description"
              :rules="descriptionRequired
                ? [$validator.maxLength(400), $validator.required()]
                : [$validator.maxLength(400)]"
              :class="descriptionRequired ? 'required' :''"
            />
          </c-info>
        </v-col>
      </v-row>

      <v-row dense>
        <v-col>
          <file-manager
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { StudentOtherDocumentModel } from '@/models/studentOtherDocumentModel.js';
import FileManager from '@/components/common/FileManager.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from 'vuex';

export default {
  name: "OtherDocumentForm",
  components: {
    FileManager,
    SchoolYearSelector,
    CustomAutocomplete
  },
  props: {
    document: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    disabledInstitution: {
      type: Boolean,
      default() {
        return false;
      }
    },
    descriptionRequired: {
      type: Boolean,
      default: false,
    },
    seriesRequired: {
      type: Boolean,
      default: false,
    },
    factoryNumberRequired: {
      type: Boolean,
      default: false,
    },
    regNumberTotalRequired: {
      type: Boolean,
      default: false,
    },
    registrationNumberRequired: {
      type: Boolean,
      default: false,
    },
    issueDateRequired: {
      type: Boolean,
      default: false,
    },
    deliveryDateRequired: {
      type: Boolean,
      default: false,
    },
    institutionRequired: {
      type: Boolean,
      default: false,
    },
    documentTypeRequired: {
      type: Boolean,
      default: false,
    },
    schoolYearRequired: {
      type: Boolean,
      default: false,
    },
    minSchoolYear: {
      type: [Number, String],
      default() {
        return undefined;
      }
    },
    maxSchoolYear: {
      type: [Number, String],
      default() {
        return undefined;
      }
    },
  },
  data() {
    return {
      model: this.document ?? new StudentOtherDocumentModel()
    };
  },
  computed: {
    ...mapGetters(['studentFinalizedLods'])
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
