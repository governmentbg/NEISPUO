<template>
  <div>
    <v-combobox
      v-model="model"
      :loading="loading"
      :items="optionsList"
      :search-input="search"
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
    </v-combobox>
  </div>
</template>

<script>
import Constants from '@/common/constants';
import http from '@/services/http.service';
import isEqual from 'lodash.isequal';
import QueryString from "query-string";

export default {
  name: 'Combobox',
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
    optionsUrl: { // Url на api-то за зареждане на опциите. Идва от схемите на динамичните форми.
      type: String,
      default() {
        return '';
      }
    },
    deferOptionsLoading: { // Зарежда опциите чак след въвеждане на Constants.SEARCH_INPUT_MIN_LENGTH на брой символи.
      type: Boolean,
      default() {
        return false;
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
  computed: {
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
      if(this.value) {
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
        })
        .catch(() => {})
        .then(() => { this.loading = false; });
    },
    loadOptionsPromise(searchStr) {
      let queryList = {
        searchStr: searchStr ?? '',
        selectedValue: 
        this.value && typeof this.value === 'object' && Object.prototype.hasOwnProperty.call(this.value, 'value')
          ? this.value.value 
          : (this.value && Array.isArray(this.value) ? this.value.join('|') : this.value),
        pageSize: this.pageSize
      };

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

      if(!this.deferOptionsLoading) {
        return;
      }
      
      const value = event.currentTarget.value;
      if(value.length < Constants.SEARCH_INPUT_MIN_LENGTH) {
        return;
      }

      this.loadOptions(value);
    }
  }
};
</script>
