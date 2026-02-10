<template>
  <v-card
    v-if="value"
  >
    <v-card-title>{{ `${value.schoolYearName} - ${$helper.getMonthName(value.month)}` }}</v-card-title>
    <v-card-subtitle>{{ `${value.fromDate ? $moment(value.fromDate).format(dateFormat) : ''} - ${value.toDate ? $moment(value.toDate).format(dateFormat) : ''}` }}</v-card-subtitle>
    <v-card-text>
      <v-simple-table dense>
        <template v-slot:default>
          <tbody>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.schoolYear') }}</td>
              <td>{{ value.schoolYearName }}</td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.month') }}</td>
              <td>{{ $helper.getMonthName(value.month) }}</td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.fromDate') }}</td>
              <td>{{ value.fromDate ? $moment(value.fromDate).format(dateFormat) : '' }}</td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.toDate') }}</td>
              <td>{{ value.toDate ? $moment(value.toDate).format(dateFormat) : '' }}</td>
            </tr>
            <tr>
              <td>{{ $t('absenceCampaign.headers.name') }}</td>
              <td>{{ value.name }}</td>
            </tr>
            <tr>
              <td>{{ $t('absenceCampaign.headers.description') }}</td>
              <td>{{ value.description }}</td>
            </tr>
            <tr>
              <td>{{ $t('absenceCampaign.headers.status') }}</td>
              <td>
                <v-chip
                  v-if="value.isActive"
                  color="success"
                  small
                >
                  {{ $t('common.active') }}
                </v-chip>
                <v-chip
                  v-else
                  color="light"
                  small
                >
                  {{ $t('common.unactive') }}
                </v-chip>
              </td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.isManuallyActivated') }}</td>
              <td>
                <v-chip
                  v-if="value.isManuallyActivated"
                  color="success"
                  small
                >
                  <yes-no
                    :value="value.isManuallyActivated"
                  />
                </v-chip>
                <v-chip
                  v-else
                  color="light"
                  small
                >
                  <yes-no
                    :value="value.isManuallyActivated"
                  />
                </v-chip>
              </td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.creator') }}</td>
              <td>{{ value.creator }}</td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.createDate') }}</td>
              <td>{{ value.createDate ? $moment(value.createDate).format(dateAndTimeFormat) : '' }}</td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.updater') }}</td>
              <td>{{ value.updater }}</td>
            </tr>
            <tr v-if="showExtendedDetails">
              <td>{{ $t('absenceCampaign.headers.modifyDate') }}</td>
              <td>{{ value.modifyDate ? $moment(value.modifyDate).format(dateAndTimeFormat) : '' }}</td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-card-text>

    <v-card-actions>
      <v-spacer />
      <no-absences-import
        v-if="isAbsenceImportCampaing && !!value.importId == false"
        :month="value.month"
        :school-year="value.schoolYear"
        @noAbsencesSubmited="$emit('noAbsencesSubmited')"
      />
    </v-card-actions>
  </v-card>
</template>

<script>
import NoAbsencesImport from '@/components/absence/NoAbsencesImport';
import Constants from '@/common/constants.js';

export default {
  name: 'AbsenceCampaignDetailsCard',
  components: { NoAbsencesImport },
  props: {
    value: {
      type: Object,
      default() {
        return null;
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
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT
    };
  },
  computed: {
    isAbsenceImportCampaing() {
      return this.value && this.value.campaignType === 0;
    },
  }
};
</script>
