<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <slot name="schoolYear">
          <school-year-selector
            v-if="model.institutionId"
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
            :institution-id="model.institutionId"
            class="required"
            :rules="[$validator.required()]"
            :clearable="false"
            show-current-school-year-button
            outlined
            persistent-placeholder
          />
        </slot>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info
          uid="award.category"
        >
          <v-text-field
            v-if="disabled"
            :value="model.awardCategoryName"
            :label="$t('lod.awards.awardCategory')"
            outlined
            persistent-placeholder
          />
          <v-select
            v-else
            v-model="model.awardCategoryId"
            :items="awardCategoryOptions"
            :label="$t('lod.awards.awardCategory')"
            clearable
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        v-if="model.awardCategoryId === 7"
        cols="12"
        lg="8"
      >
        <c-info
          uid="award.additionalInformation"
        >
          <v-text-field
            v-model="model.additionalInformation"
            :label="$t('lod.awards.additionalInformation')"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info
          uid="award.type"
        >
          <v-text-field
            v-if="disabled"
            :value="model.awardTypeName"
            :label="$t('lod.awards.awardType')"
            outlined
            persistent-placeholder
          />
          <v-select
            v-else
            v-model="model.awardTypeId"
            :items="awardTypeOptions"
            :label="$t('lod.awards.awardType')"
            clearable
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="3"
        md="6"
        lg="3"
      >
        <c-info
          uid="award.founder"
        >
          <v-text-field
            v-if="disabled"
            :value="model.founderName"
            :label="$t('lod.awards.founder')"
            outlined
            persistent-placeholder
          />
          <v-select
            v-else
            v-model="model.founderId"
            :items="awardFounderOptions"
            :label="$t('lod.awards.founder')"
            clearable
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="13"
        md="6"
        lg="3"
      >
        <c-info
          uid="award.reason"
        >
          <v-text-field
            v-if="disabled"
            :value="model.awardReasonName"
            :label="$t('lod.awards.awardReason')"
            outlined
            persistent-placeholder
          />
          <v-select
            v-else
            v-model="model.awardReasonId"
            :items="awardReasonOptions"
            :label="$t('lod.awards.awardReason')"
            clearable
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info
          uid="award.orderNumber"
        >
          <v-text-field
            v-model="model.orderNumber"
            :label="$t('lod.awards.orderNumber')"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info
          uid="award.date"
        >
          <date-picker
            id="date"
            ref="date"
            v-model="model.date"
            :label="$t('lod.awards.date')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            :show-debug-data="false"
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        lg="8"
      >
        <c-info
          uid="award.institution"
        >
          <v-text-field
            v-if="disabled"
            :value="model.institutionName"
            :label="$t('documents.institutionDropdownLabel')"
            outlined
            persistent-placeholder
          />
          <autocomplete
            v-else
            id="institution"
            ref="institution"
            v-model="model.institutionId"
            api="/api/lookups/GetInstitutionOptions"
            :label="$t('documents.institutionDropdownLabel')"
            :placeholder="$t('buttons.search')"
            hide-no-data
            hide-selected
            clearable
            disabled
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
      >
        <c-info
          uid="award.description"
        >
          <v-textarea
            v-model="model.description"
            :label="$t('common.detailedInformation')"
            prepend-icon="mdi-comment"
            outlined
            clearable
            rows="3"
            persistent-placeholder
          />
        </c-info>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <file-manager
          v-model="model.documents"
          :disabled="disabled"
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import { StudentAwardModel } from "@/models/studentAwardModel.js";
import FileManager from '@/components/common/FileManager.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { mapGetters } from 'vuex';

export default {
  name: 'AwardForm',
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
    }
  },
  data() {
    return {
      awardTypeOptions: [],
      awardCategoryOptions: [],
      awardFounderOptions: [],
      awardReasonOptions: [],
      institutionOptions: [],
      model: this.value ?? new StudentAwardModel()
    };
  },
  computed: {
  ...mapGetters(['studentFinalizedLods'])
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getStudentAwardTypes()
      .then((response) => {
        if(response.data) {
          this.awardTypeOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.studentAwardsLoad'));
        console.log(error.response);
      });

      this.$api.lookups.getAwardCategories()
      .then((response) => {
        if(response.data) {
          this.awardCategoryOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.studentAwardCategoryLoad'));
        console.log(error.response);
      });

      this.$api.lookups.getFounders()
      .then((response) => {
        if(response.data) {
          this.awardFounderOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.founderLoad'));
        console.log(error.response);
      });

      this.$api.lookups.getAwardReasons()
      .then((response) => {
        if(response.data) {
          this.awardReasonOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.awardReasonsLoad'));
        console.log(error.response);
      });
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
  }
};
</script>
