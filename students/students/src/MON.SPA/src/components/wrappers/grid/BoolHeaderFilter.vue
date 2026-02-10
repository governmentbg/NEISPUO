<template>
  <v-menu
    v-model="menu"
    offset-y
    :close-on-content-click="false"
  >
    <template v-slot:activator="{ on, attrs }">
      <v-btn
        icon
        v-bind="attrs"
        v-on="on"
      >
        <v-icon
          small
          :color="model != null ? 'primary': ''"
        >
          mdi-filter
        </v-icon>
      </v-btn>
    </template>
    <v-card>
      <!-- <div class="text-end pr-1 pt-1">
        <v-btn
          icon
          small
          @click.stop="menu=false"
        >
          <v-icon>
            mdi-close
          </v-icon>
        </v-btn>
      </div> -->
      <v-card-text class="pb-0">
        <v-radio-group
          v-model="model"
          row
          class="mt-0"
          @change="$emit('input', $event)"
        >
          <v-radio
            :value="true"
            :label="$t('common.yes')"
          />
          <v-radio
            :value="false"
            :label="$t('common.no')"
          />
        </v-radio-group>
      </v-card-text>
      <v-card-actions
        v-if="value != null"
        class="pt-0"
      >
        <v-spacer />
        <button-tip
          color="error"
          iclass=""
          icon-name="mdi-close"
          tooltip="buttons.clear"
          bottom
          small
          @click="clear"
        />
      </v-card-actions>
    </v-card>
  </v-menu>
</template>

<script>
export default {
  name: 'BoolHeaderFilter',
  props: {
    value: {
      type: Boolean,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      menu: false,
      model: this.value
    };
  },
  methods: {
    clear() {
      this.model = null;
      this.$emit('input', this.model);
    }
  }
};
</script>
