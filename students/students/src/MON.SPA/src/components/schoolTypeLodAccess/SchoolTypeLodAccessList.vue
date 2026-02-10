<template>
  <v-card>
    <v-card-title>
      {{ $t("schoolTypeLodAccess.list") }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        ref="schoolTypeLodAccessTable"
        :headers="headers"
        :items="schoolTypeLodAccessData"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar
            flat
          >
            <v-toolbar-title>
              <GridExporter
                :items="schoolTypeLodAccessData"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="$t('schoolTypeLodAccess.list')"
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
        <template v-slot:[`item.detailedSchoolTypeName`]="{ item }">
          {{ item.detailedSchoolTypeName }}
        </template>
        <template v-slot:[`item.isLodAccessAllowed`]="{ item }">
          <v-checkbox
            v-model="item.isLodAccessAllowed"
            :disabled="true"
            hide-details
          />
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <button-tip
              v-if="hasUpdatePermission"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="loading"
              :to="`/administration/schoolTypeLodAccess/${item.detailedSchoolTypeId}/edit`"
            />
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>
  </v-card>
</template>

<script>
import { mapGetters } from 'vuex';
import GridExporter from "@/components/wrappers/gridExporter";
import { SchoolTypeLodAccessModel } from '@/models/admin/schoolTypeLodAccessModel.js';
import { Permissions } from '@/enums/enums';

export default {
  name: 'SchoolTypeLodAccessList',
  components: {
    GridExporter
  },
  data() {
    return {
      search: '',
      loading: false,
      schoolTypeLodAccessData: [],
      headers: [
        {
          text: this.$t('schoolTypeLodAccess.institutionTypeId'),
          value: 'detailedSchoolTypeId',
        },
        {
          text: this.$t('schoolTypeLodAccess.institutionType'),
          value: 'detailedSchoolTypeName',
        },
        {
          text: this.$t('schoolTypeLodAccess.lodAccessAllowed'),
          value: 'isLodAccessAllowed',
        },
        {
          text: this.$t('common.actions'),
          value: 'actions',
          align: 'end',
          sortable: false
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['permissions', 'gridItemsPerPageOptions']),
    hasUpdatePermission() {
      return this.permissions.includes(Permissions.PermissionNameForLodAccessConfigurationEdit);
    },
  },
  mounted() {
    this.loadData();
  },
  methods: {
    loadData() {
      this.loading = true;

      this.$api.schoolTypeLodAccess
        .getAll()
        .then((response) => {
          if(response.data) {
            this.schoolTypeLodAccessData = response.data.map(el => new SchoolTypeLodAccessModel(el));
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.schoolTypeLodAccessLoad'));
          console.log(error.response.data.message);
        })
        .then(() => { this.loading = false; });
    }
  }
};
</script>
