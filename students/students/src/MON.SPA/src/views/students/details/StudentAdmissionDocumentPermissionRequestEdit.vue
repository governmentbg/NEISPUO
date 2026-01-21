<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('admissionDocument.permissionRequestEditTitle') }}</h3>
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
  name: 'StudentAdmissionDocumentPermissionRequestEdit',
  components: {
    StudentAdmissionDocumentPermissionRequestForm
  },
  props: {
    id: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: false,
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

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentAdmissionDocumentPermissionRequest
        .getById(this.id)
        .then((response) => {
          if(response.data) {
            this.document = new StudentAdmissionDocumentPermissionRequestModel(response.data);
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('documents.documentLoadErrorMessage'), 5000);
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    async onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;
      this.$api.studentAdmissionDocumentPermissionRequest
        .update(this.document)
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