<template>
  <div>
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
        <v-card-title
          :id="section.id"
        >
          {{ section.title }}
        </v-card-title>

        <v-card-text
          v-if="section.useVuetifyGridSystem === true"
        >
          <!-- Махаме bootstrap-ските класове. Използваме VuetifyGridSystem -->

          <div 
            :class="section.class ? section.class.replace('d-flex', '').replace('flex-wrap', '') : ''"
          >
            <v-row>
              <v-col 
                v-for="(itemField, itemIndex) in section.items"
                :key="itemField + '_' + itemIndex"
                :cols="getColsProp(itemField)"
                :xl="getXlProp(itemField)"
                :lg="getLgProp(itemField)"
                :md="getMdProp(itemField)"
                :sm="getSmProp(itemField)"
              >
                <component
                  :is="itemField.type"
                  :id="itemField.id"
                  :key="itemIndex"
                  ref="diplomaSectionItem"
                  v-model="diplomaData[itemField.id]"
                  v-bind="itemField"
                  :person-unique-id-type="diplomaData.personalIdType"
                  :readonly="itemField.readonly"
                  :clearable="itemField.clearable || !itemField.required"
                  :disabled="itemField.disabled || !renderField(itemField.id) || !itemField.editable"
                  :class="(itemField.class ? itemField.class.replace(/w-[0-9]*/gi,'') : '') + (itemField.required === true ? ' required' : '' )"
                  :hint="itemField.hint"
                  :persistent-hint="itemField.persistentHint"
                  @input="updateDiplomaData(itemField.id, $event)"
                  @inputCustomField="updateDiplomaData"
                />
              </v-col>
            </v-row>
          </div>
        </v-card-text>

        <v-card-text
          v-else
        >
          <div 
            :class="section.class"
          >     
            <template v-for="(itemField, itemIndex) in section.items">
              <component
                :is="itemField.type"
                v-if="renderField(itemField.id)"
                :id="itemField.id"
                :key="itemIndex"
                ref="diplomaSectionItem"
                :value="diplomaData[itemField.id]"
                v-bind="itemField"
                :person-unique-id-type="diplomaData.personalIdType"
                :readonly="itemField.readonly"
                :clearable="itemField.clearable || !itemField.required"
                :disabled="itemField.disabled || !renderField(itemField.id)"
                :class="itemField.class"
                :hint="itemField.hint"
                :persistent-hint="itemField.persistentHint"
                @input="updateDiplomaData(itemField.id, $event)"
                @inputCustomField="updateDiplomaData"
              />
            </template>
          </div>
          <!-- <div 
            :class="section.class"
          >
            <component
              :is="itemField.type"
              v-if="renderField(itemField.id)"
              :id="itemField.id"
              :key="itemIndex"
              ref="diplomaSectionItem"
              :value="diplomaData[itemField.id]"
              v-bind="itemField"
              :person-unique-id-type="diplomaData.personalIdType"
              :readonly="itemField.readonly"
              :clearable="itemField.clearable || !itemField.required"
              :disabled="itemField.disabled || !renderField(itemField.id) || !itemField.editable"
              :class="itemField.class"
              :hint="itemField.hint"
              :persistent-hint="itemField.persistentHint"
              @input="updateDiplomaData(itemField.id, $event)"
              @inputCustomField="updateDiplomaData"
            />     
          </div> -->
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>
<script>
import TextField from '@/components/wrappers/form/TextField';
import NumberField from '@/components/wrappers/form/NumberField';
import SelectList from '@/components/wrappers/form/SelectList';
import DateField from '@/components/wrappers/form/DateField';
import PersonUniqueIdField from "@/components/wrappers/form/PersonUniqueIdField";
import SchoolYearField from "@/components/wrappers/form/SchoolYearField";
import LabelField from "@/components/wrappers/form/LabelField";
import BooleanField from "@/components/wrappers/form/BooleanField";
import AutoComplete from "@/components/wrappers/form/CustomAutocomplete";
import YearPicker from "@/components/wrappers/form/YearPicker";

export default {
  name: 'DiplomaSectionGenerator',
  components:{
    TextField,
    NumberField,
    DateField,
    SelectList,
    PersonUniqueIdField,
    SchoolYearField,
    LabelField,
    BooleanField,
    AutoComplete,
    YearPicker
  },
  props: {
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
    showDiplomaData: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      diplomaData: {}
    };
  },
  computed: {
    sortedSections() {
      if(!this.schema) return null;

      let sortedSections = this.schema.filter(x => x.visible);	
      sortedSections = sortedSections.sort((a, b) => { return a.order - b.order; });
      return sortedSections;
    },
  },
  watch: {
    initialData() {
      this.diplomaData = {};
      this.setInitialData();
    }
  },
  mounted() {
  },
  methods: {
    setInitialData() {
      if(!this.initialData) return;
      
      for (const property in this.initialData) {
        this.updateDiplomaData(property, this.initialData[property]);
      }
    },
    updateDiplomaData(fieldId, value) {
      this.$set(this.diplomaData, fieldId, value);
    },
    renderField() {
      return true;
    },
    getColsProp(field) {
      if(!field || !field.cols) return false;

      return field.cols;
    },
    getXlProp(field) {
      if(!field || !field.xl) return false;

      return field.xl;
    },
    getLgProp(field) {
      if(!field || !field.ld) return false;

      return field.ld;
    },
    getMdProp(field) {
      if(!field || !field.md) return false;

      return field.md;
    },
    getSmProp(field) {
      if(!field || !field.sm) return false;

      return field.sm;
    },
  }
};
</script>
