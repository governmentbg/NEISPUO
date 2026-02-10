<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout>
        <template #title>
          <h3>{{ $t("reassessment.reviewTitle") }}</h3>
        </template>

        <template #default>
          <reassessment-form
            v-if="document !== null"
            :ref="'reassessmentForm' + _uid"
            :document="document"
            disabled
          />
        </template>

        <template #actions>
          <v-spacer />
          <v-btn
            raised
            color="primary"
            @click.stop="backClick"
          >
            <v-icon left>
              fas fa-chevron-left
            </v-icon>
            {{ $t("buttons.back") }}
          </v-btn>
        </template>
      </form-layout>
    </div>
  </div>
</template>

<script>
import ReassessmentForm from "@/components/students/ReassessmentForm.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import { ReassessmentModel } from "@/models/reassessmentModel";

export default {
  name: "StudentReassessmentDetails",
  components: { ReassessmentForm },
  props: {
    pid: {
      type: Number,
      required: true,
    },
    docId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission"]),
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
        .getById(this.docId)
        .then((response) => {
          if (response.data) {
            this.document = new ReassessmentModel(response.data);
          }
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            this.$t("documents.documentLoadErrorMessage", 5000)
          );
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    backClick() {
      this.$router.go(-1);
    },
  },
};
</script>
