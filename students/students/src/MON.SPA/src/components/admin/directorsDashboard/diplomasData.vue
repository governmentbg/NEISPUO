<template>
  <v-card>
    <v-card-title>
      <v-icon left>
        fa-certificate
      </v-icon>{{ this.$t('menu.diplomas.title') }}
    </v-card-title>
    <v-card-text>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('dashboards.totalFinalizedDiplomasCount') }}
        </v-col>
        <v-col
          cols="6"
        >
          {{ diplomasData.totalFinalizedDiplomasCount }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('dashboards.diplomasFinalizedForCurrentYearCount') }}
        </v-col>
        <v-col
          cols="6"
        >
          {{ diplomasData.diplomasFinalizedForCurrentYearCount }}
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>

import { DiplomasDataModel } from "@/models/admin/diplomasDataModel.js";

    export default{
        name: 'DiplomasData',
        components: { 
           
        },
        props: {
            diplomasData: {
                type:DiplomasDataModel,
                    default(){
                        return null;
                    }
            },
        },
        data() {
            return{
               
            };
        },
        beforeMount(){
            this.init();
        },
         methods:{
            async init(){
             
            },
           
        }
    };
</script>