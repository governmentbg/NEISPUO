<template>
  <div>
    <form-layout
      :disabled="disabled"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('documents.relocationDocumentCreateTitle') }}</h3>
      </template>

      <template #default>
        <relocation-document-create
          :ref="'relocationDocumentEdit' + _uid"
          :person-id="personId"
          :hosting-institution="hostingInstitution"
          :disabled="disabled"
        />
      </template>
    </form-layout>

    <v-alert
      v-if="signLODPermission"
      class="mt-2"
      border="left"
      colored-border
      type="info"
      elevation="2"
    >
      При запис на документа за преместване със статус "Потвърден" се генерира ЛОД (docx файл) за учебните години, през които детето/ученикът се е обучавал в институцията и файлът се подписва с електронен подпис! ЛОД (електронната партида) за текущата учебна година НЕ се заключва. Това трябва да бъде извършено в края на учебната година от институцията, в която детето/ученикът завършва.
    </v-alert>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import RelocationDocumentCreate from '@/components/tabs/studentMovement/RelocationDocumentForm';
import { Permissions, DocumentStatusType } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { DocumentModel } from '@/models/documentModel.js';

export default {
  name: 'StudentRelocationDocumentCreate',
  components: {
    RelocationDocumentCreate
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    hostingInstitution: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      saving: false
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userSelectedRole', 'userDetails']),
    disabled() {
      return this.saving;
    },
    signLODPermission(){
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentSignLOD);
    }
  },
  mounted() {
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentCreate)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    async onSave() {
      const form = this.$refs['relocationDocumentEdit' + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      const payload = {
        status: form.model.status,
        personId: this.personId,
        noteNumber: form.model.noteNumber,
        noteDate: form.model.noteDate ? this.$helper.parseDateToIso(form.model.noteDate, '') : form.model.noteDate,
        dischargeDate: form.model.dischargeDate ? this.$helper.parseDateToIso(form.model.dischargeDate, '') : form.model.dischargeDate,
        documents: form.model.documents,
        institutionId: form.model.institutionId,
        relocationReasonTypeId: form.model.relocationReasonTypeId,
        ruoOrderNumber: form.model.ruoOrderNumber,
        ruoOrderDate: this.$helper.parseDateToIso(form.model.ruoOrderDate, ''),
        currentStudentClassId: form.model.currentStudentClassId,
        sendingInstitutionId: form.model.sendingInstitutionId,
        sendingInstitution: form.model.sendingInstitution,
      };

      if (form.model.status == DocumentStatusType.Confirmed && this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentSignLOD)) {
        // Прикачваме личен картон само при статус Потвърден
        try {
          this.saving = true;
          let response = await this.$api.studentLod.generatePersonalFileForStay({
            personId: this.personId,
            relocationDocument: payload
          });

          const docxBase64 = await this.$helper.blobToBase64(response.data);

          const hasLocalServer = await this.$helper.checkForLocalServer(this);
          if (!hasLocalServer) {
            this.$notifier.modalError(
              this.$t('menu.localServer'),
              this.$t('errors.scan.localServerError')
            );
            return;
          }

          const signOptions = {
            name: this.userSelectedRole?.Username,
            title: `${this.userDetails?.institution ?? ''} - ${String(this.userSelectedRole?.InstitutionID ?? '')}`,
            email: this.userSelectedRole?.Username,
          };

          let signResponse = await this.$api.certificate.signDocxThumbprint(docxBase64, null, signOptions);
          if (signResponse && signResponse.isError == false) {
            let signature = signResponse.contents; // base64
            let fileModel = new DocumentModel({
              noteFileName: 'Личен картон.docx',
              uid: this.$uuid.v4()
            });
            fileModel.noteContents = this.$helper.base64ToByteArray(signature);
            if (form.model?.documents != null) {
              form.model.documents.push(fileModel);
            }
            else {
              form.model.documents = [fileModel];
            }
          }
          else{
            this.$notifier.modalError(
              this.$t('errors.localServer'),
              this.$t('errors.signError')
            );
            return;
          }
        } finally {
          this.saving = false;
        }
      }

      this.saving = true;
      this.$api.relocationDocument
        .create(payload)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
          this.$notifier.success('', this.$t('common.saveSuccess', 5000));
          this.$router.go(-1);
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.saveError'), error.response?.data?.message ?? this.$t('documents.editRelocationDocumentErrorMessage', 5000));
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
