<template>
  <div class="locale-switch">
    <v-select
      v-model="$i18n.locale"
      :items="langs"
      item-text="description"
      item-value="name"
      single-line
      prepend-icon="mdi-web"
      dense
      @change="changeLocale()"
    />
  </div>
</template>

<script>
export default {
  name: "LocaleSwitch",
  data() {
    return {
      langs: [
        { name: "bg", description: "Български" },
        { name: "en", description: "English" },
      ],
    };
  },
  methods: {
    changeLocale() {
      this.$vuetify.lang.current = this.$i18n.locale;
      localStorage.setItem('lang', this.$i18n.locale);
    },
  },
};
</script>


<style scoped>
.locale-switch {
  width: 150px;
}
</style>



