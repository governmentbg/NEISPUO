<template>
  <v-card>
    <v-card-title>
      {{ $t("personalDevelopment.title") }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="items"
        :loading="loading"
        :search="search"
        :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions }"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <v-toolbar-title>
              <GridExporter
                :items="items"
                :headers="headers"
                :file-extensions="['xlsx', 'csv', 'txt']"
                file-name="StudentPersonalDevelopment"
              />
            </v-toolbar-title>
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
        <template v-slot:[`item.details`]="{ item }">
          <v-chip
            v-if="
              item.earlyEvaluationReasons &&
                Array.isArray(item.earlyEvaluationReasons) &&
                item.earlyEvaluationReasons.length > 0
            "
            outlined
            small
            class="mx-1"
            color="primary"
          >
            {{ $t("lod.earlyEvaluationTitle") }}
          </v-chip>
          <v-chip
            v-if="
              item.commonSupportTypeReasons &&
                Array.isArray(item.commonSupportTypeReasons) &&
                item.commonSupportTypeReasons.length > 0
            "
            outlined
            small
            class="mx-1"
            color="primary"
          >
            {{ $t("lod.commonPersonalDevelopmentSupport") }}
          </v-chip>
          <v-chip
            v-if="
              item.additionalSupportTypeReasons &&
                Array.isArray(item.additionalSupportTypeReasons) &&
                item.additionalSupportTypeReasons.length > 0
            "
            outlined
            small
            class="mx-1"
            color="primary"
          >
            {{ $t("lod.additionalPersonalDevelopmentSupport") }}
          </v-chip>
        </template>
        <!-- <template v-slot:[`item.earlyEvaluationReasons`]="{ item }">
          <v-chip
            v-for="(detail, index) in item.earlyEvaluationReasons"
            :key="index"
            outlined
            small
            class="mx-1"
            color="primary"
          >
            {{ detail.reasonName }}
          </v-chip>
        </template>
        <template v-slot:[`item.commonSupportTypeReasons`]="{ item }">
          <v-chip
            v-for="(detail, index) in item.commonSupportTypeReasons"
            :key="index"
            outlined
            small
            class="mx-1"
            color="primary"
          >
            {{ detail.reasonName }}
          </v-chip>
        </template>
        <template v-slot:[`item.additionalSupportTypeReasons`]="{ item }">
          <v-chip
            v-for="(detail, index) in item.additionalSupportTypeReasons"
            :key="index"
            outlined
            small
            class="mx-1"
            color="primary"
          >
            {{ detail.reasonName }}
          </v-chip>
        </template> -->
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
              :to="`/student/${personId}/personalDevelopment/${item.id}/details`"
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
              :to="`/student/${personId}/personalDevelopment/${item.id}/edit`"
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
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasManagePermission"
              small
              color="primary"
              :to="`/student/${personId}/personalDevelopment/create`"
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
import DocDownloader from "@/components/common/DocDownloader.vue";
import GridExporter from "@/components/wrappers/gridExporter";
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: "StudentPersonalDevelopmentList",
  components: {
    GridExporter,
    DocDownloader
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      search: "",
      loading: false,
      items: [],
      headers: [
        {
          text: this.$t("common.schoolYear"),
          value: "schoolYearName",
          sortable: true,
        },
        { text: this.$t("buttons.details"), value: "details", sortable: true },
        // {text: this.$t('lod.earlyEvaluationTitle'), value: "earlyEvaluationReasons", sortable: true},
        // {text: this.$t('lod.commonPersonalDevelopmentSupport'), value: "commonSupportTypeReasons", sortable: true},
        // {text: this.$t('lod.additionalPersonalDevelopmentSupport'), value: "additionalSupportTypeReasons", sortable: true},
        {
          text: this.$t("recognition.headers.documents"),
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
    };
  },
  computed: {
    ...mapGetters([
      "hasStudentPermission",
      "gridItemsPerPageOptions",
    ]),
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
  created() {
    this.$studentHub.$on('personal-development-modified', this.load);
  },
  mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }
    this.load();
  },
  destroyed() {
    this.$studentHub.$off('personal-development-modified');
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentPDS
        .getListForPerson(this.personId)
        .then((response) => {
          if (response.data) {
            this.items = response.data;
          }
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            this.$t("errors.studentResourceSupportLoad")
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

        this.$api.studentPDS
          .delete(item.id)
          .then(() => {
            this.load();
            this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("errors.fileDelete"));
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
  },
};
</script>
