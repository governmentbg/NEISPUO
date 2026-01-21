<template>
  <div>
    <v-app-bar
      app
      color="stratos"
      dark
    >
      <v-img
        class="mx-2"
        src="@/assets/logo.png"
        max-height="30"
        max-width="30"
        contain
      />

      <router-link
        class="homeLink"
        to="/"
      >
        <v-toolbar-title>
          <span class="font-weight-bold tracking-wider">{{ $t('moduleTitle') }}</span> {{ $t('appTitle') }}
        </v-toolbar-title>
      </router-link>

      <v-spacer />

      <profile-menu
        :show-profile-link="false"
      />
    </v-app-bar>

    <v-main>
      <v-container
        fill-height
        fluid
      >
        <v-row
          align="center"
          justify="center"
        >
          <v-col>
            <v-alert
              outlined
              type="error"
              prominent
            >
              <p class="text-h3 text-center">
                {{ $t("errors.pageNotFound") }}
              </p>
            </v-alert>
          </v-col>
        </v-row>
        <v-row
          justify="center"
        >
          <v-btn
            raised
            color="primary"
            @click.stop="backClick"
          >
            <v-icon left>
              fas fa-chevron-left
            </v-icon>
            {{ $t('buttons.back') }}
          </v-btn>
        </v-row>
      </v-container>
    </v-main>
  </div>
</template>

<style>
  .paragraph-text {
    font-size: 18px;
  }
</style>

<script>
import ProfileMenu from '@/ProfileMenu.vue';

export default
{
  name: 'NotFound',
  components: {
    ProfileMenu
  },
  beforeCreate() {
    this.$store.commit('hideMainMenu');
  },
  beforeDestroy() {
    this.$store.commit('showMainMenu');
  },
  methods: {
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>

<style scoped>
.homeLink {
    text-decoration: none;
    color: white;
}
</style>

