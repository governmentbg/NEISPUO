<template>
  <div>
    <!-- {{ model }} -->
    <v-combobox
      v-model="model"
      :items="optionsList"
      :return-object="returnObject"
      :item-disabled="itemDisabled"
      :item-text="itemText"
      :item-value="itemValue"
      :disabled="disabled"
      :label="label"
      :clearable="clearable"
      :readonly="readonly"
      :placeholder="placeholder"
      :hint="showDeferredLoadingHint ? defferedLoadingHint : hint"
      :persistent-hint="(showDeferredLoadingHint && model !== undefined && model !== null && optionsList.find(x => x.text === model) === undefined ) || persistentHint"
      :hide-no-data="hideNoData"
      :hide-selected="hideSelected"
      :loading="loading"
      :error-messages="errorMessages"
      :single-line="singleLine"
      :type="disableTyping ? 'button': 'text'"
      :rules="rules"
      :persistent-placeholder="persistentPlaceholder"
      :outlined="outlined"
      @blur="onBlur($event)"
      @change="onChange($event)"
      @keyup="onKeyUp($event)"
      @input="onInput($event)"
      @update:search-input="updateTargetInput"
    >
      <!-- <template v-slot:selection="{ item }">
        <span class="pr-2">
          {{ item[itemText] }}
        </span>
      </template> -->
      <template
        v-for="(_, slot) of $scopedSlots"
        v-slot:[slot]="scope"
      >
        <slot
          :name="slot"
          v-bind="scope"
        />
      </template>
    </v-combobox>
  </div>
</template>

<script>
import Constants from '@/common/constants';
import http from '@/services/http.service';

export default {
  name: 'CustomCombo',
  props: {
    value: {
      type: [String, Number, Object],
      default() {
        return undefined;
      }
    },
    api: { // Url на api-то за зареждане на опциите.
      type: String,
      default() {
        return undefined;
      }
    },
    isCustomCombo: {
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
    clearable: { // Показва X бутончето за изчистване на стойноста на combo-то когато има стойност.
      type: Boolean,
      default() {
        return true;
      }
    },
    deferOptionsLoading: { // Зарежда опциите чак след въвеждане на Constants.SEARCH_INPUT_MIN_LENGTH на брой символи.
      type: Boolean,
      default() {
        return false;
      }
    },
    returnObject: { // Връща обект за синхронизиране, а не itemValue-то на избраната опция.
      type: Boolean,
      default() {
        return true;
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
        return false;
      }
    },
    persistentHint: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideSelected: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hideNoData: {
      type: Boolean,
      default() {
        return false;
      }
    },
    itemDisabled: {
      type: [String, Array, Function],
      default() {
        return 'disabled';
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
    placeholder: {
      type: String,
      default() {
        return undefined;
      }
    },
    hint: {
      type: String,
      default() {
        return undefined;
      }
    },
    removeItemsOnClear: { // Изчиства this.optionsList след изчистване на input-a.
      type: Boolean,
      default() {
        return false;
      }
    },
    showDeferredLoadingHint: { // Покзва hint, които описва какви са изискванията за отложено зареждане на опциите.
      type: Boolean,
      default() {
        return false;
      }
    },
    errorMessages: {
      type: String,
      default() {
        return undefined;
      }
    },
    predefinedItems: {
      type: Array,
      default() {
        return [];
      }
    },
    singleLine: {
      type: Boolean,
      default() {
        return false;
      }
    },
    disableTyping: {
      type: Boolean,
      default() {
        return false;
      }
    },
    rules: {
      type: Array,
      default() {
        return [];
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
      optionsList: [],
      loading: false,
      defferedLoadingHint: this.$t('common.comboSearchHint', [Constants.SEARCH_INPUT_MIN_LENGTH]),
    };
  },
  watch: {
    value: function (newVal, oldVal) {
      if(this.removeItemsOnClear && !newVal && oldVal) {
        this.optionsList = [];
      }
      this.model = newVal;
    },
    predefinedItems() {
      this.optionsList = this.predefinedItems;
    }
  },
  mounted() {
    if(!this.api) {
      this.optionsList = this.predefinedItems;
    }
    else {
      if(!this.deferOptionsLoading) {
        // Не е избрана опция за зареждане чак след въвеждане на символи и зареждаме всичко.
        this.loadOptions();
      } else {
        // Избрана е опция за зареждане след въвеждане на някакви символи (Constants.SEARCH_INPUT_MIN_LENGTH на брой).
        // Въпреки това ако е подадена стойност на компонента ще се опитаме да заредим поне тази опция.
        // Todo: Не работи както трябва. Зарежда опцията (има я в optionsList), но не е видима в dropwown-а.
        if (this.value) {
          this.loadOptions(this.value);
        }
      }
    }
  },
  methods: {
    loadOptions(searchStr) {
      this.loading = true;

      const url = this.deferOptionsLoading
        ? `${this.api}?searchStr=${searchStr}`
        : this.api;
      http.get(url)
      .then((response) => {
        this.optionsList = response.data;
      })
      .catch((error) => {
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    onInput(event) {
      this.$emit('input', event, this.model);
    },
    onBlur(event) {
      this.$emit('blur', event);
    },
    onChange(event) {
      this.$emit('change', event);
    },
    onKeyUp(event) {
      if(event.keyCode === 27) { // Escape pressed Или не е разрешено търсенето
        event.preventDefault();
        this.model = undefined;
        return;
      }

      if(!this.deferOptionsLoading) {
        return;
      }
      const value = event.currentTarget.value;
      if(value.length < Constants.SEARCH_INPUT_MIN_LENGTH) {
        return;
      }

      this.loadOptions(value);
    },
    updateTargetInput(currentValue) {
      if(!this.returnObject) {
        // По подразбиране синхронизира модела чак след blur event-а.
        // Вика се при search-input.sync event-a.
        // Ръчно синхронизираме модела и райзваме input event за да се синхронизира и value-то.
        this.model = currentValue;
        this.$emit('input', this.model);
      }
    },
    getOptionsList() {
      return this.optionsList;
    },
  }
};
</script>

<style scoped>
  /* .v-autocomplete > input[type="button"] {
    text-align:  start !important;
  } */



  input[type="button"] {
    text-align: left !important;
  }
</style>
