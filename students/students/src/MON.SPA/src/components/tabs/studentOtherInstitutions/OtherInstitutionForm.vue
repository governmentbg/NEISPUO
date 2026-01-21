<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-card>
      <v-card-subtitle>
        {{ $t('studentOtherInstitutions.subtitle') }}
      </v-card-subtitle>
      <v-card-text class="ml-0 mr-0 pa-0">
        <v-row
          style=" margin-left: 5px;"
        >
          <v-col
            cols="12"
            sm="6"
          >
            <v-text-field
              v-model="model.reason"
              :label="$t('studentOtherInstitutions.reasonLabel')"
              autocomplete="reason"
            />
          </v-col>
          <v-col
            cols="12"
            sm="5"
          >
            <combo
              id="institutionDropdown"
              ref="institutionDropdown"
              v-model="model.institution"
              api="/api/lookups/GetInstitutionOptions"
              :label="$t('documents.institutionDropdownLabel')"
              :placeholder="$t('buttons.search')"
              :return-object="true"
              :defer-options-loading="true"
              :clearable="true"
              :hide-no-data="true"
              :hide-selected="true"
              :remove-items-on-clear="true"
              :show-deferred-loading-hint="true"
              :rules="[$validator.required()]"
              class="required"
            />
          </v-col>
        </v-row>
        <v-row
          style=" margin-left: 5px;"
        >
          <v-col
            cols="12"
            sm="6"
          >
            <date-picker
              id="validFrom"
              ref="validFrom"
              v-model="model.validFrom"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('common.validFrom')"
            />
          </v-col>
          <v-col
            cols="12"
            sm="5"
          >
            <date-picker
              id="validTo"
              ref="validTo"
              v-model="model.validTo"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('common.validTo')"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
  </v-form>
</template>

<script>
import { StudentOtherInstitutionModel } from '@/models/studentOtherInstitutionModel.js';

export default {
name: 'OtherInstitutionForm',
components: {
    
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
      institutionOptions: [],
      model: this.value ?? new StudentOtherInstitutionModel()
    };
},
mounted() {
    //this.loadOptions();
},
methods: {
    validate() {
        const form = this.$refs['form' + this._uid];
        return form ? form.validate() : false;
    }
},

      
};
</script>
      
