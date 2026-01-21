<template>
  <div>
    <v-form
      ref="form"
      @submit.prevent="onSubmit"
    >
      <v-card>
        <v-card-title>
          <v-icon left>
            {{ nomenclature.icon }}
          </v-icon> {{ $t('nomenclature.title', { title: nomenclature.title }) }}
        </v-card-title>
        <v-card-text>
          <v-row>
            <v-col
              cols="12"
              sm="12"
            >
              <v-btn
                raised
                color="primary"
                class="mt-3"
                @click="onAddNomenclatureClick"
              >
                <v-icon left>
                  fas fa-plus-circle
                </v-icon>
                {{ $t('buttons.newRecord') }}
              </v-btn>

              <v-progress-linear
                v-if="sending"
                indeterminate
              />

              <v-data-table
                ref="nomenclaturesTable"
                :items="nomenclatureData"
                :headers="headers"
                :loading="loading"
                class="elevation-1"
                disable-pagination
                hide-default-footer
              >
                <template v-slot:body="{ items }">
                  <tbody>
                    <tr
                      v-for="item in items"
                      :key="item.value"
                    >
                      <td>
                        {{ item.value }}
                      </td>
                      <td>
                        {{ item.name }}
                      </td>

                      <td>
                        {{ item.text }}
                      </td>

                      <td>
                        <v-checkbox
                          v-model="item.isValid"
                          :value="item.isValid"
                          :disabled="true"
                          style="margin:0px;padding:0px"
                          hide-details
                        />
                      </td>

                      <td>
                        {{ item.validFrom }}
                      </td>
                      <td>
                        {{ item.validTo }}
                      </td>
                      <td>
                        <v-icon
                          small
                          :disabled="sending"
                          class="mr-2"
                          @click="editItem(item)"
                        >
                          mdi-pencil
                        </v-icon>
                  
                        <v-icon
                          small
                          @click="deleteItem(item)"
                        >
                          mdi-delete
                        </v-icon>
                      </td>
                    </tr>
                  </tbody>
                </template>

                <template v-slot:top>
                  <GridExporter 
                    v-if="!loading"
                    :items="nomenclatureData"
                    :file-extensions="['xlsx', 'csv', 'txt']"
                    file-name="Nomenclature"
                    :headers="headers"
                  />
                  <v-spacer />
                </template>
              </v-data-table>
            </v-col>
          </v-row>
          <confirm-dlg ref="confirm" />
        </v-card-text>
      </v-card>
    </v-form>

    <v-dialog
      v-model="showNomenclatureDialog"
      persistent
      max-width="555"
      :retain-focus="false"
    >
      <v-form 
        @submit.stop.prevent="validate"
      >
        <v-card>
          <v-card-title>
            Нов номенклатурен запис.
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col
                cols="12"
                sm="12"
              >
                <v-text-field
                  v-model="currentNomenclatureRecord.name"    
                  label="Наименование"
                  :disabled="sending"
                  :error-messages="getRequiredFieldValidationMessage('name')"
                  class="required"
                  @input="$v.currentNomenclatureRecord.name.$touch()"
                  @blur="$v.currentNomenclatureRecord.name.$touch()"
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col
                cols="12"
                sm="12"
              >
                <v-text-field
                  v-model="currentNomenclatureRecord.text"    
                  label="Описание"
                  :disabled="sending"
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col
                cols="12"
                sm="12"
                class="d-flex justify-center"
              >
                <v-checkbox
                  v-model="currentNomenclatureRecord.isValid"
                  label="Валиден"
                  style="margin:0px;padding:0px"
                  hide-details
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col
                cols="12"
                sm="12"
              >
                <date-picker
                  id="validFrom"
                  ref="validFrom"
                  v-model="currentNomenclatureRecord.validFrom"
                  :show-buttons="false"
                  :scrollable="false"
                  :disabled="sending"
                  :no-title="true"
                  :show-debug-data="false"
                  :label="$t('common.validFrom')"
                />
              </v-col>
            </v-row>
            <v-row>
              <v-col
                cols="12"
                sm="12"
              >
                <date-picker
                  id="validTo"
                  ref="validTo"
                  v-model="currentNomenclatureRecord.validTo"
                  :show-buttons="false"
                  :scrollable="false"
                  :disabled="sending"
                  :no-title="true"
                  :show-debug-data="false"
                  :label="$t('common.validTo')"
                />
              </v-col>
            </v-row>
          </v-card-text>
          <v-card-actions
            class="justify-center"
          >
            <v-btn
              ref="submit"
              raised
              color="primary"
              type="submit"
              :disabled="sending"
            >
              <v-icon left>
                fas fa-save
              </v-icon>          
              {{ $t('buttons.saveChanges') }}
            </v-btn>

            <v-btn
              raised
              color="error"
              :disabled="sending"
              @click="onCancel"
            >
              <v-icon left>
                fas fa-times
              </v-icon>          
              Откажи
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-form>
    </v-dialog>
  </div>
</template>

<script>

import { validationMixin } from 'vuelidate';
import { required } from 'vuelidate/lib/validators';
import Helper from '@/components/helper.js';
import { NomenclatureModel } from '@/models/nomenclatureModel.js';
import Constants from '@/common/constants.js';
import Nomenclatures from '@/common/nomenclatures.js';
import GridExporter from "@/components/wrappers/gridExporter";

