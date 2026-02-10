<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('lodAssessmentTemplate.editTitle') }}</h3>
        </template>

        <template #default>
          <lod-assessment-template-form
            v-if="model !== null"
            :ref="'lodAssessmentTemplateCreateForm' + _uid"
            v-model="model"
            :disabled="saving"
          />
        </template>
      </form-layout>
      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
    </div>
  </div>
</template>

<script>
import LodAssessmentTemplateForm from '@/components/lod/assessment/LodAssessmentTemplateForm.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'AssessmentTemplateEditView',
  components: {
    LodAssessmentTemplateForm
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: true,
      saving: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.lodAssessmentTemplate.getById(this.id)
        .then((response) => {
          if (response.data) {
            this.model = response.data;
          }
        })
        .catch((error) => {
          this.$notifier.success('', this.$t('common.loadError'), 5000);
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onSave() {
      const form = this.$refs['lodAssessmentTemplateCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }


      this.saving = true;
      this.$api.lodAssessmentTemplate.update(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error?.response?.data?.message ?? this.$t('common.error'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
