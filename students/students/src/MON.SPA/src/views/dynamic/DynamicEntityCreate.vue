<template>
  <div>
    DynamicEntityCreate
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

    <form-layout
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>Създаване на {{ type }}</h3>
      </template>

      <template #default>
        <v-form
          v-if="form"
          :ref="'dynamicCreateForm' + _uid"
          :disabled="saving"
        >
          <div
            v-for="section in visibleSections"
            :key="section.id"
          >
            <v-card-title>
              {{ section.title }}
            </v-card-title>

            <v-row>
              <v-col 
                v-for="(itemField, itemIndex) in section.items.filter(x => x.editable === true)"
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
                  <component
                    :is="itemField.type"
                    v-model="form[itemField.id]"
                    v-bind="itemField"
                    :clearable="itemField.clearable || !itemField.required"
                    :readonly="itemField.readonly"
                    :class="itemField.class + (itemField.required === true ? ' required' : '' )"
                    :rules="getRules(itemField)"
                    :hint="itemField.hint"
                    :persistent-hint="itemField.persistentHint"
                    :disabled="itemField.disabled || !itemField.editable"
                  />
                </c-info>
              </v-col>
            </v-row>
          </div>
        </v-form>
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
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
  name: 'DynamicEntityCreate',
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
      form: null,
      saving: false,
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
        ? schema.security.requiredRermissions.create
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

    this.form = {
      entityTypeName: this.type
    };
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
    getRules(item) {
      const rules = [];
      if(item.required) rules.push(this.$validator.required());
      if(item.type === 'TextField') {
        if(item.min) rules.push(this.$validator.minLength(item.min));
        if(item.max) rules.push(this.$validator.maxLength(item.max));
      }

      if(item.type === 'NumberField') {
        rules.push(this.$validator.numbers());

        const min = Number(item.min);
        if(min) rules.push(this.$validator.min(min));

        const max = Number(item.max);
        if(max) rules.push(this.$validator.max(max));
      }

      return rules;
    },
    onSave() {
      const form = this.$refs['dynamicCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;
      this.$api.dynamicForm
        .create(this.form)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.goBack();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error.response.message, 5000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; }); 
    },
    onCancel() {
      this.goBack();
    },
    goBack() {
      if(this.returnUrl) {
        this.$router.push(this.returnUrl);
      } else {
        this.$router.go(-1);
      }
    }
  },
};
</script>