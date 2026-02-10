<template>
  <v-menu
    v-model="menu"
    offset-y
    :close-on-content-click="false"
  >
    <template v-slot:activator="{ on, attrs }">
      <v-btn
        icon
        plain
        v-bind="attrs"
        v-on="on"
      >
        <v-icon
          small
          :color="value && value.filter ? 'primary': ''"
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
        <v-select
          v-if="model"
          v-model="model.op"
          :items="opOptions"
        />
        <v-text-field
          v-if="model"
          v-model="model.filter"
          autofocus
          @keydown.enter="onEnter"
        />
      </v-card-text>
      <v-card-actions
        v-if="model && model.filter != null"
        class="pt-0"
      >
        <v-spacer />
        <button-tip
          color="success"
          iclass=""
          icon-name="mdi-check"
          tooltip="buttons.apply"
          bottom
          small
          bclass="mr-1"
          @click="apply"
        />
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
import clonedeep from 'lodash.clonedeep';

export default {
  name: 'TextHeaderFilter',
  props: {
    value: {
      type: Object,
      default() {
        return {
          filter: null,
          op: 'equal'
        };
      }
    }
  },
  data() {
    return {
      menu: false,
      model: clonedeep(this.value ?? { filter: null, op: 'equal' }),
      opOptions: [
        { value: 'equal', text: 'Равно на'},
        { value: 'notEqual', text: 'Не е равно на'},
        { value: 'contains', text: 'Съдържа'},
        { value: 'startsWith', text: 'Започва с'},
        { value: 'endsWith', text: 'Завършва с'}
      ]
    };
  },
  methods: {
    onEnter() {
      if(this.model && this.model.filter) {
        this.apply();
      }
    },
    apply() {
      this.$emit('input', clonedeep(this.model));
    },
    clear() {
      this.model = { filter: null, op: 'equal' };
      this.$emit('input', clonedeep(this.model));
    }
  }
};
</script>
