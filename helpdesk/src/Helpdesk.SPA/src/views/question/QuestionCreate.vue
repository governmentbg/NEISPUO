<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('question.createTitle') }}</h3>
      </template>

      <template #default>
        <question-form
          v-if="question !== null"
          :ref="'questionForm' + _uid"
          :question="question"
          :disabled="saving"
        />
      </template>
    </form-layout>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import { QuestionModel } from '@/models/questionModel';
import QuestionForm from '@/components/question/QuestionForm.vue';

export default {
  name: 'CreateQuestionView',
  components: {
    QuestionForm
  },
  data() {
    return {
      saving: false,
      question: null
    };
  },
  mounted() {
    this.question = new QuestionModel();
  },
  methods: {
    onSave() {
      const form = this.$refs['questionForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('errors.validationErrors'), 5000);
      }

      this.saving = true;
      this.$api.question
        .create(this.question)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', error?.response?.data?.message ?? this.$t('errors.saveError'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
