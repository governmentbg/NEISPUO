<template>
  <v-card>
    <v-card-title>
      {{ $t("student.menu.reassessment") }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="reassessments"
        class="elevation-1"
        :loading="loading"
        :search="search"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <v-spacer />
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              :label="$t('common.search')"
              single-line
              hide-details
            />
          </v-toolbar>
        </template>
        <template v-slot:[`item.creationDate`]="props">
          {{
            props.item.creationDate
              ? $moment.utc(props.item.creationDate).local().format(dateFormat)
              : ""
          }}
        </template>
        <template v-slot:[`item.id`]="{ item }">
          <doc-downloader
            v-for="document in item.documents"
            :key="document.id"
            :value="document"
            small
          />
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
              :to="`/student/${pid}/reassessments/${item.id}/details`"
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
              :to="`/student/${pid}/reassessments/${item.id}/edit`"
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
              @click="deleteItem(item)"
            />
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${pid}/reassessments/create`"
            >
              {{ $t("buttons.newRecord") }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click="load"
            >
              {{ $t("buttons.reload") }}
            </v-btn>
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";
import Constants from "@/common/constants.js";
import DocDownloader from "@/components/common/DocDownloader.vue";

export default {
  name: "StudentReassessmentsList",
  components: { DocDownloader },
  props: {
    pid: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      search: "",
      loading: false,
      reassessments: [],
      headers: [
        {
          text: this.$t("reassessment.headers.reason"),
          value: "reason",
          sortable: false,
        },
        {
          text: this.$t("reassessment.headers.schoolYear"),
          value: "schoolYear",
          sortable: false,
        },
        {
          text: this.$t("reassessment.headers.inClass"),
          value: "inClass",
          sortable: false,
        },
        {
          text: this.$t("reassessment.headers.date"),
          value: "creationDate",
          sortable: false,
        },
        {
          text: this.$t("otherDocuments.attachedDocs"),
          value: "id",
          sortable: false,
          filterable: false,
        },
        {
          text: this.$t("common.actions"),
          value: "actions",
          sortable: false,
          align: "end",
        },
      ],
      dateFormat: Constants.DATEPICKER_FORMAT,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission"]),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentReassessmentRead
      );
    },
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentReassessmentManage
      );
    },
  },
  mounted() {
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentReassessmentRead
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.reassessment
        .getListForPerson(this.pid)
        .then((response) => {
          if (response.data) {
            this.reassessments = response.data;
          }
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            this.$t("documents.otherDocumentsLoadErrorMessage")
          );
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    async deleteItem(item) {
      if (
        await this.$refs.confirm.open(
          this.$t("common.delete"),
          this.$t("common.confirm")
        )
      ) {
        this.saving = true;

        this.$api.reassessment
          .delete(item.id)
          .then(() => {
            this.load();
            this.$notifier.success("", this.$t("common.deleteSuccess"), 5000);
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("errors.fileDelete"));
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
        this.saving = false;
      }
    },
  },
};
</script>
