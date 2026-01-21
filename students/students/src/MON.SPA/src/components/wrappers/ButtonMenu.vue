<template>
  <v-menu offset-y>
    <template v-slot:activator="{ on: menu }">
      <v-tooltip bottom>
        <template v-slot:activator="{ on: tooltip }">
          <v-btn
            color="normal"
            icon
            depressed
            plain
            v-bind="$attrs"
            v-on="{ ...tooltip, ...menu }"
          >
            <v-icon>fas fa-ellipsis-v</v-icon>
          </v-btn>
        </template>
        <span>{{ $t('common.actions') }}</span>
      </v-tooltip>
    </template>

    <v-list flat>
      <v-list-item-group
        color="normal"
      >
        <slot />
      </v-list-item-group>
    </v-list>
  </v-menu>
</template>

<script>
export default {
  name: 'ButtonMenu'
};
</script>