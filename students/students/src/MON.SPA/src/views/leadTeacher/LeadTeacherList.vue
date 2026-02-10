<template>
  <div>
    <grid
      :ref="'leadTeacherGrid' + _uid"
      class="pa-3"
      url="/api/leadTeacher/list"
      :headers="headers"
      :title="$t('leadTeacher.title')"
      file-export-name="Списък с класни ръководители на служебни паралелки"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    >
      <template v-slot:[`item.applicationDate`]="{ item }">
        {{ item.applicationDate ? $moment(item.applicationDate).format(dateFormat) : "" }}
      </template>

      <template v-slot:[`item.leadTeacherName`]="{ item }">
        <v-chip
          v-if="item.leadTeacherName"
          class="ma-2"
          color="success"
          label
          small
        >
          {{ item.leadTeacherName }}
        </v-chip>
        <v-chip
          v-else
          class="ma-2"
          :color="info"
          label
          small
        >
          Няма
        </v-chip>
      </template>

      <template v-slot:[`item.actions`]="{ item }">
        <button-group>
          <button-tip
            v-if="hasReadPermission && item.staffPositionId"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :disabled="saving"
            :to="`/leadTeacher/${item.classId}/details`"
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
            :to="`/leadTeacher/${item.classId}/edit`"
          />
          <button-tip
            v-if="hasManagePermission && item.staffPositionId"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="deleteLeadTeacher(item.classId)"
          />
        </button-group>
      </template>
    </grid>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: "LeadTeacherListView",
  components: {
    Grid,
  },
  data() {
    return {
      saving: false,
      cancellationReason: null,
      expanded: [],
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATE_FORMAT,
      headers: [
        { text: "№", value: "id", sortable: true, align: ' d-none' },
        {
          text: this.$t("leadTeacher.headers.classGroupName"),
          value: "classGroupName",
          sortable: true,
        },
        {
          text: this.$t("leadTeacher.headers.leadTeacherName"),
          value: "leadTeacherName",
          sortable: true,
        },
        {
          value: "classId",
          align: ' d-none'
        },
        {
          value: "staffPositionId",
          align: ' d-none'
        },
        {
          text: "",
          value: "actions",
          inFavourOfIdentifier: false,
          sortable: false,
          align: "end",
        },
      ],
    };
  },
  computed: {
    ...mapGetters(["hasPermission"]),
    hasReadPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForLeadTeacherManage
      );
    },
    hasManagePermission() {
      return this.hasPermission(
        Permissions.PermissionNameForLeadTeacherManage
      );
    }
    },
  methods: {
    async deleteLeadTeacher(classId) {
      if (await this.$refs.confirm.open(this.$t("buttons.delete"),this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.leadTeacher.deleteLeadTeacher(classId)
          .then(() => {
            this.$notifier.success("", this.$t("common.deleteSuccess"));
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.deleteError"));
            console.error(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    gridReload() {
      const grid = this.$refs["leadTeacherGrid" + this._uid];
      if (grid) {
        grid.get();
      }
    },
  },
};
</script>
