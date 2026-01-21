<template>
  <v-card>
    <v-tabs
      v-model="tab"
      align-with-title
    >
      <v-tabs-slider color="yellow" />

      <v-tab
        v-for="diploma in diplomas"
        :key="diploma.id"
      >
        {{ diploma.basicDocument.name }}
      </v-tab>
    </v-tabs>

    <v-tabs-items v-model="tab">
      <v-tab-item
        v-for="diploma in diplomas"
        :key="diploma.id"
      >
        <diploma-details :diploma="diploma" />
      </v-tab-item>
    </v-tabs-items>
  </v-card>
</template>

<script>
import DiplomaDetails from "@/components/DiplomaDetails.vue";

export default {
  name: "Details",
  components: {
    DiplomaDetails,
  },
  props: {
    diplomas: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      tab: null,
    };
  },
};
</script>

<style scoped>
.image {
  cursor: pointer;
  background-size: cover;
}
</style>
