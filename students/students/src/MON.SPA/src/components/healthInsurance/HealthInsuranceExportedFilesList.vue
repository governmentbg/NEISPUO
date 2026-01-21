<template>
  <v-card>
    <v-card-text>
      <grid
        :ref="'exportedFilesGrid' + _uid"
        url="/api/healthInsurance/exportsList"
        :title="$t('healthInsurance.exportedFilesList.title')"
        :headers="headers"
        :file-exporter-extensions="['xlsx', 'csv', 'txt']"
        class="elevation-1"
      >
        <template v-slot:[`item.createDate`]="{ item }">
          {{ item.createDate ? $moment.utc(item.createDate).local().format(dateTimeFormat) : '' }}
        </template>

        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <v-tooltip bottom>
              <template v-slot:activator="{ on: tooltip }">
                <doc-downloader
                  v-if="item.blobId"
                  :value="item"
                  show-icon
                  x-small
                  :show-file-name="false"
                  v-on="{ ...tooltip }"
                />
              </template>
              <span>{{ $t('buttons.download') }}</span>
            </v-tooltip>
            <button-tip
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              @click="deleteFile(item.id)"
            />
          </button-group>
        </template>
      </grid>
    </v-card-text>
    <confirm-dlg ref="confirm" />
    <v-overlay
      :value="saving"
    >
      <div class="text-center">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </div>
    </v-overlay>
  </v-card>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import DocDownloader from '@/components/common/DocDownloader.vue';
import Constants from "@/common/constants.js";

export default {
  name: "HealthInsuranceExportedFilesListComponent",
  components: {
    Grid,
    DocDownloader
  },
  data() {
    return {
      saving: false,
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      headers:[
        { text: this.$t("healthInsurance.exportedFilesList.headers.year"), value: "year" },
        { text: this.$t("healthInsurance.exportedFilesList.headers.month"), value: "monthName" },
        { text: this.$t("healthInsurance.exportedFilesList.headers.recordsCount"), value: "recordsCount" },
        { text: this.$t("healthInsurance.exportedFilesList.headers.creator"), value: "creatorUsername" },
        { text: this.$t("healthInsurance.exportedFilesList.headers.createDate"), value: "createDate" },
        { text: '', value: "actions", sortable: false, filterable: false, align: 'end' },
      ]
    };
  },
  methods: {
    async deleteFile(id){
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        await this.$api.healthInsurance.delete(id).then(() => {
          this.gridReload();
          this.$notifier.success(this.$t('buttons.delete'), this.$t('common.deleteSuccess'));
        })
        .catch((error) => {
          this.$notifier.error(this.$t('buttons.delete'), this.$t('common.deleteError'));
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
      }
    },
    gridReload() {
      const grid = this.$refs['exportedFilesGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    }
  }
};
</script>
