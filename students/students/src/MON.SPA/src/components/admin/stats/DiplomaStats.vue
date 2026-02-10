<template>
  <v-card
    :loading="loading"
  >
    <v-card-title>
      <v-icon left>
        fa-certificate
      </v-icon>{{ this.$t('menu.diplomas.title') }}
    </v-card-title>
    <v-card-text
      v-if="model"
    >
      <v-row
        dense
      >
        <v-col
          cols="6"
          right
        >
          {{ this.$t('dashboards.totalFinalizedDiplomasCount') }}
        </v-col>
        <v-col
          cols="6"
        >
          {{ model.total | number('0,0', { thousandsSeparator: ' ' }) }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('dashboards.diplomasFinalizedForCurrentYearCount') }}
        </v-col>
        <v-col
          cols="6"
        >
          {{ model.totalForCurrentSchoolYear | number('0,0', { thousandsSeparator: ' ' }) }}
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  name: 'DiplomaStats',
  data() {
    return {
      loading: false,
      model: null
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.admin.getDiplomaStats()
      .then((response) => {
        if(response.data) {
          this.model = response.data;
        }
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('errors.load'));
        console.log(error.response);
      })
      .then(() => {
        this.loading = false;
      });

    }
  }
};
</script>
