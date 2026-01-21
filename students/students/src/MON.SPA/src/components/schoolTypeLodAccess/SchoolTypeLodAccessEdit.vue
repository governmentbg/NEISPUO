<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('schoolTypeLodAccess.updateTitle') }}</h3>
      </template>

      <template #default>
        <v-form
          :ref="'form' + _uid"
        >
          <v-row>
            <v-col>
              <c-info
                uid="schoolTypeLodAccessEdit.detailedSchoolTypeName"
              >
                <v-text-field
                  id="detailedSchoolTypeName"
                  ref="detailedSchoolTypeName"
                  v-model="model.detailedSchoolTypeName"
                  :label="$t('schoolTypeLodAccess.detailedSchoolTypeNameLabel')"
                  :disabled="true"
                />
              </c-info>
            </v-col>
            <v-col>
              <c-info
                uid="schoolTypeLodAccessEdit.isLodAccessAllowed"
              >
                <v-checkbox
                  id="isLodAccessAllowed"
                  ref="isLodAccessAllowed"
                  v-model="model.isLodAccessAllowed"
                  color="primary"
                  :label="$t('schoolTypeLodAccess.lodAccessAllowedLabel')"
                />
              </c-info>
            </v-col>
          </v-row>
        </v-form>
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import { SchoolTypeLodAccessModel } from '@/models/admin/schoolTypeLodAccessModel.js';

export default {
  name: 'SchoolTypeLodAccessEdit',
  props: {
    detailedSchoolTypeId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      model: new SchoolTypeLodAccessModel()
    };
  },
  computed: {
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.schoolTypeLodAccess.getById(this.detailedSchoolTypeId)
        .then((response) => {
          if(response.data) {
            this.model = new SchoolTypeLodAccessModel(response.data);
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$tc('errors.schoolTypeLodAccessLoad', 2));
          console.log(error.response.data.message);
        })
        .then(() => { this.loading = false; });
    },
    async onSave() {
      this.saving = true;

      this.$api.schoolTypeLodAccess
        .update(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.$router.go(-1);
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.schoolTypeLodAccessSave'), 5000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
