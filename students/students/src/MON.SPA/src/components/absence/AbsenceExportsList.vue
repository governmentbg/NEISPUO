<template>
  <grid
    v-if="hasReadPermission"
    :ref="'absenceExportsGrid' + _uid"
    url="/api/absence/listExportedFiles"
    :file-export-name="$t('absence.exportedFilesTitle')"
    :headers="headers"
    :title="$t('absence.exportedFilesTitle')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
  >
    <template v-slot:[`item.month`]="props">
      {{ $helper.getMonthName(props.item.month) }}
    </template>


    <!-- <template v-slot:[`item.isSigned`]="props">
      <v-chip
        :class="props.item.isSigned === true ? 'success': 'light'"
        small
      >
        <yes-no
          :value="props.item.isSigned"
        />
      </v-chip>
    </template> -->

    <!-- <template v-slot:[`item.isFinalized`]="props">
      <v-chip
        :class="props.item.isFinalized === true ? 'success': 'light'"
        small
      >
        <yes-no
          :value="props.item.isFinalized"
        />
      </v-chip>
    </template> -->

    <template #timestampUtc="props">
      {{ props.item.timestampUtc ? $moment.utc(props.item.timestampUtc).local().format(dateTimeFormat) : '' }}
    </template>

    <!-- <template v-slot:[`item.finalizedDate`]="props">
      {{ props.item.finalizedDate ? $moment.utc(props.item.finalizedDate).local().format(dateTimeFormat) : '' }}
    </template>

    <template v-slot:[`item.signedDate`]="props">
      {{ props.item.signedDate ? $moment.utc(props.item.signedDate).local().format(dateTimeFormat) : '' }}
    </template> -->

    <template #actions="item">
      <button-group>
        <!-- <signing-button
          v-if="hasManagePermission && !item.item.isSigned && !item.item.isFinalized"
          bottom
          small
          :disabled="saving"
          @click="onSignClick(item.item)"
        /> -->
        <doc-downloader
          v-if="item.item.blobId"
          :value="item.item"
          show-icon
          x-small
          :show-file-name="false"
          :disabled="saving"
        />
      </button-group>
    </template>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </grid>
</template>

<script>
import DocDownloader from '@/components/common/DocDownloader.vue';
import Constants from "@/common/constants.js";
import Grid from "@/components/wrappers/grid.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "AbsenceExportsList",
  components: {
      DocDownloader,
      Grid
  },
  data() {
      return {
        saving: false,
        dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
        headers: [
          {
            text: this.$t('absence.headers.schoolYear'), value: "schoolYearName", sortable: true
          },
          {
            text: this.$t('absence.headers.month'), value: "month", sortable: true
          },
          {
            text: this.$t('absence.headers.createDate'), value: "timestampUtc",  sortable: true
          },
          {
            text: this.$t('absence.headers.absencesCount'), value: "recordsCount", sortable: true
          },
          {
            text: this.$t('absence.headers.zpCount'), value: "zpCount", sortable: true
          },
          // {
          //   text: this.$t('absence.headers.isSigned'),
          //   value: "isSigned",
          // },
          // {
          //   text: this.$t('absence.headers.signedDate'),
          //   value: "signedDate",
          // },
          // {
          //   text: this.$t('absence.headers.isFinalized'),
          //   value: "isFinalized",
          // },
          // {
          //   text: this.$t('absence.headers.finalizedDate'),
          //   value: "finalizedDate",
          // },
          { text: '', value: 'controls', filterable: false, sortable: false, align: 'end' }
        ],
      };
  },
  computed: {
      ...mapGetters(['hasPermission', 'userDetails']),
      hasReadPermission() {
        return this.hasPermission(Permissions.PermissionNameForAbsencesExportRead);
      },
      hasManagePermission() {
        return this.hasPermission(Permissions.PermissionNameForAbsencesExportManage);
      }
  },
  mounted() {
    this.$studentHub.$on('absence-campaign-modified', this.onAbsenceCampaignModified);
  },
  destroyed() {
    this.$studentHub.$off('absence-campaign-modified');
  },
  methods: {
    gridReload() {
      const grid = this.$refs['absenceExportsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async onSignClick(absenceExport) {
      this.saving = true;

      const xml = (await this.$api.absence.constructAbsenceExportAsXml(absenceExport)).data;
      if(!xml) {
        this.saving = false;
        this.$helper.logError({ action: 'AbsenceExportSign', message: 'Empty xml model'}, absenceExport, this.userDetails);
        return this.$notifier.error(this.$t('common.sign'), 'Empty xml model', 5000);
      }

      try {
        const signingResponse = await this.$api.certificate.signXml(xml);

        if (signingResponse && signingResponse.isError == false && signingResponse.contents) {
          let signingAttributes = {
            absenceExportId: absenceExport.id,
            signature: this.$helper.utf8ToBase64(signingResponse.contents)
          };

          try {
            await this.$api.absence.setAbsenceExportSigningAtrributes(signingAttributes);
          } catch (error) {
            // Ако не успее да записи signature (дава странна CORS грешка)
            // ще маркирам импорта като подписан но без signature.
            signingAttributes.signature = null;
            await this.$api.absence.setAbsenceExportSigningAtrributes(signingAttributes);
          }

          this.$notifier.success('', this.$t('common.signSuccess'));
        } else {
          this.$helper.logError({ action: 'AbsenceExportSign', message: signingResponse}, absenceExport, { userDetails: this.userDetails, xml: xml });
          console.error(signingResponse);
          this.$notifier.error(this.$t('common.sign'), signingResponse?.message ?? this.$t('common.signError'), 5000);
        }

      } catch (error) {
        this.$helper.logError({ action: 'AbsenceExportSign', message: error}, absenceExport, { userDetails: this.userDetails, xml: xml });
        console.error(error);
        this.$notifier.error(this.$t('common.sign'), this.$t('common.signError'), 5000);
      }

      this.saving = false;
      this.gridReload();
    },
    // eslint-disable-next-line no-unused-vars
    onAbsenceCampaignModified(id) {
      this.gridReload();
    }
  }
};
</script>
