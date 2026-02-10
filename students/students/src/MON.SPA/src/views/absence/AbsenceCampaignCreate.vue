<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('absenceCampaign.addTitle') }}</h3>
      </template>
      <template #default>
        <absence-campaign-form
          v-if="form !== null"
          :ref="'form' + _uid"
          :value="form"
          :disabled="disabled"
          :min-year="currentSchoolYear"
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
import AbsenceCampaignForm from "@/components/absence/AbsenceCampaignForm";
import { AbsenceCampaignModel } from "@/models/absence/absenceCampaignModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'AbsenceCampaignCreate',
  components: {
    AbsenceCampaignForm,
  },
  data()
  {
    return {
      saving: false,
      form: null,
      currentSchoolYear: undefined
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.currentSchoolYear = (await this.$api.institution.getCurrentYear()).data;
    const form = new AbsenceCampaignModel();
    this.form = form;
  },
  methods: {
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.form.fromDate = this.$helper.parseDateToIso(this.form.fromDate, '');
      this.form.toDate = this.$helper.parseDateToIso(this.form.toDate, '');

      this.saving = true;

      this.$api.absenceCampaign.create(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.$router.go(-1);
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("common.saveError"), 5000);
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
