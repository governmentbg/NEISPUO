<template>
  <v-card>
    <v-card-title>
      {{ this.$t('resourceSupport.resourceSupportTitle') }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        ref="sanctionsListTable"
        :items="resourceSupportData"
        :headers="headers"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="resourceSupportData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('resourceSupport.resourceSupportTitle')"
              :headers="headers"
            />
            <v-spacer />
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              :label="$t('common.search')"
              single-line
              hide-details
            />
          </v-toolbar>
        </template>
        <template v-slot:[`item.reportDate`]="{ item }">
          {{ item.reportDate ? $moment(item.reportDate).format(dateFormat) : '' }}
        </template>
        <template v-slot:[`item.documents`]="{ item }">
          <doc-downloader
            v-for="doc in item.documents"
            :key="doc.id"
            :value="doc"
            small
          />
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <template>
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
                :to="`/student/${personId}/resourceSupport/${item.id}/details`"
              />
              <button-tip
                v-if="hasManagePermission && !item.relatedAdditionalPersonalDevelopmentSupportId"
                icon
                icon-name="mdi-pencil"
                icon-color="primary"
                tooltip="buttons.edit"
                iclass=""
                small
                bottom
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                :to="`/student/${personId}/resourceSupport/${item.id}/edit`"
              />
              <button-tip
                v-if="hasPersonalDevelopmentManagePermission && item.relatedAdditionalPersonalDevelopmentSupportId"
                icon
                icon-name="mdi-pencil"
                icon-color="primary"
                tooltip="additionalPersonalDevelopment.editRelatedAdditionalPersonalDevelopment"
                iclass=""
                small
                bottom
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                :to="`/student/${personId}/additionalPersonalDevelopment/${item.relatedAdditionalPersonalDevelopmentSupportId}/edit`"
              />
              <button-tip
                v-if="hasManagePermission && !item.relatedAdditionalPersonalDevelopmentSupportId"
                icon
                icon-name="mdi-delete"
                icon-color="error"
                tooltip="buttons.delete"
                iclass=""
                small
                bottom
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                @click="deleteResourceSupport(item)"
              />
              <button-tip
                v-if="hasPersonalDevelopmentRaadPermission && item.relatedAdditionalPersonalDevelopmentSupportId"
                icon
                icon-name="mdi-relation-one-to-one"
                icon-color="primary"
                tooltip="additionalPersonalDevelopment.relatedAdditionalPersonalDevelopment"
                bottom
                iclass=""
                small
                :disabled="saving"
                :to="`/student/${personId}/additionalPersonalDevelopment/${item.relatedAdditionalPersonalDevelopmentSupportId}/details`"
              />
            </template>
          </button-group>
        </template>

        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="!personalDevelopmentSuppert_v2 && hasManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/resourceSupport/create`"
            >
              {{ $t('buttons.newRecord') }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click="load"
            >
              {{ $t('buttons.reload') }}
            </v-btn>
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import DocDownloader from '@/components/common/DocDownloader.vue';
import Constants from "@/common/constants.js";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'ResourceSupportList',
  components: {
    GridExporter,
    DocDownloader,
  },
  props: {
    personId: {
      type: Number,
      default() {
        return undefined;
      }
    },
  },
  data() {
    return {
      loading: false,
      search:'',
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
          text: this.$t('resourceSupport.history.schoolYear'),
          value: 'schoolYearName'
        },
        {
          text: this.$t('resourceSupport.reportNumber'),
          value: 'reportNumber'
        },
        {
          text: this.$t('resourceSupport.reportDate'),
          value: 'reportDate'
        },
        // {
        //   text: this.$t('resourceSupport.resourceSupportTypeName'),
        //   value: 'resourceSupportTypeName'
        // },
        {
          text: this.$tc('documents.title', 2),
          value: 'documents',
          sortable: false,
          filterable: false,
        },
        { text: '', value: 'actions', sortable: false, align: 'end' }
      ],
      resourceSupportData:[],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions', 'personalDevelopmentSuppert_v2']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportManage);
    },
    hasPersonalDevelopmentRaadPermission() {
      return this.hasStudentPermission( Permissions.PermissionNameForStudentPersonalDevelopmentRead);
    },
    hasPersonalDevelopmentManagePermission() {
      return this.hasStudentPermission( Permissions.PermissionNameForStudentPersonalDevelopmentManage);
    },
  },
  created() {
    this.$studentHub.$on('resource-support-modified', this.load);
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  destroyed() {
    this.$studentHub.$off('resource-support-modified');
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.resourceSupport.getByPersonId(this.personId)
      .then(response => {
        if (response.data) {
          this.resourceSupportData = response.data;
        }
      })
      .catch(error => {
          this.$notifier.error('', this.$t('errors.studentSanctionsLoad'));
          console.log(error.response);
      })
      .then(() => {
          this.loading = false;
      });
    },
    async deleteResourceSupport(item){
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.resourceSupport.delete(item.id)
        .then(() => {
          this.$notifier.success('', this.$t('common.deleteSuccess'));
          this.load();
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
