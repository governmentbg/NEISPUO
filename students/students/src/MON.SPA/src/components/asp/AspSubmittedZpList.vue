<template>
  <grid-wrapper
    :title="$t('aspZp.listTitle')"
    :headers="headers"
    :url="apiUrl"
    :items-per-page-options="[5,10,15,50]"
    :items-per-page="5"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    file-export-name="MonZp_Export"
    :filter="{ statusTypeFilter: statusTypeFilter }"
  >
    <template v-slot:[`item.pin`]="{ item }">
      {{ `${item.pin} - ${item.pinType}` }}
    </template>
    <template #subtitle>
      <v-radio-group
        v-model="statusTypeFilter"
        row
      >
        <v-radio
          :label="$t('aspZp.filter.enrolled')"
          :value="1"
        />
        <v-radio
          :label="$t('aspZp.filter.discharged')"
          :value="3"
        />
        <v-radio
          :label="$t('aspZp.filter.all')"
          :value="0"
        />
      </v-radio-group>
    </template>
  </grid-wrapper>
</template>

<script>
import GridWrapper from "@/components/wrappers/grid";
export default {
  name: 'AspSubmittedZpList',
  components: { GridWrapper },
  props: {
    campaignId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      statusTypeFilter: 0, //  1 - Записан, 2 - Отписан, 3 - Отписан, 0 - Всички
      headers: [
        {
          text: this.$t('aspZp.headers.firstName'),
          value: 'firstName'
        },
        {
          text: this.$t('aspZp.headers.middleName'),
          value: 'middleName'
        },
        {
          text: this.$t('aspZp.headers.lastName'),
          value: 'lastName'
        },
        {
          text: this.$t('aspZp.headers.pin'),
          value: 'pin'
        },
        {
          text: this.$t('aspZp.headers.institutionCode'),
          value: 'institutionCode'
        },
        {
          text: this.$t('aspZp.headers.basicClass'),
          value: 'basicClass'
        },
        {
          text: this.$t('aspZp.headers.eduForm'),
          value: 'eduForm'
        },
        {
          text: this.$t('aspZp.headers.status'),
          value: 'status'
        }
      ],
      apiUrl: `/api/AbsenceCampaign/GetAspZpForCampaign/${this.campaignId}`
    };
  }
};
</script>
