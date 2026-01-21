<template>
  <div>
    <v-card
      v-for="(rs, index) in model"
      :key="index"
      class="my-1"
      elevation="3"
    >
      <v-alert
        outlined
        color="primary"
        class="pa-0"
      >
        <v-card-title>
          {{ $t('student.menu.resourceSupportEPLR') }}
          <v-spacer />
          <button-tip
            v-if="!disabled && !disableAdd"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="resourceSupport.deleteResourceSupport"
            bottom
            iclass="mx-2"
            small
            @click="onRemove(index)"
          />
        </v-card-title>
        <v-card-text>
          <v-row
            dense
          >
            <v-col
              cols="12"
            >
              <c-text-field
                v-if="disabled"
                v-model="rs.resourceSupportTypeName"
                ::label="$t('resourceSupport.performedBy')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
              <c-select
                v-else
                v-model="rs.resourceSupportTypeId"
                :items="filteredResourceSupportTypesOptions"
                :label="$t('resourceSupport.performedBy')"
                :rules="[$validator.required()]"
                class="required"
                clearable
                dense
                persistent-placeholder
                outlined
              >
                <template v-slot:selection="data">
                  {{ data.item.text }}
                </template>
              </c-select>
            </v-col>
          </v-row>

          <resource-support-specialist-component
            v-model="rs.resourceSupportSpecialists"
            :disabled="disabled"
            :resource-support-specialists-type-options="resourceSupportSpecialistsTypeOptions"
            :resource-support-specialists-work-place-options="resourceSupportSpecialistsWorkPlaceOptions.filter(filterResourceSupportSpecialistsWorkPlace(rs.resourceSupportTypeId))"
          />
        </v-card-text>
      </v-alert>
    </v-card>
    <v-card-actions
      v-if="!disabled && !disableAdd"
      class="pt-0 px-0"
    >
      <button-tip
        icon-name="mdi-image-plus"
        icon-color="primary"
        color="primary"
        text="resourceSupport.addResourceSupport"
        tooltip="resourceSupport.addResourceSupport"
        iclass="mr-1"
        outlined
        bottom
        small
        @click="onAdd"
      />
    </v-card-actions>
  </div>
</template>

<script>
import ResourceSupportSpecialistComponent from './ResourceSupportSpecialistComponent.vue';
export default {
  name: 'ResourceSupportComponent',
  components: {
    ResourceSupportSpecialistComponent
  },
  props: {
    value: {
      type: Array,
      required: true
    },
    disabled: {
      type: Boolean,
      default: false
    },
    disableAdd: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      model: this.value,
      resourceSupportTypeOptions: [],
      resourceSupportSpecialistsTypeOptions: [],
      resourceSupportSpecialistsWorkPlaceOptions: [],
    };
  },
  computed: {
    filteredResourceSupportTypesOptions() {
      return this.resourceSupportTypeOptions.filter(x => {
        if(this.model.some(e => e.resourceSupportTypeId === x.value)) {
          x.disabled = true;
        } else {
          x.disabled = false;
        }
        return x;
      });
    },
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getResourceSupportTypeOptions()
        .then((response) => {
          this.resourceSupportTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.resourceSupportTypeOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getResourceSupportSpecialistTypesOptions()
        .then((response) => {
          this.resourceSupportSpecialistsTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.resourceSupportSpecialistsTypeOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getResourceSupportSpecialistWorkPlaces()
        .then((response) => {
          this.resourceSupportSpecialistsWorkPlaceOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.ressourceSupportSpecialistWorkPlaces'));
          console.log(error);
        });
    },
    onAdd() {
      this.model.push({ resourceSupportSpecialists: [], });
    },
    onRemove(index) {
      if (!this.model) return;
      this.model.splice(index, 1);
    },
    onResourceSupportSpecialistAdd(index) {
      this.model[index].resourceSupportSpecialists.push({});
    },
    onResourceSupportSpecialistRemove(index, rSSindex) {
      this.model[index].resourceSupportSpecialists.splice(rSSindex, 1);
    },

    filterResourceSupportSpecialistsWorkPlace(resourceSupportTypeId) {
      return (item) => item.relatedObjectId === resourceSupportTypeId;
    },
  }
};
</script>
