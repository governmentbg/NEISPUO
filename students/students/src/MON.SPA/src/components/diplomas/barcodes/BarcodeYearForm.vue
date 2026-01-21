<template>
  <v-card>
    <v-card-title>
      <div v-if="isEditFormMode">
        {{ $t('diplomas.barcodes.editBarcodeTitle') }}
      </div>
      <div v-else>
        {{ $t('diplomas.barcodes.addBarcodeTitle') }}
      </div>
      <v-spacer v-if="isEditFormMode" />
      <edit-button
        v-if="isEditFormMode"
        v-model="isEditMode"
      />
    </v-card-title>
    <v-card-text>
      <v-row>
        <v-col
          cols="12"
          md="3"
        >
          <c-info
            uid="barcodeYear.schoolYear"
          >
            <school-year-selector
              v-if="form"
              v-model="form.schoolYear"
              :label="$t('common.schoolYear')"
              class="required"
              :disabled="saving || !isEditMode"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="3"
        >
          <c-info
            uid="barcodeYear.edition"
          >
            <v-text-field
              v-model="form.edition"
              type="number"
              :label="$t('diplomas.barcodes.edition')"
              :rules="[$validator.required(), $validator.min(1900), $validator.max(3000)]"
              clearable
              class="required"
              :disabled="saving || !isEditMode"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="3"
        >
          <c-info
            uid="barcodeYear.headerPage"
          >
            <v-text-field
              v-model="form.headerPage"
              :label="$t('diplomas.barcodes.headerPage')"
              :rules="[$validator.required(), $validator.alphanumeric()]"
              clearable
              class="required"
              :disabled="saving || !isEditMode"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="3"
        >
          <c-info
            uid="barcodeYear.internalPage"
          >
            <v-text-field
              v-model="form.internalPage"
              :label="$t('diplomas.barcodes.internalPage')"
              :rules="[$validator.alphanumeric()]"
              clearable
              :disabled="saving || !isEditMode"
            />
          </c-info>
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-btn
        raised
        color="primary"
        class="mb-3 ml-3"
        :disabled="saving"
        @click.stop="redirectToList"
      >
        {{ $t('menu.diplomas.barcodesList') }}
      </v-btn>
      <v-spacer />
      <v-btn
        ref="submit"
        raised
        color="primary"
        :disabled="saving || !isEditMode"
        type="submit"
      >
        <v-icon left>
          fas fa-save
        </v-icon>
        {{ $t("buttons.saveChanges") }}
      </v-btn>
      <v-btn
        v-if="!isEditFormMode"
        raised
        color="error"
        :disabled="saving || !isEditMode"
        @click="onReset"
      >
        <v-icon left>
          fas fa-times
        </v-icon>
        {{ $t("buttons.clear") }}
      </v-btn>
    </v-card-actions>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import { BarcodeYearModel } from "@/models/diploma/barcodeYearModel.js";

export default {
    name: 'BarcodeYearForm',
    components: {
        SchoolYearSelector
    },
    props: {
        basicDocumentId: {
            type: Number,
            required: true
        },
        form:{
            type: BarcodeYearModel,
            required: true
        },
        isEditFormMode:{
            type: Boolean,
            required: true
        },
        saving:{
            type: Boolean,
            required: true
        }
    },
  data()
    {
        return {
            isEditMode: false
        };
    },
    mounted() {
        if(!this.isEditFormMode){
            this.isEditMode = true;
        }
        this.loadData();
    },
    methods: {
        redirectToList() {
            this.$router.push(`/basicDocument/${this.basicDocumentId}/barcodes/`);
        },
        loadData() {
          
        },
        async onReset() {
            if(await this.$refs.confirm.open(this.$t('buttons.clear'), this.$t('lod.awards.resetData'))) {
                this.$emit('ResetForm');
            }
        },
    }
};
</script>