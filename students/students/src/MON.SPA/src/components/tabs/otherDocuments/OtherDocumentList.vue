<template>
  <v-card>
    <v-card-title>
      {{ $t("otherDocuments.title") }}
    </v-card-title>
    <v-card-text>
      <v-alert
        outlined
        type="warning"
        prominent
        dense
      >
        <h4>{{ $t('otherDocuments.subtitle') }}</h4>
      </v-alert>

      <v-data-table
        ref="otherDocumentListTable"
        :headers="headers"
        :items="otherDocuments"
        :loading="loading"
        :search="search"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <v-toolbar-title>
              <GridExporter
                :items="otherDocuments"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="$t('otherDocuments.title')"
                :headers="headers"
              />
            </v-toolbar-title>
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
        <template v-slot:[`item.id`]="{ item }">
          <doc-downloader
            v-for="document in item.documents"
            :key="document.id"
            :value="document"
            small
          />
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <button-tip
              v-if="hasReadPermission || hasManagePermission(item)"
              icon
              icon-name="mdi-eye"
              icon-color="primary"
              tooltip="buttons.review"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${personId}/otherDocument/${item.id}/details`"
            />
            <button-tip
              v-if="hasManagePermission(item)"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="saving || item.isLodFinalized"
              :lod-finalized="item.isLodFinalized"
              :to="`/student/${personId}/otherDocument/${item.id}/edit`"
            />
            <button-tip
              v-if="hasManagePermission(item)"
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              :disabled="saving || item.isLodFinalized"
              :lod-finalized="item.isLodFinalized"
              @click="deleteItem(item)"
            />
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/otherDocument/create`"
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
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import DocDownloader from '@/components/common/DocDownloader.vue';
import GridExporter from "@/components/wrappers/gridExporter";
import { StudentOtherDocumentModel } from "@/models/studentOtherDocumentModel.js";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
    name: "OtherDocumentsList",
      components: {
        GridExporter,
        DocDownloader
     },
    props: {
        personId: {
            type: Number,
            required: true,
        },
    },
    data() {
        return {
            search: '',
            loading: false,
            saving: false,
            otherDocuments: [],
            headers: [
                {
                    text: this.$t("otherDocuments.schoolYear"),
                    value: "schoolYearName",
                },
                {
                    text: this.$t("otherDocuments.institution"),
                    value: "institutionName",
                },
                {
                    text: this.$t("otherDocuments.documentType"),
                    value: "documentTypeName",
                },
                {
                    text: this.$t("otherDocuments.regNumberTotal"),
                    value: "regNumberTotal",
                },
                {
                    text: this.$t("otherDocuments.regNumber"),
                    value: "regNumber",
                },
                {
                    text: this.$t("otherDocuments.issueDate"),
                    value: "issueDate",
                },
                {
                  text: this.$t("otherDocuments.attachedDocs"),
                  value: "id",
                  sortable: false,
                  filterable: false,
                },
                {
                  text: this.$t("common.actions"),
                  value: "actions",
                  sortable: false,
                },
            ],
        };
    },
    computed: {
      ...mapGetters(['gridItemsPerPageOptions', 'hasStudentPermission', 'hasPermission', 'userInstitutionId']),
      hasReadPermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentOtherDocumentRead);
      },
    },
    mounted() {
      this.load();
    },
    methods: {
      load() {
        this.loading = true;
        this.$api.otherDocument
          .getListForPerson(this.personId)
          .then((response) => {
            if(response.data) {
              this.otherDocuments = response.data.map((el) => new StudentOtherDocumentModel(el, this.$moment));
            }
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('common.loadError'));
            console.log(error.response);
          })
          .then(() => { this.loading = false; });
      },
      async deleteItem(item) {
        if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
          this.saving = true;

          this.$api.otherDocument.delete(item.id)
            .then(() => {
              this.load();
              this.$notifier.success('', this.$t('common.deleteSuccess'));
            })
            .catch((error) => {
              this.$notifier.error('', this.$t('common.deleteError'));
              console.log(error.response);
            })
            .finally(() => { this.saving = false; });
        }
      },
      hasManagePermission(item) {
        const hasStudentManagePermission = this.hasStudentPermission(Permissions.PermissionNameForStudentOtherDocumentManage);
        if (hasStudentManagePermission) return hasStudentManagePermission;

        return item && this.userInstitutionId && this.hasPermission(Permissions.PermissionNameForStudentOtherDocumentManage)
          && this.userInstitutionId == item.institutionId;
      },
    },
};
</script>

