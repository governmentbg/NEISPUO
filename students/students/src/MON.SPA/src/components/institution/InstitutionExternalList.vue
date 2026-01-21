<template>
  <grid
    url="/api/institution/ExternalDetailsList"
    :headers="headers"
    :filter="{ institutionId: institutionId }"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    :title="$t('institution.externalSubtitle')"
    :file-export-name="$t('institution.externalSubtitle')"
    ref-key="institutionExternalList"
    :debounce="1000"
  >
    <template v-slot:[`item.pin`]="{ item }">
      {{ `${item.pin} - ${item.pinType}` }}
    </template>
    <template #actions="item">
      <button-group>
        <button-tip
          icon
          icon-name="mdi-eye"
          icon-color="primary"
          tooltip="buttons.details"
          bottom
          iclass=""
          small
          :to="`/student/${item.item.personId}/details`"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
  import Grid from "@/components/wrappers/grid.vue";

  export default {
    name: 'InstitutionExternal',
    components: { Grid },
    props:{
      institutionId: {
        type: Number,
        required: true
      }
    },
    data(){
      return {
        headers: [
          {
            text: this.$t('institutionExternalDetails.headers.name'),
            value: 'fullName'
          },
          {
            text: this.$t('institutionExternalDetails.headers.identifier'),
            value: 'pin'
          },
          {
            text: this.$t('institutionExternalDetails.headers.position'),
            value: 'positionName'
          },
          {
            text: this.$t('institutionExternalDetails.headers.institutionCode'),
            value: 'institutionId'
          },
          {
            text: this.$t('institutionExternalDetails.headers.institution'),
              value: 'institutionName'
            },
          {
            text: this.$t('institutionExternalDetails.headers.class'),
            value: 'className'
          },
          {
              text: this.$t('institutionExternalDetails.headers.institutionTown'),
              value: 'institutionTown'
          },
          { text: '', value: 'controls', sortable: false, filterable: false, align: 'end' },
        ]
      };
    },
    methods: {}
  };
</script>
