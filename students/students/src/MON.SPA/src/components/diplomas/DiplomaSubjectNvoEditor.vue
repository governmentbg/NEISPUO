<template>
  <v-row
    dense
  >
    <v-col
      cols="12"
      md="6"
    >
      <v-text-field
        v-model.number="value.nvoPoints"
        type="number"
        :label="$t('externalEval.points')"
        persistent-hint
        :rules="[$validator.min(0), $validator.max(100)]"
        class="custom-small-text"
        v-bind="$attrs"
        v-on="$listeners"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      <autocomplete
        v-model="value.flLevel"
        api="/api/lookups/GetFLLevelOptions"
        :label="$t('externalEval.flLevel')"
        :defer-options-loading="false"
        item-value="text"
        item-text="text"
        class="custom-small-text"
        v-bind="$attrs"
        v-on="$listeners"
      />
    </v-col>
  </v-row>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'DiplomaSubjectNvoEditorComponent',
  components: {
    Autocomplete
  },
  props: {
    value: {
      type: Object,
      required: true,
    },
  },
  beforeDestroy() {
    this.value.otherGrade = null;
  }
};
</script>
