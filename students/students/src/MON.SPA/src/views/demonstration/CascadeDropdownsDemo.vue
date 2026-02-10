<template>
  <v-row>
    <v-col
      cols="12"
      md="6"
    >
      {{ buildingAreas }}
      <custom-autocomplete
        id="buildingArea"
        ref="buildingArea"
        v-model="buildingAreas"
        api="/api/lookups/GetBuildingAreas"
        label="Building areas"
        :placeholder="$t('buttons.search')"
        clearable
        hide-no-data
        :defer-options-loading="false"
        :disabled="disabled"
        multiple
        no-filter
        chips
        deletable-chips
        loading
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      {{ buildingRooms }}
      <custom-autocomplete
        id="buildingRooms"
        ref="buildingRooms"
        v-model="buildingRooms"
        api="/api/lookups/GetBuildingRooms"
        label="Building rooms"
        :placeholder="$t('buttons.search')"
        clearable
        hide-no-data
        hide-selected
        :defer-options-loading="false"
        :disabled="disabled || !buildingAreasStr"
        :filter="buildingAreasFilter"
        multiple
        no-filter
        chips
        deletable-chips
        loading
      />
    </v-col>
    <v-col
      cols="12"
      md="6"
    >
      {{ buildingEquipments }}
      <custom-autocomplete
        id="buildingEquipments"
        ref="buildingEquipments"
        v-model="buildingEquipments"
        api="/api/lookups/GetSpecialEquipment"
        label="Building equipments"
        :placeholder="$t('buttons.search')"
        clearable
        hide-no-data
        hide-selected
        :defer-options-loading="false"
        :disabled="disabled || !buildingRoomsStr"
        :filter="buildingRoomsFilter"
        multiple
        no-filter
        chips
        deletable-chips
        loading
      />
    </v-col>
  </v-row> 
</template>

<script>
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'CascadeDropdownsDemo',
  components: {
    CustomAutocomplete
  },
  data() {
    return {
      buildingAreas: [], // [1],
      buildingRooms: [], // [2],
      buildingEquipments: [],
      disabled: false
    };
  },
  computed: {
    buildingAreasFilter() {
      return { buildingAreas: this.buildingAreasStr};
    },
    buildingRoomsFilter() {
      return { buildingRooms: this.buildingRoomsStr};
    },
    buildingAreasStr() {
      return this.buildingAreas && Array.isArray(this.buildingAreas) ? this.buildingAreas.join() : '';
    },
    buildingRoomsStr() {
      return this.buildingRooms && Array.isArray(this.buildingRooms) ? this.buildingRooms.join() : '';
    }
  },
  watch: {
    buildingAreas(val) {
      if(!val || (Array.isArray(val) && val.length === 0)) {
        // Махаме всички buildingAreas. Следва да махмен и всички buildingRooms и buildingEquipments.
        this.buildingRooms.splice(0);
        this.buildingEquipments.splice(0);
      } else {
        // Махаме елемент от buildingAreas. Следва да махмен всички от buildingRooms,
        // чиито parantId не съществувау в buildingAreas. Това ще промени buildingRooms,
        // което от своя стрвана ще тригерне watcher-а на buildingRooms.
        if(this.buildingRooms && Array.isArray(this.buildingRooms) && this.buildingRooms.length > 0) {
          const options = this.$refs['buildingRooms'].getOptionsList();

          for (const roomId of this.buildingRooms){
            const item = options.find(x => x.value === roomId);
            if(item && !val.some(x => x === item.relatedObjectId)) {
              this.buildingRooms.splice(this.buildingRooms.indexOf(roomId), 1);
            }
          }
        }
      }
    },
    buildingRooms(val) {
      if(!val || (Array.isArray(val) && val.length === 0)) {
        // Махаме всички buildingRooms. Следва да махмен и всички buildingEquipments.
        this.buildingEquipments.splice(0);
      } else {
        // Махаме елемент от buildingRooms. Следва да махмен всички от buildingEquipments,
        // чиито parantId не съществувау в buildingRooms. 
        if(this.buildingEquipments && Array.isArray(this.buildingEquipments) && this.buildingEquipments.length > 0) {
          const options = this.$refs['buildingEquipments'].getOptionsList();

          for (const eqId of this.buildingEquipments){
            const item = options.find(x => x.value === eqId);
            if(item && !val.some(x => x === item.relatedObjectId)) {
              this.buildingEquipments.splice(this.buildingRooms.indexOf(eqId), 1);
            }
          }
        }
      }
    }
  },
};
</script>