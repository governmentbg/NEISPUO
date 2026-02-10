<template>
  <v-form
    :ref="'docManagementAdditiionalCampaignForm' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col
        cols="12"
        md="8"
      >
        <c-info
          uid="docManagementCampaign.institution"
        >
          <v-text-field
            v-if="disabled"
            v-model="model.institutionName"
            :label="$t('docManagement.campaign.headers.institution')"
          />
          <autocomplete
            v-else
            v-model="model.institutionId"
            :defer-options-loading="false"
            api="/api/lookups/getInstitutionOptions"
            :label="$t('docManagement.campaign.headers.institution')"
            clearable
            :rules="!disabled ? [$validator.required()] : []"
            :class="!disabled ? 'required' : ''"
            :filter="{
              regionId: userRegionId
            }"
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="4"
      >
        <c-info
          uid="docManagementCampaign.name"
        >
          <v-text-field
            v-model="model.name"
            :label="$t('docManagement.campaign.headers.name')"
            clearable
          />
        </c-info>
      </v-col>
      <v-col
        cols="12"
        md="6"
        lg="4"
      >
        <c-info
          uid="docManagementCampaign.fromDate"
        >
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
        lg="4"
      >
        <c-info
          uid="docManagementCampaign.toDate"
        >
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
      v-if="disabled && model.labels && model.labels.length > 0"
      dense
    >
      <v-col
        cols="12"
      >
        <v-chip-group
          v-model="model.labels"
          column
        >
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
        </v-chip-group>
      </v-col>
    </v-row>
  </v-form>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { DocManagementCampaignModel } from '@/models/docManagement/docManagementCampaignModel.js';
import { mapGetters } from 'vuex';

export default {
  name: 'DocManagementAdditiionalCampaignForm',
  components: {
    Autocomplete
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
      model: this.value ?? new DocManagementCampaignModel()
    };
  },
  computed: {
    ...mapGetters(['userRegionId'])
  },
  methods: {
    validate() {
      return this.$refs['docManagementAdditiionalCampaignForm' + this._uid].validate();
    }
  },
};
</script>
