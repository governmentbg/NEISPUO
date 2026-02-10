<template>
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
        <h3>{{ $t('documents.dischargeDocumentCreateTitle') }}</h3>
      </template>

      <template #default>
        <discharge-document-form
          :ref="'dischargeDocumentForm' + _uid"
          :document="document"
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
      При запис на документа за отписване със статус "Потвърден" се генерира ЛОД (docx файл) за учебните години, през които детето/ученикът се е обучавал в институцията и файлът се подписва с електронен подпис! ЛОД (електронната партида) за текущата учебна година се заключва. При последващо записване в същата или друга институция през текущата учебна година, ЛОД ще се отключи автоматично. Не е необходимо инстутцията, подписала ЛОД, да извършва допълнителни действия в НЕИСПУО.
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
import DischargeDocumentForm from '@/components/tabs/studentMovement/DischargeDocumentForm.vue';
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import { NewStudentDischargeDocumentModel } from '@/models/studentMovement/newStudentDischargeDocumentModel.js';
import { Permissions, DocumentStatusType } from "@/enums/enums";
import { mapGetters } from 'vuex';
import { DocumentModel } from '@/models/documentModel.js';

export default {
  name: 'StudentDischargeDocumentCreate',
  components: {
    DischargeDocumentForm,
    StudentClassDetails
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    studentClassId: {
      type: Number,
      default() {
        return undefined;
      }
    },
  },
  data() {
    return {
      saving: false,
      document: null,
      studentClass: null
    };
  },
  computed: {
    ...mapGetters(['currentStudentSummary', 'userSelectedRole', 'hasStudentPermission', 'userInstitutionId']),
    disabled() {
      return this.saving;
    },
    signLODPermission(){
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentSignLOD);
    }
  },
  watch: {
    currentStudentSummary: function (newVal) {
      // Зареждане на профилни(детайли) данните за избрания ученик.
      if(newVal) {
        this.loadStudentClass();
      }
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentCreate)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.init();
    this.loadStudentClass();
  },
  methods: {
    async init() {
      this.document = new NewStudentDischargeDocumentModel({ personId: this.personId, institutionId: this.userInstitutionId, status: 1 }, this.$moment);
    },
    loadStudentClass() {
      if(this.studentClass !== null) return; // Вече е заредено.

      // Зареждаме детайли за StudentClass на избрания ученик, за който isCurrent === true и е за институцията на логнатия директор.
      let currentStudentClassId = undefined;
      if (this.studentClassId) {
        // Има подадено Id на StudentClass отвън.
        currentStudentClassId = this.studentClassId;
      } else {
          // Взимаме го от заредените профилни(детайли) данни на избрания ученик.
          const first = this.currentStudentSummary && this.currentStudentSummary.allCurrentClasses
            ? this.currentStudentSummary.allCurrentClasses.filter(x => x.institutionId === this.userSelectedRole.InstitutionID && x.isCurrent === true && x.positionId === 3)[0]
            : undefined;
          currentStudentClassId = first ? first.studentClassId : undefined;
      }

      if(currentStudentClassId) {
        this.$api.studentClass.getById(currentStudentClassId)
          .then(response => {
            this.studentClass = response.data;
            if(this.studentClass) {
              this.document.studentClassId = this.studentClass.id;
              this.document.institutionId = this.studentClass.classGroup.institutionId;
            }
          })
          .catch(error => {
            console.log(error.response);
          });
      }
    },
    onCancel() {
      this.$router.go(-1);
    },
    async onSave() {
      const form = this.$refs['dischargeDocumentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.document.noteDate = this.document.noteDate? this.$helper.parseDateToIso(this.document.noteDate, '') : this.document.noteDate;
      this.document.dischargeDate = this.document.dischargeDate ? this.$helper.parseDateToIso(this.document.dischargeDate, '') : this.document.dischargeDate;

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
        .create(this.document)
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
    }
  }
};
</script>
