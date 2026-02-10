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
          <span class="font-weight-bold tracking-wider">{{ $t('moduleTitle') }}</span> <span class="hidden-sm-and-down">{{ $t('appTitle') }}</span>
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
                {{ $t("errors.accessDenied") }}
              </p>
              <p class="text-center">
                {{ $t("errors.accessDeniedInfo") }}
              </p>
            </v-alert>
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </div>
</template>

<style scoped>
  .paragraph-text {
    font-size: 18px;
  }
.homeLink {
    text-decoration: none;
    color: white;
}
</style>

<script>
import ProfileMenu from '@/ProfileMenu.vue';

export default
{
  name: "AccessDenied",
  components: {
    ProfileMenu
  }
};
</script>
