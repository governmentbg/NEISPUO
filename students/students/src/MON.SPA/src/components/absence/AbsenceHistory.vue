<template>
  <v-dialog
    v-model="show"
  >
    <template v-slot:activator="{ on: dialog }">
      <v-tooltip bottom>
        <template v-slot:activator="{ on: tooltip }">
          <v-btn
            icon
            color="secondary"
            small
            v-on="{ ...tooltip, ...dialog }"
          >
            <v-icon
              color="secondary"
            >
              mdi-history
            </v-icon>
          </v-btn>
        </template>
        <span>{{ $t('buttons.history') }}</span>
      </v-tooltip>
    </template>
    <v-card>
      <v-card-title>
        {{ $t("absence.historyTitle") }}
      </v-card-title>
      <v-data-table
        :headers="headers"
        :items="absenceHistory"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="absenceHistory"
              :file-extensions="['xlsx', 'csv', 'txt']"
              file-name="Списък с предишни данни"
              :headers="headers"
            />
            <v-spacer />
          </v-toolbar>
        </template>
        <template v-slot:[`item.createDate`]="{ item }">
          {{ item.createDate ? $moment(item.createDate).format(dateFormat) : '' }}
        </template>
      </v-data-table>
      <v-card-actions>
        <v-spacer />
        <v-btn
          color="blue darken-1"
          text
          @click="close"
        >
          {{ $t('buttons.cancel') }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import Constants from "@/common/constants.js";

export default {
  name: "AbsenceHistory",
  components: {GridExporter},
  props: {
    absenceId: {
      type: Number,
      default: 0
      }
  },
  data() {
    return {
      dateFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      headers: [
        {
          text: this.$t("absence.historyHeaders.excused"),
          value: "excused",
        },
        {
          text: this.$t("absence.historyHeaders.unexcused"),
          value: "unexcused",
        },
        {
          text: this.$t("absence.historyHeaders.username"),
          value: "username",
        },
        {
          text: this.$t("absence.historyHeaders.timestampUtc"),
          value: "createDate",
        },
      ],
      show: false,
      absenceHistory: [{}],
    };
  },
  watch: {
    show(val) {
      if(val === true) {
        this.load();
      }
    }
  },
  methods: {
    load() {
      this.$api.absence
        .getStudentAbsencesHistory(this.absenceId)
        .then((response) => {
          this.absenceHistory = response.data;
        })
        .then(() => (this.show = true));
    },
    close () {
        this.show = false;
    }
  },
};
</script>
