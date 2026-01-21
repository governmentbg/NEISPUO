<template>
  <v-card
    v-if="value"
  >
    <v-card-title>{{ `${value.schoolYearName} - ${$helper.getMonthName(value.month)}` }}</v-card-title>
    <v-card-subtitle>{{ `${value.fromDate ? $moment(value.fromDate).format(dateFormat) : ''} - ${value.toDate ? $moment(value.toDate).format(dateFormat) : ''}` }}</v-card-subtitle>
    <v-card-text>
      <v-row
        v-if="value.labels && value.labels.length > 0"
        class="mb-1"
      >
        <v-chip
          v-for="(label, index) in value.labels"
          :key="index"
          :color="label.value"
          small
          class="mr-1"
        >
          {{ label.key }}
        </v-chip>
      </v-row>
      <v-simple-table
        v-if="details"
        dense
      >
        <template v-slot:default>
          <tbody>
            <tr
              v-for="(item, index) in details"
              :key="index"
            >
              <td>{{ $t(item.key) }}</td>
              <td>{{ item.value }}</td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-card-text>
    <v-row
      align="center"
      class="py-2 px-4"
    >
      <v-tooltip
        v-if="value.isRelatedInstitutionImportSigned"
        bottom
      >
        <template v-slot:activator="{ on: tooltip }">
          <v-icon
            color="primary"
            x-large
            v-on="{ ...tooltip }"
          >
            fa-solid fa-signature
          </v-icon>
        </template>
        <span>{{ $t('common.isSigned') }}</span>
      </v-tooltip>
      <v-btn
        v-if="isAspImportCampaign && hasAspManagePermission && hasToBeSigned"
        color="red"
        text
        small
        to="asp/monthlyBenefitsImport"
      >
        <v-icon
          left
        >
          mdi-file-sign
        </v-icon>
        <span class="ml-1">{{ $t('common.sign') }}</span>
      </v-btn>
      <v-btn
        v-if="isAbsenceImportCampaing && hasStudentAbsenceManagePermission && hasToBeSigned"
        color="red"
        text
        small
        to="absence/import"
      >
        <v-icon
          left
        >
          mdi-file-sign
        </v-icon>
        <span class="ml-1">{{ $t('common.sign') }}</span>
      </v-btn>
      <v-btn
        v-if="isAbsenceImportCampaing && hasStudentAbsenceManagePermission && hasToBeSubmitted"
        color="red"
        text
        small
        to="absence/import"
      >
        <v-icon
          left
        >
          fas fa-file-import
        </v-icon>
        <span class="ml-1">{{ $t('buttons.submit') }}</span>
      </v-btn>
      <v-spacer />
      <no-absences-import
        v-if="isAbsenceImportCampaing && !!value.importId == false"
        :month="value.month"
        :school-year="value.schoolYear"
        @noAbsencesSubmited="$emit('noAbsencesSubmited')"
      />
      <v-btn
        v-if="value"
        color="primary"
        text
        small
        @click.stop="onDetailsBtnClick"
      >
        <v-icon
          left
        >
          mdi-details
        </v-icon>
        {{ $t('buttons.details') }}
      </v-btn>
    </v-row>
  </v-card>
</template>

<script>
import NoAbsencesImport from '@/components/absence/NoAbsencesImport';
import Constants from '@/common/constants.js';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'CampaignDetailsCard',
  components: { NoAbsencesImport },
  props: {
    value: {
      type: Object,
      default() {
        return undefined;
      }
    },
    showExtendedDetails: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      details: null
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId', 'hasPermission']),
    hasAspManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForASPManage);
    },
    hasStudentAbsenceManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceManage);
    },
    isAspImportCampaign() {
      return this.value && this.value.campaignType === 1;
    },
    isAbsenceImportCampaing() {
      return this.value && this.value.campaignType === 0;
    },
    allCount() {
      const key = 'absenceReport.filter.all';
      if (!this.details) return 0;

      const obj = this.details.find(x => x.key === key);
      if(!obj) return 0;
      return obj.value;
    },
    pendingCount() {
      const key = 'asp.headers.forReview';
      if (!this.details) return 0;

      const obj = this.details.find(x => x.key === key);
      if(!obj) return 0;
      return obj.value;
    },
    unsubmittedCount() {
      const key = 'absenceReport.filter.unsubmitted';
      if (!this.details) return 0;

      const obj = this.details.find(x => x.key === key);
      if(!obj) return 0;
      return obj.value;
    },
    unsignedCount() {
      const key = 'absenceReport.filter.unsigned';
      if (!this.details) return 0;

      const obj = this.details.find(x => x.key === key);
      if(!obj) return 0;
      return obj.value;
    },
    pendingProcessing() {
      if (!this.isAspImportCampaign || !this.userInstitutionId) {
        return false;
      }
      return this.allCount > 0 && this.allCount === this.pending;
    },
    unfinishedProcessing() {
      if (!this.isAspImportCampaign || !this.userInstitutionId) {
        return false;
      }

      return this.allCount > 0 && this.pendingCount > 0 && this.allCount !== this.pendingCount;
    },
    hasToBeSigned() {
      if (!this.userInstitutionId) {
        return false;
      }

      if (this.isAspImportCampaig) {
        return this.allCount > 0 && this.pendingCount <= 0 && !this.value.isRelatedInstitutionImportSigned;
      }

      if(this.isAbsenceImportCampaing) {
        return this.unsignedCount === 1;
      }

      return false;
    },
    hasToBeSubmitted() {
      if (!this.userInstitutionId) {
        return false;
      }

      if(this.isAbsenceImportCampaing) {
        return this.unsubmittedCount === 1;
      }

      return false;
    }
  },
  mounted() {
    if (this.showExtendedDetails) {
      this.loadDetails();
    }
  },
  methods: {
    loadDetails() {
      switch (this.value.campaignType) {
        case 0: // Подаване на отсъствия
          this.$api.absenceCampaign.getStats(this.value.id)
          .then(response => {
            if (response.data) {
              this.details = response.data;
            }
          })
          .catch((error) => {
            console.log(error.response);
          });
          break;
        case 1: // АСП потвърждаване
          this.$api.asp.getCampaignStats(this.value.id)
          .then(response => {
            if (response.data) {
              this.details = response.data;
            }
          })
          .catch((error) => {
            console.log(error.response);
          });
          break;
        default:
          break;
      }
    },
    onDetailsBtnClick(){
      switch (this.value.campaignType) {
        case 0: // Подаване на отсъствия
          if(this.value.importId && this.hasPermission(Permissions.PermissionNameForStudentAbsenceRead)) {
            return this.$router.push(`/absence/import/${this.value.importId}/details`);
          }

          if (this.userInstitutionId && this.unsubmittedCount > 0 && this.hasStudentAbsenceManagePermission) {
            return this.$router.push('/absence/import');
          }

          if (this.hasPermission(Permissions.PermissionNameForStudentAbsenceRead)) {
            return this.$router.push('/absence/reports');
          }

          if (this.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignRead)) {
            return this.$router.push(`/absence/campaign/${this.value.id}/details`);
          }

          break;
        case 1: // АСП потвърждаване
          if (this.hasAspManagePermission) {
          return this.$router.push(`/asp/monthlyBenefitsImport/${this.value.schoolYear}/${this.value.month}/benefitDetails/${this.value.id}?gridStatusFilter=-1`);
          }
          return this.$router.push({ path: `/asp/monthlyBenefitsImport/${this.value.schoolYear}/${this.value.month}/benefitDetails/${this.value.id}` });
        default:
          break;
      }
    }
  }
};
</script>
