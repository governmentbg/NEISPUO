<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row dense>
        <v-col cols="12">
          <text-field
            v-model="model.title"
            :label="$t('issue.title')"
            clearable
            counter
            maxlength="200"
            :rules="[$validator.required(), $validator.maxLength(200)]"
            class="required"
          />
        </v-col>
        <v-col cols="12">
          <v-textarea
            v-model="model.description"
            :label="$t('issue.description')"
            prepend-icon="mdi-text"
            outlined
            clearable
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col cols="12">
          <text-field
            v-model="model.phone"
            :label="$t('issue.phone')"
            clearable
            counter
            maxlength="255"
            prepend-icon="mdi-phone"
            :rules="[$validator.maxLength(255)]"
          />
        </v-col>
        <v-col cols="12">
          <v-alert
            v-if="selectedCategory && selectedCategory.description"
            border="top"
            colored-border
            type="info"
            elevation="2"
          >
            {{ selectedCategory.description }}
          </v-alert>
        </v-col>
        <v-col
          cols="12"
          md="6"
          xl="4"
        >
          <custom-autocomplete
            v-model="model.categoryId"
            api="/api/lookups/GetCategories"
            :label="$t('issue.category')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :defer-options-loading="false"
            :disabled="disabled"
            no-filter
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          xl="4"
        >
          <custom-autocomplete
            id="issueSubcategory"
            ref="issueSubcategory"
            v-model="model.subcategoryId"
            api="/api/lookups/GetSubcategories"
            :label="$t('issue.subcategory')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :defer-options-loading="false"
            :disabled="disabled || !model.categoryId"
            :filter="categoryFilter"
            no-filter
            loading
          />
        </v-col>
        <v-col
          cols="12"
          md="6"
          xl="2"
        >
          <custom-autocomplete
            v-model="model.priorityId"
            api="/api/lookups/GetPriorities"
            :label="$t('issue.priority')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :defer-options-loading="false"
            :disabled="disabled"
            no-filter
            :rules="[$validator.required()]"
            class="required"
            loading
          />
        </v-col>
        <v-col
          v-if="isElevated"
          cols="12"
          md="6"
          xl="2"
        >
          <v-checkbox
            v-model="model.isEscalated"
            :label="$t('issue.isEscalated')"
            :disabled="disabled"
          />
        </v-col>
        <v-col
          v-if="isLevel2SupportGroup"
          cols="12"
          md="6"
          xl="2"
        >
          <v-checkbox
            v-model="model.isLevel3Support"
            :label="$t('issue.isLevel3Support')"
            :disabled="disabled"
          />
        </v-col>
        <v-col
          v-if="isElevated"
          cols="12"
          md="6"
          xl="6"
        >
          <custom-autocomplete
            v-model="model.assignedToSysUserId"
            api="/api/lookups/GetUsers"
            :label="$t('issue.assignee')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :defer-options-loading="false"
            :disabled="disabled"
          />
        </v-col>
        <v-col
          v-if="isLevel2SupportGroup"
          cols="12"
          md="6"
          xl="2"
        >
          <v-switch
            v-model="model.requestForInformation"
            :label="$t('issue.requestForInformation')"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
      <v-row dense>
        <v-col>
          <v-alert
            v-if="selectedCategory && selectedCategory.surveySchema"
            border="left"
            colored-border
            type="info"
            elevation="2"
          >
            <v-jsf
              v-model="model.surveyObject"
              :schema="JSON.parse(selectedCategory.surveySchema)"
            />
          </v-alert>
        </v-col>
      </v-row>
      <v-row dense>
        <v-col>
          <file-manager
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-form>
  </div>
</template>

<script>
import { IssueModel } from "@/models/issueModel";
import CustomAutocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import FileManager from "@/components/wrappers/FileManager.vue";
import { mapGetters } from "vuex";
import { UserRole } from "@/enums/enums";
import VJsf from '@koumoul/vjsf/lib/VJsf.js';
import '@koumoul/vjsf/lib/VJsf.css';
import '@koumoul/vjsf/lib/deps/third-party.js';

export default {
  name: "IssueForm",
  components: {
    CustomAutocomplete,
    FileManager,
    VJsf
  },
  props: {
    issue: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model: this.issue ?? new IssueModel(),
      selectedCategory: null,
      statuses: [],
      priorities: [],
      categories: [],
      valid: false,
    };
  },
  computed: {
    ...mapGetters(["isInRole"]),
    isElevated() {
      return (
        this.isInRole(UserRole.Ruo) ||
        this.isInRole(UserRole.RuoExpert) ||
        this.isInRole(UserRole.Mon) ||
        this.isInRole(UserRole.MonExpert) ||
        this.isInRole(UserRole.Cioo) ||
        this.isInRole(UserRole.Consortium)
      );
    },
    isLevel2SupportGroup() {
      return (
        this.isInRole(UserRole.Mon) ||
        this.isInRole(UserRole.MonExpert) ||
        this.isInRole(UserRole.Cioo) ||
        this.isInRole(UserRole.Consortium)
      );
    },
    categoryFilter() {
      if (!this.model) return { parentId: null };
      return { parentId: this.model.categoryId };
    },
  },
  watch: {
    "model.categoryId": {
      handler: function (val) {
        if (!val) {
          // Махнали сме категорията и следва да махнем подкатегорията.
          this.model.subcategoryId = null;
          this.selectedCategory = null;
        } else {
          this.$api.lookups
            .getCategory(this.model.categoryId)
            .then((response) => {
              if (response.data) {
                this.selectedCategory = response.data;
              }
            })
            .catch((error) => {
              console.log(error.response);
            });

          // Сменили сме категорията и следва да манем подкатегрията, ако не е от избраната категория.
          const subcategoryId = this.model?.subcategoryId;
          if (subcategoryId) {
            const options = this.$refs["issueSubcategory"].getOptionsList();
            const item = options.find((x) => x.value === subcategoryId);
            if (!item || (item && val != item.relatedObjectId)) {
              this.model.subcategoryId = null;
            }
          }
        }
      },
    },
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
    loadOptions() {
      this.$api.lookups
        .getStatuses()
        .then((response) => {
          if (response.data) {
            this.statuses = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });

      this.$api.lookups
        .getPriorities()
        .then((response) => {
          if (response.data) {
            this.priorities = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });

      this.$api.lookups
        .getCategories()
        .then((response) => {
          if (response.data) {
            this.categories = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });

        if (this.model?.categoryId){
        this.$api.lookups
            .getCategory(this.model.categoryId)
            .then((response) => {
              if (response.data) {
                this.selectedCategory = response.data;
              }
            })
            .catch((error) => {
              console.log(error.response);
            });
          }
    },
  },
};
</script>
