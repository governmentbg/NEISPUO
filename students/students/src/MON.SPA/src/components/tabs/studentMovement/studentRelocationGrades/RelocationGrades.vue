<template>
  <v-card>
    <v-card-title>
      <div />
    </v-card-title>

    <v-card-text>
      <CurrentTermGrades />
    </v-card-text>
  </v-card>
</template>

<script>
//import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import CurrentTermGrades from '@/components/tabs/studentMovement/studentRelocationGrades/CurrentTermGrades.vue';

export default {
name: "RelocationGrades",
components: { CurrentTermGrades },
props: {
     personId: {
      type: Number,
      required: true,
    },

},
data() {
    return {

    };
},
computed:{
  
},
watch: {

},
methods: {

}
};
</script>