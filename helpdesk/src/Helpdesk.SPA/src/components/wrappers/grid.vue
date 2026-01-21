<template>
  <v-card>
    <v-card-title>
      {{ title }}
      <v-spacer />
      <text-field
        v-model="search"
        prepend-inner-icon="mdi-magnify"
        :label="$t('buttons.search')"
        clearable
        single-line
        hide-details
        dense
      />
    </v-card-title>
    <v-card-subtitle class="pa-0">
      <slot name="subtitle" />
    </v-card-subtitle>
    <v-data-table
      v-model="selected"
      :headers="headers"
      :items="pagedListOfItems.items"
      :options.sync="options"
      :server-items-length="pagedListOfItems.totalCount"
      :loading="loading"
      :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true, showFirstLastPage: true }"
      :show-select="showSelect"
      :item-key="itemKey"
    >
      <template v-slot:top>
        <GridExporter
          v-if="fileExporterExtensions && !loading"
          :key="_uid"
          :items="pagedListOfItems.items"
          :file-extensions="fileExporterExtensions"
          :file-name="fileExportName"
          :headers="headers"
          class="mt-0"
        />
      </template>

      <template v-slot:[`item.documents`]="props">
        <slot
          :item="props.item"
          name="documents"
        />
      </template>

      <template v-slot:[`item.isEscalated`]="props">
        <slot
          :item="props.item"
          name="isEscalated"
        />
      </template>

      <template v-slot:[`item.commentsCount`]="props">
        <slot
          :item="props.item"
          name="commentsCount"
        />
      </template>

      <template v-slot:[`item.attachmentsCount`]="props">
        <slot
          :item="props.item"
          name="attachmentsCount"
        />
      </template>

      <template v-slot:[`item.createDate`]="props">
        <slot
          :item="props.item"
          name="createDate"
        />
      </template>

      <template v-slot:[`item.resolveDate`]="props">
        <slot
          :item="props.item"
          name="resolveDate"
        />
      </template>

      <template v-slot:[`item.lastActivityDate`]="props">
        <slot
          :item="props.item"
          name="lastActivityDate"
        />
      </template>

      <template v-slot:[`item.controls`]="props">
        <slot
          :item="props.item"
          name="actions"
        />
      </template>

      <template v-slot:[`item.title`]="props">
        <slot
          :item="props.item"
          name="title"
        >
          {{ props.item.title }}
        </slot>
      </template>

      <template v-slot:[`item.status`]="props">
        <slot
          :item="props.item"
          name="status"
        >
          {{ props.item.status }}
        </slot>
      </template>

      <template v-slot:[`item.priority`]="props">
        <slot
          :item="props.item"
          name="priority"
        >
          {{ props.item.priority }}
        </slot>
      </template>

      <template v-slot:[`item.assignedToSysUser`]="props">
        <slot
          :item="props.item"
          name="assignedToSysUser"
        >
          {{ props.item.assignedToSysUser }}
        </slot>
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
        v-for="(_, slot) of $scopedSlots"
        v-slot:[slot]="scope"
      >
        <slot
          :name="slot"
          v-bind="scope"
        />
      </template>
    </v-data-table>
  </v-card>
</template>

<script>
import QueryString from "query-string";
import http from "@/services/http.service";
import GridExporter from "./gridExporter";
import debounce from 'lodash.debounce';
import { mapGetters } from 'vuex';

export default {
  name: 'GridWrapper',
  components: { GridExporter },
  props: {
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
    filter: { // Постоянен филтър, който се добавя към v-data-table.options
      type: Object,
      default() {
        return undefined;
      }
    },
    debounce: {
      type: Number,
      default() {
        return 600; // The number of milliseconds to delay
      }
    },
    showSelect: {
      type: Boolean,
      default() {
        return false;
      }
    },
    itemKey: {
      type: String,
      default() {
        return 'id';
      }
    },
    refKey: {
      type: String,
      default() {
        return 'unknown';
      }
    }
  },
  data() {
    return {
      loading: true,
      search: '',
      defaultGridOptions: {},
      pagedListOfItems: {
        totalCount: 0,
        items: [],
      },
      queryString: QueryString,
      selected: []
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
    }
  },
  watch: {
    options: function () {
      this.get();
    },
    // search: function () {
    //   this.get();
    // },
    filter: {
      handler(){
        this.get();
      },
      deep: true
    }
  },
  created() {
      this.unwatch = this.$watch('search', debounce(function() {
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
    get() {
      const opt = this.options;
      let userListInput = {
          filter: this.search,
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
          userListInput = {...userListInput, ...this.filter};
      }

      const query = "?" + this.queryString.stringify(userListInput);

      this.loading = true;
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
  }
};
</script>
