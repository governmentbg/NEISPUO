<template>
  <div>
    <v-dialog
      v-model="dialog"
      max-width="500px"
    >
      <form-layout
        v-if="model"
        skip-cancel-prompt
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <v-text-field
            :value="model.key"
            :label="$t('contextualInformation.headers.key')"
            disabled
          />
        </template>
        <template>       
          <v-textarea
            v-if="model"
            v-model="model.description"
            :label="$t('contextualInformation.headers.description')"
            outlined
            filled
            auto-grow
            clearable
          />
          <v-textarea
            v-if="model"
            v-model="model.value"
            :label="$t('contextualInformation.headers.value')"
            outlined
            filled
            auto-grow
            clearable   
          />
        </template>
      </form-layout>
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
import { mapActions } from 'vuex';

export default {
  name: 'ContextualInformationModal',
  props: {
    value: {
      type: String,
      default: null
    },
  },
  data() {
    return {
      model: null,
      dialog: false,
      saving: false,
    };
  },
  watch: {
    value(val) {
      if(val) {
        this.load();
        this.dialog = true;
      } else {
        this.model = null;
        this.dialog = false;
      }
    },
  },
  methods: {
    ...mapActions(['loadContextualInfo']),
    load() {
      this.$api.administration.getContextualInformationByKey(this.value)
      .then(result => {
        if(result.data) {
          this.model = result.data;
        } else {
          this.model = null;
        }
      })
      .catch(error => {
        console.log(error.respose);
      });
    },
    onCancel() {
      this.$emit('input', null);
      this.dialog = false;
      this.model = null;
    },
    onSave() {
      this.saving = true;
      this.$api.administration.updateContextualInformation(this.model)
      .then(() => {
        this.$emit('save');
        this.$notifier.success(this.$t('common.save'), this.$t('common.saveSuccess'), 5000);
        this.loadContextualInfo();
        this.onCancel();
      })
      .catch(error => {
        console.log(error);
      })
      .then(() => { this.saving = false; });
    },
  }
};
</script>