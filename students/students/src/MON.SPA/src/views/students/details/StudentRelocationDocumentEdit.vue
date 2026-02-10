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
        <h3>{{ $t('documents.relocationDocumentEditTitle') }}</h3>
      </template>

      <template #default>
        <relocation-document-edit
          :ref="'relocationDocumentEdit' + _uid"
          :person-id="personId"
          :document="document"
          :disabled="disabled"
        />
        <admission-documents-for-relocation
          v-if="document"
          :doc-id="document.id"
          class="mt-4"
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
import RelocationDocumentEdit from '@/components/tabs/studentMovement/RelocationDocumentForm';
import AdmissionDocumentsForRelocation from '@/components/tabs/studentMovement/AdmissionDocumentsForRelocation';
import { StudentRelocationDocumentModel } from "@/models/studentMovement/studentRelocationDocumentModel.js";
import DocumentStatuses from "@/common/documentStatuses.js";
import { Permissions, DocumentStatusType } from "@/enums/enums";
import { mapGetters } from "vuex";
import { DocumentModel } from '@/models/documentModel.js';

export default {
  name: 'StudentRelocationDocumentEdit',
  components: {
    RelocationDocumentEdit,
    AdmissionDocumentsForRelocation
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    docId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: false,
      saving: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    disabled() {
      return this.saving;
    },
    documentTitle() {
      return this.$t('documents.relocationDocumentViewTitle') ;
    },
    signLODPermission(){
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentSignLOD);
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentUpdate)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.relocationDocument
        .getById(this.docId)
        .then((response) => {
          if(response.data) {
            console.log(response.data);
            this.document = new StudentRelocationDocumentModel(response.data, this.$moment);
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    admissionDocumentStatus(status) {
      return DocumentStatuses.filter((el) => el.value === status)[0].text;
    },
    async onSave() {
      const form = this.$refs['relocationDocumentEdit' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      const payload = {
        id: this.document.id,
        status: this.document.status,
        personId: this.personId,
        noteNumber: this.document.noteNumber,
        noteDate: this.document.noteDate ? this.$helper.parseDateToIso(this.document.noteDate, '') : this.document.noteDate,
        dischargeDate: this.document.dischargeDate ? this.$helper.parseDateToIso(this.document.dischargeDate, '') : this.document.dischargeDate,
        documents: this.document.documents,
        institutionId: this.document.institutionId,
        relocationReasonTypeId: this.document.relocationReasonTypeId,
        ruoOrderNumber: this.document.ruoOrderNumber,
        ruoOrderDate:  this.$helper.parseDateToIso(this.document.ruoOrderDate, ''),
        currentStudentClassId: this.document.currentStudentClassId
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
        .update(payload)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
          this.$notifier.success('', this.$t('common.saveSuccess', 5000));
          this.$router.go(-1);
        })
        .catch(error => {
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
