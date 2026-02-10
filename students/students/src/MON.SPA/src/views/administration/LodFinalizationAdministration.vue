<template>
  <v-card>
    <v-card-title>
      {{ $t('lodFinalization.title') }}
    </v-card-title>
    <v-card-text>
      <v-row dense>
        <v-col cols="12">
          <school-year-selector
            v-model="schoolYear"
            :label="$t('common.schoolYear')"
            show-current-school-year-button
          />
        </v-col>
        <v-col cols="12">
          <c-textarea
            v-model="personIds"
            label="Списък с personId, разделени с '|'"
            outlined
            persistent-placeholder
            clearable
            rows="3"
          />
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <v-btn
        outlined
        color="primary"
        @click.stop="finalizeSelected"
      >
        {{ $t("buttons.save") }}
      </v-btn>
    </v-card-actions>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';

export default {
  name: 'LodFinalizationAdministrationView',
  components: {
    SchoolYearSelector
  },
  data() {
    return {
      schoolYear: null,
      personIds: null,
      saving: false
    };
  },
  methods: {
    finalizeSelected() {
      this.saving = true;

      this.$api.administration
        .finalizeLods({
          schoolYear: this.schoolYear,
          personIds: this.personIds.split(`|`).map(x => parseInt(x))
        })
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.personIds = null;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'));
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    }
  }
};
</script>
