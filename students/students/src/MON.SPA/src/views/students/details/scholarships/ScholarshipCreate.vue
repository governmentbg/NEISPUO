<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('studentScholarships.addTitle') }}</h3>
        <v-spacer />
        <v-chip
          v-if="form"
          color="light"
        >
          {{ form.institutionName }}
        </v-chip>
      </template>

      <template #default>
        <scholarship-form
          :ref="'scholarshipForm' + _uid"
          :document="form"
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
import ScholarshipForm from "@/components/tabs/studentScholarships/ScholarshipForm";
import { StudentScholarshipModel } from '@/models/studentScholarship/studentScholarshipModel.js';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'ScholarshipCreate',
  components: {
    ScholarshipForm
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
      form: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userRoles', 'userInstitutionId', 'userDetails']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentScholarshipManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    const f = new StudentScholarshipModel({
      personId: this.personId,
      institutionId: this.userInstitutionId,
      institutionName: this.userDetails.institution,
      schoolYear: (await this.$api.institution.getCurrentYear(this.userInstitutionId)).data
    });

    this.form = f;
  },
  methods: {
    onSave() {
      const form = this.$refs['scholarshipForm' + this._uid];
      const isValid = form.validate();
      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.form.orderDate = this.$helper.parseDateToIso(this.form.orderDate, '');
      this.form.commissionDate = this.$helper.parseDateToIso(this.form.commissionDate, '');
      this.form.startingDateOfReceiving = this.$helper.parseDateToIso(this.form.startingDateOfReceiving, '');
      this.form.endDateOfReceiving = this.$helper.parseDateToIso(this.form.endDateOfReceiving, '');

      this.saving = true;

      this.$api.scholarship.create(this.form)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'));
        this.onCancel();
      })
      .catch((error) => {
        this.$notifier.error('',this.$t("studentScholarships.scholarshipAddError"));
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
