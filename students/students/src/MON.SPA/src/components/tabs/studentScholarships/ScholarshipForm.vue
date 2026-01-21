<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row dense>
        <v-col
          cols="12"
          sm="6"
          lg="2"
        >
          <slot name="schoolYear">
            <school-year-selector
              v-if="form.institutionId"
              v-model="form.schoolYear"
              :label="$t('common.schoolYear')"
              :institution-id="form.institutionId"
              class="required"
              :clearable="false"
              :rules="[$validator.required()]"
              show-current-school-year-button
              outlined
              persistent-placeholder
            />
          </slot>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="10"
        >
          <v-autocomplete
            v-model="form.scholarshipFinancingOrganId"
            :items="financingOrganOptions"
            :label="$t('studentScholarships.scholarshipFinancingOrgan')"
            clearable
            class="required"
            :rules="[$validator.required()]"
            outlined
            persistent-placeholder
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="2"
        >
          <c-select
            v-model="form.scholarshipAmountId"
            :items="scholarshipAmountOptions"
            :label="$t('studentScholarships.scholarshipAmount')"
            clearable
            class="required"
            :rules="[$validator.required()]"
            outlined
            persistent-placeholder
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="2"
        >
          <c-text-field
            v-if="!disabled"
            v-model.number="form.amountRate"
            :label="$t('studentScholarships.amountRate')"
            type="number"
            autocomplete="amount"
            :rules="[$validator.required(), $validator.min(0), amountRateValidator()]"
            class="required"
            outlined
            persistent-placeholder
          >
            <template
              v-if="currency.showAltCurrency"
              v-slot:prepend-inner
            >
              <span class="mt-1">{{ form.currency || currency.currency.code }}</span>
            </template>
          </c-text-field>
          <c-text-field
            v-else
            :value="form.amountRateStr"
            :label="$t('studentScholarships.amountRate')"
            class="required"
            outlined
            persistent-placeholder
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="2"
        >
          <v-select
            v-model="form.periodicity"
            :items="periodicityOptions"
            :label="$t('studentScholarships.periodicity.periodicityLabel')"
            clearable
            class="required"
            :rules="[$validator.required()]"
            outlined
            persistent-placeholder
          />
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="6"
        >
          <c-select
            v-model="form.scholarshipTypeId"
            :items="scholarshipTypeOptions.filter(filterScholarshipTypes(form.periodicity, form.scholarshipFinancingOrgan))"
            :label="$t('studentScholarships.scholarshipType')"
            clearable
            class="required"
            :rules="[$validator.required()]"
            outlined
            persistent-placeholder
          />
        </v-col>
      </v-row>

      <v-row dense>
        <v-row dense>
          <v-col cols="12">
            <date-picker
              id="startingDateOfReceiving"
              ref="startingDateOfReceiving"
              v-model="form.startingDateOfReceiving"
              :label="$t('studentScholarships.startingDateOfReceiving')"
              :show-buttons="false"
              :scrollable="false"
              no-title
              :rules="[$validator.required()]"
              class="required"
              outlined
              persistent-placeholder
            />
          </v-col>
          <v-col cols="12">
            <date-picker
              id="endDateOfReceiving"
              ref="endDateOfReceiving"
              v-model="form.endDateOfReceiving"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :label="$t('studentScholarships.endDateOfReceiving')"
              outlined
              persistent-placeholder
            />
          </v-col>
        </v-row>

        <v-divider
          vertical
          class="mx-4"
        />

        <v-row dense>
          <v-col cols="12">
            <c-text-field
              v-model="form.orderNumber"
              :label="$t('studentScholarships.orderNumber')"
              outlined
              persistent-placeholder
            />
          </v-col>
          <v-col cols="12">
            <date-picker
              id="orderDate"
              ref="orderDate"
              v-model="form.orderDate"
              :label="$t('studentScholarships.orderDate')"
              :show-buttons="false"
              :scrollable="false"
              no-title
              outlined
              persistent-placeholder
            />
          </v-col>
        </v-row>

        <v-divider
          vertical
          class="mx-4"
        />

        <v-row dense>
          <v-col cols="12">
            <c-text-field
              v-model="form.commissionNumber"
              :label="$t('studentScholarships.commissionNumber')"
              outlined
              persistent-placeholder
            />
          </v-col>
          <v-col cols="12">
            <date-picker
              id="commissionDate"
              ref="commissionDate"
              v-model="form.commissionDate"
              :show-buttons="false"
              :scrollable="false"
              no-title
              :label="$t('studentScholarships.commissionDate')"
              outlined
              persistent-placeholder
            />
          </v-col>
        </v-row>
      </v-row>

      <v-row dense>
        <v-col cols="12">
          <c-textarea
            v-model="form.description"
            :label="$t('studentScholarships.description')"
            outlined
            persistent-placeholder
            clearable
            rows="3"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col>
          <file-manager
            v-model="form.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-form>
  </div>
