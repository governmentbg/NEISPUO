<template>
  <v-card>
    <v-card-title>
      {{ this.$t('diplomas.barcodes.barcodesList') + ' ' + this.$t('diplomas.barcodes.forDiplomaType') + ': ' + diplomaTypeName }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        ref="barcodeYearsListTable"
        :items="barcodesData"
        :items-pers-page="10"
        :headers="headers"
        :loading="loading"
        :search="search"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <GridExporter
              :items="barcodesData"
              :file-extensions="['xlsx', 'csv', 'txt']"
              :file-name="$t('diplomas.barcodes.barcodesList')"
              :headers="headers"
            />
            <v-spacer />
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              :label="$t('common.search')"
              single-line
              hide-details
            />
          </v-toolbar>
        </template>
        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <template>
              <button-tip
                icon
                icon-name="mdi-numeric-1-circle-outline"
                icon-color="primary"
                iclass=""
                small
                tooltip="buttons.download"
                bottom
                :href="downloadBarcode(item,1)"
              />
              <button-tip
                icon
                icon-name="mdi-numeric-2-circle-outline"
                icon-color="primary"
                iclass=""
                small
                tooltip="buttons.download"
                bottom
                :href="downloadBarcode(item,2)"
              />
              <button-tip
                icon
                icon-name="mdi-pencil"
                icon-color="primary"
                iclass=""
                small
                tooltip="buttons.edit"
                bottom
                @click="editItem(item)"
              />
              <button-tip
                icon
                icon-name="mdi-delete"
                icon-color="error"
                iclass=""
                small
                tooltip="buttons.delete"
                bottom
                @click="deleteBarcode(item)"
              />
            </template>
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>

    <v-row>
      <v-col
        cols="6"
        md="6"
      >
        <v-btn
          raised
          color="primary"
          class="mb-3 ml-3"
          :to="`/basicDocuments`"
        >
          {{ $t('menu.diplomas.toBasicDocuments') }}
        </v-btn>
      </v-col>

      <v-spacer />

      <v-col
        cols="6"
        md="6"
        align="right"
      >
        <v-btn
          raised
          color="primary"
          class="mb-3 ml-3"
          :to="`/basicDocument/${basicDocumentId}/barcodes/add`"
          style="margin-right:10px"
        >
          {{ $t('buttons.newRecord') }}
        </v-btn>
      </v-col>
    </v-row>

    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import { BarcodeYearModel } from "@/models/diploma/barcodeYearModel.js";
import { config }  from '@/common/config';

export default {
  name: 'BarcodeYearList',
  components: {
    GridExporter
  },
    props: {
        basicDocumentId: {
            type: Number,
            default() {
                return null;
            }
        },
    },
   data() {
    return {
        headers: [
            {text: this.$t('diplomas.barcodes.edition'), value: "edition", sortable: true},
            {text: this.$t('diplomas.barcodes.schoolYear'), value: "schoolYear", },
            {text: this.$t('diplomas.barcodes.headerPage'), value: "headerPage", },
            {text: this.$t('diplomas.barcodes.internalPage'), value: "internalPage", },
            { text: '', value: 'actions', sortable: false, align: 'end' }
        ],
        barcodesData:[],
        search:'',
        loading: false,
        diplomaTypeName: ''
    };
  },
   mounted() {
        this.loadData();
    },
     methods: {
          loadData() {
               this.loading = true;

                this.$api.barcodeYear.getBarcodeYears(this.basicDocumentId)
                .then(response => {
                    if (response.data) {
                        this.barcodesData = [];
                        response.data.barcodeYears.forEach(el => {
                            const barcode = new BarcodeYearModel(el, this);
                            this.barcodesData.push(barcode);
                        });

                        this.diplomaTypeName = response.data.diplomaTypeName;
                    }
                })
                .catch(error => {
                    this.$notifier.error('', this.$t('errors.diplomaBarcodesLoad'));
                    console.log(error.response);
                })
                .finally(() => {
                    this.loading = false;
                });
          },
         async deleteBarcode(item){
            if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
                this.loading = true;

                this.$api.barcodeYear.deleteBarcodeYear(item.id).catch(error => {
                    this.$notifier.error('', this.$t('errors.diplomaBarcodesDelete'));
                    console.log(error);
                })
                .finally(() => {
                    this.loadData();
                });
            }
          },
           downloadBarcode(item, page){
             return `${config.apiBaseUrl}api/Barcode/Encode?text=${page == 1? item.headerPage: item.internalPage}&title=${this.diplomaTypeName + '_стр.' + page}`;
          },
          editItem(item){
             this.$router.push(`/basicDocument/${this.basicDocumentId}/barcodes/edit/${item.id}`);
          }
     }
};
</script>
