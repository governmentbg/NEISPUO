<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />

    <year-picker
      v-else
      :ref="'schoolYearSelector' + _uid"
      :value="value"
      :min="minYear || min"
      :max="maxYear || max"
      :label="label"
      :items="years"
      :finalized-lods-years="finalizedLodsYears"
      :item-value="itemValue"
      :item-text="itemText"
      :clearable="clearable"
      :persistent-placeholder="persistentPlaceholder"
      :outlined="outlined"
      v-bind="$attrs"
      v-on="$listeners"
      @input="v => $emit('input', v)"
    >
      <template
        v-if="showCurrentSchoolYearButton && userDetails"
        #customPrepend
      >
        <button-tip
          v-if="userDetails.currentSchoolYear"
          icon
          icon-name="mdi-timer-refresh-outline"
          icon-color="secondary"
          tooltip="buttons.currentSchoolYearSelect"
          bottom
          iclass=""
          small
          @click="$refs['schoolYearSelector' + _uid].setYear(userDetails.currentSchoolYear)"
        />
      </template>
    </year-picker>
  </div>
</template>

<script>
import Constants from '@/common/constants';
import { mapGetters } from 'vuex';

export default {
  name: 'SchoolYearSelector',
  props: {
    value: {
      type: [Number, String],
      default() {
        return null;
      }
    },
    label: {
      type: String,
      default() {
        return 'Учебна година';
      }
    },
    minYear: {
      type: [Number, String],
      default() {
        return null;
      }
    },
    maxYear: {
      type: [Number, String],
      default() {
        return null;
      }
    },
    itemValue: {
      type: String,
      default() {
        return 'value';
      }
    },
    itemText: {
      type: String,
      default() {
        return 'text';
      }
    },
    clearable: {
      type: Boolean,
      default: true
    },
    finalizedLodsYears: {
      type: Array,
      default(){
        return [];
      }
    },
    institutionId: {
      type: Number,
      default(){
        return null;
      }
    },
    showCurrentSchoolYearButton: {
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
    skipFinalizedYearsCheck: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data: () => ({
    loading: false,
    min: null,
    max: null,
    years: []
  }),
  computed: {
    ...mapGetters(['userDetails'])
  },
  watch: {
    finalizedLodsYears(){
      this.load();
    }
  },
  mounted() {
    this.load();
  },
  methods: {
    load(){
      if(this.skipFinalizedYearsCheck && this.maxYear && this.minYear) {
        // Няма да зареждаме учебните години на институцията
      } else {
        this.loading = true;

        this.$api.lookups.getSchoolYears({ institutionId: this.institutionId, selectedValue: this.value })
        .then((result) => {
          if(result.data) {
            const allYears = result.data;
            this.years = allYears.filter(item => !this.finalizedLodsYears.includes(item.value));
            const arr = result.data.map(x => x.value);
            this.min = Math.min(...arr);
            this.max = Math.max(...arr);
          }

        }).catch((err) => {
          console.log((err));
          this.$notifier.error('', this.$t('errors.schoolYearsLoad'));
        })
        .then(() => { this.loading = false; });
      }

    }
  }
};
</script>
