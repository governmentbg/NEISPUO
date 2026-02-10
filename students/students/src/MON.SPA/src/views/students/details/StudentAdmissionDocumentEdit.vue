<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        indeterminate
        color="primary"
      />
    </div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <v-icon
          v-if="editConfirmedDocument"
          color="warning"
          x-large
          class="mx-3"
        >
          mdi-alert
        </v-icon>
        <h3>{{ $t('admissionDocument.editTitle') }}</h3>
      </template>

      <template #default>
        <admission-document-form
          :ref="'admissionDocumentForm' + _uid"
          :person-id="personId"
          :document="document"
          :disabled="disabled"
          :edit-confirmed-document="editConfirmedDocument"
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
import AdmissionDocumentForm from '@/components/tabs/studentMovement/AdmissionDocumentForm';
import { StudentAdmissionDocumentModel } from "@/models/studentMovement/studentAdmissionDocumentModel.js";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentAdmissionDocumentEdit',
  components: {
    AdmissionDocumentForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    docId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      document: null,
      editConfirmedDocument: false // При редакция да потвърден документ не е позволено да се редактират полетата Институция, Позиция и Статус
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
    disabled() {
      return this.saving;
    }
  },
  async mounted() {
    try {
      this.loading = true;
      await this.load();
      if (this.checkPermission()) {
        this.loading = false;
      }
    } catch (error) {
      console.log(error);
    }
  },
  methods: {
    async load() {
      const doc = (await this.$api.admissionDocument.getById(this.docId)).data;
      if (doc) {
        this.document = new StudentAdmissionDocumentModel(doc, this.$moment);
        this.editConfirmedDocument = doc.status === 2; // 1 - чернова, 2 - потвърден
      }
    },
    checkPermission() {
      if (!this.document || !this.userInstitutionId) {
        this.$router.push('/errors/AccessDenied');
        return false;
      }

      if (!this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentUpdate)
        && !this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentRead)) {
        this.$router.push('/errors/AccessDenied');
        return false;
      }

      if (this.document.institutionId !== this.userInstitutionId) {
        this.$router.push('/errors/AccessDenied');
        return false;
      }

      return true;
    },
    async onSave() {
      const form = this.$refs['admissionDocumentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      const payload = {
        id: this.document.id,
        status: this.document.status,
        personId: this.personId,
        noteNumber: this.document.noteNumber,
        noteDate: form.$refs.noteDate && !form.$refs.noteDate.isValidIsoFormattedDate() ? form.$refs.noteDate.isoDate : this.document.noteDate,
        admissionDate: form.$refs.admissionDate && !form.$refs.admissionDate.isValidIsoFormattedDate() ? form.$refs.admissionDate.isoDate : this.document.admissionDate,
        dischargeDate: form.$refs.dischargeDate && !form.$refs.dischargeDate.isValidIsoFormattedDate() ? form.$refs.dischargeDate.isoDate : this.document.dischargeDate,
        documents: this.document.documents,
        institutionId: this.document.institutionId,
        admissionReasonTypeId: this.document.admissionReasonTypeId,
        relocationDocumentId: this.document.relocationDocument.value,
        position: this.document.position,
        createdBySysUserId: this.document.createdBySysUserId,
        hasHealthStatusDocument: this.document.hasHealthStatusDocument,
        hasImmunizationStatusDocument: this.document.hasImmunizationStatusDocument
      };

      this.saving = true;

      this.$api.admissionDocument
        .update(payload)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch(error => {
          this.$notifier.error(this.$t('common.save'), this.$t('common.error'), 5000);
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
