<template>
  <v-card>
    <v-card-title>
      {{ this.$t('lod.awards.studentAwardsTitle') }}
    </v-card-title>

    <v-card-text>
      <v-data-table
        :items="awardsData"
        :headers="headers"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="awardsData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('lod.awards.studentAwardsTitle')"
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
        <template v-slot:[`item.date`]="{ item }">
          {{ item.date ? $moment(item.date).format(dateFormat) : '' }}
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
                :to="`/student/${personId}/lod/award/${item.id}/details`"
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
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                :to="`/student/${personId}/lod/award/${item.id}/edit`"
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
                :disabled="item.isLodFinalized"
                :lod-finalized="item.isLodFinalized"
                @click="deleteAward(item)"
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
              :to="`/student/${personId}/lod/award/create`"
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
  name: 'AwardsList',
  components: {
    GridExporter,
    DocDownloader
  },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
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
          text: this.$t('common.schoolYear'),
          value: 'schoolYearName'
        },
        {
          text: this.$t('lod.awards.date'),
          value: 'date'
        },
        {
          text: this.$t('lod.awards.awardType'),
          value: 'awardTypeName'
        },
        {
          text: this.$t('lod.awards.awardCategory'),
          value: 'awardCategoryName'
        },
        {
          text: this.$t('lod.awards.additionalInformation'),
          value: 'additionalInformation'
        },
        {
          text: this.$t('lod.awards.institution'),
          value: 'institutionName'
        },
        {
          text: this.$t('lod.awards.founder'),
          value: 'founderName'
        },
        {
          text: this.$t('lod.awards.awardReason'),
          value: 'awardReasonName'
        },
        {
          text: this.$t('lod.awards.orderNumber'),
          value: 'orderNumber'
        },
        {
          text: this.$t('lod.awards.description'),
          value: 'description'
        },
        {
          text: this.$tc('documents.title', 2),
          value: 'documents',
          sortable: false,
          filterable: false,
        },
        { text: '', value: 'actions', sortable: false, align: 'end' }
      ],
      awardsData:[],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAwardRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAwardManage);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentAwardRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.studentAwards.getByPersonId(this.personId)
      .then(response => {
        if (response.data) {
          this.awardsData = response.data;
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
    async deleteAward(item){
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.studentAwards.delete(item.id)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.load();
        })
        .catch(error => {
            this.$notifier.error('', this.$t('errors.studentSanctionsDelete'));
            console.log(error.response);
        })
        .then(() => { this.saving = false; });
      }
    },
  }
};
</script>
