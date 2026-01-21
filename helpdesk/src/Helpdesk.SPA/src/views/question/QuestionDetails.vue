<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <div v-else-if="!loading && question">
      <form-layout
        v-if="isInEditMode"
        :disabled="disabled"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ `${question.question}` }}</h3>
        </template>

        <template #default>
          <question-form
            v-if="question !== null"
            :ref="'questionForm' + _uid"
            :question="question"
            :disabled="disabled"
          />
        </template>
      </form-layout>
      <div v-else>
        <v-card-title>
          <v-spacer />
          <button-tip
            v-if="!isInEditMode && hasEditPermission"
            icon-name="mdi-pencil"

            iclass=""
            small
            tooltip="buttons.edit"
            text="buttons.edit"
            bottom
            raised

            @click="onEditModeClick"
          />
        </v-card-title>
        <v-card-subtitle />
        <v-card-text>
          <question-details
            class="mt-3"
            :question="question"
          />
        </v-card-text>
      </div>
    </div>

    <v-tooltip right>
      <template v-slot:activator="{ on: goBackBtnTooltip }">
        <v-fab-transition>
          <v-btn
            class="md-1 mr-3 elevation-21"
            transition="fab-transition"
            dark
            fab
            button
            fixed
            left
            bottom
            color="primary"
            v-on="{ ...goBackBtnTooltip }"
            @click.stop="$router.go(-1)"
          >
            <v-icon dark>
              fa-chevron-left
            </v-icon>
          </v-btn>
        </v-fab-transition>
      </template>
      <span>{{ $t('buttons.back') }}</span>
    </v-tooltip>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import { QuestionModel } from "@/models/questionModel";
import QuestionForm from "@/components/question/QuestionForm.vue";
import QuestionDetails from "@/components/question/QuestionDetails.vue";
import { mapGetters } from "vuex";
import { UserRole } from "../../enums/enums";

export default {
  name: "EditQuestionView",
  components: {
    QuestionForm,
    QuestionDetails,
  },
  props: {
    questionId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      saving: false,
      question: null,
      isInEditMode: false,
    };
  },
  computed: {
    ...mapGetters(['priorityColor', 'statusColor', 'statusText', 'isInRole']),
    hasEditPermission() {
      return this.isElevated;
    },
 isElevated() {
      return this.isInRole(UserRole.Consortium);
    },
    disabled() {
      return this.saving || !this.isInEditMode;
    },
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      if (this.isInEditMode){
      this.$api.question
        .getEditModelById(this.questionId)
        .then((response) => {
          if (response.data) {
            this.question = new QuestionModel(response.data);
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
      }
      else{
      this.$api.question
        .getById(this.questionId)
        .then((response) => {
          if (response.data) {
            this.question = new QuestionModel(response.data);
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
      }
    },
    async onSave() {
      const form = this.$refs["questionForm" + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error(
          "",
          this.$t("errors.validationErrors"),
          5000
        );
      }

      this.saving = true;
      this.$api.question
        .update(this.question)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            error?.response?.data?.message ?? this.$t("errors.saveError"),
            7000
          );
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.load();
      this.isInEditMode = false;
    },
    onEditModeClick() {
      if (!this.hasEditPermission) {
        return this.$notifier.warn("", this.$t("errors.accessDenied"), 5000);
      }

      this.isInEditMode = !this.isInEditMode;
      this.load();
    },
    onCommentSave() {
      this.load();
      this.isInEditMode = false;
    }
  },
};
</script>
