<template>
  <v-tooltip
    v-if="hasStudentAbsenceManagePermission"
    bottom
  >
    <template v-slot:activator="{ on }">
      <slot>
        <v-btn
          color="warning"
          text
          small
          @click.stop="noAbsencesImportClick()"
          v-on="on"
        >
          <v-icon
            left
          >
            mdi-chart-donut
          </v-icon>
          {{ $t('absence.noAbsencesImportBtnText') }}
        </v-btn>
      </slot>
    </template>
    <span> {{ $t('absence.noAbsencesImportBtnTooltip') }} </span>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-tooltip>
</template>

<script>
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'NoAbsencesImportComponent',
  props: {
    month: {
      type: Number,
      default() {
        return null;
      }
    },
    schoolYear: {
      type: Number,
      default() {
        return null;
      }
    },
  },
  data() {
    return {
      saving: false
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userDetails']),
    hasStudentAbsenceManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceManage);
    },
  },
  methods: {
    async noAbsencesImportClick() {
      if(await this.$refs.confirm.open(this.$t('absence.noAbsencesImportBtnTooltip'), this.$t('common.confirm'))) {
        const absenceImportId = await this.noAbsencesImportCreate();
        if(absenceImportId) {
          await this.noAbsenceImportSign(absenceImportId);
        }
      }
    },
    async noAbsencesImportCreate() {
      this.saving = true;

      try {
        const payload = { schoolYear: this.schoolYear, month: this.month };
        const absenceImportId = (await this.$api.absence.createNoAbsencesImport(payload)).data;
        console.log(absenceImportId);
        this.saving = false;
        this.$emit('noAbsencesSubmited');
        return absenceImportId;

      } catch (error) {
        this.saving = false;
        console.log(error);
      }
    },
    async noAbsenceImportSign(absenceImportId) {
      this.saving = true;

      const hasLocalServerError = await this.checkForLocalServer();
      if (hasLocalServerError) {
        this.saving = false;
        this.$notifier.modalError(this.$t('menu.localServer'), this.$t('errors.scan.localServerError'));
        return;
      }

      const xml = (await this.$api.absence.constructNoAbsenceImportAsXml(absenceImportId)).data;
      if(!xml) {
        this.saving = false;
        this.$helper.logError({ action: 'NoAbsenceImportSign', message: 'Empty xml model'}, { absenceImportId: absenceImportId }, this.userDetails);
        return this.$notifier.error(this.$t('common.sign'), 'Empty xml model', 5000);
      }

      try {
        const signingResponse = await this.$api.certificate.signXml(xml);
        if (signingResponse && signingResponse.isError == false && signingResponse.contents) {
          let signingAttributes = {
            absenceImportId: absenceImportId,
            signature: btoa(signingResponse.contents)
          };

          try {
            await this.$api.absence.setAbsenceImportSigningAtrributes(signingAttributes);
          } catch (error) {
            // Ако не успее да записи signature (дава странна CORS грешка)
            // ще маркирам импорта като подписан но без signature.
            signingAttributes.signature = null;
            await this.$api.absence.setAbsenceImportSigningAtrributes(signingAttributes);
          }

          this.$notifier.success('', this.$t('common.signSuccess'));
          this.$emit('noAbsencesSubmited');
        } else {
          this.$helper.logError({ action: 'NoAbsenceImportSign', message: signingResponse}, { absenceImportId: absenceImportId }, { userDetails: this.userDetails, xml: xml });
          console.error(signingResponse);
          this.$notifier.error(this.$t('common.sign'), signingResponse?.message ?? this.$t('common.signError'), 5000);
        }


      } catch (error) {
        this.$helper.logError({ action: 'NoAbsenceImportSign', message: error}, { absenceImportId: absenceImportId }, { userDetails: this.userDetails, xml: xml });
        console.error(error);
        this.$notifier.error(this.$t('common.sign'), this.$t('common.signError'), 5000);
      }

      this.saving = false;
    },
    async checkForLocalServer() {
      let hasLocalServerError = false;
       await this.$api.localServer.version()
        .then( () => {
          hasLocalServerError = false;
        })
        .catch(() => {
          hasLocalServerError = true;
        });

      return hasLocalServerError;
    },
  }
};
</script>
