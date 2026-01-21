<template>
  <div>
    <v-row>
      <v-col>
        <v-select
          v-if="nomenclaturesDropdowns"
          v-model="selectedEntityType"
          :items="nomenclaturesDropdowns"
          item-value="code"
          :label="$t('nomenclature.label')"
          clearable
        />
      </v-col>
    </v-row>

    <v-card
      v-if="selectedEntityType && headers"
    >
      <v-card-title>
        <button-tip
          v-if="allowCreateEntityType"
          icon
          icon-name="mdi-plus"
          iclass=""
          tooltip="buttons.newRecordToolTip"
          bottom
          icon-color="primary"
          @click="onCreateBtnClick()"
        />
        <h3>{{ selectedEntityType }}</h3>
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

      <v-data-table
        :headers="headers"
        :items="pagedListOfItems.items"
        :options.sync="options"
        :server-items-length="pagedListOfItems.totalCount"
        :loading="loading"
        :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true }"
        multi-sort
      >
        <template v-slot:top>
          <GridExporter
            v-if="!loading"
            :headers="headers"
            :items="pagedListOfItems.items"
            :file-extensions="['xlsx', 'csv', 'txt']"
            :file-name="selectedEntityType"
          />
        </template>

        <template v-slot:[`item.controls`]="props">
          <button-group>
            <button-tip
              icon
              icon-name="mdi-eye"
              iclass=""
              tooltip="buttons.details"
              bottom
              small
              icon-color="secondary"
              @click="onDetailsBtnClick(props.item)"
            />
            <button-tip
              v-if="allowUpdateEntityType"
              icon
              icon-name="mdi-pencil"
              iclass=""
              tooltip="buttons.edit"
              bottom
              small
              icon-color="primary"
              @click="onEditBtnClick(props.item)"
            />
            <button-tip
              v-if="allowDeleteEntityType"
              icon
              icon-name="mdi-delete"
              tooltip="buttons.delete"
              bottom
              small
              icon-color="error"
              @click="onDeleteBtnClick(props.item)"
            />
          </button-group>
        </template>
      </v-data-table>
    </v-card>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>

import QueryString from "query-string";
import GridExporter from "@/components/wrappers/gridExporter.vue";
import { mapGetters } from 'vuex';

