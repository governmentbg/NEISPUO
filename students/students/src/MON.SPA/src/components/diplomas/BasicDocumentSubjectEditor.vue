<template>
  <v-card flat>
    <v-card-text class="px-0">
      <v-row dense>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <autocomplete
            v-model="model.subjectId"
            api="/api/lookups/GetSubjectOptions"
            :label="$t('basicDocumentSubject.subjectName')"
            :placeholder="$t('buttons.search')"
            hide-no-data
            hide-selected
            clearable
            :page-size="50"
            :rules="[$validator.required()]"
            class="required"
            defer-options-loading
            persistent-hint
            :hint="$t('common.comboSearchHint', [searchInputMinLength])"
          >
            <template v-slot:item="data">
              <v-list-item-content
                v-text="data.item.text"
              />
              <v-list-item-icon>
                <v-chip
                  color="light"
                  small
                  outlined
                >
                  {{ data.item.value }}
                </v-chip>
              </v-list-item-icon>
            </template>
          </autocomplete>
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-autocomplete
            v-model="model.subjectTypeId"
            :items="subjectTypeOptions"
            :label="$t('basicDocumentSubject.subjectType')"
            clearable
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="12"
          md="2"
          sm="6"
        >
          <v-text-field
            v-model="model.position"
            type="number"
            :label="$t('basicDocumentSubject.position')"
            :rules="[$validator.required(), $validator.min(1), $validator.max(1000)]"
            oninput="if(Number(this.value) < 1) this.value = 1;"
            clearable
            class="required"
          />
        </v-col>
        <v-col
          cols="10"
          md="2"
          sm="6"
        >
          <v-checkbox
            v-model="model.subjectCanChange"
            color="primary"
            :label="$t('basicDocumentSubject.subjectCanChange')"
          />
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions class="pa-0">
      <button-tip
        v-if="!disabled"
        icon
        icon-name="mdi-delete"
        icon-color="error"
        tooltip="buttons.delete"
        iclass="mx-2"
        small
        left
        @click="$emit('subject-delete', model.uid)"
      />
    </v-card-actions>
  </v-card>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import Constants from '@/common/constants';

export default {
  name: 'BasicDocumentSubjectEditor',
  components: {
    Autocomplete
  },
  props: {
    value: {
      type: Object,
      required: true
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    subjectTypeOptions: {
      type: Array,
      default() {
        return [];
      }
    },
    subjectOptions: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
      model: this.value,
      searchInputMinLength: Constants.SEARCH_INPUT_MIN_LENGTH
    };
  },
  mounted() {},
  methods: { }
};
</script>
