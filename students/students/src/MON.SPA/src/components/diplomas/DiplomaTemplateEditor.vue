<template>
  <v-card
    v-if="value"
  >
    <v-card-text>
      <!-- {{ value }} -->
      <v-form
        :ref="'DiplomaTemplateEditorForm_' + _uid"
        :disabled="disabled"
      >
        <v-row
          dense
        >
          <v-col
            cols="12"
            md="6"
            :lg="filteredBasicClassOptions && filteredBasicClassOptions.length > 0 ? '8' : '10'"
          >
            <v-text-field
              v-model="value.name"
              :label="$t('diplomas.template.name')"
              :rules="[$validator.maxLength(255), $validator.required()]"
              clearable
              class="required"
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
            lg="2"
          >
            <school-year-selector
              v-model="value.schoolYear"
              :institution-id="value.institutionId"
              :rules="[$validator.required()]"
              class="required"
            />
          </v-col>
          <v-col
            v-if="filteredBasicClassOptions && filteredBasicClassOptions.length > 0"
            cols="12"
            md="6"
            lg="2"
          >
            <v-select
              v-model="value.basicClassId"
              :items="filteredBasicClassOptions"
              :label="$t('student.class')"
              :placeholder="$t('common.choose')"
              clearable
              :defer-options-loading="false"
            />
          </v-col>
          <v-col
            cols="12"
          >
            <c-textarea
              v-model="value.description"
              :label="$t('diplomas.template.description')"
              persistent-placeholder
              outlined
            />
          </v-col>
          <!-- <v-col
            cols="12"
            md="6"
          >
            <v-text-field
              v-model="value.principal"
              :label="$t('diplomas.template.principal')"
              clearable
            />
          </v-col> -->
          <!-- <v-col
            cols="12"
            md="6"
          >
            <v-text-field
              v-model="value.deputy"
              :label="$t('diplomas.template.deputy')"
              clearable
            />
          </v-col> -->
        </v-row>
      </v-form>
    </v-card-text>
  </v-card>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
export default {
  name: 'DiplomaTemplateEditorComponent',
  components: {
    SchoolYearSelector
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      basicClassOptions: []
    };
  },
  computed: {
    filteredBasicClassOptions() {
      if(!Array.isArray(this.basicClassOptions) || this.basicClassOptions.length === 0
        || !Array.isArray(this.value.basicClassIds) || this.value.basicClassIds === 0) {
        return [];
      }

      return this.basicClassOptions.filter(x => this.value.basicClassIds.includes(x.value));
    }
  },
  mounted() {
    this.loadBasicClassOptions();
  },
  methods: {
    validate() {
      const form = this.$refs['DiplomaTemplateEditorForm_' + this._uid];
      return form ? form.validate() : false;
    },
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassOptions({minId: 1, maxId: 13})
      .then((result) => {
        if(result) {
          this.basicClassOptions = result.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    }
  }
};
</script>
