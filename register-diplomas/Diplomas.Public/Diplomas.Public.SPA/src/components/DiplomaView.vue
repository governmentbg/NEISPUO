<template>
  <div>
    <v-card>
      <v-card-title />
      <v-card-text>
        <diploma-section-generator
          ref="diplomaSectionGenerator"
          :initial-data="diplomaData"
          :schema="schema"
        />
      </v-card-text>
    </v-card>
  </div>
</template>
<script>
import DiplomaSectionGenerator from './DiplomaSectionGenerator.vue';

export default {
  components: {
    DiplomaSectionGenerator
  },
  props: {
    diplomaData: {
      type: [String, Object],
      required: true
    },
    schema: {
      type: Array,
      required: true
    },
  }
};
</script>

<style lang="scss">
  #general {
    text-transform: uppercase;
    text-align: center;
    //h2 styles
    display: block;
    font-size: 1.5em;
    margin-top: 0.83em;
    margin-bottom: 0.83em;
    margin-left: 0;
    margin-right: 0;
    font-weight: bold;
  }

  #subSection,
  #schoolMaturationCommittee {
    text-align: center;
    //h4 styles
    display: block;
    font-size: 1em;
    margin-top: 1.33em;
    margin-bottom: 1.33em;
    margin-left: 0;
    margin-right: 0;
    font-weight: bold;
  }

  #eduTypeString {
    font-weight: bold;
  }
  
  .v-application .personUniqueId {
    justify-content: unset !important;
  }

  .v-application .personUniqueId div {
    max-width: unset;
  }

  .v-application .personUniqueId .col-md-4 {
    flex: 1;
  }

  .w-100 {
    width: 100%;
  }

  .w-75 {
    width: 75%;
  }

  .w-50 {
    width: 50%;
  }

  .w-33 {
    width: 33.3%;
  }

  .w-25 {
    width: 25%;
  }

  .w-20 {
    width: 20%;
  }

  .w-10 {
    width: 10%;
  }
</style>