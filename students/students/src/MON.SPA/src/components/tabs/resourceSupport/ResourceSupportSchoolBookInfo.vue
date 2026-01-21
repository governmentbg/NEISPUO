<template>
  <div>
    <v-alert
      v-if="$t('additionalPersonalDevelopment.schoolBookData.info').length > 0"
      border="top"
      colored-border
      type="info"
      elevation="2"
    >
      {{ $t('additionalPersonalDevelopment.schoolBookData.info') }}
    </v-alert>

    <grid
      :headers="headers"
      url="/api/AdditionalPersonalDevelopmentSupport/ListSchoolBookData"
      :ref-key="refKey"
      title=""
      :filter="{
        personId: personId,
        schoolYear: schoolYear,
      }"
    >
      <template v-slot:[`item.institutionId`]="{ item }">
        {{ `${item.institutionId} - ${item.institutionName}` }}
      </template>
      <template v-slot:[`item.date`]="{ item }">
        {{ item.date ? $moment(item.date).format(dateFormat) : '' }}
      </template>
    </grid>
  </div>
</template>


<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from '@/common/constants.js';

export default {
  name: 'ResourceSupportSchooBookInfoComp',
  components: { Grid },
  props: {
    personId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
            text: this.$t('additionalPersonalDevelopment.schoolBookData.headers.institutionCode'),
            value: 'institutionId'
        },
        {
            text: this.$t('additionalPersonalDevelopment.schoolBookData.headers.institutionName'),
            value: 'institutionName'
        },
        {
            text: this.$t('additionalPersonalDevelopment.schoolBookData.headers.position'),
            value: 'positionName'
        },
        {
            text: this.$t('additionalPersonalDevelopment.schoolBookData.headers.subjectName'),
            value: 'subjectName'
        },
        {
            text: this.$t('additionalPersonalDevelopment.schoolBookData.headers.subjectTypeName'),
            value: 'subjectTypeName'
        },
        {
            text: this.$t('additionalPersonalDevelopment.schoolBookData.headers.hourNumber'),
            value: 'hourNumber'
        },
      ]
    };
  }
};
</script>
