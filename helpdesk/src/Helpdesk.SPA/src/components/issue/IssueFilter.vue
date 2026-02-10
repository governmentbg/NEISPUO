<template>
  <v-row
    dense
  >
    <v-col
      cols="12"
      md="6"
      lg="4"
      xl="2"
    >
      <custom-autocomplete
        v-model="value.category"
        api="/api/lookups/GetCategories"
        :label="$t('issue.category')"
        clearable
        hide-no-data
        hide-selected
        no-filter
        dense
        :defer-options-loading="false"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      lg="4"
      xl="2"
    >
      <custom-autocomplete
        v-model="value.priority"
        api="/api/lookups/GetPriorities"
        :label="$t('issue.priority')"
        clearable
        hide-no-data
        hide-selected
        no-filter
        dense
        :defer-options-loading="false"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      lg="4"
      xl="2"
    >
      <custom-autocomplete
        v-model="value.status"
        api="/api/lookups/GetStatuses"
        :label="$t('issue.status')"
        clearable
        hide-no-data
        hide-selected
        no-filter
        dense
        :defer-options-loading="false"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      lg="4"
      xl="2"
    >
      <v-select
        v-model="value.supportLevel"
        :label="$t('issue.isEscalated')"
        :items="SupportLevelOptions"
        clearable
        outlined
        dense
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      lg="4"
      xl="2"
    >
      <v-select
        v-model="value.assignLevel"
        :label="$t('issue.assignLevel')"
        :items="AssignLevelOptions"
        clearable
        outlined
        dense
      />
    </v-col>

    <!-- <v-col
      cols="12"
      md="6"
      lg="4"
      xl="2"
    >
      <v-checkbox
        v-model="value.assignedToMe"
        :label="$t('issue.assignedToMe')"
        class="mt-1"
      />
    </v-col> -->
    <v-col
      cols="12"
      md="6"
      lg="2"
      xl="2"
    >
      <v-switch
        v-model="value.searchEverywhere"
        :label="$t('issue.searchEverywhere')"
        class="mt-1"
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
      lg="2"
      xl="2"
    >
      <v-switch
        v-model="value.requestForInformation"
        :label="$t('issue.requestForInformation')"
        class="mt-1"
      />
    </v-col>
  </v-row>
</template>

<script>
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'IssueFilter',
  components: {
    CustomAutocomplete,
  },
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      isEscalatedOptions: [{ value: false, text: 'Не'}, { value: true, text: 'Да'}],
      SupportLevelOptions: [{ value: 0, text: 'Не'}, { value: 1, text: 'Ескалиран'}, { value: 2, text: 'L3 поддръжка'}],
      AssignLevelOptions: [{ value: 0, text: 'без ограничение'}, { value: 1, text: 'на мен'}, { value: 2, text: 'НЕ на мен'}, { value: 3, text: 'без назначение'}]
    };
  }
};
</script>
