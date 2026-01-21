<template>
  <div>
    <!-- {{ model }} -->
    <!-- {{ $attrs }} -->
    <!-- {{ returnObject }}
    {{ itemValue }}
    {{ itemText }} -->
    <!-- {{ deferOptionsLoading }} -->
    <v-autocomplete
      v-model="model"
      :loading="loading"
      :items="optionsList"
      :search-input="search"
      :persistent-placeholder="persistentPlaceholder"
      :outlined="outlined"
      v-bind="$attrs"
      v-on="$listeners"
      @keyup="onKeyUp($event)"
    >
      <template
        v-for="(_, slot) of $scopedSlots"
        v-slot:[slot]="scope"
      >
        <slot
          :name="slot"
          v-bind="scope"
        />
      </template>

      <template #no-data>
        <v-card-subtitle class="text-h6">
          {{ $t('common.noData') }}
        </v-card-subtitle>
        <v-card-actions
          v-if="deferOptionsLoading"
        >
          <v-spacer />
          <v-btn
            small
            color="secondary"
            outlined
            @click.stop="loadOptions('')"
          >
            {{ $t("buttons.reload") }}
          </v-btn>
        </v-card-actions>
      </template>

      <template #append-outer>
        <slot
          name="append-outer"
        />
      </template>
    </v-autocomplete>
  </div>
</template>

<script>
import http from '@/services/http.service';
import QueryString from "query-string";
import isEqual from 'lodash.isequal';
import rangeArr from 'lodash.range';
import Constants from '@/common/constants';

export default {
  name: 'CustomAutocomplete',
  props: {
    value: {
      type: [String, Number, Object, Array],
      default() {
        return null;
      }
    },
    api: { // Url на api-то за зареждане на опциите.
      type: String,
      default() {
        return '';
      }
    },
    optionsUrl: { // Url на api-то за зареждане на опциите. Идва от схемите на динамичните форми.
      type: String,
      default() {
        return '';
      }
    },
    isCustomCombo: {
      type: Boolean,
      default() {
        return true;
      }
    },
    deferOptionsLoading: {
      type: Boolean,
      default() {
        return true;
      }
    },
    items:{
      type: Array,
      required: false,
      default(){
        return null;
      }
    },
    filter: { // Постоянен филтър
      type: Object,
      default() {
        return undefined;
      }
    },
    pageSize:{
        type: Number,
        default(){
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
      optionsList: [],
      loading: false,
      search: null,
      queryString: QueryString
    };
  },
  watch: {
    value: {
      handler(newVal, oldVal){
        const hasChanges = !isEqual(newVal, oldVal);
        if(hasChanges) {
          this.model = newVal;
          // В опциите имаме елемент за this.model и няма да ги зареждаме отново.
          if(!this.model || this.optionsList.some(x => isEqual(x.value, this.model))) return;

          this.loadOptions();
        }
      },
      deep: true
    },
    filter: {
      handler(newVal, oldVal){
        const hasChanges = !isEqual(newVal, oldVal);
        if(hasChanges) {
          this.doFilter();
        }
      },
      deep: true
    }
  },
  mounted() {
    if(this.deferOptionsLoading) {
      // Зареждаме опциите чак след search
      if (this.value) {
        // Имаме първонавална стойност и зареждаме поне тази опция.
        // Едва след това добавяме watcher на search-a.
        this.loadOptionsPromise(this.value)
        .then((response) => {
          this.optionsList = response.data;
        })
        .catch(() => {})
        .then(() => {
          this.$watch('search', () => this.doSearch());
        });
      } else {
        // Нямаме първоначална стойност.
        // Добавяме watcher на search-a.
        this.$watch('search', () => this.doSearch());
      }
    } else {
      //Зареждаме взички опции още в самото начало и след това добавяме watcher на search-a.
      if(this.apiUrl()) {
        this.loadOptionsPromise()
          .then((response) => {
            this.optionsList = response.data;
          })
          .catch(() => {})
          .then(() => {
            this.$watch('search', () => this.doSearch());
          });
      }
    }

    if(this.items !== null){
        this.optionsList = JSON.parse(JSON.stringify(this.items));
    }
  },
  methods: {
    apiUrl() {
      return this.api || this.optionsUrl;
    },
    loadOptions(searchStr) {
      this.loading = true;

      this.loadOptionsPromise(searchStr)
      .then((response) => {
        this.optionsList = response.data;
        this.$emit('optionsLoaded', this.optionsList);
      })
      .catch(() => {})
      .then(() => { this.loading = false; });
    },
    loadOptionsPromise(searchStr) {
      let queryList = {
        searchStr: searchStr ?? '',
        pageSize: this.pageSize
      };

      if(this.value && typeof this.value === 'object' && Object.prototype.hasOwnProperty.call(this.value, 'value')) {
        queryList = {...queryList, selectedValue: this.value.value};
      } else if (this.value && Array.isArray(this.value)) {
        queryList = {...queryList, selectedValue: this.value.join('|')};
      } else if (this.value) {
        queryList = {...queryList, selectedValue: this.value};
      }

      if(this.filter) {
        queryList = {...queryList, ...this.filter};
      }

      const api = this.apiUrl();
      if(!api) return;

      const query = "?" + this.queryString.stringify(queryList);

      return new Promise((resolve, reject) => {
          http.get(`${api}${query}`).then((response) => {
            return resolve(response);
          }).catch((err) => {
              console.log(err.response);
              return reject(err);
          });
      });
    },
    doSearch() {
      if(!this.search) return; // Намя Search стринг. Не търсим;
      if(this.search === this.model) return; // Search стринга съвпада с modela. Не търсим.

      if(this.optionsList && this.optionsList.some(x => {
        if(typeof(x) !== 'object') return false; //

        for (const prop in x) {
          if(x[prop] === this.search){
            return true;
          }
        }

        return false;
      })) return; // Има заредени опции, които са списък от обекти и в тях има обект, притежаващ свойство със стойност като на модела. Не търсим.

      this.loadOptions(this.search);
    },
    doFilter() {
      this.loadOptions(this.search);
    },
    getOptionsList() {
      return this.optionsList;
    },
    onKeyUp(event) {
      if(event.keyCode === 27) { // Escape pressed Или не е разрешено търсенето
        event.preventDefault();
        this.model = undefined;
        return;
      }

      if(rangeArr(37, 41).includes(event.keyCode)) { //Изключване на Up and Down Arrows за избор на опциите и Left and Right за навигиране на курсора.
        event.preventDefault();
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
  }
};
</script>
