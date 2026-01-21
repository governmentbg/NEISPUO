<template>
  <div>
    <!-- {{ schema }} -->
    <!-- {{ initialData }} -->
    <!-- {{ diplomaData }} -->
    <v-expansion-panels
      v-if="showDiplomaData"
    >
      <v-expansion-panel>
        <v-expansion-panel-header>
          Модел
        </v-expansion-panel-header>
        <v-expansion-panel-content>
          <pre>
            {{ diplomaData }}
          </pre>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </v-expansion-panels>

    <div
      v-for="(section, index) in sortedSections"
      :key="index"
    >
      <v-card
        class="mb-2"
      >
        <v-card-title>
          {{ section.title }}
        </v-card-title>

        <v-card-text>
          <v-row
            dense
          >
            <v-col
              v-for="(itemField, itemIndex) in section.items"
              :key="itemField + '_' + itemIndex"
              :cols="$helper.getColsProp(itemField)"
              :xl="$helper.getXlProp(itemField)"
              :lg="$helper.getLgProp(itemField)"
              :md="$helper.getMdProp(itemField)"
              :sm="$helper.getSmProp(itemField)"
            >
              <c-info
                :info-text="itemField.contextInfo"
              >
                <!-- {{ diplomaData[itemField.id] }} -->
                <component
                  :is="itemField.type"
                  :id="itemField.id"
                  :key="`${itemIndex}-${itemField.optionsUrl}-${itemField.deferOptionsLoading.toString()}`"
                  :ref="'diplomaSectionItem' + itemField.uid"
                  v-model="diplomaData[itemField.id]"
                  v-bind="itemField"
                  :person-unique-id-type="diplomaData.personalIdType"
                  :readonly="itemField.readonly"
                  :clearable="itemField.clearable || itemField.type === 'YearPicker'"
                  :disabled="itemField.disabled || !renderField(itemField.id) || !itemField.editable"
                  :rules="getRules(itemField)"
                  :class="(itemField.class || '') + (itemField.required === true ? ' required' : '' )"
                  :hint="itemField.hint"
                  :persistent-hint="itemField.persistentHint"
                  :static-text="itemField.label"
                  :min="itemField.min || undefined"
                  :max="itemField.max || undefined"
                  :show-current-school-year-button="itemField.type === 'SchoolYearField'"
                  :show-current-year-button="itemField.type === 'YearPicker'"
                  dense
                  @input="updateDiplomaData(itemField.id, $event)"
                  @inputCustomField="updateDiplomaData"
                />
              </c-info>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>
<script>
import Constants from "@/common/constants.js";

import TextField from '@/components/wrappers/form/TextField';
import TextAreaField from '@/components/wrappers/form/TextAreaField';
import NumberField from '@/components/wrappers/form/NumberField';
import SelectList from '@/components/wrappers/form/SelectList';
import DateField from '@/components/wrappers/form/DateField';
import PersonUniqueIdField from "@/components/wrappers/form/PersonUniqueIdField";
import SchoolYearField from "@/components/wrappers/form/SchoolYearField";
import LabelField from '@/components/wrappers/form/LabelField';
import BooleanField from '@/components/wrappers/form/BooleanField';
import AutoComplete from '@/components/wrappers/CustomAutocomplete';
import YearPicker from '@/components/wrappers/YearPickerCombo';
import Combobox from '@/components/wrappers/Combobox';

