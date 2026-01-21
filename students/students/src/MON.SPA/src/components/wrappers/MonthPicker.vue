<template>
  <v-select
    ref="monthPicker"
    v-model="month.value"
    :items="months"
    menu-props="auto"
    clearable
    :disabled="disabled"
    :label="label"
    :rules="rules"
    :error-messages="errorMessages"
    type="string"
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
      slot="selection"
      slot-scope="data"
    >
      {{ data.item.monthName }}
    </template>

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
        @click="setMonth(new Date().getMonth())"
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
import { Months } from '@/enums/enums';

export default {
  name: 'Monthpicker',
  props: {
    label: {
      type: String,
      default() {
        return 'Месец';
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
    errorMessages: {
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
    showCurrentMonthButton: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data: () => ({
    month:{ monthName: undefined, value: undefined},
    loading: false,
    months: Months
  }),
  methods: {
    getDisplayText(item) {
      return item.monthName;
    },
    setMonth(value){
      if(value) {
        var newMonthIndex;

        if(this.month.value === undefined){
          if(value === -1){
            newMonthIndex = 12;
          }
          else{
            newMonthIndex = 1;
          }
        }else{
          newMonthIndex  = this.month.value + value;
        }

        if(newMonthIndex < 1){
          newMonthIndex = 12;
        }
        if(newMonthIndex > 12){
          newMonthIndex = 1;
        }

        this.month = JSON.parse(JSON.stringify(this.months.filter((el) => { return el.value === newMonthIndex; })[0]));
      }

      this.$emit('input', this.month.value);
    },
  }
};
</script>
