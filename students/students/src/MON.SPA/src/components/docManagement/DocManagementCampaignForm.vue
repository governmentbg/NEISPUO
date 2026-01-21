<template>
  <v-form
    :ref="'docManagementCampaignForm' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        cols="12"
        md="12"
        lg="4"
      >
        <c-info uid="docManagementCampaign.name">
          <v-text-field
            v-model="model.name"
            :label="$t('docManagement.campaign.headers.name')"
            clearable
            :rules="!disabled ? [$validator.required()] : []"
            :class="!disabled ? 'required' : ''"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="2"
      >
        <c-info uid="docManagementCampaign.schoolYear">
          <v-text-field
            v-if="disabled"
            v-model="model.schoolYearName"
            :label="$t('common.schoolYear')"
          />
          <school-year-selector
            v-else
            v-model="model.schoolYear"
            :min="minYear"
            :show-navigation-buttons="false"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info uid="docManagementCampaign.fromDate">
          <date-picker
            id="fromDate"
            ref="fromDate"
            v-model="model.fromDate"
            :label="$t('docManagement.campaign.headers.fromDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            :show-debug-data="false"
            :rules="!disabled ? [$validator.required()] : []"
            :min="$moment().format()"
            :class="!disabled ? 'required' : ''"
            :disabled="disabled"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="3"
      >
        <c-info uid="docManagementCampaign.toDate">
          <date-picker
            id="toDate"
            ref="toDate"
            v-model="model.toDate"
            :label="$t('docManagement.campaign.headers.toDate')"
            :show-buttons="false"
            :scrollable="false"
            no-title
            :show-debug-data="false"
            :rules="!disabled ? [$validator.required()] : []"
            :min="$moment().format()"
            :class="!disabled ? 'required' : ''"
            :disabled="disabled"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="12"
      >
        <v-textarea
          v-model="model.description"
          :label="$t('docManagement.campaign.headers.description')"
          prepend-icon="mdi-comment"
          outlined
          clearable
          :rows="3"
          hide-details
        />
      </v-col>
    </v-row>

     <v-row
      v-if="model?.labels?.length > 0"
      dense
    >
      <v-col>
        <v-chip
          v-for="(label, index) in model.labels"
          :key="index"
          :color="label.value"
          small
          class="mr-1"
          label
        >
          {{ label.key }}
        </v-chip>
      </v-col>
    </v-row>

    <v-row>
      <v-card-title>
        {{ $t('common.attachedFiles') }}
      </v-card-title>
      <v-card-text>
        <file-manager
          v-model="model.attachments"
          :disabled="disabled"
        />
      </v-card-text>
    </v-row>

    <v-row
      dense
      class="mt-3"
    >
      <v-col v-if="model?.id && (model.parentId ?? 0 ) <= 0">
        <doc-management-additional-campaigns-list :parent-id="model.id" />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { DocManagementCampaignModel } from '@/models/docManagement/docManagementCampaignModel.js';
import DocManagementAdditionalCampaignsList from '@/components/docManagement/DocManagementAdditionalCampaignsList.vue';
import FileManager from '@/components/common/FileManager.vue';

export default {
  name: 'DocManagementCampaignForm',
  components: {
    SchoolYearSelector,
    DocManagementAdditionalCampaignsList,
    FileManager
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      },
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
    minYear: {
      type: [Number, String],
      default() {
        return undefined;
      },
    },
  },
  data() {
    return {
      model: this.value ?? new DocManagementCampaignModel()
    };
  },
  watch: {
    value: {
      handler(val) {
        this.model = val ?? new DocManagementCampaignModel();
      },
      deep: true,
    },
  },
  methods: {
    validate() {
      const form = this.$refs['docManagementCampaignForm' + this._uid];
      return form && typeof form.validate === 'function' ? form.validate() : true;
    }
  },
};
</script>
