<template>
  <div
    v-if="model"
  >
    <v-expansion-panel-header>
      <v-card-actions>
        {{ model.order }} / {{ $t('dynamicField.title') }}<strong class="ml-2"><i>{{ model.label }}</i></strong>
        <v-spacer />
        <button-tip
          icon
          icon-name="mdi-delete"
          icon-color="error"
          iclass=""
          tooltip="dynamicField.removeFieldBtnTooltip"
          bottom
          color="error"
          @click="$emit('fieldRemove', model)"
        />
      </v-card-actions>
    </v-expansion-panel-header>

    <v-expansion-panel-content>
      <v-row>
        <v-col
          cols="12"
        >
          {{ model }}
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-combobox
            v-model="model.id"
            :items="sortedIds"
            :return-object="false"
            :label="$t('dynamicField.id')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
            class="required"
            :rules="[$validator.required()]"
            persistent-hint
            hint="Използва се за извличането на стойността на полето в дизайнера на справки. Следва да се съблюдава стриктна конвенция."
            @change="onIdChange"
          />
        </v-col>
        <v-col
          v-if="showDbSettings"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-text-field
            v-model="model.columnName"
            :label="$t('dynamicField.columnName')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
            :class="showDbSettings ? 'required' : ''"
            :rules="showDbSettings ? [$validator.required()] : []"
            persistent-hint
            hint="Използва се при динамичните номенклатури"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-select
            v-model="model.type"
            :items="fieldTypeOptions"
            :label="$t('dynamicField.type')"
            :disabled="disabled"
            :readonly="readonly"
            class="required"
            :rules="[$validator.required()]"
            clearable
          />
        </v-col>
        <v-col
          v-if="isFielsWithApiCall"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-combobox
            v-model="model.optionsUrl"
            :items="predefinedFields"
            :return-object="false"
            :label="$t('dynamicField.optionsUrl')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
            :class="requiredOptionsUrl ? 'required' : ''"
            :rules="requiredOptionsUrl ? [$validator.required()] : []"
            persistent-hint
            :hint="requiredOptionsUrl ? 'Задължително поле' : ''"
          />
        </v-col>
        <v-col
          v-if="isFielsWithApiCall"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-checkbox
            v-model="model.deferOptionsLoading"
            :label="$t('dynamicField.deferOptionsLoading')"
            :disabled="disabled"
            :readonly="readonly"
            :hint="$t('dynamicField.deferOptionsLoadingHint')"
            persistent-hint
          />
        </v-col>
        <v-col
          v-if="isFielsWithApiCall"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-checkbox
            v-model="model.showDeferredLoadingHint"
            :label="$t('dynamicField.showDeferredLoadingHint')"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <v-col
          v-if="isFielsWithApiCall"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-checkbox
            v-model="model.returnObject"
            :label="$t('dynamicField.returnObject')"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <v-col
          v-if="showItemValue"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-text-field
            v-model="model.itemValue"
            :label="$t('dynamicField.itemValue')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
            persistent-hint
            :hint="$t('dynamicField.itemValueHint')"
          />
        </v-col>
        <v-col
          v-if="showItemText"
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-text-field
            v-model="model.itemText"
            :label="$t('dynamicField.itemText')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
            persistent-hint
            :hint="$t('dynamicField.itemTextHint')"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-text-field
            v-model="model.label"
            :label="$t('dynamicField.label')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
            class="required"
            :rules="[$validator.required()]"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="3"
        >
          <v-text-field
            v-model="model.labelEn"
            :label="$t('dynamicField.labelEn')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="6"
          md="3"
        >
          <v-text-field
            v-model="model.class"
            :label="$t('dynamicField.class')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="6"
          md="3"
        >
          <v-text-field
            v-model="model.order"
            type="number"
            :label="$t('dynamicField.order')"
            :rules="[$validator.min(1), $validator.max(1000)]"
            oninput="if(Number(this.value) < 1) this.value = 1;"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <!-- <v-col
          v-if="isDateField"
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-text-field
            v-model="model.format"
            :label="$t('dynamicField.dateFormat')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col> -->
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.required"
            :label="$t('dynamicField.required')"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <!-- <v-col
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.editable"
            :label="$t('dynamicField.editable')"
            :disabled="disabled"
            :readonly="readonly"
            :hint="$t('dynamicField.editableHint')"
            persistent-hint
          />
        </v-col> -->
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.readonly"
            :label="$t('dynamicField.readonly')"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.clearable"
            :label="$t('dynamicField.clearable')"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <v-col
          v-if="showMinMax"
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-text-field
            v-model="model.min"
            type="number"
            :label="isNumericField ? $t('dynamicField.min') : $t('dynamicField.minLength')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          v-if="showMinMax"
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-text-field
            v-model="model.max"
            type="number"
            :label="isNumericField ? $t('dynamicField.max') : $t('dynamicField.maxLength')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.persistentHint"
            :label="$t('dynamicField.persistentHint')"
            :disabled="disabled"
            :readonly="readonly"
          />
        </v-col>
        <v-col
          v-if="showDbSettings"
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.visible"
            :label="$t('dynamicField.visible')"
            :disabled="disabled"
            :readonly="readonly"
            persistent-hint
            hint="Определя дали ще се покаже като колона в грида"
          />
        </v-col>
        <v-col
          v-if="showDbSettings"
          cols="12"
          md="6"
          lg="4"
          xl="2"
        >
          <v-checkbox
            v-model="model.filterable"
            :label="$t('dynamicField.filterable')"
            :disabled="disabled"
            :readonly="readonly"
            persistent-hint
            hint="Определя дали по колоната в грида ще се позволява филтриране"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="model.hint"
            :label="$t('dynamicField.hint')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <v-text-field
            v-model="model.contextInfo"
            :label="$t('dynamicField.contextInfo')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          sm="2"
        >
          <v-select
            v-model="model.cols"
            :items="colsOptions"
            :label="$t('dynamicField.vuetifyGridSystem.cols')"
            :disabled="disabled"
            :readonly="readonly"
            :class="useVuetifyGridSystem ? 'required' : ''"
            :rules="useVuetifyGridSystem ? [$validator.required()] : []"
            clearable
          />
        </v-col>
        <v-col
          cols="6"
          sm="2"
        >
          <v-select
            v-model="model.sm"
            :items="colsOptions"
            :label="$t('dynamicField.vuetifyGridSystem.sm')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="6"
          sm="2"
        >
          <v-select
            v-model="model.md"
            :items="colsOptions"
            :label="$t('dynamicField.vuetifyGridSystem.md')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="6"
          sm="2"
        >
          <v-select
            v-model="model.lg"
            :items="colsOptions"
            :label="$t('dynamicField.vuetifyGridSystem.lg')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
        <v-col
          cols="6"
          sm="2"
        >
          <v-select
            v-model="model.xl"
            :items="colsOptions"
            :label="$t('dynamicField.vuetifyGridSystem.xl')"
            :disabled="disabled"
            :readonly="readonly"
            clearable
          />
        </v-col>
      </v-row>
      <v-alert
        border="left"
        colored-border
        type="success"
        dense
      >
        <v-card-title>
          Стойност по подразбиране
        </v-card-title>

        <v-row dense>
          <v-col
            :key="model + '_' + _uid"
            :cols="$helper.getColsProp(model)"
            :xl="$helper.getXlProp(model)"
            :lg="$helper.getLgProp(model)"
            :md="$helper.getMdProp(model)"
            :sm="$helper.getSmProp(model)"
          >
            <c-info
              :info-text="model.contextInfo"
            >
              <component
                :is="model.type"
                :id="model.id"
                :key="`${_uid}-${model.optionsUrl}-${model.deferOptionsLoading.toString()}`"
                :ref="'diplomaSectionItem' + model.uid"
                v-model="model.defaultValue"
                v-bind="model"
                :readonly="model.readonly"
                :clearable="model.clearable || model.type === 'YearPicker'"
                :class="(model.class || '') + (model.required === true ? ' required' : '' )"
                :hint="model.hint"
                :persistent-hint="model.persistentHint"
                :static-text="model.label"
                :min="model.min || undefined"
                :max="model.max || undefined"
                :show-current-school-year-button="model.type === 'SchoolYearField'"
                :show-current-year-button="model.type === 'YearPicker'"
                dense
              />
            </c-info>
          </v-col>
        </v-row>
      </v-alert>
    </v-expansion-panel-content>
  </div>
