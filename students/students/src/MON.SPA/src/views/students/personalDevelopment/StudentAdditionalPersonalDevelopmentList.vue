<template>
  <div>
    <grid
      :ref="'additionalPersonalDevelopmentSupportListTable' + _uid"
      url="/api/AdditionalPersonalDevelopmentSupport/list"
      :headers="headers"
      :title="$t('lod.additionalPersonalDevelopmentSupport')"
      :filter="{
        studentId: personId
      }"
    >
      <!-- <template v-slot:[`item.items`]="{ item }">
        <v-chip
          v-for="reason in item.items"
          :key="reason.id"
          color="light"
          small
          label
          class="mr-1"
        >
          {{ reason.typeName }}
        </v-chip>
      </template> -->
      <template v-slot:[`item.orders`]="{ item }">
        <doc-downloader
          v-for="document in item.orders"
          :key="document.id"
          :value="document"
          small
        />
        <doc-downloader
          v-for="document in item.scorecards"
          :key="document.id"
          :value="document"
          small
        />
        <doc-downloader
          v-for="document in item.plans"
          :key="document.id"
          :value="document"
          small
        />
        <doc-downloader
          v-for="document in item.documents"
          :key="document.id"
          :value="document"
          small
        />
      </template>

      <template v-slot:[`item.orderDate`]="{ item }">
        {{ item.orderDate ? $moment(item.orderDate).format(dateFormat) : '' }}
      </template>

      <template v-slot:[`item.studentTypeName`]="{ item }">
        {{ `${item.periodTypeName} - ${item.studentTypeName}` }}
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
            :to="`/student/${personId}/additionalPersonalDevelopment/${item.id}/details`"
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
            :to="`/student/${personId}/additionalPersonalDevelopment/${item.id}/edit`"
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
          :to="`/student/${personId}/additionalPersonalDevelopment/create`"
        >
          {{ $t("buttons.newRecord") }}
        </v-btn>
        <v-btn
          v-if="validationError"
          color="error"
          small
          @click="dialog = true"
        >
          <v-icon
            class="mr-2"
            color="white"
            small
          >
            mdi-alert-circle-outline
          </v-icon>
          Виж детайли на последната грешка
        </v-btn>
      </template>
    </grid>
    <confirm-dlg ref="confirm" />
    <v-dialog
      v-model="dialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        color="red"
        outlined
      >
        <v-btn
          icon
          dark
          @click="dialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-spacer />
        <v-toolbar-items>
          <button-tip
            icon
            icon-name="fa-copy"
            tooltip="buttons.copy"
            bottom
            iclass=""
            small
            @click="copyError()"
          />
        </v-toolbar-items>
      </v-toolbar>

      <api-error-details :value="validationError" />
    </v-dialog>
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import DocDownloader from "@/components/common/DocDownloader.vue";
import ApiErrorDetails from "@/components/admin/ApiErrorDetails.vue";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import Constants from "@/common/constants.js";

export default {
  name: 'StudentAdditionalPersonalDevelopmentListView',
  components: {
    Grid,
    DocDownloader,
    ApiErrorDetails
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
      validationError: null,
      dialog: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
          text: this.$t("common.schoolYear"),
          value: "schoolYearName",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$t("additionalPersonalDevelopment.studetType"),
          value: "studentTypeName",
          sortable: false,
          filterable: false,
        },
        {
          text: this.$t("additionalPersonalDevelopment.finalSchoolYear"),
          value: "finalSchoolYearName",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$t("resourceSupport.numberLabel"),
          value: "orderNumber",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$t("resourceSupport.reportDate"),
          value: "orderDate",
          sortable: true,
          filterable: true,
        },
        {
          text: this.$t("common.attachedFiles"),
          value: "orders",
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
    ...mapGetters(['hasStudentPermission', 'userDetails']),
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
      const grid = this.$refs['additionalPersonalDevelopmentSupportListTable' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async deleteItem(item) {
      if (await this.$refs.confirm.open(this.$t("common.delete"), this.$t("common.confirm")))
      {
        this.validationError = '';
        this.saving = true;

        this.$api.studentAdditionalPDS
          .delete(item.id)
          .then(() => {
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
            this.gridReload();
          })
          .catch((error) => {
            const { message, errors } = this.$helper.parseError(error.response);
            this.validationError = { date: new Date(), ...error.response.data };
            this.$notifier.modalError(message, errors);
            this.$helper.logError({ action: "AdditionalPersonalDevelopmentDelete", message: message }, errors, this.userDetails);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    copyError() {
      let vm = this;
      navigator.clipboard.writeText(JSON.stringify(this.validationError)).then(
        function () {
          vm.$notifier.success("", vm.$t("common.clipboardSuccess"));
        },
        function () {
          vm.$notifier.error("", vm.$t("common.clipboardError"));
        }
      );
    },
  }
};
</script>
