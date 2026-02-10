<template>
  <div>
    <grid
      :ref="'issuesGrid' + _uid"
      url="/api/issue/list"
      :headers="headers"
      :title="$t('issue.list')"
      class="text-small"
      :filter="issuesFilter"
      ref-key="issuesGrid"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :file-export-name="$t('issue.list')"
    >
      <template #title="item">
        <v-tooltip bottom>
          <template v-slot:activator="{ on: tooltip }">
            <v-icon
              v-if="item.item.statusId === 1"
              color="primary"
              v-on="{ ...tooltip }"
            >
              mdi-label
            </v-icon>
            <v-icon
              v-if="item.item.statusId === 2"
              color="info"
              v-on="{ ...tooltip }"
            >
              mdi-bullseye
            </v-icon>
            <v-icon
              v-if="item.item.statusId === 3"
              color="success"
              v-on="{ ...tooltip }"
            >
              mdi-check
            </v-icon>
          </template>
          <span>{{ statusText(item.item.statusId) }}</span>
        </v-tooltip>
        <span class="ml-2">
          {{ item.item.title }}
        </span>
      </template>

      <template #subtitle>
        <issue-filter
          v-model="issuesFilter"
          class="mt-2 px-4"
        />
      </template>

      <template v-slot:[`item.id`]="{ item }">
        <v-tooltip
          v-if="item.hasUnreadChanges === true"
          bottom
        >
          <template v-slot:activator="{ on: unreadChangesIcon }">
            <v-icon
              color="warning"
              v-on="{ ...unreadChangesIcon }"
            >
              mdi-message-badge-outline
            </v-icon>
          </template>
          <span> {{ $t('issue.unreadChanges') }} </span>
        </v-tooltip>
        <v-tooltip
          v-if="item.requestForInformation === true"
          bottom
        >
          <template v-slot:activator="{ on: requestForInfomationIcon }">
            <v-icon
              color="error"
              v-on="{ ...requestForInfomationIcon }"
            >
              mdi-flash-alert
            </v-icon>
          </template>
          <span> {{ $t('issue.requestForInformation') }} </span>
        </v-tooltip>
        {{ item.id }}
      </template>

      <template #status="item">
        <v-chip
          v-if="item.item.status"
          class="ma-2"
          :color="statusColor(item.item.statusId)"
          label
          small
        >
          {{ item.item.status }}
        </v-chip>
      </template>

      <template #isEscalated="item">
        <div v-if="item.item.isLevel3Support">
          <v-chip
            v-if="item.item.isLevel3Support"
            class="ma-2"
            color="error"
            label
            small
          >
            <v-icon left>
              mdi-arrow-up-bold
            </v-icon>
            {{ $t("issue.isLevel3Support") }}
          </v-chip>
        </div>
        <div v-else>
          <v-chip
            v-if="item.item.isEscalated"
            class="ma-2"
            color="warning"
            label
            small
          >
            {{ $t("issue.isEscalated") }}
          </v-chip>
        </div>
      </template>

      <template #priority="item">
        <v-chip
          v-if="item.item.priority"
          class="ma-2"
          :color="priorityColor(item.item.priorityId)"
          label
          small
        >
          {{ item.item.priority }}
        </v-chip>
      </template>

      <template #commentsCount="item">
        <div
          v-if="item.item.commentsCount > 0"
          class="text-small"
        >
          <v-icon
            x-small
            left
            class="mr-0"
          >
            far fa-comment-alt fa-xs
          </v-icon>
          {{ item.item.commentsCount }}
        </div>
      </template>
      <template #attachmentsCount="item">
        <div
          v-if="item.item.attachmentsCount > 0"
          class="text-small"
        >
          <v-icon
            x-small
            left
            class="mr-0"
          >
            fas fa-paperclip fa-xs
          </v-icon>
          {{ item.item.attachmentsCount }}
        </div>
      </template>

      <template #createDate="item">
        <span>{{
          item && item.item.createDate
            ? $moment.utc(item.item.createDate).local().format(dateTimeFormat)
            : ""
        }}</span>
      </template>

      <template #lastActivityDate="item">
        <span>{{
          item && item.item.lastActivityDate
            ? $moment.utc(item.item.lastActivityDate).local().format(dateTimeFormat)
            : ""
        }}</span>
      </template>

      <template #resolveDate="item">
        <span>{{
          item && item.item.resolveDate
            ? $moment.utc(item.item.resolveDate).local().format(dateTimeFormat)
            : ""
        }}</span>
      </template>

      <template #assignedToSysUser="item">
        <span v-if="item.item.assignedToSysUser">
          {{ item.item.assignedToSysUser }}
        </span>
        <!-- Бутонът за назначаване на задача на себе си. Да се помисли дали трябва да се вижда и тук. -->
        <!-- <span
          v-else
          class="float-right"
        >
          <button-tip
            icon
            icon-name="mdi-share"
            icon-color="primary"
            iclass=""
            tooltip="issue.assigneToMyselfTooltip"
            bottom
            small
            raised
            fab
            @click="assignToMyself(item.item.id)"
          />
        </span> -->
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            icon
            icon-name="mdi-details"
            icon-color="primary"
            iclass=""
            small
            tooltip="buttons.details"
            bottom
            raised
            fab
            :to="`/issue/${item.item.id}`"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            small
            color="primary"
            :to="`/issue/create`"
          >
            {{ $t("issue.newBtn") }}
          </v-btn>
        </button-group>
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
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import IssueFilter from "@/components/issue/IssueFilter.vue";
import { mapGetters } from "vuex";

