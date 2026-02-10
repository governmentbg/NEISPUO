<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('lod.selfGovernment.addTitle') }}</h3>
      </template>
      <template #default>
        <self-government-form
          v-if="form !== null"
          :ref="'selfGovernmentForm' + _uid"
          :value="form"
          :disabled="disabled"
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

import SelfGovernmentForm from '@/components/tabs/selfGovernment/SelfGovernmentForm.vue';
import { StudentSelfGovernmentModel } from '@/models/studentSelfGovernmentModel.js';
import { Permissions} from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'SelfGovernmentCreate',
  components: {
    SelfGovernmentForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
  },
  data()
  {
    return {
      saving: false,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInRole', 'userInstitutionId', 'userDetails']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSelfGovernmentManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    const form = new StudentSelfGovernmentModel();
    const currentSchoolYear = (await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data;
    if (currentSchoolYear) {
      form.schoolYear = currentSchoolYear;
    }

    const studentInfo = (await this.$api.student.getSummaryById(this.personId))?.data;
    if(studentInfo) {
      form.email = studentInfo.email;
      form.mobilePhone = studentInfo.mobilePhone;
    }

    this.form = form;
  },
  methods: {
    onSave() {
      const form = this.$refs['selfGovernmentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.personId = this.personId;

      this.saving = true;
      this.$api.selfGovernment.create(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("lod.selfGovernment.аddError"));
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
