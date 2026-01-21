<template>
  <v-select
    ref="monthPicker"
    v-model="month"
    :items="options"
    :item-text="itemText"
    :item-value="itemValue"
    :label="label"
    :clearable="clearable"
    v-bind="$attrs"
    v-on="$listeners"
    @input="v => $emit('input', v)"
  >
    <template #prepend>
      <v-tooltip
        bottom
      >
        <template #activator="{ on }">
          <v-icon
            ref="prevBtn"
            :disabled="loading"
            v-on="on"
            @click.stop="setMonth(-1)"
          >
            mdi-menu-left-outline
          </v-icon>
        </template>
        {{ $t('buttons.prev_a') }}
      </v-tooltip>
      <button-tip
        v-if="showCurrentMonthButton"
        icon
        icon-name="mdi-timer-refresh-outline"
        icon-color="secondary"
        tooltip="buttons.currentMonthSelect"
        bottom
        iclass=""
        small
        @click="setCurrentMonth"
      />
    </template>

    <template #append-outer>
      <v-tooltip
        bottom
      >
        <template #activator="{ on }">
          <v-icon
            :disabled="loading"
            v-on="on"
            @click.stop="setMonth(1)"
          >
            mdi-menu-right-outline
          </v-icon>
        </template>
        {{ $t('buttons.next_a') }}
      </v-tooltip>
    </template>
  </v-select>
</template>

<script>
export default {
  name: 'CustomMonthPicker',
  props: {
    value: {
      type: [Number, String],
      default() {
        return undefined;
      }
    },
    label: {
      type: String,
      default() {
        return 'Месец';
      }
    },
    itemText: {
      type: [String, Array, Function],
      default() {
        return 'text';
      }
    },
    itemValue: {
      type: [String, Array, Function],
      default() {
        return 'value';
      }
    },
    clearable: {
      type: Boolean,
      default: true
    },
    showCurrentMonthButton: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      month: this.value,
      loading: false,
      options: [ {text:'Януари', value:1}, {text:'Февруари', value:2}, {text:'Март', value:3}, {text:'Април' , value:4}, {text:'Май', value:5}, {text:'Юни', value:6 }, {text:'Юли', value:7}, {text:'Август', value:8}, {text:'Септември', value:9}, {text:'Октомври', value:10},  {text:'Ноември', value:11},  {text:'Декември', value:12}]
    };
  },
  watch: {
    value(val) {
      this.month = val;
    }
  },
  methods: {
    setMonth(value) {
      if(this.itemValue === 'value') {
        return this.setNumberMonth(value);
      }

      if(this.itemValue === 'text') {
        return this.setStringMonth(value);
      }
    },
    setNumberMonth(value) {
      let newVal = this.month;
      if(isNaN(newVal)) {
        if(value < 0) {
          newVal = 1;
        } else {
          newVal = 12;
        }
      } else {
        newVal = newVal + value;
        if(newVal < 1) {
          newVal = 12;
        } else if(newVal > 12) {
          newVal = 1;
        }
      }

      this.month = newVal;
      this.$emit('input', this.month);
    },
    setStringMonth(value) {
      const item = this.options.find(x => x.text === this.month);
      let newVal = item ? item.value : NaN;

      if(isNaN(newVal)) {
        if(value < 0) {
          newVal = 1;
        } else {
          newVal = 12;
        }
      } else {
        newVal = newVal + value;
        if(newVal < 1) {
          newVal = 12;
        } else if(newVal > 12) {
          newVal = 1;
        }
      }

      const newItem = this.options.find(x => x.value === newVal);
      if(newVal) {
        this.month = newItem.text;
        this.$emit('input', this.month);
      }
    },
    setCurrentMonth() {
      this.month = new Date().getMonth() + 1;
      this.$emit('input', this.month);
    }
  }
};
</script>
