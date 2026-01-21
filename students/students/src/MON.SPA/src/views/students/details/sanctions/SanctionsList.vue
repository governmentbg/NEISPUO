<template>
  <div>
    <grid
      :ref="'sanctionsListGrid' + _uid"
      url="/api/studentSanctions/list"
      :file-export-name="$t('lod.sanctions.studentSanctionsTitle')"
      :headers="headers"
      :title="$t('lod.sanctions.studentSanctionsTitle')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ personId: personId }"
    >
      <template #topAppend>
        <v-row
          dense
          class="mr-2"
        >
          <v-spacer />
          <button-tip
            v-if="hasManagePermission"
            small
            icon-name="fas fa-file-upload"
            color="primary"
            iclass="mx-2"
            bclass="mr-1"
            tooltip="lod.sanctions.sanctionsImport"
            bottom
            fab
            @click="onSanctionsImportClick()"
          />
        </v-row>
      </template>

      <template v-slot:[`item.orderDate`]="{ item }">
        {{ item.orderDate ? $moment(item.orderDate).format(dateFormat) : '' }}
      </template>
      <template v-slot:[`item.startDate`]="{ item }">
        {{ item.startDate ? $moment(item.startDate).format(dateFormat) : '' }}
      </template>
      <template v-slot:[`item.endDate`]="{ item }">
        {{ item.endDate ? $moment(item.endDate).format(dateFormat) : '' }}
      </template>
      <template v-slot:[`item.documents`]="{ item }">
        <doc-downloader
          v-for="doc in item.documents"
          :key="doc.id"
          :value="doc"
          small
        />
      </template>

      <template
        v-if="!hideActionButtons"
        v-slot:[`item.controls`]="{ item }"
      >
        <button-group>
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :disabled="saving"
            :to="`/student/${personId}/lod/sanction/${item.id}/details`"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            iclass=""
            small
            bottom
            :to="`/student/${personId}/lod/sanction/${item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            iclass=""
            small
            bottom
            @click="deleteSanction(item)"
          />
        </button-group>
      </template>

      <template
        v-if="!hideActionButtons"
        #footerPrepend
      >
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          :to="`/student/${personId}/lod/sanction/create`"
        >
          {{ $t('buttons.newRecord') }}
        </v-btn>
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
  name: 'SanctionsList',
  components: {
    Grid,
    DocDownloader
  },
  props: {
    personId: {
      type: Number,
      default() {
        return undefined;
      }
    },
    hideActionButtons: {
      type: Boolean,
      default() {
        return false;
      }
    },
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
          text: this.$t('lod.sanctions.orderNumber'),
          value: 'orderNumber'
        },
        {
          text: this.$t('lod.sanctions.orderDate'),
          value: 'orderDate'
        },
        {
          text: this.$t('lod.sanctions.schoolYear'),
          value: 'schoolYearName'
        },
        {
          text: this.$t('lod.sanctions.description'),
          value: 'description'
        },
        {
          text: this.$t('lod.sanctions.institutionCode'),
          value: 'institutionId'
        },
        {
          text: this.$t('lod.sanctions.institution'),
          value: 'institutionName'
        },
        {
          text: this.$t('lod.sanctions.startDate'),
          value: 'startDate'
        },
        {
          text: this.$t('lod.sanctions.endDate'),
          value: 'endDate'
        },
        {
          text: this.$tc('documents.title', 2),
          value: 'documents',
          sortable: false,
          filterable: false,
        },
        { text: '', value: 'controls', filterable: false, sortable: false, align: 'end' }
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSanctionRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSanctionManage);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSanctionRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    refresh() {
      const grid = this.$refs['sanctionsListGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
    async onSanctionsImportClick(){
      if(await this.$refs.confirm.open(this.$t('lod.sanctions.sanctionsImport'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.studentSanctions.import(this.personId)
        .then(() => {
          this.$notifier.success('', this.$t('lod.sanctions.sanctionsImportSuccess'));
          this.refresh();
        })
        .catch(error => {
            this.$notifier.error('', this.$t('errors.sanctionsImport'));
            console.log(error);
        })
        .then(() => {
            this.saving = false;
        });
      }
    },
    async deleteSanction(item){
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.studentSanctions.delete(item.id)
        .then(() => {
          this.$notifier.success('', this.$t('common.deleteSuccess'));
          this.refresh();
        })
        .catch(error => {
          this.$notifier.error('', this.$t('common.deleteError'));
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
      }
    },
  }
};
</script>
