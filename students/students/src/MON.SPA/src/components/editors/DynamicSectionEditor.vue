<template>
  <div
    v-if="model"
  >
    <v-expansion-panel-header>
      <v-card-actions>
        {{ $t('dynamicSection.section') }}<strong class="ml-2"><i>{{ model.title }}</i></strong>
        <v-spacer />
        <button-tip
          icon
          icon-name="mdi-delete"
          icon-color="error"
          iclass=""
          tooltip="dynamicSection.removeSectionBtnTooltip"
          bottom
          color="error"
          @click="$emit('sectionRemove', model)"
        />
      </v-card-actions>
    </v-expansion-panel-header>
    <v-expansion-panel-content>
      <v-card flat>
        <v-card-text>
          <v-row dense>
            <v-col
              cols="12"
            >
              <!-- {{ model }} -->
            </v-col>
            <v-col
              cols="12"
              md="5"
            >
              <v-text-field
                v-model="model.title"
                :label="$t('dynamicSection.title')"
                :disabled="disabled"
                :readonly="readonly"
                class="required"
                :rules="[$validator.required()]"
                clearable
              />
            </v-col>
            <v-col
              cols="12"
              md="5"
            >
              <v-text-field
                v-model="model.titleEn"
                :label="$t('dynamicSection.titleEn')"
                :disabled="disabled"
                :readonly="readonly"
                clearable
              />
            </v-col>
            <v-col
              cols="12"
              md="2"
            >
              <v-text-field
                v-model="model.order"
                type="number"
                :label="$t('dynamicSection.order')"
                :rules="[$validator.min(1), $validator.max(1000)]"
                oninput="if(Number(this.value) < 1) this.value = 1;"
                :disabled="disabled"
                :readonly="readonly"
              />
            </v-col>
          </v-row>
        </v-card-text>
        <v-card-actions>
          <v-btn
            raised
            color="secondary"
            outlined
            small
            class="my-2"
            @click.stop="onNewFieldAdd"
          >
            <v-icon left>
              mdi-plus
            </v-icon>
            {{ $t('dynamicField.newFieldBtnAddTooltip') }}
          </v-btn>
        </v-card-actions>
      </v-card>

      <v-expansion-panels
        v-if="model"
      >
        <draggable
          :list="sortedFields"
          class="row"
          ghost-class="ghost"
        >
          <v-expansion-panel
            v-for="(field, index) in sortedFields"
            :key="field.uid"
          >
            <dynamic-section-field-editor
              :ref="'dynamicSectionFieldEditor-' + field.id"
              v-model="sortedFields[index]"
              :show-db-settings="showDbSettings"
              :use-vuetify-grid-system="model.useVuetifyGridSystem"
              @fieldRemove="onFieldRemove"
            />
          </v-expansion-panel>
        </draggable>
      </v-expansion-panels>
    </v-expansion-panel-content>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import DynamicSectionFieldEditor from '@/components/editors/DynamicSectionFieldEditor.vue';
import { DynamicFieldModel } from '@/models/dynamic/dynamicFieldModel';
import Draggable from 'vuedraggable';

export default {
  name: 'DynamicSectionEditor',
  components: {
    DynamicSectionFieldEditor, Draggable
  },
  props: {
    value: {
      type: Object,
      default() {
        return {};
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    readonly: {
      type: Boolean,
      default() {
        return false;
      }
    },
    showDbSettings: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.value,
    };
  },
  computed: {
    sortedFields() {
      if(!this.hasFields) return [];

      let sortedFields = this.model.items;
      sortedFields = sortedFields.sort((a, b) => { return a.order - b.order; });
      return sortedFields;
    },
    hasFields() {
      return this.model && this.model.items && Array.isArray(this.model.items);
    }
  },
  methods: {
    onNewFieldAdd() {
      if(!this.hasFields) {
        this.model.items = [];
      }

      this.model.items.push(new DynamicFieldModel({
        visible: true,
        editable: true,
        order: 1,
        label: '...Моля, въведете ИД и етикет!',
        cols: "12",
        md: "6" }));
    },
    async onFieldRemove(item) {
      if(!item) {
        console.log('Empty field');
        return;
      }

      if(this.hasFields) {
        const index = this.model.items.findIndex(x => x.uid == item.uid);
        if(index >= 0) {
          if(await this.$refs.confirm.open(this.$t('dynamicField.removeFieldBtnTooltip'), this.$t('common.confirm'))) {
            this.model.items.splice(index, 1);
          }
        }
      }
    }
  }
};
</script>
