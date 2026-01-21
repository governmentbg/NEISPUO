<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('admissionDocument.permissionRequestCreateTitle') }}</h3>
      </template>

      <template #default>
        <student-admission-document-permission-request-form
          :ref="'form' + _uid"
          :document="document"
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
import StudentAdmissionDocumentPermissionRequestForm from '@/components/tabs/studentMovement/StudentAdmissionDocumentPermissionRequestForm';
import { StudentAdmissionDocumentPermissionRequestModel } from "@/models/studentMovement/studentAdmissionDocumentPermissionRequestModel.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentAdmissionDocumentPermissionRequestCreate',
  components: {
    StudentAdmissionDocumentPermissionRequestForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    requestingInstitutionId: {
      type: Number,
      required: true
    },
    authorizingInstitutionId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      saving: false,
      document: null
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForAdmissionPermissionRequestManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.document = new StudentAdmissionDocumentPermissionRequestModel({ personId: this.personId, requestingInstitutionId: this.requestingInstitutionId, authorizingInstitutionId: this.authorizingInstitutionId });
  },
  methods: {
    async onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;
      this.$api.studentAdmissionDocumentPermissionRequest
        .create(this.document)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'));
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
