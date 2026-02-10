<template>
  <v-card>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-card-title />
      <v-card-text>
        <v-row>
          <v-col cols="5">
            <c-info uid="printTemplate.name">
              <v-text-field
                v-model="form.name"
                :value="form.name"
                :label="$t('printTemplate.name')"
                class="required"
                :rules="[$validator.required()]"
              />
            </c-info>
          </v-col>
          <v-col cols="5">
            <c-info uid="basicDocument.type">
              <autocomplete
                v-model="form.basicDocumentId"
                api="/api/lookups/GetBasicDocumentTypes"
                :label="$t('printTemplate.basicDocument')"
                :disabled="disabled"
                :disable-typing="true"
                clearable
                class="required"
                :defer-options-loading="false"
                :rules="[$validator.required()]"
                :filter="{ isRuoDoc: isRuo, filterByDetailedSchoolType: true }"
              />
            </c-info>
          </v-col>
          <v-col cols="2">
            <c-info uid="basicDocument.printForm">
              <autocomplete
                v-model="form.printFormId"
                api="/api/basicDocument/PrintFormDropdownOptions"
                :label="$t('printTemplate.printForm')"
                :placeholder="$t('common.choose')"
                clearable
                :defer-options-loading="false"
                persistent-hint
                class="required"
                :rules="[$validator.required()]"
                :filter="{ basicDocumentId: form.basicDocumentId }"
              />
            </c-info>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="pr-4">
            <v-slider
              v-model="form.left1Margin"
              label="Отместване отляво 1 стр."
              class="align-center"
              :max="20"
              :min="-20"
              prepend-icon="fa-arrows-alt-h"
              hide-details
            >
              <template v-slot:append>
                <v-text-field
                  v-model="form.left1Margin"
                  class="mt-0 pt-0"
                  hide-details
                  single-line
                  type="number"
                  suffix="mm"
                  style="width: 100px"
                />
              </template>
            </v-slider>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="pr-4">
            <v-slider
              v-model="form.top1Margin"
              class="align-center"
              label="Отместване отгоре 1 стр."
              :max="20"
              :min="-20"
              hide-details
              prepend-icon="fa-arrows-alt-v"
            >
              <template v-slot:append>
                <v-text-field
                  v-model="form.top1Margin"
                  class="mt-0 pt-0"
                  hide-details
                  single-line
                  suffix="mm"
                  type="number"
                  style="width: 100px"
                />
              </template>
            </v-slider>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="pr-4">
            <v-slider
              v-model="form.left2Margin"
              label="Отместване отляво 2 стр."
              class="align-center"
              :max="20"
              :min="-20"
              prepend-icon="fa-arrows-alt-h"
              hide-details
            >
              <template v-slot:append>
                <v-text-field
                  v-model="form.left2Margin"
                  class="mt-0 pt-0"
                  hide-details
                  single-line
                  type="number"
                  suffix="mm"
                  style="width: 100px"
                />
              </template>
            </v-slider>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="pr-4">
            <v-slider
              v-model="form.top2Margin"
              class="align-center"
              label="Отместване отгоре 2 стр."
              :max="20"
              :min="-20"
              hide-details
              prepend-icon="fa-arrows-alt-v"
            >
              <template v-slot:append>
                <v-text-field
                  v-model="form.top2Margin"
                  class="mt-0 pt-0"
                  hide-details
                  single-line
                  suffix="mm"
                  type="number"
                  style="width: 100px"
                />
              </template>
            </v-slider>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <c-info uid="printTemplate.description">
              <v-textarea
                v-model="form.description"
                outlined
                prepend-icon="mdi-comment"
                :label="$t('common.detailedInformation')"
                :value="form.description"
              />
            </c-info>
          </v-col>
        </v-row>
      </v-card-text>
    </v-form>
  </v-card>
</template>

<script>
import { PrintTemplateModel } from "@/models/printTemplateModel.js";
import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import { mapGetters } from 'vuex';
import { UserRole } from '@/enums/enums';

export default {
  name: "PrintTemplateForm",
  components:{
    Autocomplete
  },
  props: {
    form: {
      type: PrintTemplateModel,
      required: true,
    },
    disabled: {
      type: Boolean,
      default(){
        return false;
      }
    },
  },
  data() {
    return {};
  },
  computed: {
    ...mapGetters(['userRegionId', 'isInRole']),
    isRuo() {
      return (this.isInRole(UserRole.Ruo) || this.isInRole(UserRole.RuoExpert)) && !!this.userRegionId;
    }
  },
  mounted() {

  },
  methods: {
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
  },
};
</script>
