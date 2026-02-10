<template>
  <div>
    <grid
      :ref="'refugeeApplicationsGrid' + _uid"
      url="/api/refugee/applicationList"
      :headers="headers"
      :title="$t('refugee.title')"
      :expanded.sync="expanded"
      show-expand
      :single-expand="true"
      file-export-name="Списък със заявления"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    >
      <template v-slot:expanded-item="{ item }">
        <td
          v-if="item.children && item.children.length > 0"
          colspan="12"
        >
          <v-data-table
            :headers="headersChild"
            :items="item.children"
            disable-filtering
            disable-pagination
            disable-sort
            hide-default-footer
            class="ma-2"
          >
            <template v-slot:[`item.statusName`]="props">
              <v-chip
                v-if="props.item.status != undefined && props.item.status != null"
                class="ma-2"
                :color="props.item.status == 0 ? 'info' : (props.item.status == 1 ? 'success' : 'error')"
                label
                small
              >
                {{ props.item.statusName }}
              </v-chip>
            </template>
            <template v-slot:[`item.ruoDocDate`]="props">
              {{ props.item.ruoDocDate ? $moment(props.item.ruoDocDate).format(dateFormat) : '' }}
            </template>
            <template v-slot:[`item.personalId`]="props">
              {{ `${props.item.personalId} - ${props.item.personalIdTypeName}` }}
            </template>

            <template v-slot:[`item.actions`]="props">
              <button-group>
                <button-tip
                  v-if="hasManagePermission && props.item.canBeCompleted"
                  icon
                  icon-name="mdi-check"
                  icon-color="success"
                  tooltip="refugee.complete"
                  bottom
                  iclass=""
                  small
                  :disabled="saving"
                  @click="completeApplicationChild(props.item.id)"
                />
                <button-tip
                  v-if="hasManagePermission && props.item.canBeCancelled"
                  icon
                  icon-name="mdi-cancel"
                  icon-color="error"
                  tooltip="buttons.annulment"
                  bottom
                  iclass=""
                  small
                  :disabled="saving"
                  @click="cancelApplicationChild(props.item.id)"
                />
                <button-tip
                  v-if="hasManagePermission && props.item.canBeDeleted"
                  icon
                  icon-name="mdi-delete"
                  icon-color="error"
                  tooltip="buttons.delete"
                  bottom
                  iclass=""
                  small
                  :disabled="saving"
                  @click="deleteApplicationChild(props.item.id)"
                />
                <button-tip
                  v-if="hasUnlockPermission && props.item.canBeSetAsEditable"
                  icon
                  icon-name="mdi-lock-open-variant-outline"
                  icon-color="secondary"
                  tooltip="diplomas.setAsEditable"
                  bottom
                  iclass=""
                  small
                  :disabled="saving"
                  @click="unlockApplicationChild(props.item.id)"
                />
              </button-group>
            </template>
          </v-data-table>
        </td>
      </template>

      <template v-slot:[`item.applicationDate`]="{ item }">
        {{ item.applicationDate ? $moment(item.applicationDate).format(dateFormat) : "" }}
      </template>

      <template v-slot:[`item.statusName`]="{ item }">
        <v-chip
          class="ma-2"
          :color="item.status == 0 ? 'info' : (item.status == 1 ? 'success' : 'error')"
          label
          small
        >
          {{ item.statusName }}
        </v-chip>
      </template>

      <template v-slot:[`item.actions`]="{ item }">
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
            :to="`/refugee/application/${item.id}/details`"
          />
          <button-tip
            v-if="hasManagePermission && item.canBeCompleted"
            icon
            icon-name="mdi-check"
            icon-color="success"
            tooltip="refugee.complete"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="completeApplication(item.id)"
          />
          <button-tip
            v-if="hasManagePermission && item.canBeCancelled"
            icon
            icon-name="mdi-cancel"
            icon-color="error"
            tooltip="buttons.annulment"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="cancelApplication(item.id)"
          />
          <button-tip
            v-if="hasManagePermission && item.canBeEdited"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :disabled="saving"
            :to="`/refugee/application/${item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission && item.canBeDeleted"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="deleteApplication(item.id)"
          />
          <button-tip
            v-if="hasUnlockPermission && item.canBeSetAsEditable"
            icon
            icon-name="mdi-lock-open-variant-outline"
            icon-color="secondary"
            tooltip="diplomas.setAsEditable"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="unlockApplication(item.id)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            v-if="hasManagePermission"
            small
            color="primary"
            :to="`/refugee/application/create`"
          >
            {{ $t("refugee.newApplication") }}
          </v-btn>
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

    <prompt-dlg
      ref="cancellationPrompt"
      persistent
    >
      <template>
        <v-textarea
          v-model="cancellationReason"
          :label="$t('common.cancellationReason')"
          outlined
          filled
          auto-grow
          clearable
          :rules="[$validator.required()]"
          class="required"
        />
      </template>
    </prompt-dlg>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: "RefugeeApplicationListView",
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
        { text: "№", value: "id", sortable: true },
        {
          text: this.$t("refugee.headers.applicationDate"),
          value: "applicationDate",
          sortable: true,
        },
        {
          text: this.$t("refugee.headers.applicantFullName"),
          value: "applicantFullName",
          sortable: true,
        },
        {
          text: this.$t("refugee.headers.nationality"),
          value: "nationality",
          sortable: true,
        },
        {
          text: this.$t("refugee.headers.region"),
          value: "region",
          sortable: true,
        },
        {
          text: this.$t("refugee.headers.childrenCount"),
          value: "childrenCount",
          sortable: true,
        },
        {
          text: this.$t("refugee.headers.childrenWithOrderCount"),
          value: "childrenWithOrderCount",
          sortable: true,
        },
        {
          text: this.$t("refugee.headers.status"),
          value: "statusName",
        },
        {
          text: "",
          value: "actions",
          inFavourOfIdentifier: false,
          sortable: false,
          align: "end",
        },
      ],
      headersChild: [
        {
          text: this.$t("refugee.headers.firstName"),
          value: "fullName",
        },
        {
          text: this.$t("refugee.headers.personalIdTypeName"),
          value: "personalId",
        },
        {
          text: this.$t("documents.institutionDropdownLabel"),
          value: "institution",
        },
        {
          text: this.$t("refugee.headers.institutionCode"),
          value: "institutionId",
        },
        {
          text: this.$t("refugee.headers.ruoDocNumber"),
          value: "ruoDocNumber",
        },
        {
          text: this.$t("refugee.headers.ruoDocDate"),
          value: "ruoDocDate",
        },
        {
          text: this.$t("refugee.headers.status"),
          value: "statusName",
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
        Permissions.PermissionNameForRefugeeApplicationsRead
      );
    },
    hasManagePermission() {
      return this.hasPermission(
        Permissions.PermissionNameForRefugeeApplicationsManage
      );
    },
    hasUnlockPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForRefugeeApplicationsUnlock
      );
    }
  },
  mounted() {
    if (
      !this.hasPermission(
        Permissions.PermissionNameForRefugeeApplicationsManage
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }
  },
  methods: {
    async deleteApplication(id) {
      if (await this.$refs.confirm.open(this.$t("buttons.delete"),this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.refugee.deleteApplication(id)
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
    async deleteApplicationChild(id) {
      if (await this.$refs.confirm.open(this.$t("buttons.delete"),this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.refugee.deleteApplicationChild(id)
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
    async unlockApplication(id) {
      if (await this.$refs.confirm.open(this.$t("diplomas.setAsEditable"),this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.refugee.unlockApplication(id)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"));
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.saveError"));
            console.error(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async unlockApplicationChild(id) {
      if (await this.$refs.confirm.open(this.$t("diplomas.setAsEditable"),this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.refugee.unlockApplicationChild(id)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"));
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.saveError"));
            console.error(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async completeApplication(id) {
      if (await this.$refs.confirm.open(this.$t("refugee.complete"),this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.refugee.completeApplication(id)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"));
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.saveError"));
            console.error(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async completeApplicationChild(id) {
      if (await this.$refs.confirm.open( this.$t("refugee.complete"), this.$t("common.confirm"))) {
        this.saving = true;
        this.$api.refugee.completeApplicationChild(id)
          .then(() => {
            this.$notifier.success('', this.$t("common.saveSuccess"));
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error('', this.$t("common.saveError"));
            console.error(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async cancelApplication(id) {
       if (await this.$refs.cancellationPrompt.open('', this.$t('diplomas.annulment'))) {
        if (!this.cancellationReason) {
          return this.$notifier.error('',`${this.$t("diplomas.annulmentReasonError")}`);
        }

        this.saving = true;
        this.$api.refugee.cancelApplication({ applicationId: id, cancellationReason: this.cancellationReason })
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.saveError"), 5000);
            console.error(error.response);
          })
          .finally(() => {
            this.cancellationReason = null;
            this.saving = false;
          });
       }
    },
    async cancelApplicationChild(id) {
      if ( await this.$refs.cancellationPrompt.open('', this.$t('diplomas.annulment'))) {
        if (!this.cancellationReason) {
          return this.$notifier.error('',`${this.$t("diplomas.annulmentReasonError")}`);
        }

        this.saving = true;
        this.$api.refugee.cancelApplicationChild({ applicationChildId: id, cancellationReason: this.cancellationReason })
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("common.saveError"), 5000);
            console.error(error.response);
          })
          .finally(() => {
            this.cancellationReason = null;
            this.saving = false;
          });
       }
    },
    gridReload() {
      const grid = this.$refs["refugeeApplicationsGrid" + this._uid];
      if (grid) {
        grid.get();
      }
    },
  },
};
</script>
