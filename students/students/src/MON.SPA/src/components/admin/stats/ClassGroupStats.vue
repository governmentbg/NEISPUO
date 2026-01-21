<template>
  <v-card
    :loading="loading"
  >
    <v-card-title>
      <v-icon left>
        fa-graduation-cap
      </v-icon>{{ this.$t('profile.classes') }}
    </v-card-title>
    <v-card-text
      v-if="model"
    >
      <div
        v-for="(item, index) in model"
        :key="index"
      >
        <v-row
          v-if="index < visibleItemsCount"
          dense
        >
          <v-col
            cols="6"
          >
            {{ item.classGroup }}
          </v-col>
          <v-col
            cols="6"
          >
            {{ item.count | number('0,0', { thousandsSeparator: ' ' }) }}
          </v-col>
        </v-row>
      </div>
      <v-row dense>
        <v-spacer />
        <v-btn
          v-if="model && model.length > visibleItemsCount"
          text
          plain
          small
          color="primary"
          @click.stop="visibleItemsCount = Number.MAX_SAFE_INTEGER"
        >
          <v-icon>
            mdi-chevron-down
          </v-icon>
          {{ $t('common.showMore') }}
        </v-btn>
        <v-btn
          v-if="model && visibleItemsCount > model.length"
          text
          plain
          small
          color="primary"
          @click.stop="visibleItemsCount = defaultVisibleItemsCount"
        >
          <v-icon>
            mdi-chevron-up
          </v-icon>
          {{ $t('common.showLess') }}
        </v-btn>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  name: 'ClassGroupStats',
  data() {
    return {
      loading: false,
      defaultVisibleItemsCount: 4,
      visibleItemsCount: 4,
      model: null
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.admin.getClassGroupStats()
      .then((response) => {
        if(response.data && response.data.classGroupsByType) {
          this.model = Object.entries(response.data.classGroupsByType).map(([classGroup, count]) => ({ classGroup, count }));
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
