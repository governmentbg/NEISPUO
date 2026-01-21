<template>
  <grid
    :ref="'lodAssessmentTeplateListGrid' + _uid"
    url="/api/lodAssessmentTemplate/list"
    :headers="headers"
    :title="$t('lodAssessmentTemplate.listTitle')"
  >
    <template #footerPrepend>
      <v-btn
        v-if="hasManagePermission"
        small
        color="primary"
        :to="`/lod/assessment/template/create`"
      >
        {{ $t('buttons.newRecord') }}
      </v-btn>
    </template>
    <template #actions="item">
      <button-group>
        <button-tip
          icon
          icon-name="mdi-eye"
          icon-color="primary"
          tooltip="buttons.review"
          bottom
          iclass=""
          small
          :to="`/lod/assessment/template/${item.item.id}/details`"
          :disabled="saving"
        />
        <button-tip
          v-if="hasManagePermission"
          icon
          icon-name="mdi-pencil"
          icon-color="primary"
          tooltip="buttons.edit"
          bottom
          iclass=""
          small
          :disabled="saving"
          :to="`/lod/assessment/template/${item.item.id}/edit`"
        />
        <button-tip
          v-if="hasManagePermission"
          icon
          icon-name="mdi-delete"
          icon-color="error"
          tooltip="buttons.delete"
          bottom
          iclass=""
          small
          :disabled="saving"
          @click="deleteTemplate(item.item.id)"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import Grid from "@/components/wrappers/grid.vue";

export default {
  name: 'AssessmentTemplateListView',
  components: {
    Grid
  },
  data() {
    return {
      saving: false,
      headers: [
        {
          text: this.$t('lodAssessmentTemplate.headers.name'),
          value: "name"
        },
        {
          text: this.$t('lodAssessmentTemplate.headers.description'),
          value: "description"
        },
        {
          text: this.$t('lodAssessmentTemplate.headers.basicClass'),
          value: "basicClassName"
        },
        {
          text: '',
          value: 'controls',
          filterable: false,
          sortable: false,
          align: 'end' }
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateManage);
    }
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['lodAssessmentTeplateListGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    deleteTemplate(id) {
      this.saving = true;

      this.$api.lodAssessmentTemplate.delete(id)
        .then(() => {
          this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
          this.gridReload();
        })
        .catch(error => {
          this.$notifier.error('', this.$t('common.deleteError'), 5000);
          console.error(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    }
  }
};
</script>
