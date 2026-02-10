<template>
  <v-alert
    v-if="hasExternalSoPrivider"
    border="left"
    colored-border
    type="warning"
    elevation="2"
  >
    ВАЖНО! За текущата учебна година е направен избор за външен доставчик на СО. Данни в този раздел на ЛОД се попълват само във външното приложение.
  </v-alert>
</template>


<script>
import { mapGetters } from 'vuex';

export default {
  name: 'InstitutionExternalSoProviderChecker',
  data() {
    return {
      hasExternalSoPrivider: null
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId'])
  },
  mounted() {
    if (this.userInstitutionId) {
      this.checkForExternalSoProvider();
    }
  },
  methods: {
    checkForExternalSoProvider() {
      this.$api.institution.hasExternalSoProviderForLoggedInstitution()
      .then((response) => {
        this.hasExternalSoPrivider = response.data;
      })
      .catch((error) => {
        console.log(error.response);
      });
    }
  }
};
</script>
