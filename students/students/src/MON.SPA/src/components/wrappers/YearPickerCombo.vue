<template>
  <div>
    <div v-if="allowTyping">
      <v-combobox
        ref="yearPicker"
        v-model="year"
        :items="options"
        :item-text="itemText"
        :item-value="itemValue"
        :min="min"
        :max="max"
        menu-props="auto"
        :clearable="clearable"
        type="number"
        :persistent-placeholder="persistentPlaceholder"
        :outlined="outlined"
        v-bind="$attrs"
        v-on="$listeners"
        @keyup="onKeyUp($event)"
        @input="onInput($event)"
        @blur="onBlur($event)"
        @update:search-input="updateTargetInput"
      >
        <template
          slot="item"
          slot-scope="data"
        >
          {{ getDisplayText(data.item) }}
        </template>

        <template
          v-if="showNavigationButtons"
          #prepend
        >
          <v-tooltip
            v-if="!year || year > min"
            bottom
          >
            <template #activator="{ on }">
              <v-icon
                ref="prevBtn"
                :disabled="loading"
                v-on="on"
                @click.stop="addYear(-1)"
              >
                mdi-menu-left-outline
              </v-icon>
            </template>
            {{ $t('buttons.prev_a') }}
          </v-tooltip>
        </template>

        <template
          v-if="showNavigationButtons"
          #append-outer
        >
          <v-tooltip
            v-if="!year || year < max"
            bottom
          >
            <template #activator="{ on }">
              <v-icon
                :disabled="loading"
                v-on="on"
                @click.stop="addYear(1)"
              >
                mdi-menu-right-outline
              </v-icon>
            </template>
            {{ $t('buttons.next_a') }}
          </v-tooltip>
        </template>
      </v-combobox>
    </div>
    <div v-else>
      <v-autocomplete
        ref="yearPicker"
        v-model="year"
        :items="options"
        :item-text="itemText"
        :item-value="itemValue"
        menu-props="auto"
        :clearable="clearable"
        :persistent-placeholder="persistentPlaceholder"
        :outlined="outlined"
        v-bind="$attrs"
        v-on="$listeners"
      >
        <template
          slot="item"
          slot-scope="data"
        >
          {{ getDisplayText(data.item) }}
        </template>

        <template
          #prepend
        >
          <v-tooltip
            v-if="showNavigationButtons && (!year || year > min)"
            bottom
          >
            <template #activator="{ on }">
              <v-icon
                ref="prevBtn"
                :disabled="loading"
                v-on="on"
                @click.stop="addYear(-1)"
              >
                mdi-menu-left-outline
              </v-icon>
            </template>
            {{ $t('buttons.prev_a') }}
          </v-tooltip>
          <slot name="customPrepend">
            <button-tip
              v-if="showCurrentYearButton"
              icon
              icon-name="mdi-timer-refresh-outline"
              icon-color="secondary"
              tooltip="common.currentYear"
              bottom
              iclass=""
              small
              @click="setYear(new Date().getFullYear())"
            />
          </slot>
        </template>

        <template
          v-if="showNavigationButtons"
          #append-outer
        >
          <v-tooltip
            v-if="!year || year < max"
            bottom
          >
            <template #activator="{ on }">
              <v-icon
                :disabled="loading"
                v-on="on"
                @click.stop="addYear(1)"
              >
                mdi-menu-right-outline
              </v-icon>
            </template>
            {{ $t('buttons.next_a') }}
          </v-tooltip>
        </template>
      </v-autocomplete>
    </div>
  </div>
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'CustomComboYearPicker',
  props: {
    value: {
      type: [Number,String],
      default() {
        return undefined;
      }
    },
    isYearComboPicker: {
      type: Boolean,
      default() {
        return true;
      }
    },
    allowTyping: {
      type: Boolean,
      default() {
        return false;
      }
    },
    min: {
      type: Number,
      default() {
        return 1900;
      }
    },
    max: {
      type: Number,
      default() {
        return 2050;
      }
    },
    items: {
      type: Array,
      default() {
        return null;
      }
    },
    itemText: {
      type: String,
      default() {
        return 'text';
      }
    },
    itemValue: {
      type: String,
      default() {
        return 'value';
      }
    },
    clearable: {
      type: Boolean,
      default: true
    },
    showCurrentYearButton: {
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
    },
    showNavigationButtons: {
      type: Boolean,
      default() {
        return true;
      }
    }
  },
  data: () => ({
    year: undefined,
    loading: false,
    options: []
  }),
  computed: {
    optionsTitle() {
      return `${this.year}г.`;
    }
  },
  watch: {
    value(val) {
      this.setYear(val);
    },
    min() {
      this.setOptions();
    },
    max() {
      this.setOptions();
    }
  },
  mounted() {
    this.loading = true;

    this.setOptions();
    this.setYear(this.value);

    this.loading = false;
  },
  methods: {
    setOptions() {
      if(this.items && this.items.length > 0) {
        //Опциите се подават отвън. Трявба да ги филтрираме в зависимост от мин и макс.
        this.options = this.items.filter(x => {
          // Min и Max винаги имат стойност.
          let result = x[this.itemValue] >= this.min && x[this.itemValue] <= this.max;

          if(!result && this.value) {
            // Ако има мин или макс и не попада там проверяваме дали не е равно на първоначалната стойност.
            result = x[this.itemValue] == this.value;
          }

          return result;
        });
      } else {
        const arr = Array.from({length: this.max - this.min}, (v, k) => k + this.min).reverse();

        if(this.value && !(this.min <= this.value && this.value <= this.max)) {
          // Има първоначална стойност, която е извън Min и Max. Трябва да я добавим
          this.options = [this.value, ...arr];
        } else {
          this.options = [...arr];
        }
      }
    },
    setYear(val) {
      if(val) {
        let year = parseInt(val);
        if(isNaN(year)) {
          year = new Date().getFullYear();
        }
        this.year = year;

      } else {
        this.year = undefined;
      }

      this.$emit('input', this.year);
    },
    addYear(val) {
      // Стойност за добавяне към годината, 1 или -1 в зависимост от бутона, който сме натиснали(Предишна Следваща)
      if(!val || isNaN(val)) return;

      this.loading = true;

      try {
        let year = parseInt(this.year);
        if(isNaN(year)) {
          // Натиснали сме бутон Предишна или Следваща но годината е празна.
          year = new Date().getFullYear();
          if(year < this.min) {
            // Ако сегашната година е по-малка от min сетваме годината на min
            this.year = this.min;
          } else if(year > this.max) {
            // Ако сегашната година е по-голяма от max сетваме годината на max
            this.year = this.max;
          } else {
            // Сетваме сегашната година
            this.year = year;
          }
          this.$emit('input', this.year);
        } else {
          const newVal = year + val;
          if(newVal >= this.min  && newVal <= this.max) {
            // Променяме годината само ако сме в min-max range-а
            this.year = newVal;
            this.$emit('input', this.year);
          }
        }

      } catch {
        // Ignore
      }

      this.loading = false;
    },
    onInput(event) {
      this.$emit('input', event, this.year);
    },
    onKeyUp(event) {
      if(event.keyCode === 27) { // Escape pressed
        event.preventDefault();
        this.year = undefined;
        return;
      }
    },
    onBlur(event) {
      const year = this.year;
      if(year) {
        if(year < this.min) {
          this.year = this.min;
          this.$emit('input', this.year);
        } else if(year > this.max) {
          this.year = this.max;
          this.$emit('input', this.year);
        }
      }
      this.$emit('blur', event);
    },
    updateTargetInput(currentValue) {
      this.$emit('input', currentValue);
    },
    getDisplayText(item) {
      return isNaN(item) ? item[this.itemText] : `${item} г.`;
    }
  },
};
</script>
