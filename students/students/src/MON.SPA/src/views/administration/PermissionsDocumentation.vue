<template>
  <div>
    <grid
      url="/api/administration/GetPermissionDocumentations"
      file-export-name="PermissionsDocumentation"
      :headers="headers"
      :title="$t('permissionsDocumentation.title')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    >
      <template #actions="item">
        <button-group>
          <button-tip
            icon
            icon-color="primary"
            icon-name="mdi-pencil"
            iclass=""
            tooltip="buttons.edit"
            bottom
            small
            @click="onEditItem(item.item)"
          />
        </button-group>
      </template>
    </grid>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
    <v-dialog
      v-model="dialog"
      max-width="500px"
    >
      <form-layout
        v-if="editedItem"
        skip-cancel-prompt
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <v-text-field
            :value="editedItem.permissionName"
            :label="$t('permissionsDocumentation.headers.permissionName')"
            disabled
          />
        </template>
        <template>
          <v-textarea
            v-if="editedItem"
            v-model="editedItem.description"
            :label="$t('permissionsDocumentation.headers.description')"
            outlined
            filled
            auto-grow
            clearable
          />
          <v-textarea
            v-if="editedItem"
            v-model="editedItem.usage"
            :label="$t('permissionsDocumentation.headers.usage')"
            outlined
            filled
            auto-grow
            clearable
          />
        </template>
      </form-layout>
    </v-dialog>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'PermissionsDocumentation',
  components: {
    Grid
  },
  data() {
    return {
      headers: [
        {text: this.$t('permissionsDocumentation.headers.permissionName'), value: "permissionName", sortable: true},
        {text: this.$t('permissionsDocumentation.headers.permission'), value: "permission", sortable: true},
        {text: this.$t('permissionsDocumentation.headers.description'), value: "description", sortable: true},
        {text: this.$t('permissionsDocumentation.headers.usage'), value: "usage", sortable: true},
        {text: '', value: 'controls', inFavourOfIdentifier: false, align: 'end'},
      ],
      saving: false,
      dialog: false,
      editedItem: null,
      origValue: null
    };
  },
  computed: {
    ...mapGetters(['permissions']),
    hasManagePermission() {
       return this.permissions.includes(Permissions.PermissionNameForContextualInformationManage);
    }
  },
  methods: {
    onEditItem(item) {
      if(!item) return;
      this.origValue = item.value;
      this.editedItem = item;
      this.dialog = true;
    },
    onCancel() {
      this.dialog = false;
      this.editedItem.value = this.origValue;
      this.editedItem = null;
    },
    onSave() {
      this.saving = true;
      this.$api.administration.updatePermissionDocumentation(this.editedItem)
      .then(() => {
        this.$notifier.success(this.$t('common.save'), this.$t('common.saveSuccess'), 5000);
        this.dialog = false;
        this.editedItem = null;
      })
      .catch(error => {
        console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
  }
};
</script>
