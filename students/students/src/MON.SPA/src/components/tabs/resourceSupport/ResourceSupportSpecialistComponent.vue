<template>
  <div>
    <v-row
      v-if="model"
      dense
    >
      <v-col
        v-for="(item, index) in model"
        :key="index"
        cols="12"
        sm="6"
        md="4"
      >
        <v-card
          flat
        >
          <v-alert
            outlined
            color="indigo lighten-1"
            class="pa-0"
          >
            <v-card-text
              class="my-1 pb-0 pt-2"
            >
              <v-row dense>
                <v-col
                  cols="12"
                >
                  <c-text-field
                    v-if="disabled"
                    v-model="item.resourceSupportSpecialistTypeName"
                    :label="$t('resourceSupport.resourceSupportSpecialistLabel')"
                    disabled
                    dense
                    persistent-placeholder
                    outlined
                  />
                  <v-select
                    v-else
                    v-model="item.resourceSupportSpecialistTypeId"
                    :items="filteredResourceSupportSpecialistsTypeOptions"
                    :label="$t('resourceSupport.resourceSupportSpecialistLabel')"
                    :rules="[$validator.required()]"
                    class="required"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                    @change="model.specialNeedsSubTypeId = null"
                  >
                    <template v-slot:selection="data">
                      {{ data.item.text }}
                    </template>
                  </v-select>
                </v-col>
                <v-col
                  cols="12"
                >
                  <c-text-field
                    v-if="disabled"
                    v-model="item.workPlaceName"
                    :label="$t('resourceSupport.resourceSupportSpecialistWork')"
                    disabled
                    dense
                    persistent-placeholder
                    outlined
                  />
                  <v-select
                    v-else
                    v-model="item.workPlaceId"
                    :items="resourceSupportSpecialistsWorkPlaceOptions"
                    :label="$t('resourceSupport.resourceSupportSpecialistWork')"
                    :rules="[$validator.required()]"
                    class="required"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                    @change="model.specialNeedsSubTypeId = null"
                  >
                    <template v-slot:selection="data">
                      {{ data.item.text }}
                    </template>
                  </v-select>
                </v-col>
                <v-col
                  cols="12"
                >
                  <c-text-field
                    v-model="item.weeklyHours"
                    type="number"
                    :label="$t('resourceSupport.specialistWeeklyHours')"
                    :disabled="disabled"
                    dense
                    persistent-placeholder
                    outlined
                    clearable
                    :class="disabled ? '' : 'required'"
                    :rules="disabled ? [] : [$validator.required(), $validator.min(0), $validator.max(999)]"
                    @wheel="$event.target.blur()"
                  />
                </v-col>
                <v-col
                  v-if="item.resourceSupportSpecialistTypeId === 10"
                  cols="12"
                >
                  <c-text-field
                    v-model="item.organizationType"
                    :label="$t('resourceSupport.organizationType')"
                    :disabled="disabled"
                    dense
                    persistent-placeholder
                    outlined
                    clearable
                  />
                </v-col>
                <v-col
                  v-if="item.resourceSupportSpecialistTypeId === 10"
                  cols="12"
                >
                  <c-text-field
                    v-model="item.organizationName"
                    :label="$t('resourceSupport.organizationName')"
                    :disabled="disabled"
                    dense
                    persistent-placeholder
                    outlined
                    clearable
                  />
                </v-col>
                <v-col
                  v-if="item.resourceSupportSpecialistTypeId === 10"
                  cols="12"
                >
                  <c-text-field
                    v-model="item.name"
                    :label="$t('resourceSupport.teacherNameLabel')"
                    :disabled="disabled"
                    dense
                    persistent-placeholder
                    outlined
                    clearable
                  />
                </v-col>
                <v-col
                  v-if="item.resourceSupportSpecialistTypeId === 10"
                  cols="12"
                >
                  <c-text-field
                    v-model="item.specialistType"
                    :label="$t('resourceSupport.specialistType')"
                    :disabled="disabled"
                    dense
                    persistent-placeholder
                    outlined
                    clearable
                  />
                </v-col>
              </v-row>
            </v-card-text>
            <v-card-actions
              class="my-0 pt-0"
            >
              <v-spacer />
              <button-tip
                v-if="!disabled"
                icon
                icon-name="mdi-delete"
                icon-color="red"
                iclass="mx-2"
                :disabled="disabled"
                tooltip="buttons.resourceSupportSpecialistDeleteTfooltip"
                small
                bottom
                @click="onRemove(index)"
              />
            </v-card-actions>
          </v-alert>
        </v-card>
      </v-col>
    </v-row>
    <v-card-actions
      v-if="!disabled"
      class="pa-0"
    >
      <button-tip
        icon-name="mdi-account-plus"
        icon-color="indigo lighten-1"
        color="indigo lighten-1"
        text="buttons.resourceSupportSpecialistAddTooltip"
        tooltip="buttons.resourceSupportSpecialistAddTooltip"
        iclass="mr-1"
        outlined
        bottom
        small
        @click="onAdd()"
      />
    </v-card-actions>
  </div>
</template>

<script>
export default {
  name: 'ResourceSupportSpecialistComponent',
  props: {
    value: {
      type: Array,
      required: true
    },
    disabled: {
      type: Boolean,
      default: false
    },
    resourceSupportSpecialistsTypeOptions: {
      type: Array,
      default() {
        return [];
      }
    },
    resourceSupportSpecialistsWorkPlaceOptions: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
      model: this.value,
    };
  },
  computed: {
    filteredResourceSupportSpecialistsTypeOptions() {
      return this.resourceSupportSpecialistsTypeOptions.filter(x => {
        if(this.model.some(e => e.resourceSupportSpecialistTypeId === x.value)) {
          x.disabled = true;
        } else {
          x.disabled = false;
        }
        return x;
      });
    },
  },
  watch: {
    resourceSupportSpecialistsWorkPlaceOptions(value) {
      if(!this.disabled && value.length > 0) {
        this.model.forEach(x => {
          if(x.workPlaceId && !value.some(el => el.value == x.workPlaceId)) {
            x.workPlaceId = null;
          }
        });
      }
    }
  },
  methods: {
    onAdd() {
      this.model.push({ });
    },
    onRemove(index) {
      if (!this.model) return;
      this.model.splice(index, 1);
    },
    filterResourceSupportSpecialistsWorkPlace(specialNeedsTypeId) {
      return (item) => item.relatedObjectId === specialNeedsTypeId;
    },
  }
};
</script>
