<template>
  <v-card
    :flat="flat"
    :tile="tile"
  >
    <v-card-title>
      <slot name="title">
        {{ title }}
      </slot>
      <v-spacer />
    </v-card-title>
    <v-card-subtitle>
      <slot name="subtitle" />
    </v-card-subtitle>
    <v-data-table
      :headers="headers"
      :items="pagedListOfItems.items"
      :options.sync="options"
      :server-items-length="pagedListOfItems.totalCount"
      :loading="loading"
      :footer-props="{ itemsPerPageOptions: itemsPerPageOptions || gridItemsPerPageOptions, showCurrentPage: true, showFirstLastPage: true }"
      class="custom-grid"
      v-bind="$attrs"
      v-on="$listeners"
    >
      <template v-slot:top>
        <v-row
          dense
          class="mb-1"
        >
          <div
            d-inline-flex
            class="px-2"
          >
            <GridExporter
              v-if="fileExporterExtensions"
              :items="pagedListOfItems.items"
              :file-extensions="fileExporterExtensions"
              :file-name="fileExportName"
              :auto-fit-columns="fileAutoFitColumns"
              :headers="headers"
            />
          </div>
          <v-text-field
            v-model="gridFilters.search"
            append-icon="mdi-magnify"
            :label="searchLabel || $t('buttons.search')"
            clearable
            single-line
            hide-details
            dense
            :disabled="false"
          />
          <v-col
            cols="6"
            class="px-3 py-0"
          >
            <slot
              name="topAppend"
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.selected`]="props">
        <slot
          :item="props.item"
          name="testSelected"
        />
      </template>

      <template v-slot:[`item.month`]="props">
        <slot
          :item="props.item"
          name="month"
        />
      </template>

      <template v-slot:[`item.documents`]="props">
        <slot
          :item="props.item"
          name="documents"
        />
      </template>

      <template v-slot:[`item.timestampUtc`]="props">
        <slot
          :item="props.item"
          name="timestampUtc"
        />
      </template>

      <template v-slot:[`item.controls`]="props">
        <slot
          :item="props.item"
          name="actions"
        />
      </template>

      <template v-slot:[`item.edit`]="props">
        <slot
          :item="props.item"
          name="edit"
        />
      </template>

      <template
        v-for="(_, slot) of $scopedSlots"
        v-slot:[slot]="scope"
      >
        <slot
          :name="slot"
          v-bind="scope"
        />
      </template>

      <template v-slot:[`footer.prepend`]>
        <button-group>
          <slot
            name="footerPrepend"
          />
          <v-btn
            small
            color="secondary"
            outlined
            @click="get"
          >
            {{ $t('buttons.reload') }}
          </v-btn>
        </button-group>
      </template>


      <template
        v-for="(col, index) in headers.filter(x => x.filterable === true)"
        v-slot:[`header.${col.value}`]="{ header }"
      >
        {{ header.text }}
        <text-header-filter
          v-if="col.type == 'text' || col.type == 'string' || col.type == 'html'"
          :key="index"
          v-model="headerFilters[col.value]"
        />
        <bool-header-filter
          v-if="col.type == 'boolean' || col.type == 'bool'"
          :key="index"
          v-model="headerFilters[col.value]"
        />
      </template>
    </v-data-table>
  </v-card>
</template>

<script>
import QueryString from "query-string";
import http from "@/services/http.service";
import GridExporter from "./gridExporter";
import BoolHeaderFilter from "@/components/wrappers/grid/BoolHeaderFilter.vue";
import TextHeaderFilter from "@/components/wrappers/grid/TextHeaderFilter.vue";

import debounce from 'lodash.debounce';
import isEqual from 'lodash.isequal';
import { mapGetters, mapMutations } from 'vuex';

export default {
  name: 'GridWrapper',
  components: { GridExporter, BoolHeaderFilter, TextHeaderFilter },
  props: {
    gridKey: {
      type: String,
      default() {
        return undefined;
      }
    },
    headers: {
      required: true,
      type: Array,
    },
    url: {
      required: true,
      type: String,
    },
    title: {
      required: false,
      default: 'title',
      type: String,
    },
    proccessData: {
      required: false,
      type: Function,
      default: undefined,
    },
    fileExporterExtensions: {
      type: Array,
      default() {
        return [];
      }
    },
    fileExportName: {
      type: String,
      default() {
        return 'Export';
      }
    },
    fileAutoFitColumns: {
      type: Boolean,
      default() {
        return false;
      }
    },
    filter: { // Постоянен филтър, който се добавя към v-data-table.options
      type: Object,
      default() {
        return undefined;
      }
    },
    refKey: {
      type: String,
      default() {
        return 'unknown';
      }
    },
    debounce: {
      type: Number,
      default() {
        return 600; // The number of milliseconds to delay
      }
    },
    searchLabel: {
      type: String,
      default() {
        return null;
      }
    },
    itemsPerPageOptions: {
      type: Array,
      default() {
        return null;
      }
    },
    usePostVerb: {
      type: Boolean,
      default() {
        return false;
      }
    },
    flat: {
      type: Boolean,
      default() {
        return false;
      }
    },
    tile: {
      type: Boolean,
      default() {
        return false;
      }
    },
  },
  data() {
    return {
      loading: true,
      defaultGridOptions: {},
      defaultGridFilters: {},
      pagedListOfItems: {
        totalCount: 0,
        items: [],
      },
      queryString: QueryString,
      selected: [],
      freeze: false,
      headerFilters: {}
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions']),
    options: {
      get () {
        if (this.refKey in this.$store.state.gridOptions) {
          return this.$store.state.gridOptions[this.refKey] || {};
        }
        else {
          return this.defaultGridOptions;
        }
      },
      set (value) {
        if (this.refKey in this.$store.state.gridOptions) {
          this.$store.commit('updateGridOptions', { options: value, refKey: this.refKey });
        }
        else {
          return this.defaultGridOptions = value;
        }
      }
    },
    gridFilters: {
      get () {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {};
        }
        else {
          return this.defaultGridFilters;
        }
      },
      set (value) {
        if (this.refKey in this.$store.state.gridFilters) {
          this.$store.commit('updateGridFilter', { options: value, refKey: this.refKey });
        }
        else {
          return this.defaultGridFilters = value;
        }
      }
    },
    computedHeaderFilters: function() {
          return Object.assign({}, this.headerFilters);
      }
  },
  watch: {
    options: function () {
      this.get();
    },
    filter(newValue, oldValue) {
      if(newValue.hasOwnProperty && oldValue.hasOwnProperty) {
        for (const property in newValue) {
          const newValuePropVal = typeof newValue[property] !== 'undefined' ? newValue[property] : null;
          const oldValuePropVal = typeof oldValue[property] !== 'undefined' ? oldValue[property] : null;
          if(newValuePropVal !== oldValuePropVal) {
            if(this.options.page > 1) {
              this.options.page = 1;
            } else {
              this.get();
            }
          }
        }
      }
    },
    computedHeaderFilters: {
      // newValue and oldValue parameters are the same when deep watching an objec
      // https://github.com/vuejs/vue/issues/2164
      deep: true,
      handler:function (newValue, oldValue) {
          if(newValue.hasOwnProperty && oldValue.hasOwnProperty) {
            for (const property in newValue) {
              const newValuePropVal = typeof newValue[property] !== 'undefined' ? newValue[property] : null;
              const oldValuePropVal = typeof oldValue[property] !== 'undefined' ? oldValue[property] : null;
              if(!isEqual(newValuePropVal,oldValuePropVal)) {
                if(this.options.page > 1) {
                  this.options.page = 1;
                } else {
                  this.get();
                }
              }
            }
          }
      }
    },
  },
  created() {
      this.unwatch = this.$watch('gridFilters.search', debounce(function() {
        if(this.options.page > 1) {
          this.options.page = 1;
        } else {
          this.get();
        }
      }, this.debounce));
  },
  beforeDestroy() {
    this.unwatch();
  },
  methods: {
    ...mapMutations(['setGridState']),
    getGridUniqueKey() {
      return this.gridKey || this.url || this.$route.path;
    },
    get() {
      const opt = this.options;
      let userListInput = {
          filter: this.gridFilters.search ? this.gridFilters.search.trim() : this.gridFilters.search,
          pageIndex: opt.page - 1,
          pageSize: opt.itemsPerPage,
      };

      if (opt && Array.isArray(opt.sortBy) && opt.sortBy.length > 0) {
          const map = opt.sortBy.map((sort, index) => {
              return `${sort}${(Array.isArray(opt.sortDesc) && opt.sortDesc.length > 0 && opt.sortDesc[index] ? " desc" : "")}`;
          });

          userListInput.sortBy = map.join();
      }

      if(this.filter) {
        for (const [key, value] of Object.entries(this.filter)) {
          if(value !== null && value !== undefined) {
            userListInput[key] = value;
          }
        }
      }

      const columnFilters = [];
      if(this.headerFilters) {
        for (const [key, value] of Object.entries(this.headerFilters)) {
          if(value && value.filter !== null && value.filter !== undefined ) {
            columnFilters.push( {...{ field: key}, ...value});
          }
        }
      }

      const query = "?" + this.queryString.stringify(userListInput);

      this.loading = true;

      if(this.usePostVerb) {
        http.post(`${this.url}${query}`, columnFilters, false)
            .then((response) => {
                this.pagedListOfItems = response.data;
                this.$emit('gridDataLoaded', this.pagedListOfItems.totalCount);
                if (this.proccessData && this.pagedListOfItems.items) {
                    this.pagedListOfItems.items.forEach((x) => {
                        this.proccessData?.(x);
                    });
                }
            })
            .catch((error) => {
                console.log(error.response);
            })
            .then(() => {
                this.loading = false;
            });
      } else {
        http.get(`${this.url}${query}`, undefined, false)
            .then((response) => {
                this.pagedListOfItems = response.data;
                this.$emit('gridDataLoaded', this.pagedListOfItems.totalCount);
                if (this.proccessData && this.pagedListOfItems.items) {
                    this.pagedListOfItems.items.forEach((x) => {
                        this.proccessData?.(x);
                    });
                }
            })
            .catch((error) => {
                console.log(error.response);
            })
            .then(() => {
                this.loading = false;
            });
      }

    },
    setFreeze(val) {
      this.freeze = val;
    },
  }
};
</script>

<style>
  .custom-grid table {
    border-collapse: collapse !important;
  }
</style>
