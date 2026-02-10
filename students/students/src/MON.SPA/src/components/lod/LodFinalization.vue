<template>
  <div class="d-inline-flex">
    <button-group>
      <button-tip
        v-if="isTeacher && !isPrevSchoolYearClass"
        icon
        icon-name="fa-check"
        iclass="mx-2"
        icon-color="primary"
        tooltip="buttons.lodApproval"
        bottom
        small
        :disabled="lodApprovalDisabled"
        @click="approveLod()"
      />
      <button-tip
        v-if="isTeacher && !isPrevSchoolYearClass"
        icon
        icon-name="fa-undo"
        iclass="mx-2"
        icon-color="primary"
        tooltip="buttons.lodApprovalUndo"
        bottom
        small
        :disabled="!canBeUndone || lodApprovalUndoDisabled"
        @click="approveLodUndo()"
      />
      <button-tip
        v-if="isSchoolDirector && showGenerateLod"
        color="blue-grey darken-3"
        icon
        icon-color="blue-grey darken-3"
        icon-name="fas fa-print"
        iclass=""
        tooltip="lod.generatePersonalFileForStay"
        bottom
        small
        :disabled="lodGenerateForStayDisabled"
        @click="generateForStay()"
      />
      <button-tip
        v-if="isSchoolDirector"
        icon
        icon-name="fa-file-signature"
        small
        iclass="mx-2"
        icon-color="primary"
        tooltip="buttons.lodFinalization"
        bottom
        :disabled="lodFinalizationDisabled"
        @click="finalizeLod()"
      />
      <button-tip
        v-if="isSchoolDirector && !isPrevSchoolYearClass"
        icon
        icon-name="fa-file-contract"
        small
        iclass="mx-2"
        icon-color="primary"
        tooltip="buttons.lodFinalizationUndo"
        bottom
        :disabled="!canBeUndone || lodFinalizationUndoDisabled"
        @click="finalizeLodUndo()"
      />
      <confirm-dlg
        ref="confirmUndo"
        :show-confirm-text="true"
        :confirm-text-label="$t('common.reasonText')"
      />
      <confirm-dlg ref="confirm" />
    </button-group>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-overlay :value="signing">
      <v-row justify="center">
        <v-progress-circular
          :value="signingProgressPercentage"
          color="primary"
          size="128"
          width="13"
        >
          <h2 class="white--text">
            {{ `${signingProgressCount}/${signingItemsCount}` }}
          </h2>
        </v-progress-circular>
      </v-row>

      <h2 class="white--text mt-2">
        {{ selectedItemName }}
      </h2>
    </v-overlay>
  </div>
</template>

<script>
import Constants from "@/common/constants.js";
import { mapGetters } from 'vuex';
import { LodFinalizationModel } from '@/models/lodModels/lodFinalizationModel.js';
import { LodFinalizationUndoModel } from '@/models/lodModels/lodFinalizationUndoModel.js';
import { UserRole } from '@/enums/enums';
import { downloadZip } from 'client-zip';
import { checkForLocalServer, requireMinLocalServerVersion } from '@/utils/sign.utils.js';

