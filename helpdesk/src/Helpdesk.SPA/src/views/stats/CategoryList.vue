<template>
  <div>
    <grid
      :ref="'categoryGrid' + _uid"
      url="/api/statistics/categoryList"
      :headers="headers"
      :title="$t('stats.category.home')"
      class="text-small"
      :filter="categoryFilter"
      ref-key="categoryGrid"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :file-export-name="$t('stats.category.home')"
    >
      <template v-slot:[`item.oldest`]="item">
        <span>{{
          item && item.item.oldest
            ? $moment.utc(item.item.oldest).local().format(dateTimeFormat)
            : ""
        }}</span>
      </template>

      <!-- <template #actions="item">
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
            :to="`/stats/category/${item.item.id}`"
          />
        </button-group>
      </template> -->
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
import { mapGetters } from "vuex";
import { UserRole } from '@/enums/enums';

export default {
  name: "CategoryStats",
  components: {
    Grid
  },
  data() {
    return {
      saving: false,
      categoryFilter: {},
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      headers: [
        {
          text: "№",
          value: "id",
          sortable: true,
        },
        {
          text: this.$t("stats.category.headers.code"),
          value: "code",
          sortable: true,
        },
        {
          text: this.$t("stats.category.headers.name"),
          value: "name",
          sortable: true,
        },
        {
          text: this.$t("stats.category.headers.allIssues"),
          value: "allIssues",
          sortable: true,
        },
                {
          text: this.$t("stats.category.headers.new"),
          value: "newIssues",
          sortable: true,
        },
        {
          text: this.$t("stats.category.headers.assigned"),
          value: "assignedIssues",
          sortable: true,
        },
        {
          text: this.$t("stats.category.headers.resolved"),
          value: "resolvedIssues",
          sortable: true,
        },
        {
          text: this.$t("stats.category.headers.oldest"),
          value: "oldest",
          sortable: true,
        },
        { text: "", value: "controls", sortable: false, align: "end" },
      ],
    };
  },
  computed: {
    ...mapGetters(["statusText", "statusColor", "priorityColor", "isInRole"]),
     isElevated() {
      return this.isInRole(UserRole.Consortium);
    },
  },
  methods: {
    reloadGrid() {
      const grid = this.$refs[`categoryGrid${this._uid}`];
      if(grid) {
        grid.get();
      }
    }
  }
};
</script>

<style scoped>
.text-small {
  font-size: 0.8rem;
}
</style>
