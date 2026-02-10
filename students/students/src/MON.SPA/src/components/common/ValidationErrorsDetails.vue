<template>
  <v-expansion-panels
    v-if="showDetails"
  >
    <v-expansion-panel>
      <v-expansion-panel-header>
        <v-alert
          border="left"
          colored-border
          type="error"
          elevation="2"
        >
          {{ $t('validation.hasErrors') }}
        </v-alert>
      </v-expansion-panel-header>
      <v-expansion-panel-content>
        <vue-json-pretty
          path="res"
          :data="value"
          show-length
        />
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script>
  import VueJsonPretty from 'vue-json-pretty';
  import 'vue-json-pretty/lib/styles.css';

export default {
  name: 'ValidationErrorsDetailsComponent',
  components: {
    VueJsonPretty
  },
  props: {
    value: {
      type: [Object, Array],
      default() {
        return null;
      }
    }
  },
  computed: {
    showDetails() {
      if (!this.value) {
        return false;
      }

      if(Array.isArray(this.value) && this.value.length === 0) {
        return false;
      }

      return true;
    }
  }
};
</script>
