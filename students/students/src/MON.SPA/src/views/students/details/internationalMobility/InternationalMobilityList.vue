<template>
  <v-card>
    <v-card-title>
      {{ $t('lod.internationalMobility.listTitle') }}
    </v-card-title>
    <v-card-text class="pa-0">
      <v-data-table
        ref="internationalMobilityTable"
        :items="internationalMobilityData"
        :headers="headers"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="internationalMobilityData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('lod.internationalMobility.listTitle')"
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
        <template v-slot:[`item.fromDate`]="{ item }">
          {{ item.fromDate ? $moment(item.fromDate).format(dateFormat) : '' }}
        </template>

        <template v-slot:[`item.toDate`]="{ item }">
          {{ item.toDate ? $moment(item.toDate).format(dateFormat) : '' }}
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
                :to="`/student/${personId}/lod/internationalMobility/${item.id}/details`"
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
                :to="`/student/${personId}/lod/internationalMobility/${item.id}/edit`"
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
                @click="deleteItem(item)"
              />
            </template>
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/lod/internationalMobility/create`"
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
  name: 'InternationalMobilityList',
  components: {
    GridExporter,
    DocDownloader
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
      search:'',
      loading: false,
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
          text: this.$t('lod.internationalMobility.project'),
          value: 'project'
        },
        {
          text: this.$t('lod.internationalMobility.country'),
          value: 'country'
        },
        {
          text: this.$t('lod.internationalMobility.receivingInstitution'),
          value: 'receivingInstitution'
        },
        {
          text: this.$t('lod.internationalMobility.mainObjectives'),
          value: 'mainObjectives'
        },
        {
          text: this.$t('lod.internationalMobility.schoolYear'),
          value: 'schoolYearName'
        },
        {
          text: this.$t('lod.internationalMobility.fromDate'),
          value: 'fromDate'
        },
        {
          text: this.$t('lod.internationalMobility.toDate'),
          value: 'toDate'
        },
        {
          text: this.$tc('documents.title', 2),
          value: 'documents',
          sortable: false,
          filterable: false,
        },
        { text: '', value: 'actions', sortable: false, align: 'end' }
      ],
      internationalMobilityData:[],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalMobilityRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalMobilityManage);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalMobilityRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.studentInternationalMobility.getByPersonId(this.personId)
      .then(response => {
        if (response.data) {
          this.internationalMobilityData = response.data;
        }
      })
      .catch(error => {
          this.$notifier.error('', this.$t('errors.studentInternationalMobilityLoad'));
          console.log(error.response);
      })
      .then(() => {
          this.loading = false;
      });
    },
    async deleteItem(item){
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.studentInternationalMobility
        .delete(item.id)
        .then(() => {
          this.load();
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.internationalMobilityDelete'));
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
      }
    }
  }
};
</script>
