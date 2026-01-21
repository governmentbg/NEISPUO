<template>
  <v-menu
    ref="menu"
    v-model="menu"
    :close-on-content-click="false"
    transition="scale-transition"
    offset-y
    min-width="auto"
  >
    <template #activator="{ on, attrs }">
      <v-text-field
        v-model="year"
        v-mask="'####'"
        clearable
        :readonly="readonly"
        :rules="rules"
        :label="label"
        :disabled="disabled"
        :error-messages="errorMessages"
        :hint="hint"
        :persistent-hint="persistentHint"
        :persistent-placeholder="persistentPlaceholder"
        :outlined="outlined"
        v-bind="attrs"
        v-on="on"
        @click:clear="onClear"
        @keydown.esc="onEscape"
        @keydown.enter="onEnter"
        @blur="onBlur($event)"
      >
        <template
          v-if="showNavigationButtons"
          #prepend
        >
          <v-tooltip
            bottom
          >
            <!-- eslint-disable -->
            <template #activator="{ on }">
            <!-- eslint-enable -->
              <v-icon
                :ref="'prevBtn_' + _uid"
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
            bottom
          >
            <!-- eslint-disable -->
            <template #activator="{ on }">
            <!-- eslint-enable -->
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
      </v-text-field>
    </template>

    <v-date-picker
      ref="picker"
      v-model="year"
      type="month"
      :title-date-format="formatDate"
      :min="min"
      :max="max"
      v-bind="$attrs"
      v-on="$listeners"
      @click:year="yearClick($event)"
    />
  </v-menu>
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'CustomYearPicker',
  props: {
    value: {
      type: [Number,String],
      default() {
        return '';
      }
    },
    isYearPicker: {
      type: Boolean,
      default() {
        return true;
      }
    },
    label: {
      type: String,
      default() {
        return undefined;
      }
    },
    rules: {
        type: Array,
        required: false,
        default() {
          return [];
        }
    },
    isCustomDatepicker: {
      type: Boolean,
      default() {
        return true;
      }
    },
    errorMessages: {
      type: String,
      default() {
        return undefined;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    readonly: {
      type: Boolean,
      default() {
        return true;
      }
    },
    persistentHint: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hint: {
      type: String,
      default() {
        return undefined;
      }
    },
    min: {
      type: String,
      default() {
        return '1900-01-01';
      }
    },
    max: {
      type: String,
      default() {
        return '2050-01-01';
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
  data: (vm) => ({
    year: vm.value,
    menu: false,
    loading: false
  }),
  watch: {
    value(val) {
      this.year = val;
    },
    menu (val) {
      // val && setTimeout(() => (this.$refs.picker.activePicker = 'YEAR'));
      val && this.$nextTick(() => (this.$refs.picker.activePicker = 'YEAR'));
    },
  },
  methods: {
    yearClick(year) {
      if(year) {
        this.$refs.menu.save(year);
        this.year = year.toString();
        this.$emit('input', this.year);
      }
    },
    onClear() {
      this.year = '';
      this.$refs.menu.save(this.year);
      this.$emit('input', this.year);
    },
    onBlur(event) {
      this.$emit('blur', event);
    },
    onEnter() {
      if(this.year) {
        this.$emit('input', this.year);

        const prevBtn = this.$refs[`prevBtn_${this._uid}`];
        if (prevBtn) {
          prevBtn.$el.focus();
        }
      }
    },
    onEscape() {
      if (this.menu) {
        this.menu = false;
      }
    },
    formatDate() {
      // Използва се за да се скрие датата в title-а на менюто. Виж <style></style>.
      return '<span class="custom-year-picker-date-title">Test</span>';
    },
    addYear(val) {
      if(!val) return;

      this.loading = true;

      try {
        let year = parseInt(this.year);
        if(isNaN(year)) {
          year = new Date().getFullYear();
          this.year = year.toString();
        } else {
          this.year = (year + val).toString();
        }

        this.$emit('input', this.year);
      } catch {
        // Ignore
      }

      this.loading = false;
    }
  },
};
</script>

<style>
.custom-year-picker-date-title {
  display: none !important;
}

.v-date-picker-title__date {
  padding-bottom: 0;
}
</style>
