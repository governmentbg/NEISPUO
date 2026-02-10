<template>
  <v-menu
    offset-y
  >
    <template v-slot:activator="{ on }">
      <v-icon
        class="mx-2"
        v-on="on"
      >
        mdi-view-module
      </v-icon>
    </template>

    <v-list>
      <v-subheader>
        {{ $t('dashboardMenu.title') }}
      </v-subheader>
      <v-list-item :href="config.portal">
        <v-icon
          left
          small
        >
          fas fa-sitemap
        </v-icon>
        <span
          class="primary--text"
        >
          {{ $t('dashboardMenu.toPortalURL') }}
        </span>
      </v-list-item>
      <v-list-item to="/software/localServer">
        <v-icon
          left
          small
        >
          fas fa-desktop
        </v-icon>
        <span
          class="primary--text"
        >
          {{ $t('menu.localServer') }}
        </span>
      </v-list-item>
      <v-divider />
      <v-list-item
        :href="config.remoteSupport"
        target="_blank"
      >
        <v-icon
          left
          small
        >
          fas fa-headset
        </v-icon>
        <span
          class="primary--text"
        >
          {{ $t('dashboardMenu.remoteSupport') }}
        </span>
      </v-list-item>
    </v-list>
  </v-menu>
</template>

<script>
import { config } from '@/common/config';

export default {
  name: 'DashboardMenu',
  data(){
    return{
      config: config
    };
  }
};
</script>
