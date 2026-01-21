<template>
  <v-form
    :ref="'docManagementApplicationForm' + _uid"
    :disabled="disabled"
  >
    <v-row dense>
      <v-col cols="12">
        <v-simple-table>
          <template #default>
            <thead>
              <tr>
                <th>{{ $t('docManagement.application.basicDocument') }}</th>
                <th v-if="isExchangeRequest">{{ $t('docManagement.unusedDocs.headers.freeDocCount') }}</th>
                <th>{{ $t('docManagement.application.number') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(doc, index) in model.basicDocuments"
                :key="index"
              >
                <td>
                  <span>{{ doc.basicDocumentName }}</span>
                </td>
                <td v-if="isExchangeRequest">
                  <span>{{ doc.freeDocCount }}</span>
                </td>
                <td>
                  <v-text-field
                    v-model.number="doc.number"
                    type="number"
                    dense
                    step="1"
                    :hide-details="!isExchangeRequest"
                    :rules="isExchangeRequest ? [$validator.max(doc.freeDocCount), $validator.min(1), $validator.numbers()] : []"
                    :clearable="isExchangeRequest"
                    @wheel="$event.target.blur()"
                  />
                </td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
      </v-col>
    </v-row>
    <v-card class="ma-5">
      <v-card-title>
        {{ $t('common.attachedFiles') }}
      </v-card-title>
      <v-card-text>
        <file-manager
          v-model="model.attachments"
          :disabled="disabled"
        />
      </v-card-text>
    </v-card>
    <grid
      v-if="model.isExchangeRequest && model.requestedInstitutionId"
      url="/api/docManagementExchange/listFree"
      :headers="headers"
      :title="$tc('docManagement.unusedDocs.title', 2)"
      :filter="{
        institutionId: model.requestedInstitutionId,
      }"
    >
      <template v-slot:[`item.institutionId`]="{ item}">
        {{ `${item.institutionId} - ${item.institutionName}` }}
      </template>
    </grid>
  </v-form>
</template>

<script>
import { DocManagementApplicationModel } from '@/models/docManagement/docManagementApplicationModel.js';
import FileManager from '@/components/common/FileManager.vue';
import Grid from "@/components/wrappers/grid.vue";

export default {
  name: 'DocManagementApplicationForm',
  components: {
    FileManager,
    Grid
  },
  props: {
    value:{
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    isExchangeRequest: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.value ?? new DocManagementApplicationModel(),
      headers: [
        {
          text: this.$t('docManagement.unusedDocs.headers.basicDocument'),
          value: "basicDocumentName",
        },
        {
          text: this.$t('docManagement.unusedDocs.headers.institution'),
          value: "institutionId",
        },
        {
          text: this.$t('docManagement.unusedDocs.headers.freeDocCount'),
          value: "freeDocCount",
        }
      ],
    };
  },
  methods: {
    validate() {
      return this.$refs['docManagementApplicationForm' + this._uid].validate();
    }
  },
};
</script>
