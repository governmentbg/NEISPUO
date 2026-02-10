<template>
  <div>
    <v-card>
      <v-card-title>
        <v-col
            cols="12"
            md="6"
            lg="4"
            sm="7"
          >
            <autocomplete
              v-model="periods"
              api="/api/lookups/GetResourceSupportDataPeriods"
              :label="$t('finance.resourceSupport.periods')"
              :defer-options-loading="false"
              chips
              small-chips
              deletable-chips
              clearable
              multiple              
            />
          </v-col>
        <v-spacer />
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          :label="$t('buttons.search')"
          clearable
          single-line
          hide-details
        />
      </v-card-title>
      <v-card-text>
        <v-data-table
          class="headerTable"
          v-if="periodsSelected"
          :items="filteredItems"
          :headers="headers"
          :loading="loading"
          :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true, showFirstLastPage: true }"
          multi-sort
          dense
        >
          <template v-slot:top>
            <v-toolbar flat>
              <GridExporter
                :items="pagedListOfItems.items"
                :file-extensions="['xlsx', 'csv', 'txt']"
                file-name="Натурални показатели"
                :headers="headers"
              />
              <v-spacer />
            </v-toolbar>
          </template>
          

          <template v-slot:[`item.minimalInsuranceIncomeRate`]="{ item }">
            <span v-if="currency.showAltCurrency">{{ item.minimalInsuranceIncomeRateStr }}</span>
            <span v-else>{{ item.minimalInsuranceIncomeRate }}</span>
          </template>

          <template v-slot:[`item.insurancePayRate`]="{ item }">
            <span v-if="currency.showAltCurrency">{{ `${calcInsurancePayRate(item)} ${currency.currency.code} / ${calcAltInsurancePayRate(item)} ${currency.altCurrency.code}` }}</span>
            <span v-else>{{ calcInsurancePayRate(item) }}</span>
          </template>

          <template v-slot:[`item.Period`]="{ item }">
            <span v-if="item.Period == 0">текущи данни</span>
            <span v-else>{{item.Period}}</span>
          </template>          

          <template v-slot:[`footer.prepend`]>
            <button-group>
              <v-btn
                v-if="periodsSelected"
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
        <v-alert
          v-else
          border="top"
          colored-border
          type="info"
          elevation="2"
        >
          Моля, изберете поне един период!
        </v-alert>
      </v-card-text>
      <v-card-actions>

      </v-card-actions>
      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
    </v-card>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';
import { Permissions } from "@/enums/enums";
import GridExporter from "@/components/wrappers/gridExporter";
import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";

export default {
  name: "ResourceSupportDataComponent",
  components: {
    Autocomplete,
    GridExporter
  },
  data() {
    return {
      dateFormat: Constants.DATEPICKER_FORMAT,
      periods: [],
      selectedYear: null,
      selectedMonth: null,
      search: '',
      // debounce state for client-side search
      debouncedSearch: '',
      searchDebounceMs: 300,
      searchTimeout: null,      
      loading: false,
      saving: false,
      pagedListOfItems: {
        totalCount: 0,
        items: [],
      },
      headers: [
        { text: "Година", value: "SchoolYear", filterable: false, sortable: true, groupable: false },
        { text: "Период", value: "Period", filterable: false, sortable: true, groupable: false },        
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'gridItemsPerPageOptions', 'currency']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForResourceSupportDataManage);
    },
    periodsSelected() {
      return this.periods && this.periods.length > 0;
    },
    filteredItems() {
      const items = (this.pagedListOfItems && this.pagedListOfItems.items) || [];
      // only search when debouncedSearch has at least 3 characters
      if (!this.debouncedSearch || this.debouncedSearch.length < 3) return items;
      const s = this.debouncedSearch.toString().toLowerCase();
      return items.filter(item =>
        Object.values(item).some(v => String(v ?? '').toLowerCase().includes(s))
      );
    }    
  },
  watch: {
    periods() {
      if(!this.periods) {
        return;
      }
      this.load();
    },
    // debounce the search input and require at least 3 chars
    search(newVal) {
      clearTimeout(this.searchTimeout);
      if (!newVal || newVal.length < 3) {
        // clear debounced search so table shows full list
        this.debouncedSearch = '';
        return;
      }
      this.searchTimeout = setTimeout(() => {
        this.debouncedSearch = newVal;
      }, this.searchDebounceMs);    
    }
  },
  beforeDestroy() {
    clearTimeout(this.searchTimeout);
  },
  async beforeMount() {
    if (!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.$api.finance.getResourceSupportDataGridHeaders().then((response) => {
      if(response && response.data) {
        this.headers = [
          { text: "Година", value: "SchoolYear", filterable: false, sortable: true, groupable: false },
          { text: "Период", value: "Period", filterable: false, sortable: true, groupable: false },
          ...response.data
        ];
      }
    }).catch((error) => {
        console.log(error);
      });
    this.selectedMonth = this.$helper.getMonth();
    await this.getCurrentSchoolYear();
  },

  methods: {
    async getCurrentSchoolYear() {
      try {
        const currentSchoolYear = Number((await this.$api.institution.getCurrentYear())?.data);
        if(!isNaN(currentSchoolYear)) {
          this.selectedYear = currentSchoolYear;
        }
      } catch (error) {
        console.log(error);
      }
    },
    load() {
      if(!this.periods) {
        return;
      }

      this.loading = true;
      this.$api.finance.listResourceSupportDataPeriods(
        this.periods
      )
       .then((response) => {
        this.pagedListOfItems = response.data;
      })
      .catch(error => {
        console.log(error.response);
      })
      .then(() => (this.loading = false));            
    },
    calcInsurancePayRate(item) {
      if(!item) return;

      const insurancePayRate = ((item.minimalInsuranceIncomeRate / item.monthDays) * (item.endDayNumber - item.startDayNumber + 1)).toFixed(2);
      return insurancePayRate;
    },
    calcAltInsurancePayRate(item) {
      if(!item) return;

      const insurancePayRate = ((item.altMinimalInsuranceIncomeRate / item.monthDays) * (item.endDayNumber - item.startDayNumber + 1)).toFixed(2);
      return insurancePayRate;
    }
  }
};
</script>


<style scoped>
::v-deep .headerTable thead th {
  vertical-align: top !important;
}

/* target the inner flex/button wrapper so its content sits at the top */
::v-deep .headerTable thead th > * {
  align-items: flex-start !important;
  display: block !important;
}

::v-deep .headerTable thead th .v-btn {
  align-items: flex-start !important;
}
</style>