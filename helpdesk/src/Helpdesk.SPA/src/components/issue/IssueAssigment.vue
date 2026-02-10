<template>
  <div>
    <span
      v-if="assignedTo"
    >
      {{ $t('issue.assignedTo', { name: assignedToName }) }}
    </span>
    <span
      v-else
    >
      {{ $t('issue.missingAssigment') }} -
      <v-btn
        text
        small
        @click.stop="assignToMyself"
      >
        {{ $t('issue.assigneToMyself') }}
      </v-btn>
    </span>
    <v-dialog
      v-model="show"
      max-width="1000"
    >
      <template v-slot:activator="{ on: dialog }">
        <v-tooltip bottom>
          <template v-slot:activator="{ on: tooltip }">
            <v-btn
              v-if="!disabled"
              color="primary"
              small
              v-on="{ ...tooltip, ...dialog }"
            >
              <v-icon>
                mdi-share
              </v-icon>
              {{ $t('issue.assignTo') }}
            </v-btn>
          </template>
          <span>{{ $t('issue.assignToTooltip') }}</span>
        </v-tooltip>
      </template>
      <v-card>
        <v-card-title>
          {{ $t('issue.assignToTooltip') }}
        </v-card-title>
        <v-card-text>
          <v-row
            dense
          >
            <v-col
              cols="12"
            >
              <custom-autocomplete
                v-model="assignedToSysUserId"
                api="/api/lookups/GetUsers"
                :placeholder="$t('buttons.search')"
                clearable
                hide-no-data
                hide-selected
                :defer-options-loading="false"
              />
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            outlined
            small
            raised
            color="primary"
            @click.stop="assignToSelected"
          >
            <v-icon left>
              fas fa-save
            </v-icon>
            {{ $t('buttons.assign') }}
          </v-btn>

          <v-btn
            outlined
            small
            raised
            color="error"
            @click.stop="onCancel"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'IssueAssigment',
  components: {
    CustomAutocomplete
  },
  props: {
    issueId: {
      type: Number,
      required: true
    },
    assignedTo: {
      type: Number,
      default() {
        return null;
      }
    },
    assignedToName: {
      type: String,
      default() {
        return undefined;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      saving: false,
      show: false,
      assignedToSysUserId: this.assignedTo
    };
  },
  methods: {
    assignToMyself() {
      this.saving = true;
      this.$api.issue
        .assignToMyself(this.issueId)
        .then(() => {
          this.$emit('assigned');
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        })
        .catch((error) => {
          this.$notifier.error('', error?.response?.data?.message ?? this.$t('errors.saveError'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    assignToSelected() {
      this.saving = true;
      this.$api.issue
        .assignTo(this.issueId, this.assignedToSysUserId)
        .then(() => {
          this.$emit('assigned');
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
      this.show = false;
    }
  }
};
</script>