export default {
  name: 'TablesEdit',
  components: {
    GridExporter
  },
  mixins: [validationMixin],
  props: {
  },
  data() {
      return {
        nomenclature: null,
        helper:Helper,
        currentNomenclatureRecord: new NomenclatureModel(),
        nomenclatureData: [],
        selectedTableName: '',
        editedIndex: -1,
        dialogDeleteNomenclatureRecord: false,
        showNomenclatureDialog: false,
        sending: false,
        loading: true,
        headers:[
        {
            text: 'Сист. код',
            value: 'value',
            sortable: true,
            filterable: false,
        },
        {
            text: 'Наименование',
            value: 'name',
            sortable: false,
            filterable: false,
        },
        {
            text: 'Описание',
            value: 'text',
            sortable: false,
            filterable: false,
        },
        {
            text: 'Валиден',
            value: 'isValid',
            sortable: false,
            filterable: false,
        },
        {
            text: this.$t('common.validFrom'),
            value: 'validFrom',
            sortable: false,
            filterable: false,
        },
        {
            text: this.$t('common.validTo'),
            value: 'validTo',
            sortable: false,
            filterable: false,
        },
        { 
            text: 'Действия', 
            value: 'actions', 
            sortable: false,
            align: 'end'
        },
      ],
    };
  },
  validations: {
    currentNomenclatureRecord:{
      name: {
        required
      }
    }
  },
  watch: { 
    '$route.params.name': {
      handler: function(nameParam) {
        this.nomenclature = Nomenclatures.findName(nameParam);
        this.selectedTableName = nameParam;
        this.fillNomenclatureData(this.selectedTableName);
      },
      deep: true,
      immediate: true
    }
  },
  methods: {
    fillNomenclatureData(tableName) {
      this.loading = true;
      this.$api.lookups.getNomenclatureData(tableName)
        .then((response) => {
          response.data.forEach(element => {
            if(element.validFrom){
              element.validFrom = this.$moment(element.validFrom).format(Constants.DATEPICKER_FORMAT);
            }
            if(element.validTo){
              element.validTo = this.$moment(element.validTo).format(Constants.DATEPICKER_FORMAT);
            }
        });
          this.nomenclatureData = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.nomenclatureLoad'));
          console.log(error);
        })
        .then(() => {
          this.loading = false;
        });
    },
    getRequiredFieldValidationMessage(fieldName) {
      const field = this.$v.currentNomenclatureRecord[fieldName];

      return this.helper.getRequiredValidationMessage(field, fieldName);
    },
    async deleteItem(item) {
      this.editedIndex = this.nomenclatureData.indexOf(item);
      
       if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))){
                this.$api.lookups.deleteNomenclature(this.selectedTableName, this.nomenclatureData[this.editedIndex].value)
                    .then((response) => {
                    console.log(response);
                        this.$notifier.success('', this.$t('common.deleteSuccess'));
                    })
                    .catch((error) => {
                    this.$notifier.error('', this.$t('errors.nomenclatureDelete'));
                        console.log(error);
                    });
            this.editedIndex = this.nomenclatureData.splice(this.editedIndex, 1);
        }
    },
    onAddNomenclatureClick() {
      this.currentNomenclatureRecord = new NomenclatureModel();
      this.showNomenclatureDialog = true;
      this.editedIndex = -1;
    },
    onCancel() {
      this.showNomenclatureDialog = false;
    },
    onReset(){
      this.$notifier.success('', this.$t('common.loadSuccess'));
      this.fillNomenclatureData(this.selectedTableName);
    },
    validate(){
      this.$v.$touch();

      if (!this.$v.$invalid) {
        if(this.editedIndex === -1){
          this.addNomenclature();
        }
        else{
          this.updateNomenclature();
        }
        this.$v.$reset();
        this.showNomenclatureDialog = false;
      }
    },
    addNomenclature(){
      this.nomenclatureData.push(this.currentNomenclatureRecord);

      this.$api.lookups.addNomenclature(this.selectedTableName, this.getPayload())
        .then((response) => {
          console.log(response);
          this.fillNomenclatureData(this.selectedTableName);
          this.$notifier.success('', this.$t('common.saveSuccess'));
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.nomenclatureSave'));
          console.log(error);
        }).finally(() => {
            this.sending = false;
        });
    },
    getPayload(){
      let validFrom = null;
      if(this.currentNomenclatureRecord.validFrom){
        validFrom = !this.$refs.validFrom.isValidIsoFormattedDate() ? this.$refs.validFrom.isoDate : this.currentNomenclatureRecord.validFrom;
      }

      let validTo = null;
      if(this.currentNomenclatureRecord.validTo){
        validTo = !this.$refs.validTo.isValidIsoFormattedDate() ? this.$refs.validTo.isoDate : this.currentNomenclatureRecord.validTo;
      }
      
      const nomenclature = {
        value: this.currentNomenclatureRecord.value,
        name: this.currentNomenclatureRecord.name,
        text: this.currentNomenclatureRecord.text,
        isValid: this.currentNomenclatureRecord.isValid,
        validFrom: validFrom,
        validTo: validTo
      };

      return nomenclature;
    },
    updateNomenclature(){
      this.nomenclatureData[this.editedIndex] = this.currentNomenclatureRecord;
        this.$api.lookups.updateNomenclature(this.selectedTableName, this.getPayload())
          .then((response) => {
            console.log(response);
            this.fillNomenclatureData(this.selectedTableName);
            this.$notifier.success('', this.$t('common.saveSuccess'));
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('errors.nomenclatureSave'));
            console.log(error);
          }).finally(() => {
              this.sending = false;
          });

    },
    editItem(item){
      this.editedIndex = this.nomenclatureData.indexOf(item);
      this.currentNomenclatureRecord = JSON.parse(JSON.stringify(item));
      this.showNomenclatureDialog = true;
    }
  }
};
</script>