<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'absenceImportsGrid' + _uid"
      url="/api/absence/listImportedFiles"
      file-export-name="Списък с импортирани файлове"
      :headers="headers"
      :title="$t('absence.listTitle')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ institutionId: institutionId }"
      multi-sort
    >
      <template #timestampUtc="item">
        {{ item.item.timestampUtc ? $moment.utc(item.item.timestampUtc).local().format(dateFormat) : '' }}
      </template>

      <template v-slot:[`item.isSigned`]="props">
        <v-chip
          :class="props.item.isSigned === true ? 'success': 'light'"
          small
        >
          <yes-no
            :value="props.item.isSigned"
          />
        </v-chip>
      </template>

      <template v-slot:[`item.signedDate`]="props">
        {{ props.item.signedDate ? $moment.utc(props.item.signedDate).local().format(dateFormat) : '' }}
      </template>

      <template v-slot:[`item.createDate`]="props">
        {{ props.item.createDate ? $moment.utc(props.item.createDate).local().format(dateFormat) : '' }}
      </template>
      <template #actions="item">
        <button-group>
          <button-tip
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :to="`/absence/import/${item.item.id}/details`"
            :disabled="saving"
          />
          <signing-button
            v-if="hasManagePermission && !item.item.isSigned && !item.item.isFinalized"
            bottom
            small
            :disabled="saving"
            @click="onSignClick(item.item)"
          />
          <button-tip
            v-if="hasManagePermission && !item.item.isFinalized && item.item.hasActiveCampaign"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="deleteAbsenceImport(item.item.id)"
          />
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
    </grid>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import DocDownloader from '@/components/common/DocDownloader.vue';
import Constants from "@/common/constants.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "AbsenceImportsList",
  components: { DocDownloader, Grid },
  props: {
    institutionId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATE_AND_TIME_FORMAT,
      headers: [
        {
          text: this.$t('absence.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('absence.headers.month'),
          value: "monthName",
        },
        // {
        //   text: this.$t('absence.headers.timestamp'),
        //   value: "timestampUtc",
        // },
        {
          text: this.$t('absence.headers.createDate'),
          value: "createDate",
        },
        {
          text: this.$t('absence.headers.recordsCount'),
          value: "recordsCount",
        },
        {
          text: this.$t('absence.headers.isSigned'),
          value: "isSigned",
        },
        {
          text: this.$t('absence.headers.signedDate'),
          value: "signedDate",
        },
        {
          text: this.$t('absence.headers.importType'),
          value: "importType",
          filterable: false,
          sortable: false
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userDetails']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceManage);
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
      const grid = this.$refs['absenceImportsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async onSignClick(absenceImport) {
      this.saving = true;

      const xml = (await this.$api.absence.constructAbsenceImportAsXml(absenceImport)).data;
      if(!xml) {
        this.saving = false;
        this.$helper.logError({ action: 'AbsenceImportSign', message: 'Empty xml model'}, absenceImport, this.userDetails);
        return this.$notifier.error(this.$t('common.sign'), 'Empty xml model', 5000);
      }

      try {
        const signingResponse = await this.$api.certificate.signXml(xml);

        if (signingResponse && signingResponse.isError == false && signingResponse.contents) {
          let signingAttributes = {
            absenceImportId: absenceImport.id,
            signature: btoa(signingResponse.contents)
          };

          try {
            await this.$api.absence.setAbsenceImportSigningAtrributes(signingAttributes);
          } catch (error) {
            // Ако не успее да записи signature (дава странна CORS грешка)
            // ще маркирам импорта като подписан но без signature.
            signingAttributes.signature = null;
            await this.$api.absence.setAbsenceImportSigningAtrributes(signingAttributes);
          }

          this.$notifier.success('', this.$t('common.signSuccess'));
        } else {
          this.$helper.logError({ action: 'AbsenceImportSign', message: signingResponse}, absenceImport, { userDetails: this.userDetails, xml: xml });
          console.error(signingResponse);
          this.$notifier.error(this.$t('common.sign'), signingResponse?.message ?? this.$t('common.signError'), 5000);
        }

      } catch (error) {
        this.$helper.logError({ action: 'AbsenceImportSign', message: error}, absenceImport, { userDetails: this.userDetails, xml: xml });
        console.error(error);
        this.$notifier.error(this.$t('common.sign'), this.$t('common.signError'), 5000);
      }

      this.saving = false;
      this.gridReload();
    },
    async deleteAbsenceImport(id) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.absence.deleteAbsenceImport(id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.gridReload();
            this.$studentEventBus.$emit('absenceImportDelete');
          })
          .catch(error => {
            this.$notifier.error('', this.$t('common.deleteError'), 5000);
            console.error(error.response);
          })
          .finally(() => { this.saving = false; });
      }
    },
    // eslint-disable-next-line no-unused-vars
    onAbsenceCampaignModified(id) {
      this.gridReload();
    }
  },
};
</script>
