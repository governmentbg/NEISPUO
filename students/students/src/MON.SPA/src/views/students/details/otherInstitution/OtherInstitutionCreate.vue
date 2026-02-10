<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('studentOtherInstitutions.addTitle') }}</h3>
      </template>
      <template #default>
        <OtherInstitutionForm
          v-if="form !== null"
          :ref="'form' + _uid"
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

import OtherInstitutionForm from "@/components/tabs/studentOtherInstitutions/OtherInstitutionForm";
import { StudentOtherInstitutionModel } from '@/models/studentOtherInstitutionModel.js';
import { Permissions , UserRole} from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'OtherInstitutionCreate',
  components: {
    OtherInstitutionForm
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
      form: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInRole', 'userSelectedRole']),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    const form = new StudentOtherInstitutionModel();
    if(this.isInRole(UserRole.School)) {
      form.institutionId = this.userSelectedRole.InstitutionID;
    }

    this.form = form;
  },
  methods: {
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.personId = this.personId;
      
      this.saving = true;
      this.$api.otherInstitution.create(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("studentOtherInstitutions.аddErrorMessage"));
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
