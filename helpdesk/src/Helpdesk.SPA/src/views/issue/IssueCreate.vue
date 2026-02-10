<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('issue.createTitle') }}</h3>
      </template>

      <template #default>
        <issue-form
          v-if="issue !== null"
          :ref="'issueForm' + _uid"
          :issue="issue"
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
import { IssueModel } from '@/models/issueModel';
import IssueForm from '@/components/issue/IssueForm.vue';

export default {
  name: 'CreateIssueView',
  components: {
    IssueForm
  },
  data() {
    return {
      saving: false,
      issue: null
    };
  },
  mounted() {
    this.issue = new IssueModel({ priorityId: 1 });
  },
  methods: {
    onSave() {
      const form = this.$refs['issueForm' + this._uid];
      const isValid = form.validate();
      this.issue.survey = JSON.stringify(this.issue.surveyObject);

      if(!isValid) {
        return this.$notifier.error('', this.$t('errors.validationErrors'), 5000);
      }

      this.saving = true;
      this.$api.issue
        .create(this.issue)
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
