<template>
  <v-card>
    <v-card-title>
      {{ $t("equalization.title") }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="equalizations"
        :loading="loading"
        :search="search"
        class="elevation-1"
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
              :to="`/student/${pid}/equalization/${item.id}/details`"
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
              :to="`/student/${pid}/equalization/${item.id}/edit`"
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
              :to="`/student/${pid}/equalization/create`"
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
import DocDownloader from '@/components/common/DocDownloader.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: "StudentEqualizationsList",
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
      equalizations: [],
      expandedItems: [],
      headers: [
        {
          text: this.$t("equalization.headers.schoolYear"),
          value: "schoolYearName",
          sortable: false,
        },
        {
          text: this.$t("equalization.headers.reason"),
          value: "reason",
          sortable: false,
        },
        {
          text: this.$t("equalization.headers.equalizatedGrades"),
          value: "equalizatedGradesCount",
          sortable: false,
        },
        {
          text: this.$t("equalization.headers.inClass"),
          value: "inClass",
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
          align: 'end'
        },
      ],
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission", "gridItemsPerPageOptions"]),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentEqualizationRead
      );
    },
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentEqualizationManage
      );
    },
  },
  mounted() {
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentEqualizationRead)) {
      return this.$router.push("/errors/AccessDenied");
    }
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.equalization
        .getListForPerson(this.pid)
        .then((response) => {
          if (response.data) {
            this.equalizations = response.data;
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

        this.$api.equalization
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
