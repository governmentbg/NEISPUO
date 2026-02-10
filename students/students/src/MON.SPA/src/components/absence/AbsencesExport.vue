<template>
  <v-card>
    <v-card-text>
      <v-row dense>
        <v-col>
          <school-year-selector
            v-model="schoolYear"
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col>
          <custom-month-picker
            v-model="month"
            :rules="[$validator.required()]"
            class="required"
            :disabled="!schoolYear"
          />
        </v-col>
      </v-row>
      <asp-session-info-details
        :value="aspAskingSession"
        is-absence-session
      />
    </v-card-text>
    <v-card-actions class="justify-end">
      <!-- <button-tip
        icon-name="mdi-content-copy"
        tooltip="absence.copyAspAsking"
        text="buttons.copyAspAsking"
        bottom
        raised
        iclass="mx-2"
        :disabled="!schoolYear || !month || !aspAskingSession"
        @click="copyAspAsking"
      /> -->
      <button-tip
        icon-name="fa-file-export"
        tooltip="absence.exportAbsenceFile"
        text="buttons.export"
        bottom
        raised
        iclass="mx-2"
        :disabled="!schoolYear || !month || !aspAskingSession"
        @click="exportAbsences"
      />
    </v-card-actions>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import CustomMonthPicker from '@/components/wrappers/CustomMonthPicker';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import AspSessionInfoDetails from '@/components/asp/AspSessionInfoDetails.vue';
import { mapGetters } from 'vuex';

export default {
  name: "AbsencesExport",
  components: { CustomMonthPicker, SchoolYearSelector, AspSessionInfoDetails },
  data() {
    return {
      month: null,
      schoolYear: null,
      saving: false,
      aspAskingSession: undefined
    };
  },
  computed:{
    ...mapGetters(['userDetails']),
  },
  watch: {
    month() {
      this.checkAspAskingSession();
    },
    schoolYear() {
      this.checkAspAskingSession();
    }
  },
  mounted() {
    this.getCurrentSchoolYear();
  },
  methods: {
    async exportAbsences(){
      if(await this.$refs.confirm.open(this.$t('buttons.export'), this.$t('common.confirm'))) {
          this.saving = true;

          this.$api.absence.exportAbsencesToFile(this.schoolYear, this.month)
          .then(() => {
              this.$notifier.success('', this.$t('absence.absenceExportSuccess'));
              this.$emit('exportComplete');
          })
          .catch((error) => {
            const {message, errors} = this.$helper.parseError(error.response);
            this.$notifier.modalError(message, errors);
            this.$helper.logError({ action: 'AbsencesExport', message: message}, errors, this.userDetails);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async copyAspAsking(){
      if(await this.$refs.confirm.open(this.$t('buttons.copy'), this.$t('common.confirm'))) {
          this.saving = true;

          this.$api.absence.copyAspAsking(this.schoolYear, this.month)
          .then(() => {
              this.$notifier.success(this.$t('buttons.copy'), this.$t('common.success'));
              this.$emit('exportComplete');
          })
          .catch((error) => {
            const {message, errors} = this.$helper.parseError(error.response);
            this.$notifier.modalError(message, errors);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async getCurrentSchoolYear() {
      try {
        const currentSchoolYear = Number((await this.$api.institution.getCurrentYear())?.data);
        if(!isNaN(currentSchoolYear)) {
          this.schoolYear = currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    },
    async checkAspAskingSession() {
      this.aspAskingSession == undefined;
      if (!this.schoolYear || !this.month) {
        return;
      }

      try {
        this.aspAskingSession  = (await this.$api.absenceCampaign.getAspSession(this.schoolYear, this.month, 'ЗАПИТВАНЕ'))?.data;
      } catch (error) {
        console.log(error);
      }
    }
  }
};
</script>

