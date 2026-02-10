<template>
  <div>
    <app-loader
      v-if="loading"
    />
    <form-layout
      v-else
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h4>{{ $t('basicDocument.schemaEditTitle', { name: basicDocumentName}) }}</h4>
      </template>
      <template #subtitle>
        <basic-document-creation-help />
        <v-btn
          raised
          color="primary"
          small
          @click.stop="onNewBasicDocumentPartAdd"
        >
          <v-icon left>
            mdi-plus
          </v-icon>
          {{ $t('basicDocument.addSubjectPartButton') }}
        </v-btn>
      </template>

      <template #left-actions>
        <v-spacer />
        <v-alert
          v-if="hasUnsavedChanges"
          dense
          outlined
          type="error"
        >
          {{ $t('common.hasUnsavedChanges') }}
        </v-alert>
      </template>

      <template #default>
        <v-tabs
          v-if="model"
          v-model="tab"
        >
          <v-tab key="basicData">
            {{ $t('studentTabs.basicData') }}
          </v-tab>

          <v-tab
            v-for="part in model.basicDocumentParts"
            :key="part.uid"
          >
            {{ part.name || part.uid }}
          </v-tab>
        </v-tabs>
        <v-tabs-items
          v-model="tab"
        >
          <v-tab-item key="basicData">
            <v-form
              v-if="model && model.schema"
              :ref="'basicDocumentForm' + _uid"
              :disabled="saving"
            >
              <v-btn
                raised
                color="primary"
                outlined
                small
                class="my-2"
                @click.stop="onNewSectionAdd"
              >
                <v-icon left>
                  mdi-plus
                </v-icon>
                {{ $t('dynamicSection.newSectionBtnAddTooltip') }}
              </v-btn>

              <v-expansion-panels>
                <v-expansion-panel
                  v-for="(section, index) in sortedSections"
                  :key="section.uid"
                >
                  <dynamic-section-editor
                    :ref="'dynamicSectionEditor-' + section.id"
                    v-model="sortedSections[index]"
                    @sectionRemove="onSectionRemove"
                  />
                </v-expansion-panel>
              </v-expansion-panels>
            </v-form>
          </v-tab-item>
          <v-tab-item
            v-for="(part, index) in sortedBasicDocumentParts"
            :key="part.uid"
            eager
          >
            <basic-document-part-editor
              :ref="'BasicDocumentPartForm_' + part.uid"
              v-model="sortedBasicDocumentParts[index]"
              :disabled="saving"
              @docPartDelete="onBasicDocumentPartDelete"
            />
          </v-tab-item>
        </v-tabs-items>
      </template>
    </form-layout>

    <v-card
      v-if="model && model.schema && showPreview"
      class="mt-2"
    >
      <v-card-title>{{ $t('buttons.review') }}</v-card-title>
      <v-card-text>
        <diploma-section-generator
          :schema="model.schema"
          :include-validations="false"
          show-diploma-data
        />
      </v-card-text>
    </v-card>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import AppLoader from '@/components/wrappers/loader.vue';
import DynamicSectionEditor from '@/components/editors/DynamicSectionEditor.vue';
import DiplomaSectionGenerator from '@/components/tabs/diplomas/DiplomaSectionGenerator.vue';
import BasicDocumentPartEditor from '@/components/diplomas/BasicDocumentPartEditor.vue';
import BasicDocumentCreationHelp from '@/components/diplomas/BasicDocumentCreationHelp.vue';
import { DynamicSectionModel } from '@/models/dynamic/dynamicSectionModel';
import { BasicDocumentPartModel } from '@/models/diploma/basicDocumentPartModel';
import isEqual from 'lodash.isequal';
import clonedeep from 'lodash.clonedeep';