</template>

<script>
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


import Constants from "@/common/constants.js";

export default {
  name: 'DynamicSectionFieldEditor',
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
      default() {
        return {};
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    readonly: {
      type: Boolean,
      default() {
        return false;
      }
    },
    showDbSettings: {
      type: Boolean,
      default() {
        return false;
      }
    },
    useVuetifyGridSystem: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      dummySearch: '',
      model: this.value,
      fieldTypeOptions: Constants.DynamicField.FieldTypeOptions,
      colsOptions: Constants.DynamicField.ColsOptions,
      fieldsWithApiCall: Constants.DynamicField.FieldsWithApiCall,
      predefinedFields: Constants.DynamicField.PredefinedFields,
      ids: Constants.DynamicField.FieldIds
    };
  },
  computed: {
    isFielsWithApiCall() {
      return this.model && this.model.type && this.fieldsWithApiCall.includes(this.model.type);
    },
    sortedIds() {
      return this.ids.slice().sort((a,b)=> {return a.text.localeCompare(b.text);});
    },
    requiredOptionsUrl() {
      return this.isFielsWithApiCall;
    },
    showItemValue() {
      return this.isFielsWithApiCall || (this.model && this.model.type && this.model.type === 'SchoolYearField');
    },
    showItemText() {
      return this.showItemValue;
    },
    isNumericField() {
       return this.model && this.model.type && this.model.type === 'NumberField';
    },
    isTextField() {
      return this.model && this.model.type && this.model.type === 'TextField';
    },
    showMinMax() {
      return this.isTextField || this.isNumericField;
    },
    isDateField() {
      return this.model && this.model.type && this.model.type === 'DateField';
    }

  },
  watch: {
    'model.type': {
      handler(){
        this.resetDefaultValue();
      },
    },
    'model.optionsUrl': {
      handler(){
        this.resetDefaultValue();
      }
    },
    'model.returnObject': {
      handler(){
        this.resetDefaultValue();
      }
    },
    'model.itemValue': {
      handler(){
        this.resetDefaultValue();
      }
    },
  },
methods: {
  resetDefaultValue() {
    if(this.model.defaultValue) {
      this.model.defaultValue = null;
    }
  },
  onIdChange(event) {
    if(event) {
      const option = this.predefinedFields.find(x => x.name && (x.name || '').toLowerCase() === event.toLowerCase());
      if(option) {
        this.model.type = option.type;
        if(option.value) this.model.optionsUrl = option.value;
        if(option.label) {
          this.model.label = option.label;
          this.model.labelEn = option.label;
        }
        if(option.required) this.model.required = option.required;
        if(option.returnObject) this.model.returnObject = option.returnObject;
        if(option.itemValue) this.model.itemValue = option.itemValue;
        if(option.itemText) this.model.itemText = option.itemText;
        if(option.deferOptionsLoading) this.model.deferOptionsLoading = option.deferOptionsLoading;
        if(option.type) this.model.type = option.type;
      }
    } else {
      this.model.type = '';
      this.model.optionsUrl = '';
      this.model.returnObject = false;
      this.model.deferOptionsLoading = false;
      this.model.showDeferredLoadingHint = false;
      this.model.itemText = '';
      this.model.itemValue = '';
      this.model.persistentHint = false;
    }
  }
}
};
</script>
