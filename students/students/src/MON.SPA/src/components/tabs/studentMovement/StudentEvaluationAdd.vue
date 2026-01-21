<template>
  <div>
    <v-form 
      @submit.stop.prevent="validate"
    >
      <v-card>
        <v-card-title>
          {{ getTranslation('title') }}
        </v-card-title>
        <v-card-text>
          <v-row>
            <v-col
              cols="12"
              sm="4"
            >
              <combo
                id="subjectOptions"
                ref="subjectOptions"
                v-model="form.dropdownModel"
                api="/api/lookups/GetSubjectOptions"
                :label="getTranslation('subjectName')"
                :placeholder="$t('buttons.search')"
                :return-object="true"
                :defer-options-loading="true"
                :clearable="true"
                :hide-no-data="true"
                :hide-selected="true"
                :disabled="sending"
                :remove-items-on-clear="true"
                :show-deferred-loading-hint="true"
                :error-messages="getRequiredFieldValidationMessage('dropdownModel', true)"
                class="required"
                @blur="$v.form.dropdownModel.$touch()"
                @change="subjectChange($event)"
              />
            </v-col>
            <v-col
              cols="12"
              sm="4"
            >
              <v-text-field
                v-if="showTwoYearsAgoEvaluation()"
                v-model="form.twoYearsAgoEvaluation"    
                :label="twoYearsAgoEvaluationLabel"
                :disabled="sending"
              />
            </v-col>
            <v-col
              cols="12"
              sm="4"
            >
              <v-text-field
                v-if="showOneYearAgoEvaluation()"
                v-model="form.oneYearAgoEvaluation"    
                :label="oneYearAgoEvaluationLabel"
                :disabled="sending"
              />
            </v-col>
          </v-row>
          
          <v-row>
            <v-col
              cols="12"
              sm="4"
            >
              <v-text-field
                v-model="form.firstTermEvaluation"
                :disabled="sending"  
                :label="getTranslation('firstTermEvaluation')"
              />
            </v-col>
            <v-col
              cols="12"
              sm="4"
            >
              <v-text-field
                v-model="form.secondTermEvaluation"
                :disabled="sending"
                :label="getTranslation('secondTermEvaluation')"
              />
            </v-col>
            <v-col
              cols="12"
              sm="4"
            >
              <v-text-field
                v-model="form.annualEvaluation"
                :disabled="sending"
                :label="getTranslation('annualEvaluation')"
              />
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions
          class="justify-center"
        >
          <v-btn
            ref="submit"
            raised
            small
            :disabled="sending"
            color="primary"
            type="submit"
          >
            <v-icon left>
              fas fa-save
            </v-icon>          
            {{ getTranslation('saveChangesBtn') }}
          </v-btn>

          <v-btn
            raised
            small
            :disabled="sending"
            color="error"
            @click="onReset"
          >
            <v-icon left>
              fas fa-times
            </v-icon>          
            {{ getTranslation('cancelBtn') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Constants from '@/common/constants.js';
import Helper from '@/components/helper.js';

import { validationMixin } from 'vuelidate';
import { required  } from 'vuelidate/lib/validators';

import { StudentEvaluation } from '@/models/studentEvaluation.js';

export default {
  mixins: [validationMixin],
  props: {
    classNumber: {
      type: Number,
      default() {
        return null;
      }
    },
    sending: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      form: new StudentEvaluation(),
      helper: Helper,
    };
  },
  validations: {
    form: {
      dropdownModel: {
        required
      }
    }
  },
  computed: {
    twoYearsAgoEvaluationLabel() {
      return this.getTranslation('annualEvaluationLabel', [this.classNumber - 2]);
    },
    oneYearAgoEvaluationLabel() {
      return this.getTranslation('annualEvaluationLabel', [this.classNumber - 1]);
    },
  },
  methods: {
    getTranslation(text, params) {      
      return this.$t(`studentEvaluations.${text}`, params);
    },
    showOneYearAgoEvaluation() {
      return this.classNumber === Constants.SIXTH_GRADE || this.showTwoYearsAgoEvaluation();
    },
    showTwoYearsAgoEvaluation() {
      return this.classNumber === Constants.SEVENTH_GRADE;
    },
    subjectChange(value) {
      if(typeof value === 'object' && value !== null) {
        this.$refs.subjectOptions.optionsList = [];
      }
      else {
        this.$v.form.dropdownModel.$touch();
      }
    },
    getRequiredFieldValidationMessage(fieldName, isDropdown) {
      const field = this.$v.form[fieldName];

      return this.helper.getRequiredValidationMessage(field, isDropdown);
    },
    async validate() {
      const confirmed = await this.helper.validate(this, [this.$v.form.dropdownModel.$model]);
      if(confirmed){
          this.$emit('addNewEvaluation', this.form);
          this.onReset();
      }
    },
    onReset() {
      this.form = new StudentEvaluation();
      this.$emit('addNewEvaluationCancel');
    }
  }
};
</script>