export default {
  name: 'Nomenclatures',
  components: { GridExporter },
  props: {
    type: {
      type: String,
      default() {
        return undefined;
      }
    },
    gridOptions: {
      type: Object,
      default() {
        return {};
      }
    }
  },
  data() {
    return {
      loading: false,
      saving: false,
      search: '',
      options: this.gridOptions,
      pagedListOfItems: {
        totalCount: 0,
        items: [],
      },
      queryString: QueryString,
      nomenclaturesDropdowns: null,
      selectedEntityType: null,
      headers: [
        {text: '', value: "controls", sortable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions', 'dynamicEntitiesSchema']),
    requiredEntitySchema() {
      const schema = this.dynamicEntitiesSchema && this.dynamicEntitiesSchema.entities
        ? this.dynamicEntitiesSchema.entities.find(x => x.name === this.selectedEntityType)
        : null;
      return schema;
    },
    allowCreateEntityType() {
      const schema = this.requiredEntitySchema;
      return schema && schema.allowCreate === true;
    },
    allowUpdateEntityType() {
      const schema = this.requiredEntitySchema;
      return schema && schema.allowUpdate === true;
    },
    allowDeleteEntityType() {
      const schema = this.requiredEntitySchema;
      return schema && schema.allowDelete === true;
    }
  },
  watch: {
    options: function () {
      this.get();
    },
    search: function () {
      this.get();
    },
    async selectedEntityType(value) {
      if(!value) {
        this.headers = [];
        return;
      }

      await this.loadHeaders(value);

      // Ще викне this.get() чрез options watcher-a
      this.resetOptions();
    }
  },
  async mounted(){
    await this.loadEntitiesJsonDescription();
    await this.loadEntityTypesDropdowns();

    this.selectedEntityType = this.type;
  },
  methods: {
    async loadEntitiesJsonDescription() {
      try {
        const result = await this.$api.dynamicForm.getEntitiesJsonDescription();
        this.$store.commit('setDynamicEntitiesSchema', result ? result.data : {});
      } catch (error) {
        this.$store.commit('setDynamicEntitiesSchema', {});
      }
    },
    async loadEntityTypesDropdowns() {
      const result = await this.$api.dynamicForm.getEntityTypesDropdowns();
      if(result) this.nomenclaturesDropdowns = result.data;
    },
    async loadHeaders(nomenclatureName) {
      const result = await this.$api.dynamicForm.getGridHeaders(nomenclatureName);
      this.headers = result && result.data
        ? [...result.data, { text: '', value: 'controls', sortable: false, align: 'end'}]
        : [];
    },
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

      if(this.selectedEntityType) {
        userListInput = {...userListInput, ...{ enityName: this.selectedEntityType }};
      }

      const query = '?' + this.queryString.stringify(userListInput);

      this.loading = true;
      this.$api.dynamicForm.getList(query)
        .then((response) => {
          if(response.data) {
            try{
              this.pagedListOfItems.items = JSON.parse(response.data.itemsAsJsonStr);
              this.pagedListOfItems.totalCount = response.data.totalCount;
            } catch (error) {
              console.log(error);
            }
          } else {
            this.pagedListOfItems.items = [];
            this.pagedListOfItems.totalCount = 0;
          }
        })
        .catch((error) => {
            console.log(error);
        })
        .then(() => {
            this.loading = false;
        });
    },
    onDetailsBtnClick(item) {
      const pkHeader = this.headers.find(x => x.isPrimaryKey === true);
      const entityId = pkHeader ? item[pkHeader.value] : undefined;

      if(!entityId) {
        console.log('entityId is required', item);
        return;
      }

      if(!this.selectedEntityType) {
        console.log('selectedEntityType is required', this.selectedEntityType);
        return;
      }

      this.$router.push(`/administration/entity/${entityId}/details?type=${this.selectedEntityType}&returnUrl=/administration/nomenclatures?type=${this.selectedEntityType}`);
    },
    onCreateBtnClick() {
      if(!this.allowCreateEntityType) {
        return this.$router.push('/errors/AccessDenied');
      }

      if(!this.selectedEntityType) {
        console.log('selectedEntityType is required', this.selectedEntityType);
        return;
      }

      this.$router.push(`/administration/entity/create?type=${this.selectedEntityType}&returnUrl=/administration/nomenclatures?type=${this.selectedEntityType}`);
    },
    onEditBtnClick(item) {
      if(!this.allowUpdateEntityType) {
        return this.$router.push('/errors/AccessDenied');
      }

      const pkHeader = this.headers.find(x => x.isPrimaryKey === true);
      const entityId = pkHeader ? item[pkHeader.value] : undefined;

      if(!entityId) {
        console.log('entityId is required', item);
        return;
      }

      if(!this.selectedEntityType) {
        console.log('selectedEntityType is required', this.selectedEntityType);
        return;
      }

      this.$router.push(`/administration/entity/${entityId}/edit?type=${this.selectedEntityType}&returnUrl=/administration/nomenclatures?type=${this.selectedEntityType}`);
    },
    async onDeleteBtnClick(item) {
      if(!this.allowUpdateEntityType) {
        return this.$router.push('/errors/AccessDenied');
      }

      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))) {
        const payload = { ...{ entityTypeName: this.type }, ...item };
        this.saving = true;
        this.$api.dynamicForm
          .delete(payload)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.get();
          })
          .catch((error) => {
            this.$notifier.error(this.$t('common.delete'), error.response.message, 5000);
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
        }
    },
    resetOptions() {
      const opt = this.options;
      this.options = {
        page: 1,
        itemsPerPage: 10,
        sortBy: [],
        sortDesc: [],
        groupBy: [],
        groupDesc: [],
        multiSort: opt.multiSort ?? false,
        mustSort: opt.mustSort ?? false
      };
    }
  }
};
</script>
