<template>
  <div>
    <grid
      :ref="'commonPersonalDevelopmentSupportListTable' + _uid"
      url="/api/commonPersonalDevelopmentSupport/list"
      :headers="headers"
      :title="$t('lod.commonPersonalDevelopmentSupport')"
      :filter="{
        studentId: personId
      }"
    >
      <template v-slot:[`item.items`]="{ item }">
        <v-chip
          v-for="reason in item.items"
          :key="reason.id"
          color="light"
          small
          label
          class="ma-1"
        >
          {{ reason.typeName }}
        </v-chip>
      </template>
      <template v-slot:[`item.documents`]="{ item }">
        <doc-downloader
          v-for="document in item.documents"
          :key="document.id"
          :value="document"
          small
        />
      </template>

      <template #actions="{ item }">
        <button-group>
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :disabled="saving"
            :to="`/student/${personId}/commonPersonalDevelopment/${item.id}/details`"
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
            :disabled="saving || item.isLodFinalized"
            :lod-finalized="item.isLodFinalized"
            :to="`/student/${personId}/commonPersonalDevelopment/${item.id}/edit`"
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
            :disabled="saving || item.isLodFinalized"
            :lod-finalized="item.isLodFinalized"
            @click="deleteItem(item)"
          />
        </button-group>
      </template>
      <template #footerPrepend>
        <v-btn
          v-if="hasManagePermission"
          color="primary"
          small
          :to="`/student/${personId}/commonPersonalDevelopment/create`"
        >
          {{ $t("buttons.newRecord") }}
        </v-btn>
      </template>
    </grid>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import DocDownloader from "@/components/common/DocDownloader.vue";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentCommonPersonalDevelopmentListView',
  components: {
    Grid,
    DocDownloader
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      saving: false,
      headers: [
        {
          text: this.$t("common.schoolYear"),
          value: "schoolYearName",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$t("common.reasonText"),
          value: "items",
          sortable: false,
          filterable: false,
        },
        {
          text: this.$t("common.attachedFiles"),
          value: "documents",
          sortable: false,
          filterable: false,
        },
        {
          text: this.$t("common.actions"),
          value: "controls",
          sortable: false,
          filterable: false,
          align: "end",
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentRead
      );
    },
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      );
    },
  },
  mounted() {
    if(!this.hasReadPermission) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['commonPersonalDevelopmentSupportListTable' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async deleteItem(item) {
      if (
        await this.$refs.confirm.open(
          this.$t("common.delete"),
          this.$t("common.confirm")
        )
      ) {
        this.saving = true;

        this.$api.studentCommonPDS
          .delete(item.id)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("errors.fileDelete"));
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
  }
};
</script>
