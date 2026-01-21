<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        indeterminate
        color="primary"
      />
    </div>
    <div>
      <v-card
        v-if="studentClass"
        class="my-2"
      >
        <v-card-title>
          {{ studentClass.classGroup.className }}
        </v-card-title>
        <v-card-text>
          <student-class-details
            :value="studentClass"
          />
        </v-card-text>
      </v-card>

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
          <h3>{{ $t('dischargeDocument.editTitle') }}</h3>
        </template>

        <template #default>
          <discharge-document-form
            :ref="'dischargeDocumentForm' + _uid"
            :document="document"
            :disabled="disabled"
            :edit-confirmed-document="editConfirmedDocument"
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
        При запис на документа за отписване със статус "Потвърден" се генерира ЛОД (docx файл) за учебните години, през които детето/ученикът се е обучавал в институцията и файлът се подписва с електронен подпис! ЛОД (електронната партида) за текущата учебна година се заключва. При последващо записване в същата или друга институция през текущата учебна година, ЛОД ще се отключи автоматично. Не е необходимо инстутцията, подписала ЛОД, да извършва допълнителни действия в НЕИСПУО.
      </v-alert>
    </div>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import DischargeDocumentForm from '@/components/tabs/studentMovement/DischargeDocumentForm.vue';
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import { NewStudentDischargeDocumentModel } from '@/models/studentMovement/newStudentDischargeDocumentModel.js';
import { Permissions, DocumentStatusType } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { DocumentModel } from '@/models/documentModel.js';

export default {
  name: 'StudentDischargeDocumentEdit',
  components: {
    DischargeDocumentForm,
    StudentClassDetails
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
      loading: true,
      saving: false,
      document: null,
      studentClass: null,
      editConfirmedDocument: false // При редакция да потвърден документ не е позволено да се редактират полетата Институция, Позиция и Статус

    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
    disabled() {
      return this.saving;
    },
    signLODPermission(){
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentSignLOD);
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
      const doc = (await this.$api.dischargeDocument.getById(this.docId)).data;
      if(doc) {
        this.document = new NewStudentDischargeDocumentModel(doc, this.$moment);
        this.editConfirmedDocument = doc && doc.status == DocumentStatusType.Confirmed;

        if(this.document.studentClassId) {
          this.studentClass = (await this.$api.studentClass.getById(this.document.studentClassId)).data;
        }
      }
    },
    checkPermission() {
      if (!this.document || !this.userInstitutionId) {
        this.$router.push('/errors/AccessDenied');
        return false;
      }

      if (!this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentUpdate)
        && !this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentRead)) {
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
      const form = this.$refs['dischargeDocumentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.document.noteDate = this.document.noteDate ? this.$helper.parseDateToIso(this.document.noteDate, '') : this.document.noteDate;
      this.document.dischargeDate = this.document.dischargeDate ? this.$helper.parseDateToIso(this.document.dischargeDate, ''): this.document.dischargeDate;

      if (form.model.status == DocumentStatusType.Confirmed && this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentSignLOD)) {
        // Прикачваме личен картон само при статус Потвърден
        try {
          this.saving = true;
          let response = await this.$api.studentLod.generatePersonalFileForStay({
            personId: this.personId,
            dischargeDocument: this.document
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
      this.$api.dischargeDocument
        .update(this.document)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.$router.go(-1);
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.classDischargeSave'), 5000);
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
