<template>
  <grid-wrapper
    :title="$t('aspAbsences.listTitle')"
    :headers="headers"
    :url="apiUrl"
    :items-per-page-options="[5,10,15,50]"
    :items-per-page="5"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    file-export-name="MonAbs_Export"
  >
    <template v-slot:[`item.pin`]="{ item }">
      {{ `${item.pin} - ${item.pinType}` }}
    </template>
  </grid-wrapper>
</template>

<script>
import GridWrapper from '@/components/wrappers/grid';
export default {
  name: 'AspSubmittedAbsencesList',
  components: { GridWrapper },
  props: {
    campaignId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      headers: [
        {
          text: this.$t('aspAbsences.headers.year'),
          value: 'intYear'
        },
        {
          text: this.$t('aspAbsences.headers.month'),
          value: 'intMonth'
        },
        {
          text: this.$t('aspAbsences.headers.identifier'),
          value: 'pin'
        },
        {
          text: this.$t('aspAbsences.headers.notExcused'),
          value: 'notExcused'
        },
        {
          text: this.$t('aspAbsences.headers.institutionCode'),
          value: 'institutionCode'
        },
        {
          text: this.$t('aspAbsences.headers.basicClass'),
          value: 'basicClass'
        }
      ],
      apiUrl: `/api/AbsenceCampaign/GetAspAbsencesForCampaign/${this.campaignId}`
    };
  }
};
</script>
