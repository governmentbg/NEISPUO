<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('commonPersonalDevelopment.createTitle') }}</h3>
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
  name: 'StudentCommonPersonalDevelopmentCreateView',
  components: {
    CommonPersonalDevelopmentSupportForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      saving: false,
      model: {
        personId: this.personId,
        schoolYear: null,
        items: [],
        documents: []
      }
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      );
    },
  },
  async mounted() {
    if(!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.model.schoolYear = (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data;
  },
  methods: {
    onSave() {
      const form = this.$refs['commonPersonalDevelopmentSupportForm' + this._uid];
      const isValid = form.validate();

        if(!isValid) {
          return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
        }


        this.saving = true;
        this.$api.studentCommonPDS.create(this.model)
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
