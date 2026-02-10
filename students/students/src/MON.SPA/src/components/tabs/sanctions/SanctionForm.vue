<template>
  <v-form
    :ref="'form' + _uid"
    :disabled="disabled"
  >
    <v-row dens>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="sanction.orderNumber"
        >
          <c-text-field
            v-model="model.orderNumber"
            :label="$t('lod.sanctions.orderNumber')"
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
        lg="3"
      >
        <c-info
          uid="sanction.orderDate"
        >
          <date-picker
            id="orderDate"
            ref="orderDate"
            v-model="model.orderDate"
            :label="$t('lod.sanctions.orderDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
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
        lg="3"
      >
        <c-info
          uid="sanction.ruoOrderNumber"
        >
          <c-text-field
            v-model="model.ruoOrderNumber"
            :label="$t('lod.sanctions.ruoOrderNumber')"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="sanction.ruoOrderDate"
        >
          <date-picker
            id="ruoOrderDate"
            ref="ruoOrderDate"
            v-model="model.ruoOrderDate"
            :label="$t('lod.sanctions.ruoOrderDate')"
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
        md="6"
      >
        <c-info
          uid="sanction.type"
        >
          <c-select
            v-model="model.sanctionTypeId"
            :items="sanctionTypeOptions"
            :label="$t('lod.sanctions.sanctionType')"
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
      >
        <c-info
          uid="sanction.institution"
        >
          <slot name="institution">
            <autocomplete
              id="institution"
              ref="institution"
              v-model="model.institutionId"
              api="/api/lookups/GetInstitutionOptions"
              :label="$t('common.institution')"
              :placeholder="$t('buttons.search')"
              hide-no-data
              hide-selected
              clearable
              disabled
              outlined
              persistent-placeholder
            />
          </slot>
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info
          uid="sanction.schoolYear"
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
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info
          uid="sanction.startDate"
        >
          <date-picker
            id="startDate"
            ref="startDate"
            v-model="model.startDate"
            :label="$t('lod.sanctions.startDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
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
          uid="sanction.endDate"
        >
          <date-picker
            id="endDate"
            ref="endDate"
            v-model="model.endDate"
            :label="$t('lod.sanctions.endDate')"
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
        md="6"
        lg="3"
      >
        <c-info
          uid="sanction.cancelOrderNumber"
        >
          <c-text-field
            v-model="model.cancelOrderNumber"
            :label="$t('lod.sanctions.cancelOrderNumber')"
            clearable
            outlined
            persistent-placeholder
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info
          uid="sanction.cancelOrderDate"
        >
          <date-picker
            id="cancelOrderDate"
            ref="cancelOrderDate"
            v-model="model.cancelOrderDate"
            :label="$t('lod.sanctions.cancelOrderDate')"
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
        lg="6"
      >
        <c-info
          uid="sanction.description"
        >
          <c-textarea
            v-model="model.description"
            :label="$t('common.detailedInformation')"
            outlined
            persistent-placeholder
            clearable
            rows="3"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        lg="6"
      >
        <c-info
          uid="sanction.cancelReason"
        >
          <c-textarea
            v-model="model.cancelReason"
            :label="$t('lod.sanctions.cancelReason')"
            outlined
            persistent-placeholder
            clearable
            rows="3"
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
import { StudentSanctionModel } from "@/models/studentSanctionModel.js";
import FileManager from '@/components/common/FileManager.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';

export default {
  name: 'SanctionForm',
  components: {
    FileManager,
    Autocomplete,
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
  },
  data() {
    return {
      sanctionTypeOptions: [],
      model: this.value ?? new StudentSanctionModel()
    };
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getStudentSanctionTypes()
      .then((response) => {
        if(response.data) {
          this.sanctionTypeOptions = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.studentSanctionsLoad'));
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