</template>

<script>

import { StudentScholarshipModel } from '@/models/studentScholarship/studentScholarshipModel.js';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import FileManager from '@/components/common/FileManager.vue';
import { mapGetters } from 'vuex';

export default {
  name: 'ScholarshipForm',
    components: {
      FileManager,
      SchoolYearSelector
  },
  props: {
    document:{
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    },
  },
  data()
  {
    return {
      form: new StudentScholarshipModel(),
      scholarshipTypeOptions: [],
      scholarshipAmountOptions: [],
      periodicityOptions:[],
      institutionOptions: [],
      financingOrganOptions:[],
      scholarshipReasonsOptions: [],
    };
  },
  computed: {
  ...mapGetters(['studentFinalizedLods', 'currency'])
  },
  watch: {
    document() {
      this.form = this.document ?? new StudentScholarshipModel();
    }
  },
  mounted() {
    this.fillSelectOptions();
  },
  methods: {
    fillSelectOptions() {
        this.periodicityOptions.push( { text:this.$t('studentScholarships.periodicity.monthly'), value: 1});
        this.periodicityOptions.push( { text:this.$t('studentScholarships.periodicity.oneTimeOnly'), value: 2});

        this.$api.lookups.getScholarshipTypeOptions()
                .then((response) => {
                  this.scholarshipTypeOptions = response.data;
                })
                .catch((error) => {
                    this.$notifier.error('', this.$t('errors.scholarshipTypeOptionsLoad'));
                    console.log(error);
                });

        this.$api.lookups.getScholarshipAmountOptions()
                .then((response) => {
                    this.scholarshipAmountOptions = response.data;
                })
                .catch((error) => {
                    this.$notifier.error('', this.$t('errors.scholarshipAmountOptionsLoad'));
                    console.log(error);
                });

        this.$api.lookups.getScholarshipFinancingOrgans()
                .then((response) => {
                    this.financingOrganOptions = response.data;
                })
                .catch((error) => {
                    this.$notifier.error('', this.$t('errors.scholarshipFinancingOrgansLoad'));
                    console.log(error);
                });
    },
    filterScholarshipTypes(periodicity,scholarshipFinancingOrgan){
      return function(item){
        if(scholarshipFinancingOrgan){
          if(periodicity === null){
            if(scholarshipFinancingOrgan.value !== 4 || scholarshipFinancingOrgan.value === null){
              return item.value !== 6;
            }
            return true;
          }

          if(scholarshipFinancingOrgan.value === 4){
            return (item.relatedObjectId === periodicity || item.relatedObjectId === 0);
          }
        }
        return (item.relatedObjectId === periodicity || item.relatedObjectId === 0) &&  item.value !== 6;
      };
    },
    amountRateValidator() {
      return (value) => {
        const pattern = /^\d{0,4}(\.\d{1,2})?$/;
        return !value || pattern.test(value) || this.$t('validation.decimalNumberFormat', { numberOfDigitsBefore: 4, numberOfDigitsAfter: 2 });
      };
    },
    validate() {
      // Използва се в StudentAdmissionDocumentCreate.vue и StudentAdmissionDocumentEdit.vue
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
  }
};
</script>
