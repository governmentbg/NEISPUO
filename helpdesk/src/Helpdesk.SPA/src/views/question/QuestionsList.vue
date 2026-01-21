<template>
  <div>
    <grid
      :ref="'questionsGrid' + _uid"
      url="/api/question/list"
      :headers="headers"
      :title="$t('question.list')"
      class="text-small"
      :filter="questionsFilter"
      ref-key="questionsGrid"
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

      <template #subtitle />

      <template #createDate="item">
        <span>{{
          item && item.item.createDate
            ? $moment.utc(item.item.createDate).local().format(dateTimeFormat)
            : ""
        }}</span>
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
            :to="`/question/${item.item.id}`"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            v-if="isElevated"
            small
            color="primary"
            :to="`/question/create`"
          >
            {{ $t("question.newBtn") }}
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
import { mapGetters } from "vuex";
import { UserRole } from '@/enums/enums';

export default {
  name: "Home",
  components: {
    Grid
  },
  data() {
    return {
      saving: false,
      questionsFilter: {},
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      headers: [
        {
          text: "№",
          value: "id",
          sortable: true,
        },
        {
          text: this.$t("question.headers.question"),
          value: "question",
          sortable: true,
        },
        {
          text: this.$t("question.headers.createDate"),
          value: "createDate",
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
      const grid = this.$refs[`questionsGrid${this._uid}`];
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