export default {
  name: 'LodFinalizationComponent',
  components: {},
  props: {
    schoolYear: {
      type: Number,
      required: true,
    },
    selectedStudents: {
      type: Array,
      default() {
        return [];
      },
    },
    canBeUndone: {
      type: Boolean,
      default() {
        return true;
      }
    },
    showGenerateLod:{
      type: Boolean,
        default() {
          return false;
        }
    },
    lodFileName:{
      type: String,
      default() {
        return 'ЛОД';
      }
    },
    isPrevSchoolYearClass: {
      type: Boolean,
      default() {
        return false;
      }
    },
    classId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      dateFormat: Constants.DATE_FORMAT,
      saving: false,
      signing: false,
      signingItemsCount: 0,
      signingProgressCount: 0,
      selectedItemName: '',
    };
  },
  computed: {
    ...mapGetters(['isInRole', 'mode', 'userSelectedRole', 'userDetails']),
    isInDevMode() {
      return this.mode !== 'prod';
    },
    lodApprovalDisabled() {
      if (this.selectedStudents.length === 1) {
        return (
          !this.isEligibleForLodApproval(this.selectedStudents[0]) ||
          !this.schoolYear
        );
      }

      return this.disabled || false;
    },
    lodApprovalUndoDisabled() {
      if (this.selectedStudents.length === 1) {
        return (
          !this.isEligibleForLodApprovalUndo(this.selectedStudents[0]) ||
          !this.schoolYear
        );
      }

      return this.disabled || false;
    },
    lodFinalizationDisabled() {
      if (this.selectedStudents.length === 1) {
        return (
          !this.isEligibleForLodFinalization(this.selectedStudents[0]) ||
          !this.schoolYear
        );
      }

      return this.disabled || false;
    },
    lodGenerateForStayDisabled() {
      return (this.selectedStudents.length === 0);
    },
    lodFinalizationUndoDisabled() {
      if (this.selectedStudents.length === 1) {
        return (
          !this.isEligibleForLodFinalizationUndo(this.selectedStudents[0]) ||
          !this.schoolYear
        );
      }

      return this.disabled || false;
    },
    disabled() {
      return this.selectedStudents.length === 0 || !this.schoolYear;
    },
    isSchoolDirector() {
      return this.isInRole(UserRole.School);
    },
    isTeacher() {
      return this.isInRole(UserRole.Teacher);
    },
    signingProgressPercentage() {
      return (this.signingProgressCount / this.signingItemsCount) * 100;
    }
  },
  methods: {
    async approveLod() {
      const studentLodsToApprove = this.selectedStudents.filter((x) => { return this.isEligibleForLodApproval(x); });

      if (!studentLodsToApprove || studentLodsToApprove.length === 0) {
        this.$notifier.modalWarn(this.$t('buttons.lodApproval'), this.$t('lod.lodApprovalListNotCorrect'));
        return;
      }

      // Финализиране на ЛОД преди 31.05 е недопустимо
      // const today = new Date();
      // const finalizationMinDate  = new Date(2024, 5, 31);
      // if (this.schoolYear == 2023 && today < finalizationMinDate)
      // {
      //   this.$notifier.modalWarn(this.$t('buttons.lodApproval'), this.$t('lod.lodMinFializationDateWarning', [this.schoolYear, this.$moment(finalizationMinDate).format(this.dateFormat)]));
      //   return;
      // }

      if (!(await this.$refs.confirm.open(this.$t('lod.lodApprovalConfirmHeader'), this.$t('common.confirm')))) {
        return;
      }

      const studentsForLodApprovalIds = studentLodsToApprove.map((x) => {
        return x.personId;
      });


      this.saving = true;

      this.$api.lodFinalization.approveLod(
        new LodFinalizationModel({
          personIds: studentsForLodApprovalIds,
          schoolYear: this.schoolYear,
        })
      )
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
        })
        .catch((error) => {
          console.log(error.response);
          const { message } = this.$helper.parseError(error);
          this.$notifier.error(this.$t('errors.studentLodApproval'), message);
        })
        .finally(() => {
          this.$emit('lodFinalizationEnded');
          this.saving = false;
        });
    },
    async generateForStay() {

      this.signingProgressCount = 0;
      this.signingItemsCount = 0;
      this.selectedItemName = '';

      const studentLodsToGenerate = this.selectedStudents;

      if (!studentLodsToGenerate || studentLodsToGenerate.length === 0) {
        this.$notifier.modalWarn(this.$t('buttons.lodFinalization'), this.$t('lod.lodFinalizationListNotCorrect'));
        return;
      }

      const hasLocalServerError = await checkForLocalServer();
      if (hasLocalServerError) {
          this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.scan.localServerError')
        );
        return;
      }

      const needUpgrade = await requireMinLocalServerVersion("1.0.8");
      if (needUpgrade) {
          this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.upgradeLocalServer')
        );
        return;
      }

      this.signingItemsCount = studentLodsToGenerate.length;

      try {
        this.signing = true;
        let thumbprint = null;
        const errors = [];

        const signOptions = {
          name: this.userSelectedRole?.Username,
          title: `${this.userDetails?.institution ?? ''} - ${String(this.userSelectedRole?.InstitutionID ?? '')}`,
          email: this.userSelectedRole?.Username,
        };

        let signedStudentLods = [];

        for (const item of studentLodsToGenerate) {
          this.signingProgressCount += 1;
          this.selectedItemName = item.fullName;

          const personalFileBlob = (await this.$api.studentLod.generatePersonalFileForStay({ personId: item.personId, classId: this.classId }))?.data;
          if (!personalFileBlob) continue;

          const docxBase64 = await this.$helper.blobToBase64(personalFileBlob);
          if (!docxBase64) continue;

          let response = await this.$api.certificate.signDocxThumbprint(docxBase64, thumbprint, signOptions);

          if (response && response.isError == false) {
            let signature = response.contents; // base64
            signedStudentLods.push({name: `${item.fullName}.docx`, input: new Blob([new Uint8Array(this.$helper.base64ToByteArray(signature))])});
            thumbprint = response.thumbprint;
            if (
              (thumbprint &&
                thumbprint.toUpperCase() ==
                '6C943294128E6D5455F7CA6B0CE9E4A1F179BFFB') ||
              signature.toUpperCase().indexOf('KONTRAX') > 0
            ) {
              this.$notifier.modalError(
                this.$t('localServer.demoMode'),
                this.$t('localServer.demoModeError')
              );
              this.signing = false;
              return;
            }
          } else {
            // None = 0,
            // CertificateNotFound = 1,
            // SigningError = 2,
            // OperationCanceled = 100
            if (response.messageCode === 1) {
              throw new Error(response.message);
            } else {
              errors.push(`${item.fullName} : ${response.response}`);
            }
          }
        }

        if (errors.length > 0) {
          this.$notifier.modalError(this.$t('errors.studentLodFinalization'), errors);
        } else {
          // get the ZIP stream in a Blob
          const blob = await downloadZip(signedStudentLods).blob();

          // make and click a temporary link to download the Blob
          const link = document.createElement("a");
          link.href = URL.createObjectURL(blob);
          link.download = `ЛОД ${this.lodFileName}.zip`;
          link.click();
          link.remove();

          this.$notifier.success('', this.$t('common.saveSuccess'));
        }
      } catch (error) {
        const { message } = this.$helper.parseError(error);
        this.$notifier.error('', message ?? this.$t('common.signError'));
      } finally {
        this.signing = false;
        this.signingProgressCount = 0;
        this.signingItemsCount = 0;
        this.selectedItemName = '';
        this.$emit('generateForStayEnded');
      }
    },
    async finalizeLod() {

      this.signingProgressCount = 0;
      this.signingItemsCount = 0;
      this.selectedItemName = '';

      const studentLodsToFinalize = this.selectedStudents.filter(x => this.isEligibleForLodFinalization(x));

      if (!studentLodsToFinalize || studentLodsToFinalize.length === 0) {
        this.$notifier.modalWarn(this.$t('buttons.lodFinalization'), this.$t('lod.lodFinalizationListNotCorrect'));
        return;
      }

      // const today = new Date();
      // const finalizationMinDate  = new Date(2024, 5, 31);
      // if (this.schoolYear == 2023 && today < finalizationMinDate){
      //   this.$notifier.modalWarn(this.$t('buttons.lodFinalization'), this.$t('lod.lodMinFializationDateWarning', [this.schoolYear, this.$moment(finalizationMinDate).format(this.dateFormat)]));
      //   return;
      // }

      if (!(await this.$refs.confirm.open(this.$t('lod.lodApprovalConfirmHeader'), this.$t('lod.lodFinalizationConfirmText', [this.schoolYear])))) {
        return;
      }

      const hasLocalServerError = await checkForLocalServer();
      if (hasLocalServerError) {
        this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.scan.localServerError')
        );
        return;
      }

      const needUpgrade = await requireMinLocalServerVersion("1.0.8");
      if (needUpgrade) {
        this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.upgradeLocalServer')
        );
        return;
      }

      this.signingItemsCount = studentLodsToFinalize.length;

      try {
        this.signing = true;
        let thumbprint = null;
        const errors = [];

        const signOptions = {
          name: this.userSelectedRole?.Username,
          title: `${this.userDetails?.institution ?? ''} - ${String(this.userSelectedRole?.InstitutionID ?? '')}`,
          email: this.userSelectedRole?.Username,
        };

        for (const item of studentLodsToFinalize) {
          this.signingProgressCount += 1;
          this.selectedItemName = item.fullName;

          const personalFileBlob = (await this.$api.studentLod.generatePersonalFile({ personId: item.personId, classId: this.classId, schoolYears: [this.schoolYear] }))?.data;
          if (!personalFileBlob) continue;

          // const url = window.URL.createObjectURL(personalFileBlob);
          // const link = document.createElement('a');
          // link.href = url;
          // link.setAttribute('download', `${item.fullName} личен картон.docx`);
          // document.body.appendChild(link);
          // link.click();


          const docxBase64 = await this.$helper.blobToBase64(personalFileBlob);
          if (!docxBase64) continue;

          let response = await this.$api.certificate.signDocxThumbprint(docxBase64, thumbprint, signOptions);

          if (response && response.isError == false) {
            let signature = response.contents; // base64
            thumbprint = response.thumbprint;
            if (
              (thumbprint &&
                thumbprint.toUpperCase() ==
                '6C943294128E6D5455F7CA6B0CE9E4A1F179BFFB') ||
              signature.toUpperCase().indexOf('KONTRAX') > 0
            ) {
              this.$notifier.modalError(
                this.$t('localServer.demoMode'),
                this.$t('localServer.demoModeError')
              );
              this.signing = false;
              return;
            }
            const payload = {
              personId: item.personId,
              schoolYear: this.schoolYear,
              signature: signature,
              classId: this.classId
            };

            try {
              await this.$api.lodFinalization.signLod(payload);
            } catch (error) {
              const { message } = this.$helper.parseError(error);
              if (message) errors.push(`${item.fullName} : ${message}`);
            }
          } else {
            // None = 0,
            // CertificateNotFound = 1,
            // SigningError = 2,
            // OperationCanceled = 100
            if (response.messageCode === 1) {
              throw new Error(response.message);
            } else {
              errors.push(`${item.fullName} : ${response.response}`);
            }
          }
        }

        if (errors.length > 0) {
          this.$notifier.modalError(this.$t('errors.studentLodFinalization'), errors);
        } else {
          this.$notifier.success('', this.$t('common.saveSuccess'));
        }
      } catch (error) {
        const { message } = this.$helper.parseError(error);
        this.$notifier.error('', message ?? this.$t('common.signError'));
      } finally {
        this.signing = false;
        this.signingProgressCount = 0;
        this.signingItemsCount = 0;
        this.selectedItemName = '';
        this.$emit('lodFinalizationEnded');
      }
    },
    async approveLodUndo() {
      const studentLodsToUndo = this.selectedStudents.filter((x) => { return this.isEligibleForLodApprovalUndo(x); });

      if (!studentLodsToUndo || studentLodsToUndo.length === 0) {
        this.$notifier.modalWarn(this.$t('buttons.lodApprovalUndo'), this.$t('lod.lodApprovalUndoListNotCorrect'));
        return;
      }

      if (!(await this.$refs.confirmUndo.open(this.$t('lod.lodApprovalUndoConfirmHeader'), this.$t('common.confirm')))) {
        return;
      }

      const studentsForLodApprovalUndoIds = this.selectedStudents.map((x) => {
        return x.personId;
      });

      this.saving = true;

      this.$api.lodFinalization.approveLodUndo(
        new LodFinalizationUndoModel({
          personIds: studentsForLodApprovalUndoIds,
          schoolYear: this.schoolYear,
          reason: this.$refs.confirmUndo.confirmText,
        })
      )
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
        })
        .catch((error) => {
          console.log(error.response);
          const { message } = this.$helper.parseError(error);
          this.$notifier.error(this.$t('errors.studentLodApprovalUndo'), message);
        })
        .finally(() => {
          this.saving = false;
          this.$emit('lodFinalizationEnded');
        });
    },
    async finalizeLodUndo() {
      this.signingProgressCount = 0;
      this.signingItemsCount = 0;
      this.selectedItemName = '';


      const studentLodsToUndo = this.selectedStudents.filter(x => this.isEligibleForLodFinalizationUndo(x));
      if (!studentLodsToUndo || studentLodsToUndo.length === 0) {
        this.$notifier.modalWarn(this.$t('buttons.lodFinalizationUndo'), this.$t('lod.lodFinalizationUndoListNotCorrect'));
        return;
      }

      if (!(await this.$refs.confirmUndo.open(this.$t('lod.lodFinalizationUndoConfirmHeader'), this.$t("common.confirm")))) {
        return;
      }

      this.signingItemsCount = studentLodsToUndo.length;

      try {
        this.signing = true;
        const errors = [];
        for (const item of studentLodsToUndo) {
          this.signingProgressCount += 1;
          this.selectedItemName = item.fullName;

          const payload = {
            personId: item.personId,
            schoolYear: this.schoolYear,
            reason: this.$refs.confirm.confirmText,
          };

          try {
            await this.$api.lodFinalization.signLodUndo(payload);
          } catch (error) {
            const { message } = this.$helper.parseError(error);
            if (message) errors.push(`${item.fullName} : ${message}`);
          }

          if (errors.length > 0) {
            this.$notifier.modalError(this.$t('errors.studentLodFinalizationUndo'), errors);
          } else {
            this.$notifier.success('', this.$t('common.saveSuccess'));
          }
        }
      } catch (error) {
        console.log(error.response);
        const { message } = this.$helper.parseError(error);
        this.$notifier.error(this.$t('errors.studentLodFinalizationUndo'), message);
      }
      finally {
        this.signing = false;
        this.signingProgressCount = 0;
        this.signingItemsCount = 0;
        this.selectedItemName = '';
        this.$emit('lodFinalizationEnded');
      }
    },
    getErrorMsg(error) {
      return error.response ? error.response.data.message : error.message;
    },
    isEligibleForLodApproval(el) {
      return !el.isLodApproved;
    },
    isEligibleForLodApprovalUndo(el) {
      return !el.isLodFinalized && el.isLodApproved;
    },
    isEligibleForLodFinalization(el) {
      return !el.isLodFinalized;
    },
    isEligibleForLodFinalizationUndo(el) {
      return el.isLodFinalized;
    },
  },
};
</script>

<style>
.confirmLod.v-card__subtitle {
  color: red !important;
}
</style>

<!-- <style scoped>
  .my-overlay >>> .v-overlay__content {
    width: 100%;
    height: 100%;
  }
</style> -->
