<template>
  <v-autocomplete
    :ref="'numberSelector_' + _uid"
    v-model="model"
    :items="items"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    v-bind="$attrs"
    v-on="$listeners"
    @keydown.enter="onEnter"
  >
    <template
      v-if="!hideNavigation"
      #prepend
    >
      <v-icon
        :ref="'prevBtn_' + _uid"
        :disabled="model !== null && model <= min"
        @click.stop="add(-1)"
      >
        mdi-menu-left-outline
      </v-icon>
    </template>
    <template
      v-if="!hideNavigation"
      #append-outer
    >
      <v-icon
        :disabled="model !== null && model >= max"
        @click.stop="add(1)"
      >
        mdi-menu-right-outline
      </v-icon>
    </template>
  </v-autocomplete>
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'NumberSelectorWrapper',
  props: {
    value: {
      type: Number,
      default() {
        return null;
      }
    },
    min: {
      type: Number,
      default() {
        return Number.MIN_VALUE;
      }
    },
    max: {
      type: Number,
      default() {
        return Number.MAX_VALUE;
      }
    },
    excluded: {
      type: Array,
      default() {
        return null;
      }
    },
    sortOrder: {
      type: String,
      default() {
        return 'ASC';
      }
    },
    hideNavigation: {
      type: Boolean,
      default() {
        return false;
      }
    },
    persistentPlaceholder: {
      type: Boolean,
      default() {
        return Constants.FieldPersistentPlaceholder;
      }
    },
    outlined: {
      type: Boolean,
      default() {
        return Constants.FieldOutlined;
      }
    }
  },
  data: (vm) => ({
    model: vm.value,
    items: []
  }),
  watch: {
    value(val) {
      this.model = val;
    },
  },
  mounted() {
    this.init();
  },
  methods: {
    init() {
      const min = this.min === Number.MIN_VALUE ? -1000 : this.min;
      const max = this.max === Number.MAX_VALUE ? 1000 : this.max;

      // DESC order
      if((this.sortOrder || '').toLowerCase() === 'desc') {
        const range = [...Array(max - min + 1).keys()].reduce((result, x) => {
          if(!this.excluded || !Array.isArray(this.excluded) || !this.excluded.includes(max - x)) {
            result.push(max - x);
          }

          return result;
        }, []);

        this.items = range;
      }

      // ASC order
      const range = [...Array(max - min + 1).keys()].reduce((result, x) => {
        if(!this.excluded || !Array.isArray(this.excluded) || !this.excluded.includes(min + x)) {
          result.push(min + x);
        }

        return result;
      }, []);

      this.items = range;
    },
    add(val) {
      try {
        if (this.model === null) {
          this.model = this.items[0];
          this.$emit('input', this.model);
          return;
        }

        let newVal = this.model + val;

        if (this.excluded && Array.isArray(this.excluded))  {
          while (this.excluded.includes(newVal)) {
            newVal += val;
          }
        }

        if (newVal >= this.min && newVal <= this.max) {
          this.model = newVal;
          this.$emit('input', this.model);
        }
      } catch {
        // Ignore
      }
    },
    onEnter(e) {
      const newVal = Number(e.target.value);
      if (isNaN(newVal)) {
        return;
      }

      if (this.items && this.items.includes(newVal)) {
        this.model = newVal;
        this.$emit('input', this.model);

        const numSelector =  this.$refs[`numberSelector_${this._uid}`];
        if (numSelector) {
          numSelector.blur();
        }

        const prevBtn = this.$refs[`prevBtn_${this._uid}`];
        if (prevBtn) {
          prevBtn.$el.focus();
        }
      }
    },
  }
};
</script>
