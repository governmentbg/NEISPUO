<template>
  <v-text-field
    v-model="model"
    type="number"
    :persistent-placeholder="persistentPlaceholder"
    :outlined="outlined"
    v-bind="$attrs"
    v-on="inputListeners"
  />
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'NumberField',
  props: {
    value: {
      type: Number,
      default() {
        return null;
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
  data() {
    return {
      model: this.value,
    };
  },
  computed: {
    inputListeners() {
      var vm = this;
      return Object.assign({},
        // We add all the listeners from the parent
        this.$listeners,
        // Then we can add custom listeners or override the
        // behavior of some listeners.
        {
          input: (event) => {
            if (this.checkInputType(event)) {
              vm.$emit('input', parseFloat(event));
            } else {
              vm.$emit('input', parseInt(event));
            }
          }
        }
      );
    },
  },
  methods: {
    checkInputType(value) {
      const num = parseFloat(value);
      return Number.isInteger(num) ? false : true;
    }
  }
};
</script>
