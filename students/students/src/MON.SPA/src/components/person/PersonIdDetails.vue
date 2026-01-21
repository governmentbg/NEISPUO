<template>
  <v-row
    dense
  >
    <v-col
      cols="12"
      sm="4"
    >
      <v-text-field
        :value="personalIdType"
        :label="$t('createStudent.pinType')"
      />
    </v-col>

    <v-col
      cols="12"
      sm="8"
    >
      <v-text-field
        :value="personalId"
        :label="$t('createStudent.pin')"
      />
    </v-col>
  </v-row>
</template>

<script>
export default {
  name: 'PersonIdDetails',
  props: {
    personalIdType: {
      type: String,
      default() {
        return '';
      }
    },
    personalId: {
      type: String,
      default() {
        return '';
      }
    }
  }
};
</script>
