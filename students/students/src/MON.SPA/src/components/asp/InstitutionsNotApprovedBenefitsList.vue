<template>
  <v-card>
    <v-card-text>
      <grid
        :ref="'inReviewGrid' + _uid"
        url="/api/asp/GetInstitutionsStillReviewingBenefits"
        :file-export-name="$t('asp.institutionNotApprovedBenefits')"
        :headers="headers"
        :title="$t('asp.institutionNotApprovedBenefits')"
        :search="search"
        :file-exporter-extensions="['xlsx', 'csv', 'txt']"
        :filter="{ statusFilter: statusFilter, schoolYear: year, month: month }"
      >
        <template #subtitle>
          <v-row>
            <v-radio-group
              v-model="statusFilter"
              row
            >
              <v-radio
                label="Незапочнали"
                :value="0"
              />
              <v-radio
                label="Незавършили"
                :value="1"
              />
              <v-radio
                label="Завършили, но неподписани"
                :value="2"
              />
              <v-radio
                label="Подписани"
                :value="3"
              />
              <v-radio
                label="Всички"
                :value="999"
              />
            </v-radio-group>
          </v-row>
        </template>

        <template #topAppend>
          <v-toolbar
            flat
            class="d-flex justify-end mb-6"
          >
            <v-btn
              v-if="hasAspImportPermission"
              raised
              color="primary"
              class="mb-3 ml-3"
              :disabled="loading || institutions.length > 0"
              @click.stop="exportBenefits"
            >
              {{ $t('asp.exportBenefitsforASP') }}
            </v-btn>
          </v-toolbar>
        </template>

        <v-overlay :value="loading">
          <v-progress-circular
            indeterminate
            size="64"
          />
        </v-overlay>
      </grid>
    </v-card-text>
  </v-card>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import { mapGetters } from "vuex";
import { Permissions } from '@/enums/enums';

export default {
name: "InstitutionsNotApprovedBenefitsList",
components: {
  Grid
},
props: {
    year: {
        type: Number,
        required: true,
    },
    month: {
        type: Number,
        required: true,
    },
    campaignId: {
      type: Number,
      required: true
    }
},
data() {
    return {
    institutions: [],
    search: "",
    statusFilter: 0,
    loading: false,
    headers: [
        {
          text: this.$t('asp.headers.institutionCode'),
          value: "currentInstitutionCode",
        },
        {
          text: this.$t('asp.headers.schoolType'),
          value: "currentInstitutionDetailedSchoolType",
        },
        {
          text: this.$t('asp.headers.institutionName'),
          value: "currentInstitutionAbbreviation",
        },
        {
          text: this.$t('asp.headers.city'),
          value: "currentInstitutionTown",
        },
        {
          text: this.$t('asp.headers.email'),
          value: "currentInstitutionEmail",
        },
        {
          text: this.$t('asp.headers.phone'),
          value: "currentInstitutionPhone",
        },
        { text: "", value: "actions", sortable: false },
      ],
    };
},
computed: {
    ...mapGetters(["gridItemsPerPageOptions"]),
      ...mapGetters(['hasPermission']),
    hasAspImportPermission() {
      return this.hasPermission(Permissions.PermissionNameForASPImport);
    }
},
methods: {
    gridReload() {
      const grid = this.$refs['inReviewGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async exportBenefits(){
        this.loading = true;

        await this.$api.asp.exportApprovedMonthlyBenefits(this.campaignId).then(() => {
            this.$notifier.success('', this.$t('asp.exportSuccess'));
        })
        .catch((error) => {
            this.$notifier.error('', this.$t('errors.еxportApprovedBenefitsFile'));
            console.log(error);
        }).finally(() => {
            this.loading = false;
        });
    }
  },
};
</script>
