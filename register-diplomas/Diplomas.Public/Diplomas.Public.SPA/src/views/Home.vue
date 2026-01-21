<template>
  <v-container>
    <v-row>
      <v-col
        cols="12"
        sm="12"
        md="8"
      >
        <diploma-search />
      </v-col>
      <v-col
        cols="12"
        sm="12"
        md="4"
      >
        <information />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import DiplomaSearch from '@/components/DiplomaSearch.vue';
import Information from '@/components/Information.vue';

export default {
  name: "Home",
  components: {
    DiplomaSearch,
    Information
  }
};
</script>
