<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-card v-if="fileMetaData != null">
        <v-card-text>
          <v-row>
            <v-col
              cols="12"
              sm="4"
            >
              <date-picker
                id="fromDate"
                ref="fromDate"
                v-model="fileMetaData.fromDate"
                :show-buttons="false"
                :scrollable="false"
                :no-title="true"
                :show-debug-data="false"
                :label="$t('asp.headers.fromDate')"
                :rules="[$validator.required()]"
                class="required"
              />
            </v-col>
            <v-col
              cols="12"
              sm="4"
            >
              <date-picker
                id="toDate"
                ref="toDate"
                v-model="fileMetaData.toDate"
                :show-buttons="false"
                :scrollable="false"
                :no-title="true"
                :show-debug-data="false"
                :label="$t('asp.headers.toDate')"
                :rules="[$validator.required()]"
                class="required"
              />
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
    </v-form>
  </div>
</template>

<script>
import { MonthlyBenefitsViewModel } from "@/models/asp/monthlyBenefitsViewModel.js";

export default {
  name: 'ImportedMonthlyBenefitFileMetaDataForm',
  components: {

  },
  props: {
    fileMetaData: {
      type: MonthlyBenefitsViewModel,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
        
    };
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>