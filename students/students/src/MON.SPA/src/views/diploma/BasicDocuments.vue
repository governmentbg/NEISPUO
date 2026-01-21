<template>
  <div>
    <grid
      :ref="'basicDocumentsGrid' + _uid"
      url="/api/basicDocument/list"
      :headers="headers"
      :title="$t('basicDocument.title')"
      :filter="{
        hasSchema: customFilter.hasSchema,
        hasBarcode: customFilter.hasBarcode,
        isValidation: customFilter.isValidation,
        isAppendix: customFilter.isAppendix,
        isDuplicate: customFilter.isDuplicate,
        isIncludedInRegister: customFilter.isIncludedInRegister,
        isRuoDoc: customFilter.isRuoDoc,
      }"
    >
      <template v-slot:[`item.hasSchema`]="{ item }">
        <v-chip
          :color="item.hasSchema === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.hasSchema" />
        </v-chip>
      </template>

      <template v-slot:[`item.isValidation`]="{ item }">
        <v-chip
          :color="item.isValidation === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.isValidation" />
        </v-chip>
      </template>

      <template v-slot:[`item.isAppendix`]="{ item }">
        <v-chip
          :color="item.isAppendix === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.isAppendix" />
        </v-chip>
      </template>

      <template v-slot:[`item.isDuplicate`]="{ item }">
        <v-chip
          :color="item.isDuplicate === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.isDuplicate" />
        </v-chip>
      </template>

      <template v-slot:[`item.hasBarcode`]="{ item }">
        <v-chip
          :color="item.hasBarcode === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.hasBarcode" />
        </v-chip>
      </template>

      <template v-slot:[`item.isIncludedInRegister`]="{ item }">
        <v-chip
          :color="item.isIncludedInRegister === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.isIncludedInRegister" />
        </v-chip>
      </template>
      <template v-slot:[`item.isRuoDoc`]="{ item }">
        <v-chip
          :color="item.isRuoDoc === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="item.isRuoDoc" />
        </v-chip>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasEditPermission"
            icon
            icon-name="far fa-edit"
            icon-color="primary"
            iclass=""
            small
            tooltip="basicDocument.schemaEdit"
            left
            :to="`/basicDocuments/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasBarcodePermission"
            icon
            icon-name="fas fa-barcode"
            icon-color="primary"
            iclass=""
            small
            tooltip="menu.diplomas.barcodesList"
            left
            :to="`/basicDocument/${item.item.id}/barcodes`"
          />
        </button-group>
      </template>

      <template v-slot:[`item.additionalActions`]="{ item }">
        <button-menu
          v-if="hasEditPermission"
          small
        >
          <template>
            <v-list-item
              v-if="item.isIncludedInRegister === false"
              dense
              @click="onIncludeInRegister(item.id)"
            >
              <v-list-item-icon class="mr-1">
                <v-icon
                  color="success"
                >
                  mdi-check
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  {{ $t("basicDocument.includeInRegister") }}
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
            <v-list-item
              v-if="item.isIncludedInRegister === true"
              dense
              @click="onExcludeFromRegister(item.id)"
            >
              <v-list-item-icon class="mr-1">
                <v-icon
                  color="error"
                >
                  mdi-close
                </v-icon>
              </v-list-item-icon>
              <v-list-item-content>
                <v-list-item-title>
                  {{ $t("basicDocument.excludeFromRegister") }}
                </v-list-item-title>
              </v-list-item-content>
            </v-list-item>
          </template>
        </button-menu>
      </template>

      <template v-slot:[`header.hasSchema`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.hasSchema"
        />
      </template>
      <template v-slot:[`header.hasBarcode`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.hasBarcode"
        />
      </template>
      <template v-slot:[`header.isValidation`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.isValidation"
        />
      </template>
      <template v-slot:[`header.isAppendix`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.isAppendix"
        />
      </template>
      <template v-slot:[`header.isDuplicate`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.isDuplicate"
        />
      </template>
      <template v-slot:[`header.isIncludedInRegister`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.isIncludedInRegister"
        />
      </template>
      <template v-slot:[`header.isRuoDoc`]="{ header }">
        {{ header.text }}
        <bool-header-filter
          v-model="customFilter.isRuoDoc"
        />
      </template>
    </grid>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import BoolHeaderFilter from '@/components/wrappers/grid/BoolHeaderFilter.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'BasicDocumentsList',
  components: {
    Grid,
    BoolHeaderFilter
  },
  data() {
    return {
      saving: false,
      customFilter: {
        hasSchema: null,
        hasBarcode: null,
        isValidation: null,
        isAppendix: null,
        isDuplicate: null,
        isIncludedInRegister: null,
        isRuoDoc: null
      },
      headers: [
        {text: this.$t('basicDocument.headers.name'), value: "name", sortable: true},
        // {text: this.$t('basicDocument.headers.description'), value: "name", sortable: true},
        {text: this.$t('basicDocument.headers.hasSchema'), value: "hasSchema", sortable: true},
        {text: this.$t('basicDocument.headers.hasBarcode'), value: "hasBarcode", sortable: true},
        {text: this.$t('basicDocument.headers.isValidation'), value: "isValidation", sortable: true},
        {text: this.$t('basicDocument.headers.isAppendix'), value: "isAppendix", sortable: true},
        {text: this.$t('basicDocument.headers.isDuplicate'), value: "isDuplicate", sortable: true},
        {text: this.$t('basicDocument.headers.isIncludedInRegister'), value: "isIncludedInRegister", sortable: true},
        {text: this.$t('basicDocument.headers.isRuoDoc'), value: "isRuoDoc", sortable: true},
        {text: '', value: 'controls', inFavourOfIdentifier: false, sortable: false, filterable: false, align: 'end'},
        {text: '', value: 'additionalActions', inFavourOfIdentifier: false, sortable: false, filterable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['permissions']),
    hasEditPermission() {
      return this.permissions && this.permissions.includes(Permissions.PermissionNameForBasicDocumentEdit);
    },
    hasBarcodePermission() {
      return this.permissions && this.permissions.includes(Permissions.PermissionNameForDiplomaBarcodesShow);
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['basicDocumentsGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
    onIncludeInRegister(id) {
      this.saving = true;
      this.$api.basicDocument.includeInRegister(id)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.gridReload();
        })
        .catch(() => {
          this.$notifier.error('', this.$t('common.saveError'));
        })
        .then(() => { this.saving = false; });
    },
    onExcludeFromRegister(id) {
      this.saving = true;
      this.$api.basicDocument.excludeFromRegister(id)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.gridReload();
        })
        .catch(() => {
          this.$notifier.error('', this.$t('common.saveError'));
        })
        .then(() => { this.saving = false; });
    }
  }
};
</script>
