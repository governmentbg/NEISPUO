<template>
  <div
    class="my-2"
  >
    <form-layout>
      <template #title>
        <h3 />
      </template>

      <template #default>
        <v-form
          v-if="model"
          :ref="'issueCommentForm' + _uid"
          :disabled="saving"
        >
          <v-row>
            <v-col
              cols="12"
            >
              <v-textarea
                v-model="model.comment"
                :label="$t('issueComment.comment')"
                prepend-icon="mdi-comment"
                outlined
                clearable
              />
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <file-manager
                v-model="model.documents"
                :disabled="disabled"
              />
            </v-col>
          </v-row>
        </v-form>
      </template>

      <template #actions>
        <v-spacer />
        <v-btn
          v-if="hasResolvePermission && model"
          raised
          color="primary"
          @click.stop="onResolve"
        >
          <v-icon left>
            mdi-check
          </v-icon>
          {{ model.comment ? $t('issue.resolveWithComment') : $t('issue.resolve') }}
        </v-btn>
        <v-btn
          v-if="hasCommentPermission"
          raised
          color="success"
          :disabled="!model || !model.comment"
          @click.stop="onComment"
        >
          {{ $t('buttons.comment') }}
        </v-btn>
      </template>
    </form-layout>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { IssueCommentModel } from '@/models/issueCommentModel';
import FileManager from '@/components/wrappers/FileManager.vue';

export default {
  name: 'IssueComment',
  components: {
    FileManager
  },
  props: {
    issue: {
      type: Object,
      required: true
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      saving: false,
      model: null
    };
  },
  computed: {
    hasCommentPermission() {
      if(this.issue.statusId === 3) return false;
      return true;
    },
    hasResolvePermission() {
      if(this.issue.statusId === 3) return false;
      return true;
    }
  },
  mounted() {
    this.model = new IssueCommentModel({ issueId: this.issue.id});
  },
  methods: {
    onComment() {
        if(!this.model.comment) {
          return this.$notifier.warn('', this.$t('errors.nothingToComment'), 5000);
        }

        this.saving = true;
        this.$api.issue
          .comment(this.model)
          .then(() => {
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
            this.$emit('save');
          })
          .catch((error) => {
            this.$notifier.error('', error?.response?.data?.message ?? this.$t('errors.saveError'), 7000);
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
    },
    async onResolve() {
      if(await this.$refs.confirm.open( this.$t('issueComment.resolveConfirmationTitle'), this.$t('issueComment.resolveConfirmation'))) {

        this.saving = true;
        this.$api.issue
          .resolve(this.model)
          .then(() => {
            this.$notifier.success('', this.$t('common.resolveSuccess'), 5000);
            this.$emit('save');
          })
          .catch((error) => {
            this.$notifier.error('', error?.response?.data?.message ?? this.$t('errors.resolveError'), 7000);
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    },
  }
};
</script>
