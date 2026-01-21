<template>
  <div
    v-if="loading"
  >
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
  </div>
  <div
    v-else
  >
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('studentOtherInstitutions.editTitle') }}</h3>
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
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'OtherInstitutionEdit',
  components: {
    OtherInstitutionForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    institutionId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      form: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherInstitutionManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
        this.loading = true;

        this.$api.otherInstitution.getById(this.institutionId)
        .then(response => {
          if (response.data) {
            this.form = new StudentOtherInstitutionModel(response.data, this.$moment);
          }
        })
        .catch(error => {
            this.$notifier.error('', this.$t('studentOtherInstitutions.loadError'));
            console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.validFrom = this.$helper.parseDateToIso(this.form.validFrom, '');
      this.form.validTo = this.$helper.parseDateToIso(this.form.validTo, '');

      this.saving = true;
      this.$api.otherInstitution.update(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("studentOtherInstitutions.updateOtherInstitutionsErrorMessage"));
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