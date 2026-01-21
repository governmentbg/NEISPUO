<template>
  <v-form
    :ref="'docManagementDeliveryReportForm' + _uid"
    :disabled="disabled"
  >
    <v-alert
      type="info"
      outlined
      class="mb-4"
    >
      <div class="font-weight-medium mb-2">Как да въведете номерата на документите:</div>
      <ul class="mb-0">
        <li><strong>Документи без фабрична номерация:</strong> Въведете само броя на получените документи в колоната "Брой получени"</li>
        <li><strong>Документи с фабрична номерация (обикновени):</strong> Въведете номерата във формат: 100001, 200004-200010, 300001. Числата с тире представляват диапазон от номера (200004-200010 = номера от 200004 до 200010 включително). Номерата могат да бъдат без водещи нули: 1, 4-10, 301</li>
        <li><strong>Документи с фабрична номерация (дубликати):</strong> Въведете номерата с година на издаване във формат: 100001/2024, 200004-200010/2025, 300001/2025. Числата с тире представляват диапазон от номера (200004-200010/2025 = номера от 200004 до 200010 включително за 2025 г.). Номерата могат да бъдат без водещи нули: 1/2024, 4-10/2025. При дубликати серията НЕ се добавя.</li>
        <li><strong>Документи със серия (само за обикновени документи):</strong> Когато документът има серия и НЕ е дубликат, добавете серията след номера разделена с '/': 100001/Г-24, 200004-200010/ДА, 300001/П-25</li>
      </ul>
    </v-alert>

    <v-row dense>
      <v-col cols="12">
        <v-simple-table>
          <template #default>
            <thead>
              <tr>
                <th>{{ $t('docManagement.application.basicDocument') }}</th>
                <th>{{ $t('docManagement.application.number') }}</th>
                <th>{{ $t('docManagement.application.deliveredCount') }}</th>
                <th style="width: 33%;">{{ $t('docManagement.application.deliveredNumbers') }}</th>
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
                <td>
                  <v-text-field
                    :value="doc.number"
                    type="number"
                    dense
                    disabled
                    hide-details
                  />
                </td>
                <td>
                  <v-text-field
                    v-model.number="doc.deliveredCount"
                    type="number"
                    step="1"
                    min="0"
                    :disabled="disabled || doc.hasFactoryNumber"
                    @wheel="$event.target.blur()"
                  />
                </td>
                <td style="width: 33%;">
                  <v-text-field
                    v-model="doc.deliveredNumbers"
                    :disabled="disabled || !doc.hasFactoryNumber"
                    :hint="getHintForDocument(doc)"
                    :rules="getValidationRulesForDocument(doc)"
                  />
                </td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
      </v-col>
    </v-row>

    <v-row
      dense
      class="mt-4"
    >
      <v-col cols="12">
        <c-textarea
          v-model="model.deliveryNotes"
          :label="$t('docManagement.application.deliveryNotes')"
          outlined
          rows="3"
          :disabled="disabled"
          persistent-placeholder
        />
      </v-col>
    </v-row>
  </v-form>
</template>

<script>

export default {
  name: 'DocManagementDeliveryReportForm',
  props: {
    value: {
      type: Object,
      required: true
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      model: this.value,
    };
  },

  methods: {
    validate() {
      return this.$refs['docManagementDeliveryReportForm' + this._uid].validate();
    },

    getHintForDocument(doc) {
      if (!doc.hasFactoryNumber) {
        return '';
      }

      if (doc.isDuplicate) {
        // Duplicates always use year format, no series
        return this.$t('docManagement.application.deliveredNumbersHintDuplicate');
      } else if (doc.seriesFormat) {
        // Non-duplicates with series
        return this.$t('docManagement.application.deliveredNumbersHintWithSeries', { series: doc.seriesFormat });
      } else {
        // Non-duplicates without series
        return this.$t('docManagement.application.deliveredNumbersHint');
      }
    },

    getValidationRulesForDocument(doc) {
      if (!doc.hasFactoryNumber) {
        return [];
      }

      let pattern;

      if (doc.isDuplicate) {
        // Duplicates always use year format, no series
        pattern = '^\\d+(-\\d+)?/\\d{4}(,\\s*\\d+(-\\d+)?/\\d{4})*$';
      } else if (doc.seriesFormat) {
        // Non-duplicates with series format
        const seriesOptions = doc.seriesFormat.split('|').map(format => {
          // Escape special regex characters and replace YY with year pattern
          return format.replace(/[.*+?^${}()|[\]\\]/g, '\\$&').replace(/YY/g, '\\d{2}');
        }).join('|');

        // Format: number/series or range/series
        pattern = `^\\d+(-\\d+)?/(${seriesOptions})(,\\s*\\d+(-\\d+)?/(${seriesOptions}))*$`;
      } else {
        // Non-duplicates without series format
        pattern = '^\\d+(-\\d+)?(,\\s*\\d+(-\\d+)?)*$';
      }

      return [this.$validator.regex(pattern, this.$t('validation.invalidFormat'))];
    }
  }
};
</script>