export default {
  name: 'BasicDocumentEdit',
  components: {
    AppLoader,
    DynamicSectionEditor,
    DiplomaSectionGenerator,
    BasicDocumentPartEditor,
    BasicDocumentCreationHelp
  },
  props: {
    id: {
      type: Number,
      required: true,
    }
  },
  data() {
    return {
      initialData: null,
      model: null,
      loading: false,
      saving: false,
      basicDocumentName: '',
      showPreview: true,
      tab: null
    };
  },
  computed: {
    sortedSections() {
      if(!this.hasSections) return [];

      let sortedSections = this.model.schema;
      sortedSections = sortedSections.sort((a, b) => { return a.order - b.order; });
      return sortedSections;
    },
    sortedBasicDocumentParts() {
      if(!this.model || !this.model.basicDocumentParts) return [];

      let sortedBasicDocumentParts = this.model.basicDocumentParts;
      sortedBasicDocumentParts = sortedBasicDocumentParts.sort((a,b) => { return a.position - b.position; });
      return sortedBasicDocumentParts;
    },
    hasSections() {
      return this.model && this.model.schema && Array.isArray(this.model.schema);
    },
    hasBasicDocumentParts() {
      return this.model && this.model.basicDocumentParts && Array.isArray(this.model.basicDocumentParts);
    },
    hasUnsavedChanges() {
      return !isEqual(this.initialData, this.model);
    },
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.basicDocument.loadTemplate(this.id)
      .then(response => {
        if(response.data) {
          this.basicDocumentName = response.data.basicDocumentName;
          if(response.data.schema && Array.isArray(response.data.schema)) {
            this.model = {
              id: response.data.id,
              basicDocumentName: response.data.basicDocumentName,
              basicDocumentParts : response.data.basicDocumentParts ? response.data.basicDocumentParts.map(x => new BasicDocumentPartModel(x)) : [],
              schema: response.data.schema.map(x => new DynamicSectionModel(x))
            };

            this.initialData = clonedeep(this.model);
          }
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.diplomaSchemaLoad'));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    onSave() {
      const form = this.$refs['basicDocumentForm' + this._uid];
      let isValid = form.validate();

      this.model.basicDocumentParts.forEach(x => {
        const partForm = this.$refs['BasicDocumentPartForm_' + x.uid];
        if (partForm) {
          isValid = isValid && partForm[0].validate();
        }
      });

      if(!isValid) {
        const errorMessages = document.getElementsByClassName('v-messages__message');
        if(errorMessages && errorMessages.length > 0) {
          errorMessages[0].scrollIntoView();
        }

        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;
      this.$api.basicDocument
        .saveTemplate(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.goBack();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error.response.message, 5000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.goBack();
    },
    goBack() {
      if(this.returnUrl) {
        this.$router.push(this.returnUrl);
      } else {
        this.$router.go(-1);
      }
    },
    onNewBasicDocumentPartAdd() {
      if(!this.model) return;

      if(!this.hasBasicDocumentParts) {
        this.model.basicDocumentParts = [];
      }
      this.model.basicDocumentParts.push(new BasicDocumentPartModel());
      console.log(this.model.basicDocumentParts.length);
      this.tab =  this.model.basicDocumentParts.length;
    },
    onBasicDocumentPartDelete(uid) {
      const index = this.model.basicDocumentParts.findIndex(x => x.uid === uid);
      if(index > -1) {
        this.model.basicDocumentParts.splice(index, 1);
      }
    },
    onNewSectionAdd() {
      if(!this.hasSections) {
        this.model.schema = [];
      }

      this.model.schema.push(new DynamicSectionModel({
        id: this.$uuid.v4(),
        order: 1,
        visible: true,
        title: '...Моля, въведете заглавие!'
      }));
    },
    async onSectionRemove(item) {
      if(!item) {
        console.log('Empty section');
        return;
      }

      if(this.hasSections) {
        const index = this.model.schema.findIndex(x => x.uid == item.uid);
        if(index >= 0) {
          if(await this.$refs.confirm.open(this.$t('dynamicSection.removeSectionBtnTooltip'), this.$t('common.confirm'))) {
            this.model.schema.splice(index, 1);
          }
        }
      }
    }
  }
};
</script>
