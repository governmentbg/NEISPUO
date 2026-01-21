<template>
  <div>
    <div v-if="showDebugData">
      <p>Custom formatted date: {{ formattedDate }}</p>
      <p>ISO formatted date: {{ isoDate }}</p>
    </div>

    <v-menu
      v-if="showButtons"
      :ref="_uid"
      v-model="menu"
      :close-on-content-click="false"
      :nudge-right="40"
      :return-value.sync="isoDate"
      transition="scale-transition"
      offset-y
      min-width="auto"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-text-field
          :id="'withButtonsTextField' + _uid"
          v-model="formattedDate"
          :rules="rules"
          :label="label"
          :disabled="disabled"
          :error-messages="errorMessages"
          prepend-icon="mdi-calendar"
          readonly
          clearable
          :persistent-hint="persistentHint"
          :hint="hint"
          :persistent-placeholder="persistentPlaceholder"
          :outlined="outlined"
          :dense="dense"
          v-bind="attrs"
          v-on="on"
          @click:clear="onClear"
          @blur="onBlur($event)"
        />
      </template>
      <v-date-picker
        v-model="isoDate"
        v-bind="$attrs"
        :first-day-of-week="1"
        year-icon="mdi-calendar-blank"
        prev-icon="mdi-skip-previous"
        next-icon="mdi-skip-next"
        v-on="$listeners"
        @input="onInput($event)"
      >
        <v-spacer />
        <v-btn
          text
          color="primary"
          @click="menu = false"
        >
          {{ $t('buttons.cancel') }}
        </v-btn>
        <v-btn
          text
          color="primary"
          @click="onOkClick"
        >
          {{ $t('buttons.ok') }}
        </v-btn>
      </v-date-picker>
    </v-menu>

    <v-menu
      v-else
      :ref="_uid"
      v-model="menu"
      :close-on-content-click="false"
      :nudge-right="40"
      transition="scale-transition"
      offset-y
      min-width="auto"
    >
      <template v-slot:activator="{ on, attrs }">
        <v-text-field
          :id="'withNoButtonsTextField' + _uid"
          v-model="formattedDate"
          :rules="rules"
          :label="label"
          :fomat="format"
          :disabled="disabled"
          :error-messages="errorMessages"
          prepend-icon="mdi-calendar"
          readonly
          clearable
          :persistent-hint="persistentHint"
          :hint="hint"
          :persistent-placeholder="persistentPlaceholder"
          :outlined="outlined"
          :dense="dense"
          v-bind="attrs"
          v-on="on"
          @click:clear="onClear"
          @blur="onBlur($event)"
        />
      </template>
      <v-date-picker
        v-model="isoDate"
        v-bind="$attrs"
        :first-day-of-week="1"
        year-icon="mdi-calendar-blank"
        prev-icon="mdi-skip-previous"
        next-icon="mdi-skip-next"
        v-on="$listeners"
        @input="onInput($event)"
      />
    </v-menu>
  </div>
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: 'CustomDatepicker',
  props: {
    value: {
      type: [Date, String],
      default() {
        return '';
      }
    },
    showButtons: {
      type: Boolean,
      default() {
        return false;
      }
    },
    showDebugData: {
      type: Boolean,
      default() {
        return false;
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
    format: {
      type: String,
      default() {
        return null;
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
        return '';
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
    dense: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data: (vm) => ({
    formattedDate: vm.value,
    isoDate: vm.parseDateToIso(vm.value),
    menu: false,
  }),
  watch: {
    value(val) {
      this.formattedDate = val;
      this.isoDate = this.parseDateToIso(val);
    },
    isoDate(value) {
      this.formattedDate = this.formatDate(value);
    },
  },
  methods: {
    onInput() {
      if(!this.showButtons) {
        this.formattedDate = this.formatDate(this.isoDate);
        this.menu = false;
        this.$emit('input', this.formattedDate);
      }
    },
    onOkClick() {
      this.$refs[`${this._uid}`].save(this.isoDate);
      this.$emit('input', this.formattedDate);
    },
    onClear() {
      this.formattedDate = '';
      this.$emit('input', this.formattedDate);
    },
    onBlur(event) {
      this.$emit('blur', event);
    },
    formatDate (date) {
      return this.$helper.formatDate(date, this.format);
    },
    parseDateToIso (date) {
      return this.$helper.parseDateToIso(date, this.format);
    },
    isValidIsoFormattedDate(date) {
      // Тази функция е изнесена в $helper, но към нея има страшнно много референции, които за сега няма да пипаме.
      return this.$helper.isValidIsoFormattedDate(date);
    },
  }
};
</script>
