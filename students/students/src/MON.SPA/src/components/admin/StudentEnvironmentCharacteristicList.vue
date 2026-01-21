<template>
  <grid
    url="/api/admin/GetStudentEnvironmentCharacteristics"
    :headers="headers"
    title=""
    :file-export-name="$t('environmentCharacteristics.statList.title')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    item-key="uid"
  >
    <template v-slot:[`item.pin`]="{ item }">
      {{ `${item.pin} - ${item.pinType}` }}
    </template>
    <template v-slot:[`item.hasParentConsent`]="{ item }">
      <v-chip
        color="light"
        small
      >
        {{ item.hasParentConsent | yesNo }}
      </v-chip>
    </template>
    <template #actions="item">
      <button-group>
        <button-tip
          :to="`/student/${item.item.personId}/environmentCharacteristics`"
          icon
          icon-color="primary"
          iclass=""
          icon-name="mdi-eye"
          small
          tooltip="student.menu.environmentCharacteristics"
          bottom
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentEnvironmentCharacteristicListComponent',
  components: {
    Grid
  },
  data() {
    return {};
  },
  computed: {
    ...mapGetters(['userInstitutionId']),
    headers() {
      if (this.userInstitutionId) {
        return [
          {
            text: this.$t('environmentCharacteristics.statList.headers.name'),
            value: 'fullName',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.identifier'),
            value: 'pin',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.class'),
            value: 'className',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.relativeName'),
            value: 'relativeFullName',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.relativeType'),
            value: 'relativeType',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.workStatus'),
            value: 'workStatus',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.education'),
            value: 'educationType',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.hasParentConsent'),
            value: 'hasParentConsent',
            type: 'boolean'
          },
          { text: '', value: 'controls', sortable: false, align: 'end' },
        ];
      } else {
        return [
          {
            text: this.$t('environmentCharacteristics.statList.headers.name'),
            value: 'fullName',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.identifier'),
            value: 'pin',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.class'),
            value: 'className',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.relativeName'),
            value: 'relativeFullName',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.relativeType'),
            value: 'relativeType',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.workStatus'),
            value: 'workStatus',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.education'),
            value: 'educationType',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.hasParentConsent'),
            value: 'hasParentConsent',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.institutionCode'),
            value: 'institutionId',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.institution'),
            value: 'institutionName',
          },
          {
            text: this.$t('environmentCharacteristics.statList.headers.region'),
            value: 'regionName',
          },
          { text: '', value: 'controls', sortable: false, align: 'end' }
        ];
      }
    }
  }
};
</script>
