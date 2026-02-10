<template>
  <grid-wrapper
    title=""
    :headers="headers"
    :url="apiUrl"
    :items-per-page-options="[5,10,15,50]"
    :items-per-page="5"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    file-export-name="MonConfs_Export"
  >
    <template v-slot:[`item.pin`]="{ item }">
      {{ `${item.pin} - ${item.pinType}` }}
    </template>
  </grid-wrapper>
</template>

<script>
import GridWrapper from '@/components/wrappers/grid';
export default {
  name: 'AspSubmittedMonConfirmsList',
  components: { GridWrapper },
  props: {
    sessionNo: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      headers: [
        {
          text: this.$t('aspAbsences.headers.institutionCode'),
          value: 'institutionCode'
        },
        {
          text: this.$t('aspConfirms.headers.identifier'),
          value: 'pin'
        },
        {
          text: this.$t('aspConfirms.headers.firstName'),
          value: 'firstName'
        },
        {
          text: this.$t('aspConfirms.headers.middleName'),
          value: 'middleName'
        },
        {
          text: this.$t('aspConfirms.headers.lastName'),
          value: 'lastName'
        },
        {
          text: this.$t('aspConfirms.headers.aspStatus'),
          value: 'aspStatus'
        },
        {
          text: this.$t('aspConfirms.headers.monStatus'),
          value: 'monStatus'
        },
        {
          text: this.$t('aspConfirms.headers.absenceCount'),
          value: 'notExcused'
        },
        {
          text: this.$t('aspConfirms.headers.absenceCorrectionCount'),
          value: 'notExcusedCorrection'
        },
        {
          text: this.$t('aspConfirms.headers.daysCount'),
          value: 'days'
        },
        {
          text: this.$t('aspConfirms.headers.daysCorrectionCount'),
          value: 'daysCorrection'
        }
      ],
      apiUrl: `/api/Asp/GetMonConfirmsForCampaign/${this.sessionNo}`
    };
  }
};
</script>
