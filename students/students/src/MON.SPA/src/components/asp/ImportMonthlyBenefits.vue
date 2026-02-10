<template>
  <v-card>
    <v-card-title>{{ $t('asp.monthlyBenefitsImportTitle') }}</v-card-title>

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
        is-confirmation-session
        @importAspConfirmationSessionRecords="onImportAspConfirmationSessionRecordsClick"
      />
    </v-card-text>
    <v-overlay
      :value="saving"
    >
      <div class="text-center">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </div>
    </v-overlay>
  </v-card>
</template>

<script>
import CustomMonthPicker from '@/components/wrappers/CustomMonthPicker';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import AspSessionInfoDetails from '@/components/asp/AspSessionInfoDetails.vue';

export default {
  name: 'ImportMonthlyBenefit',
  components: { CustomMonthPicker, SchoolYearSelector, AspSessionInfoDetails },
  data()
  {
    return {
      month: null,
      schoolYear: null,
      file: null,
      saving: false,
      aspAskingSession: undefined,
    };
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
    onImportAspConfirmationSessionRecordsClick(aspSessonInfo) {
      this.saving = true;

      this.$api.asp
        .loadAspConfirmations(aspSessonInfo)
        .then(() =>
        {
          this.$notifier.success(this.$t('asp.importLabel'), this.$t('common.saveSuccess'));
          this.$emit('submitFile');
        })
        .catch((error) => {
          console.log(error.response);
          this.$notifier.error(this.$t('asp.importLabel'), this.$t('common.saveError'));
        })
        .then(() => {
            this.saving = false;
        });

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
      this.aspAskingSession = undefined;
      if (!this.schoolYear || !this.month) {
        return;
      }

      try {
        this.aspAskingSession  = (await this.$api.absenceCampaign.getAspSession(this.schoolYear, this.month, 'ПОТВЪРЖДЕНИЕ'))?.data;
      } catch (error) {
        console.log(error);
      }
    },
  }
};
</script>
