<template>
  <grid-wrapper
    :title="$t('aspAsking.listTitle')"
    :headers="headers"
    :url="apiUrl"
    :search-label="$t('buttons.pinSaerch')"
    :items-per-page-options="[5,10,15,50]"
    :items-per-page="5"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    file-export-name="AspAsking_Export"
  >
    <template v-slot:[`item.idNumber`]="{ item }">
      {{ `${item.idNumber} - ${item.idTypeName}` }}
    </template>
  </grid-wrapper>
</template>

<script>
import GridWrapper from "@/components/wrappers/grid";
export default {
  name: 'AspAskingList',
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
          text: this.$t('student.headers.firstName'),
          value: 'firstName'
        },
        {
          text: this.$t('student.headers.middleName'),
          value: 'middleName'
        },
        {
          text: this.$t('student.headers.lastName'),
          value: 'lastName'
        },
        {
          text: this.$t('student.headers.identifier'),
          value: 'idNumber'
        },
        {
          text: this.$t('common.publicEduNumber'),
          value: 'publicEduNumber'
        }
      ],
      apiUrl: `/api/AbsenceCampaign/GetASPStudentsForCampaign/${this.campaignId}`
    };
  }
};
</script>
