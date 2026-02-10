<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('documents.otherDocumentCreateTitle') }}</h3>
      </template>

      <template #default>
        <other-document-form
          v-if="document !== null"
          :ref="'otherDocumentForm' + _uid"
          :document="document"
          :disabled="saving"
          :disabled-institution="isSchoolDirector"
          issue-date-required
          document-type-required
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
import OtherDocumentForm from '@/components/tabs/otherDocuments/OtherDocumentForm.vue';
import { StudentOtherDocumentModel } from '@/models/studentOtherDocumentModel';
import { UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";

export default {
  name: 'StudentOtherDocumentCreate',
  components: {
    OtherDocumentForm
  },
  props: {
    pid: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      saving: false,
      document: null,
      roleSchool: UserRole.School,
      currentSchoolYear: null
    };
  },
  computed: {
    ...mapGetters(['userSelectedRole', 'isInRole', 'hasStudentPermission', 'hasPermission']),
    isSchoolDirector() {
      return this.isInRole(this.roleSchool);
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherDocumentManage)
      && !this.hasPermission(Permissions.PermissionNameForStudentOtherDocumentManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    await this.init();
  },
  methods: {
    async init() {
      this.currentSchoolYear = (await this.$api.institution.getCurrentYear(this.userSelectedRole.InstitutionID)).data;

      this.document = new StudentOtherDocumentModel({
        personId: this.pid,
        issueDate: new Date(),
        schoolYear: this.currentSchoolYear,
        institutionId: this.userSelectedRole.InstitutionID
      }, this.$moment);
    },
    onSave() {
      const form = this.$refs['otherDocumentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.document.issueDate = this.$helper.parseDateToIso(this.document.issueDate, '');
      this.document.deliveryDate = this.$helper.parseDateToIso(this.document.deliveryDate, '');

      this.saving = true;
      this.$api.otherDocument
        .create(this.document)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.otherDocumentsSaveError'), 5000);
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
