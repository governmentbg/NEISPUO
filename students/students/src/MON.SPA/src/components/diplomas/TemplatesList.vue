<template>
  <div>
    <grid
      :ref="'diplomaListTable' + _uid"
      url="/api/diplomaTemplate/list"
      :headers="headers"
      :title="$t('basicDocument.templates.list')"
    >
      <template #footerPrepend>
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          @click.stop="onAddClick"
        >
          {{ $t("menu.diplomas.createTemplate") }}
        </v-btn>
      </template>
      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-color="primary"
            icon-name="mdi-pencil"
            iclass=""
            tooltip="diplomas.editDiplomaTemplateTooltip"
            bottom
            small
            @click="onEditDiplomaTemplate(item.item.id)"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-color="primary"
            icon-name="mdi-content-copy"
            iclass=""
            tooltip="buttons.copy"
            bottom
            small
            @click="onCopyDiplomaTemplate(item.item.id)"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-color="error"
            icon-name="mdi-delete"
            iclass=""
            tooltip="diplomas.deleteDiplomaTemplateTooltip"
            bottom
            small
            @click="onDeleteDiplomaTemplate(item.item.id)"
          />
        </button-group>
      </template>
    </grid>
    <prompt-dlg
      ref="prompt"
      persistent
    >
      <template>
        <c-info
          uid="diplomaTemplate.diplomaBasicDocumentDropdown"
        >
          <autocomplete
            id="diplomaBasicDocumentDropdown"
            :ref="'DiplomaBasicDocumentDropdown_' + _uid"
            v-model="selectedBasicDocumentId"
            api="/api/lookups/GetBasicDocumentTypes"
            :placeholder="$t('common.choose')"
            clearable
            :defer-options-loading="false"
            :filter="{ schemaSpecified: true, isRuoDoc: isRuo, filterByDetailedSchoolType: true }"
            @change="diplomaBasicDocumentChange"
          />
        </c-info>

        <c-info
          v-if="filteredBasicClassOptions && filteredBasicClassOptions.length > 0"
          uid="diplomaTemplate.diplomaBasicClassDropdown"
        >
          <autocomplete
            v-model="selectedBasicClassId"
            :items="filteredBasicClassOptions"
            :label="$t('recognition.basicClass')"
            :placeholder="$t('common.choose')"
            clearable
          />
        </c-info>
      </template>
    </prompt-dlg>
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
import Grid from '@/components/wrappers/grid.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'DiplomaTemplatesListComponent',
  components: {
    Grid,
    Autocomplete
  },
  data() {
    return {
      saving: false,
      selectedBasicDocumentId: null,
      selectedBasicClassId: null,
      selectedBasicDocument: null,
      basicClassOptions: [],
      headers: [
        {
          text: this.$t('diplomas.template.name'),
          value: 'name'
        },
        {
          text: this.$t('diplomas.basicDocumentTypeName'),
          value: 'basicDocumentTypeName'
        },
        {
          text: this.$t('recognition.basicClass'),
          value: 'basicClassName'
        },
        {
          text: this.$t('common.actions'),
          value: 'controls',
          align: 'end',
          sortable: false,
          filterable: false
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userRegionId']),
    hasManagePermission() {
      return this.hasPermission(
        Permissions.PermissionNameForDiplomaTemplatesManage
      );
    },
    filteredBasicClassOptions() {
      if(!Array.isArray(this.basicClassOptions) || this.basicClassOptions.length === 0) {
        return [];
      }

      if(this.selectedBasicDocument && Array.isArray(this.selectedBasicDocument.basicClassList)
        && this.selectedBasicDocument.basicClassList.length > 0) {
          return this.basicClassOptions.filter(x => this.selectedBasicDocument.basicClassList.includes(x.value));
      }

      return [];
    },
    isRuo() {
        return !!this.userRegionId && !this.isInstitution;
      }
  },
  mounted() {
    this.loadBasicClassOptions();
  },
  methods: {
    gridReload() {
      const grid = this.$refs['diplomaListTable' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    onEditDiplomaTemplate(id) {
      this.$router.push({ name: 'DiplomaTemplateEdit' , params: { id } });
    },
    onCopyDiplomaTemplate(id) {
      // Създаване на шаблон чрез копиране/клониране на друг шаблон.
      this.$router.push({ name: 'DiplomaTemplateCreate' , query: { templateId: id } });
    },
    async onDeleteDiplomaTemplate(id) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.diplomaTemplate.delete(id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('diplomas.deleteError'));
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async onAddClick() {
      if (await this.$refs.prompt.open('', this.$t('diplomas.diplomaBasicDocumentTypeDropdownLabel'), { width: 800 })) {
        if (!this.selectedBasicDocumentId) {
          return this.$notifier.error('', `${this.$t("diplomas.template.missingBasicDocumentSelection")}`);
        }

        const basicDocumentId = this.selectedBasicDocumentId;
        const basicClassId = this.selectedBasicClassId;
        this.selectedBasicDocumentId = null;
        this.selectedBasicClassId = null;

        const queryParams = {
          basicDocumentId: basicDocumentId
        };
        if(basicClassId) {
          queryParams.basicClassId = basicClassId;
        }

        return this.$router.push({ name: 'DiplomaTemplateCreate' , query: queryParams });
      } else {
        this.selectedBasicDocumentId = null;
        this.selectedBasicClassId = null;
      }
    },
    diplomaBasicDocumentChange(basicDocumentId) {
      if(!basicDocumentId) {
        this.selectedBasicClassId = null;
        return;
      }

      const selector = this.$refs[`DiplomaBasicDocumentDropdown_${this._uid}`];
       if (selector) {
        const selectedItem = selector.getOptionsList().find(x => x.value === basicDocumentId);
        this.selectedBasicDocument = selectedItem;
      }
    },
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassOptions({minId: 1, maxId: 13})
      .then((result) => {
        if(result) {
          this.basicClassOptions = result.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    }
  }
};
</script>
