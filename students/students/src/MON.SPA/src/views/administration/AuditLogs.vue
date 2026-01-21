<template>
  <grid
    url="/api/administration/GetAuditLogs"
    file-export-name="AuditLogs"
    :headers="headers"
    :title="$t('auditLogs.title')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
  >
    <template #timestampUtc="item">
      <span>{{ item && item.item.timestampUtc ? $moment.utc(item.item.timestampUtc).local().format(dateTimeFormat) : '' }}</span>
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
          @click="showDetails(item)"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";

export default {
  name: 'AuditLogs',
  components: {
    Grid
  },
  data() {
    return {
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      headers: [
        {text: this.$t('auditLogs.headers.username'), value: "username", sortable: true},
        {text: this.$t('auditLogs.headers.entitySetName'), value: "entitySetName", sortable: true},
        {text: this.$t('auditLogs.headers.entityTypeName'), value: "entityTypeName", sortable: true},
        {text: this.$t('auditLogs.headers.state'), value: "state", sortable: true},
        {text: this.$t('auditLogs.headers.ip'), value: "ip", sortable: true},
        {text: this.$t('auditLogs.headers.timestampUtc'), value: "timestampUtc", sortable: true},
        {text: '', value: 'controls', inFavourOfIdentifier: false, align: 'end'},
      ]
    };
  },
  methods: {
    showDetails(item) {
      const data = item.item.jsonData ? JSON.stringify(JSON.parse(item.item.jsonData), undefined, 2) : '';
      const options = { showTextInPreTag: true, showSubtitle: false };
      this.$notifier.modal(this.$t('buttons.details'), data, "info", options);
    }
  }
};
</script>
