<template>
  <v-card>
    <v-card-title>
      {{ $t('lod.selfGovernment.selfGovernmentTitle') }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        ref="selfGovermentTable"
        :items="selfGovernmentData"
        :headers="headers"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="selfGovernmentData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('lod.selfGovernment.selfGovernmentTitle')"
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
              :to="`/student/${personId}/lod/selfGovernment/${item.id}/details`"
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
              :to="`/student/${personId}/lod/selfGovernment/${item.id}/edit`"
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
              :to="`/student/${personId}/lod/selfGovernment/create`"
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
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'SelfGovernmentList',
  components: {
    GridExporter
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
      headers: [
        {
            text: this.$t('common.schoolYear'),
            value: 'schoolYearName'
        },
        {
            text: this.$t('lod.selfGovernment.institution'),
            value: 'institution'
        },
        {
            text: this.$t('lod.selfGovernment.participation'),
            value: 'participation'
        },
        {
            text: this.$t('lod.selfGovernment.participationAdditionalInformation'),
            value: 'participationAdditionalInformation'
        },
        {
            text: this.$t('lod.selfGovernment.position'),
            value: 'position'
        },
        {
            text: this.$t('lod.selfGovernment.mobilePhone'),
            value: 'mobilePhone'
        },
        {
            text: this.$t('lod.selfGovernment.email'),
            value: 'email'
        },
        {
            text: this.$t('lod.selfGovernment.additionalInformation'),
            value: 'additionalInformation'
        },
        { text: '', value: 'actions', sortable: false, align: 'end' }
      ],
      selfGovernmentData:[],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSelfGovernmentRead);
    },
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentSelfGovernmentManage);
    },
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSelfGovernmentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.selfGovernment.getByPersonId(this.personId)
      .then(response => {
        if (response.data) {
          this.selfGovernmentData = response.data;
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studenSelfGovernmentLoad'));
        console.log(error.response);
      })
      .then(() => {
        this.loading = false;
      });
    },
    async deleteItem(item){
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.selfGovernment.delete(item.id)
        .then(() => {
          this.load();
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        })
        .catch(error => {
            this.$notifier.error('', this.$t('errors.selfGovernmentDelete'));
            console.log(error.response);
        })
        .then(() => { this.saving = false; });
      }
    }
  }
};
</script>
