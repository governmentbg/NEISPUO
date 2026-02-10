<template>
  <v-form
    :ref="'absenceCampaignForm' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="absenceCampaign.schoolYear"
        >
          <school-year-selector
            v-model="model.schoolYear"
            :label="$t('absenceCampaign.headers.schoolYear')"
            clearable
            :rules="[$validator.required()]"
            :min-year="minYear"
            class="required"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="absenceCampaign.month"
        >
          <custom-month-picker
            v-model="model.month"
            :label="$t('absenceCampaign.headers.month')"
            clearable
            :rules="[$validator.required()]"
            class="required"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="absenceCampaign.fromDate"
        >
          <date-picker
            id="fromDate"
            ref="fromDate"
            v-model="model.fromDate"
            :label="$t('absenceCampaign.headers.fromDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            :show-debug-data="false"
            :rules="[$validator.required()]"
            :min="$moment().format()"
            class="required"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="absenceCampaign.toDate"
        >
          <date-picker
            id="toDate"
            ref="toDate"
            v-model="model.toDate"
            :label="$t('absenceCampaign.headers.toDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            :show-debug-data="false"
            :rules="[$validator.required()]"
            :min="$moment().format()"
            class="required"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="12"
        lg="6"
      >
        <c-info
          uid="absenceCampaign.name"
        >
          <v-text-field
            v-model="model.name"
            :label="$t('absenceCampaign.headers.name')"
            clearable
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="12"
        lg="6"
      >
        <c-info
          uid="absenceCampaign.description"
        >
          <v-textarea
            v-model="model.description"
            :label="$t('absenceCampaign.headers.description')"
            prepend-icon="mdi-comment"
            outlined
            clearable
          />
        </c-info>
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import CustomMonthPicker from "@/components/wrappers/CustomMonthPicker";
import SchoolYearSelector from '@/components/common/SchoolYearSelector';

import { AbsenceCampaignModel } from "@/models/absence/absenceCampaignModel.js";

export default {
  name: 'AbsenceCampaignForm',
  components: {
    CustomMonthPicker,
    SchoolYearSelector
  },
  props: {
    value:{
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
    },
    minYear: {
      type: [Number, String],
      default() {
        return undefined;
      }
    },
  },
  data() {
    return {
      model: this.value ?? new AbsenceCampaignModel()
    };
  },
  methods: {
    validate() {
      return this.$refs['absenceCampaignForm' + this._uid].validate();
    }
  },
};
</script>
