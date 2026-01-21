<template>
  <v-card>
    <v-card-title>
      {{ this.$t('printTemplate.title') }}
    </v-card-title>

    <v-card-text>
      <v-data-table
        ref="printTemplatesListTable"
        :items="printTemplatesData"
        :items-pers-page="10"
        :headers="headers"
        :loading="loading"
        :search="search"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar flat>
            <v-toolbar-title>
              <GridExporter
                :items="printTemplatesData"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="$t('printTemplate.title')"
                :headers="headers"
              />
            </v-toolbar-title>
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
                icon-color="primary"
                icon-name="mdi-pencil"
                iclass=""
                tooltip="buttons.edit"
                bottom
                small
                @click="editItem(item)"
              />
              <button-tip
                :disabled="!item.hasContents"
                icon
                icon-name="mdi-pencil-ruler"
                icon-color="primary"
                iclass=""
                tooltip="buttons.design"
                bottom
                small
                @click="designItem(item)"
              />
              <button-tip
                icon
                icon-name="mdi-delete"
                icon-color="error"
                iclass=""
                tooltip="buttons.delete"
                bottom
                small
                @click="deletePrintTemplate(item)"
              />
            </template>
          </button-group>
        </template>
        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              small
              color="primary"
              to="/printTemplate/add"
            >
              {{ $t('buttons.newRecord') }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click="load"
            >
              {{ $t('buttons.reload') }}
            </v-btn>
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>

import { PrintTemplateModel } from "@/models/printTemplateModel.js";
import GridExporter from "@/components/wrappers/gridExporter";
import { mapGetters} from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
     name: 'PrintTemplatesList',
     components: {
        GridExporter
      },
      data() {
        return {
             loading: false,
               headers: [
                    {
                        text: this.$t('printTemplate.name'),
                        value: 'name'
                    },
                    {
                        text: this.$t('printTemplate.description'),
                        value: 'description'
                    },
                    {
                        text: this.$t('printTemplate.basicDocument'),
                        value: 'basicDocumentName'
                    },
                    {
                        text: this.$t('printTemplate.printFormEdition'),
                        value: 'printFormEdition'
                    },
                    { text: '', value: 'actions', sortable: false, align: 'end' }
               ],
               printTemplatesData:[],
               search:''
        };
    },
    computed: {
      ...mapGetters(['userInstitutionId', 'userRegionId', 'hasPermission']),
    },
    mounted() {
      if(!this.hasPermission(Permissions.PermissionNameForPrintTemplatesShow)) {
        return this.$router.push('/errors/AccessDenied');
      }

      this.load();
    },
     methods: {
          load() {
               this.loading = true;

                this.$api.printTemplate.list()
                .then(response => {
                    if (response.data) {
                        this.printTemplatesData = [];
                        response.data.forEach(el => {
                            const printTemplate = new PrintTemplateModel(el, this);
                            this.printTemplatesData.push(printTemplate);
                        });
                    }
                })
                .catch(error => {
                    this.$notifier.error('', this.$t('errors.printTemplatesLoad'));
                    console.log(error);
                })
                .finally(() => {
                    this.loading = false;
                });
          },
         async deletePrintTemplate(item){
              if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))) {
                    this.loading = true;

                    this.$api.printTemplate.deletePrintTemplate(item.id).catch(error => {
                        this.$notifier.error('', this.$t('errors.printTemplateDelete'));
                        console.log(error);
                    })
                    .finally(() => {
                        this.load();
                    });
              }
          },
          editItem(item){
            this.$router.push(`/printTemplate/${item.id}/edit`);
          },
          designItem(item){
            this.$router.push(`/printTemplate/${item.id}/reportDesigner`);
          }
     }
};
</script>