export default {
  name: 'DiplomaSectionGenerator',
  components: {
    TextField,
    TextAreaField,
    NumberField,
    SelectList,
    DateField,
    PersonUniqueIdField,
    SchoolYearField,
    LabelField,
    BooleanField,
    AutoComplete,
    YearPicker,
    Combobox
  },
  props: {
    value: {
      type: Object,
      required: false,
      default() {
        return null;
      }
    },
    schema: {
      type: Array,
      default() {
        return [];
      }
    },
    initialData: {
      type: Object,
      default() {
        return {};
      }
    },
    includeValidations: {
      type: Boolean,
      default() {
        return true;
      }
    },
    includePersonInformationFields: {
      type: Boolean,
      default() {
        return true;
      }
    },
    includeInstitutionInformationFields: {
      type: Boolean,
      default() {
        return true;
      }
    },
    showDiplomaData: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      diplomaData: this.value || {}
    };
  },
  computed: {
    schemaItems() {
      return this.schema ? this.schema.map(el => el.items).flat() : [];
    },
    sortedSections() {
      if(!this.hasSections) return [];

      return this.schema;
    },
    hasSections() {
      return this.schema && Array.isArray(this.schema);
    },
  },
  validations: {
  },
  watch: {
    initialData(val) {
      this.diplomaData = {};
      this.setInitialData();
      this.updateInstitutionProps(val);
    },
  },
  mounted() {
    this.setInitialData();
    if(this.initialData) {
      this.updateInstitutionProps(this.initialData);
    }
  },
  created() {
    if(this.value) {
      const sppooProfession = this.value['sppooProfession'] || this.value['sppooprofession'];
      if(sppooProfession) {
        this.updateSppooSpecialityProps(sppooProfession);
      }
    }
  },
  methods: {
    renderField(fieldId) {
      if(Constants.PERSON_INFORMATION_FIELD_IDS.indexOf(fieldId) > -1) {
        return this.includePersonInformationFields;
      }

      if(Constants.INSTITUTION_INFORMATION_FIELD_IDS.indexOf(fieldId) > -1) {
        return this.includeInstitutionInformationFields;
      }

      return true;
    },
    setInitialData() {
      if(!this.initialData) return;

      for (const property in this.initialData) {
        this.updateDiplomaData(property, this.initialData[property]);
      }
    },
    updateDiplomaData(fieldId, value) {
      const isDate = fieldId && fieldId.toUpperCase().indexOf('DATE') > -1;
      if(isDate) {
        const schemaItem = this.schemaItems.find(el => el.id === fieldId);
        let format = '';
        if(schemaItem) {
          format = schemaItem.format;
        }

        value = this.$helper.parseDateToIso(value, format);
      }

      this.$set(this.diplomaData, fieldId, value);

      if(fieldId && fieldId.toLowerCase() === 'institution') {
        this.updateInstitutionProps(value);
      }

      if(fieldId && fieldId.toLowerCase() === 'sppooprofession') {
        this.updateSppooSpecialityProps(value);
      }
    },
    updateInstitutionProps(value){
      if(typeof value !== 'object' || !this.schemaItems) return;

      if(value === null || value === undefined) {
        // Ресет
        if(this.schemaItems.some(x => x.id && x.id === 'institutionTown')) this.$set(this.diplomaData, 'institutionTown', null);
        if(this.schemaItems.some(x => x.id && x.id === 'institutionCity')) this.$set(this.diplomaData, 'institutionCity', null);
        if(this.schemaItems.some(x => x.id && x.id === 'institutionMunicipality')) this.$set(this.diplomaData, 'institutionMunicipality', null);
        if(this.schemaItems.some(x => x.id && x.id === 'institutionRegion')) this.$set(this.diplomaData, 'institutionRegion', null);
        if(this.schemaItems.some(x => x.id && x.id === 'institutionLocalArea')) this.$set(this.diplomaData, 'institutionLocalArea', null);
      } else {
        if(Object.prototype.hasOwnProperty.call(value, 'town')) {
          if(this.schemaItems.some(x => x.id && x.id === 'institutionTown')) this.$set(this.diplomaData, 'institutionTown', value['town']);
          if(this.schemaItems.some(x => x.id && x.id === 'institutionCity')) this.$set(this.diplomaData, 'institutionCity', value['town']);
        }

        if(Object.prototype.hasOwnProperty.call(value, 'municipality')) {
          if(this.schemaItems.some(x => x.id && x.id === 'institutionMunicipality')) this.$set(this.diplomaData, 'institutionMunicipality', value['town']);
        }

        if(Object.prototype.hasOwnProperty.call(value, 'region')) {
          if(this.schemaItems.some(x => x.id && x.id === 'institutionRegion')) this.$set(this.diplomaData, 'institutionRegion', value['town']);
        }

        if(Object.prototype.hasOwnProperty.call(value, 'localArea')) {
          if(this.schemaItems.some(x => x.id && x.id === 'institutionLocalArea')) this.$set(this.diplomaData, 'institutionLocalArea', value['town']);
        }
      }
    },
    updateSppooSpecialityProps(value) {
      const sppoospeciality = this.schemaItems.filter(x => (x.id || '').toLowerCase() === 'sppoospeciality')[0];

      if(!value || !this.schemaItems) {
        if(sppoospeciality) {
          this.$set(sppoospeciality, 'filter', {});
        }
        return;
      }

      if(sppoospeciality) {
        this.$set(sppoospeciality, 'filter', { relatedObject: value.value });
      }
    },
    getRules(item) {
      const rules = [];
      if(!this.includeValidations) return rules;

      if(item.required) rules.push(this.$validator.required());
      if(item.type === 'TextField') {
        if (item.id === 'gpa' ||  item.id === 'stateExamQualificationGrade') {
          rules.push(this.$validator.decimalNumber({integerNumbersCount: 1, realNumbersCount: 2}));
        }

        const min = item.min ? Number(item.min) : NaN;
        const max = item.max ? Number(item.max) : NaN;
        if(!isNaN(min)) rules.push(this.$validator.minLength(min));
        if(!isNaN(max)) rules.push(this.$validator.maxLength(max));
      }

      if(item.type === 'NumberField') {
        if (item.id === 'gpa' ||  item.id === 'stateExamQualificationGrade' || item.id === 'eduDuration') {
          rules.push(this.$validator.decimalNumber({integerNumbersCount: 1, realNumbersCount: 2}));
        }
        else {
          rules.push(this.$validator.numbers());
        }

        const min = item.min ? Number(item.min) : NaN;
        const max = item.max ? Number(item.max) : NaN;
        if(!isNaN(min)) rules.push(this.$validator.min(min));
        if(!isNaN(max)) rules.push(this.$validator.max(max));
      }

      return rules;
    },
  }
};
</script>
