<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('documents.admissionDocumentCreateTitle') }}</h3>
      </template>

      <template #default>
        <admission-document-form
          :ref="'admissionDocumentForm' + _uid"
          :person-id="personId"
          :disabled="disabled"
        />
      </template>
    </form-layout>
    <confirm-dlg ref="confirm" />
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
import { UserRole, Permissions, Positions } from '@/enums/enums';
import { mapGetters } from "vuex";

export default {
  name: 'StudentAdmissionDocumentCreate',
  components: {
    AdmissionDocumentForm
  },
  props: {
    personId: {
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
    ...mapGetters(['hasStudentPermission']),
    disabled() {
      return this.saving;
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAdmissionDocumentCreate)) {
      return this.$router.push('/errors/AccessDenied');
    }
    this.document = new StudentAdmissionDocumentModel();
    this.setDefaultInstitution();
  },
  methods: {
    setDefaultInstitution() {
      const selectedRole = this.$store.getters.user.profile.selected_role;
        if (selectedRole && selectedRole.SysRoleID === UserRole.School && selectedRole.InstitutionID) {
          this.$api.institution
            .getDropdownModelById(selectedRole.InstitutionID)
            .then(response => {
              if (response.data) {
                this.document.institutionId = response.data.value;
              }
            })
            .catch(error => {
              console.log(error);
            });
          }
    },
    async onSave() {
      const form = this.$refs['admissionDocumentForm' + this._uid];
      if(!form) {
        console.log('Empty form');
        return this.$notifier.error('', this.$t('common.saveError'), 5000);
      }

      if(!form.model) {
        console.log('Empty form model');
        return this.$notifier.error('', this.$t('common.saveError'), 5000);
      }

      const isValid = form.validate();
      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      if (form.model.position === Positions.StudentSpecialEducationSupport){
        if(!(await this.$refs.confirm.open(this.$t('common.confirm'), this.$t('student.admissionToSpecialEducationInfo'), { messageClass: 'confirmDeleteAzureAccount' }))) {
          return;
        }
      }

      const payload = {
        status: form.model.status,
        personId: this.personId,
        noteNumber: form.model.noteNumber,
        noteDate: form.model.noteDate,
        admissionDate: form.model.admissionDate ? this.$helper.parseDateToIso(form.model.admissionDate) : form.model.admissionDate,
        dischargeDate: form.model.dischargeDate ? this.$helper.parseDateToIso(form.model.dischargeDate) : form.model.dischargeDate,
        documents: form.model.documents,
        institutionId: form.model.institutionId,
        relocationDocumentId: form.model.relocationDocument?.value,
        admissionReasonTypeId: form.model.admissionReasonTypeId,
        position: form.model.position,
        hasHealthStatusDocument: form.model.hasHealthStatusDocument,
        hasImmunizationStatusDocument: form.model.hasImmunizationStatusDocument
      };

      this.saving = true;
      this.$api.admissionDocument
        .create(payload)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t(error.response.data.message));
          console.log(error.response.data.message);
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
