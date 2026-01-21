<template>
  <div>
    <grid
      ref="contextualInfoGrid"
      url="/api/administration/ContextualInformationList"
      file-export-name="ContextualInformationList"
      :headers="headers"
      :title="$t('contextualInformation.title')"
      
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ moduleName: 'students' }"
    >
      <template #actions="item">
        <button-tip
          v-if="hasManagePermission"
          icon
          icon-color="primary"
          icon-name="mdi-pencil"
          iclass=""
          tooltip="buttons.edit"
          bottom
          small
          @click="onEditItem(item.item)"
        />
      </template>
    </grid>
    <contextual-information-modal
      v-model="contextualInfoKey"
      @save="onContextualInfoSave"
    />
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';
import ContextualInformationModal from '@/components/admin/ContextualInformationModal.vue';

export default {
  name: 'ContextualInformation',
  components: {
    Grid,
    ContextualInformationModal
  },
  data() {
    return {
      headers: [
        {text: this.$t('contextualInformation.headers.moduleName'), value: "moduleName", sortable: true},
        {text: this.$t('contextualInformation.headers.key'), value: "key", sortable: true},
        {text: this.$t('contextualInformation.headers.description'), value: "description", sortable: true},
        {text: this.$t('contextualInformation.headers.value'), value: "value", sortable: true},
        {text: '', value: 'controls', inFavourOfIdentifier: false, align: 'end'},
      ],
      contextualInfoKey: null,
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
      this.contextualInfoKey = item.key;
    },
    onContextualInfoSave() {
      this.$refs.contextualInfoGrid.get();
    }
  }
};
</script>