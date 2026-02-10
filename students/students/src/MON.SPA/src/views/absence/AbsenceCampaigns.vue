<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'absenceCampaignsGrid' + _uid"
      url="/api/absenceCampaign/list"
      file-export-name="Списък с кампании за подаване на отсъствия"
      :headers="headers"
      :title="$t('absenceCampaign.title')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :item-class="itemRowBackground"
    >
      <template v-slot:[`item.month`]="props">
        {{ $helper.getMonthName(props.item.month) }}
      </template>

      <template v-slot:[`item.isManuallyActivated`]="props">
        <v-chip
          :color="props.item.isManuallyActivated === true ? 'success' : 'light'"
          outlined
          small
        >
          <yes-no
            :value="props.item.isManuallyActivated"
          />
        </v-chip>
      </template>

      <template v-slot:[`item.hasAspAskingSession`]="props">
        <v-chip
          :color="props.item.hasAspAskingSession === true ? 'success' : 'light'"
          outlined
          small
        >
          <yes-no
            :value="props.item.hasAspAskingSession"
          />
        </v-chip>
      </template>

      <template v-slot:[`item.fromDate`]="props">
        <del v-if="props.item.isManuallyActivated">
          {{ props.item.fromDate ? $moment(props.item.fromDate).format(dateFormat) : "" }}
        </del>
        <span v-else>
          {{ props.item.fromDate ? $moment(props.item.fromDate).format(dateFormat) : "" }}
        </span>
      </template>

      <template v-slot:[`item.toDate`]="props">
        <del v-if="props.item.isManuallyActivated">
          {{ props.item.toDate ? $moment(props.item.toDate).format(dateFormat) : "" }}
        </del>
        <span v-else>
          {{ props.item.toDate ? $moment(props.item.toDate).format(dateFormat) : "" }}
        </span>
      </template>

      <template v-slot:[`item.isActive`]="props">
        <v-chip
          v-if="props.item.isActive"
          color="success"
          small
        >
          {{ $t('common.active_a') }}
        </v-chip>
      </template>

      <template #actions="item">
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
            :to="`/absence/campaign/${item.item.id}/details`"
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
            :to="`/absence/campaign/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            :icon-name="item.item.isManuallyActivated ? 'mdi-lock' : 'mdi-lock-open-variant'"
            icon-color="primary"
            :tooltip="item.item.isManuallyActivated ? 'absenceCampaign.manuallyActivationOff' : 'absenceCampaign.manuallyActivationOn'"
            bottom
            iclass=""
            small
            @click="toggleManuallyActivation(item.item)"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isUpcoming"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteCampaign(item.item)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            v-if="hasManagePermission"
            small
            color="primary"
            :to="`/absence/campaign/create`"
          >
            {{ $t("buttons.newRecord") }}
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
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";

import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: 'AbsenceCampaigns',
  components: {
    Grid
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
          text: this.$t('absenceCampaign.headers.name'),
          value: "name",
        },
        // {
        //   text: this.$t('absenceCampaign.headers.description'),
        //   value: "description",
        // },
        {
          text: this.$t('absenceCampaign.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('absenceCampaign.headers.month'),
          value: "month",
        },
        {
          text: this.$t('absenceCampaign.headers.fromDate'),
          value: "fromDate",
        },
        {
          text: this.$t('absenceCampaign.headers.toDate'),
          value: "toDate",
        },
        {
          text: this.$t('absenceCampaign.headers.isManuallyActivated'),
          value: "isManuallyActivated",
        },
        {
          text: this.$t('absenceCampaign.headers.hasAspAskingSession'),
          value: "hasAspAskingSession",
        },
        {
          text: '',
          value: "isActive",
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
    };
  },
  computed: {
    ...mapGetters(["hasPermission"]),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceCampaignManage);
    },
  },
  mounted() {
    if (!this.hasReadPermission){
      return this.$router.push("/errors/AccessDenied");
    }

    this.$studentHub.$on('absence-campaign-modified', this.onAbsenceCampaignModified);
  },
  destroyed() {
    this.$studentHub.$off('absence-campaign-modified');
  },
  methods: {
    gridReload() {
      const grid = this.$refs['absenceCampaignsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    toggleManuallyActivation(item) {
      this.saving = true;

      this.$api.absenceCampaign.toggleManuallyActivation(item)
      .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.gridReload();
      })
      .catch((error) => {
          this.$notifier.error('',this.$t("common.saveError"), 5000);
          console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
    async deleteCampaign(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.absenceCampaign.delete(item.id)
        .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.gridReload();
        })
        .catch((error) => {
            this.$notifier.error('',this.$t("common.deleteError"), 5000);
            console.log(error.response);
        })
        .finally(() => { this.saving = false; });
      }

    },
    itemRowBackground(item) {
      return item.isActive ? 'custom-grid-row left border-success' : '';
    },
    onAbsenceCampaignModified(id) {
      console.log(id);
      this.gridReload();
    }
  }
};
</script>
