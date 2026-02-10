<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        cols="12"
        md="12"
      >
        <c-info
          uid="internationalMobility.project"
        >
          <v-text-field
            v-model="model.project"
            :label="$t('lod.internationalMobility.project')"
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        lg="2"
      >
        <c-info
          uid="internationalMobility.country"
        >
          <autocomplete
            id="country"
            ref="country"
            v-model="model.countryId"
            api="/api/lookups/GetCountriesBySearchString"
            :label="$t('lod.internationalMobility.country')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        lg="4"
      >
        <c-info
          uid="internationalMobility.receivingInstitution"
        >
          <v-text-field
            v-model="model.receivingInstitution"
            :label="$t('lod.internationalMobility.receivingInstitution')"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        lg="2"
      >
        <slot name="schoolYear">
          <school-year-selector
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
            :institution-id="model.institutionId"
            class="required"
            :clearable="false"
            :rules="[$validator.required()]"
            show-current-school-year-button
            outlined
            persistent-placeholder
            :show-navigation-buttons="false"
          />
        </slot>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        lg="2"
      >
        <c-info
          uid="internationalMobility.fromDate"
        >
          <date-picker
            id="fromDate"
            ref="fromDate"
            v-model="model.fromDate"
            :label="$t('lod.internationalMobility.fromDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        sm="6"
        lg="2"
      >
        <c-info
          uid="internationalMobility.toDate"
        >
          <date-picker
            id="toDate"
            ref="toDate"
            v-model="model.toDate"
            :label="$t('lod.internationalMobility.toDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
      >
        <c-info
          uid="internationalMobility.mainObjectives"
        >
          <c-textarea
            v-model="model.mainObjectives"
            :label="$t('lod.internationalMobility.mainObjectives')"
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
            clearable
            rows="3"
          />
        </c-info>
      </v-col>
    </v-row>
    <v-row>
      <v-col
        cols="12"
      >
        <file-manager
          v-model="model.documents"
          :disabled="disabled"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import { InternationalMobilityModel } from "@/models/internationalMobilityModel.js";
import FileManager from '@/components/common/FileManager.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';

export default {
  name: 'InternationalMobilityForm',
  components: {
      FileManager,
      Autocomplete,
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
    },
  },
  data() {
    return {
      model: this.value ?? new InternationalMobilityModel()
    };
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
  }
};
</script>
