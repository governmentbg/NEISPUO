<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout
      v-else
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('commonPersonalDevelopment.editTitle') }}</h3>
      </template>
      <template #default>
        <common-personal-development-support-form
          v-if="model != null"
          :ref="'commonPersonalDevelopmentSupportForm' + _uid"
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
</template>

<script>
import CommonPersonalDevelopmentSupportForm from '@/components/students/CommonPersonalDevelopmentSupportForm.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'StudentCommonPersonalDevelopmentEditView',
  components: {
    CommonPersonalDevelopmentSupportForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      );
    },
  },
  mounted() {
    if(!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentCommonPDS.getById(this.id)
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
    async onSave() {
      const form = this.$refs['commonPersonalDevelopmentSupportForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;
      this.$api.studentCommonPDS.update(this.model)
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
