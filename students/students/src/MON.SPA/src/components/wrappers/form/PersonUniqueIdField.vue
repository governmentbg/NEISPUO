<template>
  <person-unique-id
    ref="personUniqueId"
    :key="personUniqueIdComponentKey"
    :initial-personal-i-d="value"
    :initial-personal-type="personUniqueIdType"
    :unique-check="uniqueCheck"
    :show-g-r-a-o-search="showGRAOSearch"
    :pin-required="required"
    :disabled="disabled"
    :readonly="readonly"
    :clearable="clearable"
    :skip-initial-set="skipInitialSet"
    @update:personalID="$emit('inputCustomField','personalId', $event)"
    @update:personalIDType="$emit('inputCustomField', 'personalIdType', $event.value.toString())"
    @extractBirthDateAndGenderFromEGN="onExtractBirthDateAndGenderFromEGN"
  />
</template>

<script>
import PersonUniqueId from "@/components/person/PersonUniqueId.vue";

export default {
  name: 'PersonUniqueIdField',
  components: {
    PersonUniqueId
  },
  props: {
    value: {
      type: String,
      default() {
        return null;
      }
    },
    showGRAOSearch: {
      type: Boolean,
      default() {
        return false;
      }
    },
    required: {
      type: Boolean,
      default() {
        return false;
      }
    },
    uniqueCheck: {
      type: Boolean,
      default() {
        return false;
      }
    },
    personUniqueIdType: {
      type: [String, Number],
      default() {
        return null;
      }
    },
    componentClass: {
      type: String,
        default() {
            return undefined;
        }
    },
    disabled:{
        type:Boolean,
        default() {
          return false;
        }
    },
    readonly:{
        type:Boolean,
        default() {
          return false;
        }
    },
    clearable:{
        type:Boolean,
        default() {
          return true;
        }
    },
    skipInitialSet: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      genderOptions: [],
      personUniqueIdComponentKey: 0
    };
  },
  mounted() {
    this.setGenderOptions();
  },
  methods: {
    onExtractBirthDateAndGenderFromEGN(data) {
      this.$emit('inputCustomField', 'gender', this.genderOptions.filter(el => el.value === data.genderId)[0]);
      this.$emit('inputCustomField', 'birthDate', data.birthDate);
    },
    setGenderOptions() {
      this.$api.lookups
        .getGenders()
        .then((response) => {
          const genders = response.data;
          if (genders) {
            this.genderOptions = genders;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.genderOptionsLoad'));
          console.log(error);
        });
    }
  }
};
</script>