export default {
  name: "Home",
  components: {
    Grid,
    IssueFilter,
  },
  data() {
    return {
      saving: false,
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      headers: [
        {
          text: "№",
          value: "id",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.submitterUsername"),
          value: "submitterUsername",
          sortable: false,
        },
        {
          text: this.$t("issue.headers.title"),
          value: "title",
          sortable: false,
        },
        {
          text: this.$t("issue.headers.category"),
          value: "category",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.subcategory"),
          value: "subcategory",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.status"),
          value: "status",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.priority"),
          value: "priority",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.isEscalated"),
          value: "isEscalated",
          sortable: false,
        },
        {
          text: this.$t("issue.headers.createDate"),
          value: "createDate",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.resolveDate"),
          value: "resolveDate",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.lastActivityDate"),
          value: "lastActivityDate",
          sortable: true,
        },
        {
          text: this.$t("issue.headers.assignee"),
          value: "assignedToSysUser",
          sortable: true,
        },
        {
          text: "",
          value: "commentsCount",
          sortable: false,
        },
        {
          text: "",
          value: "attachmentsCount",
          sortable: false,
        },

        { text: "", value: "controls", sortable: false, align: "end" },
      ],
    };
  },
  computed: {
    ...mapGetters(['statusText', 'statusColor', 'priorityColor']),
    issuesFilter: {
      get () {
        return this.$store.state.issuesFilter;
      },
      set (value) {
        this.$store.commit('updateIssuesFilter', value);
      }
    }
  },
  methods: {
    assignToMyself(issueId) {
      this.saving = true;
      this.$api.issue
        .assignToMyself(issueId)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          // По някаква причини при презареждане на грида чрез викане на .get() метода му забива UI-a
          // с грешка NotFoundError: Failed to execute 'insertBefore' on 'Node': The node before which the new node is to be inserted is not a child of this node. Info: nextTick
          // Поради тази причина презареждаме странцата, поне докато не решим този проблем.
          // this.reloadGrid();
          this.$router.go();
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            error?.response?.data?.message ?? this.$t("errors.saveError"),
            7000
          );
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    reloadGrid() {
      const grid = this.$refs[`issuesGrid${this._uid}`];
      if (grid) {
        grid.get();
      }
    },
  },
};
</script>

<style scoped>
.text-small {
  font-size: 0.8rem;
}
</style>
