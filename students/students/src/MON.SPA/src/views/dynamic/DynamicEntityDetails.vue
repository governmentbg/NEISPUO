<template>
  <div>
    DynamicEntityDetails
    {{ id }}
    {{ type }}
    <hr>
    <div>
      requiredPermissions: {{ requiredPermissions }}
    </div>
    <hr>
    <div>
      hasPermisson: {{ hasPermisson }}
    </div>

    {{ form }}
    
    <form-layout>
      <template #title>
        <h3>Детайли на {{ type }}</h3>
      </template>

      <template #default>
        <div
          v-for="section in visibleSections"
          :key="section.id"
        >
          <v-card-title>
            {{ section.title }}
          </v-card-title>

          <v-row
            v-if="form"
          >
            <v-col 
              v-for="(itemField, itemIndex) in section.items.filter(x => x.editable === true)"
              :key="itemField + '_' + itemIndex"
              :cols="getColsProp(itemField)"
              :xl="getXlProp(itemField)"
              :lg="getLgProp(itemField)"
              :md="getMdProp(itemField)"
              :sm="getSmProp(itemField)"
            >
              <c-info
                :info-text="itemField.contextInfo"
              >
                <component
                  :is="itemField.type"
                  :value="form[itemField.id]"
                  v-bind="itemField"
                  :hint="itemField.hint"
                  :persistent-hint="itemField.persistentHint"
                  disabled
                />
              </c-info>
            </v-col>
          </v-row>
        </div>
      </template>

      <template #actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>          
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';

import TextField from '@/components/wrappers/form/TextField';
import NumberField from '@/components/wrappers/form/NumberField';
import SelectList from '@/components/wrappers/form/SelectList';
import DateField from '@/components/wrappers/form/DateField';
import PersonUniqueIdField from "@/components/wrappers/form/PersonUniqueIdField";
import SchoolYearField from "@/components/wrappers/form/SchoolYearField";
import LabelField from '@/components/wrappers/form/LabelField';
import BooleanField from '@/components/wrappers/form/BooleanField';
import AutoComplete from '@/components/wrappers/CustomAutocomplete';
import YearPicker from '@/components/wrappers/YearPicker';

export default {
  name: 'DynamicEntityDetails',
  components: {
    TextField,
    NumberField,
    SelectList,
    DateField,
    PersonUniqueIdField,
    SchoolYearField,
    LabelField,
    BooleanField,
    AutoComplete,
    YearPicker
  },
  props: {
    id: {
      type: [Number,String],
      required: true
    },
    type: {
      type: String,
      required: true
    },
    returnUrl: {
      type: String,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      form: null
    };
  },
  computed: {
    ...mapGetters(['dynamicEntitiesSchema', 'permissions']),
    requiredEntitySchema() {
      const schema = this.dynamicEntitiesSchema && this.dynamicEntitiesSchema.entities
        ? this.dynamicEntitiesSchema.entities.find(x => x.name === this.type)
        : null;
      return schema;
    },
    requiredPermissions() {
      const schema = this.requiredEntitySchema;
      const policies = schema
        ? schema.security.requiredRermissions.read
        : [];
      return policies;
    },
    hasPermisson() {
      const requiredPermissions = this.requiredPermissions;

      if(!requiredPermissions) return true;
      if(!this.permissions) return false;

      return this.requiredPermissions.every(x => this.permissions.includes(x));
    },
    visibleSections() {
      const schema = this.requiredEntitySchema;
      return schema && schema.sections
        ? schema.sections.filter(x => x.visible === true)
        : null;
    }
  },
  async mounted() {
    if(!this.dynamicEntitiesSchema) {
      await this.loadEntitiesJsonDescription();
    }

    await this.loadEntityModel();
  },
  methods: {
    async loadEntitiesJsonDescription() {
      try {
        const result = await this.$api.dynamicForm.getEntitiesJsonDescription();    
        this.$store.commit('setDynamicEntitiesSchema', result ? result.data : {});
      } catch (error) {
        this.$store.commit('setDynamicEntitiesSchema', {});
      }
    },
    async loadEntityModel() {
      try {
        const result = await this.$api.dynamicForm.getEntityModel(this.type, this.id);

        if(result.data) {
          this.form = {...JSON.parse(result.data)[0]};
        }
      } catch (error) {
        console.log(error.response);
        this.$notifier.error('Loading entity', error.response.data.message, 5000);
      }
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
    backClick() {
      if(this.returnUrl) {
        this.$router.push(this.returnUrl);
      } else {
        this.$router.go(-1);
      }
    }
  }
};
</script>