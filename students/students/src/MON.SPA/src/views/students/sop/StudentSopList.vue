<template>
  <v-card>
    <v-card-title>
      {{ `${$t("sop.sopTitle")}/${$t('sop.subtitle')}` }}
    </v-card-title>
    <v-card-subtitle>
      <institution-external-so-provider-checker
        class="mt-2"
      />
    </v-card-subtitle>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="items"
        :loading="loading"
        :search="search"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <v-toolbar-title>
              <GridExporter
                :items="items"
                :headers="headers"
                :file-extensions="['xlsx', 'csv', 'txt']"
                file-name="StudentSop"
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
        <template v-slot:[`item.sopDetails`]="{ item }">
          <v-chip
            v-for="(detail, index) in item.sopDetails"
            :key="index"
            outlined
            class="ma-1"
            color="primary"
            label
            small
          >
            {{ `${detail.sopTypeName}${detail.sopSubTypeName ? ' / ' + detail.sopSubTypeName : ''}` }}
          </v-chip>
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
              v-if="hasReadPermission"
              icon
              icon-name="mdi-eye"
              icon-color="primary"
              tooltip="buttons.review"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${personId}/sop/${item.id}/details`"
            />
            <button-tip
              v-if="hasManagePermission"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="saving || item.isLodFinalized"
              :lod-finalized="item.isLodFinalized"
              :to="`/student/${personId}/sop/${item.id}/edit`"
            />
            <button-tip
              v-if="hasManagePermission"
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
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/sop/create`"
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
import InstitutionExternalSoProviderChecker from '@/components/institution/InstitutionExternalSoProviderChecker';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentSopList',
  components: {
    GridExporter,
    DocDownloader,
    InstitutionExternalSoProviderChecker
  },
  props: {
    personId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      saving: false,
      search: '',
      loading: false,
      items: [],
      headers: [
        {text: this.$t('common.schoolYear'), value: "schoolYearName", sortable: true},
        {text: this.$t('common.type'), value: "sopDetails", sortable: true},
        {text: this.$t('recognition.headers.documents'), value: "id", sortable: false, filterable: false},
        {text: this.$t("common.actions"), value: 'actions', sortable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSopRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSopManage);
    },
    hasPersonalDevelopmentRaadPermission() {
      return this.hasStudentPermission( Permissions.PermissionNameForStudentPersonalDevelopmentRead);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSopRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
    this.load();
  },
  methods: {
    load() {
          this.loading = true;
          this.$api.studentSOP.getListForPerson(this.personId)
            .then((response) => {
              if(response.data) {
                this.items = response.data;
              }
            })
            .catch((error) => {
              this.$notifier.error('', this.$t('errors.studentResourceSupportLoad'));
              console.log(error.response);
            })
            .then(() => { this.loading = false; });
    },
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.studentSOP.delete(item.id)
          .then(() => {
            this.load();
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('errors.fileDelete'));
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    }
  }
};
</script>
