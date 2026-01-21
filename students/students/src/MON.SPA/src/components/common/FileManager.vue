<template>
  <div>
    <v-simple-table
      dense
    >
      <template v-slot:default>
        <thead>
          <tr>
            <th class="text-left">
              <v-icon
                left
                small
              >
                mdi-file
              </v-icon>
              {{ $t('basicDocumentPart.name') }}
            </th>
            <th
              v-if="!disabled"
              class="text-left"
            >
              Големина
            </th>
            <th class="text-left">
              {{ $t('basicDocumentPart.description') }}
            </th>
            <th />
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(file) in files"
            :key="file.uid || $uuid.v4()"
          >
            <td><span :class="file.deleted ? 'error--text text-decoration-line-through' : ''">{{ file.noteFileName }}</span></td>
            <td
              v-if="!disabled"
            >
              <span v-if="file.sizeInKb">{{ file.sizeInKb.toFixed(2) }} KB</span>
            </td>
            <td><span :class="file.deleted ? 'error--text text-decoration-line-through' : ''">{{ file.description }}</span></td>
            <td class="text-right">
              <span v-if="!file.deleted">
                <v-tooltip
                  bottom
                >
                  <template v-slot:activator="{ on }">
                    <slot>
                      <doc-downloader
                        v-if="file.id"
                        :value="file"
                        x-small
                        show-icon
                        :show-file-name="false"
                        v-on="on"
                      />
                    </slot>
                  </template>
                  <span> {{ $t('buttons.download') }} </span>
                </v-tooltip>
                <button-tip
                  v-if="!disabled"
                  icon
                  icon-name="mdi-delete"
                  icon-color="error"
                  iclass=""
                  tooltip="buttons.delete"
                  bottom
                  small
                  @click="onDelete(file)"
                />
              </span>
            </td>
          </tr>
          <tr
            v-if="!disabled && totalSizeToUploadInKb"
          >
            <td colspan="2">
              <strong>
                Общо: {{ totalSizeToUploadInKb.toFixed(2) }} KB
              </strong>
            </td>
          </tr>
        </tbody>
      </template>
    </v-simple-table>

    <v-card-actions>
      <v-dialog
        v-model="dialog"
        persistent
        max-width="600px"
      >
        <template v-slot:activator="{ on, attrs }">
          <v-row>
            <v-col>
              <v-btn
                v-show="!disabled"
                color="primary"
                outlined
                raised
                small
                v-bind="attrs"
                v-on="on"
              >
                <v-icon
                  left
                  small
                >
                  fas fa-plus
                </v-icon>
                {{ $tc('buttons.addFile', 1) }}
              </v-btn>
            </v-col>

            <v-col align="right">
              <v-btn
                v-show="!disabled"
                color="primary"
                outlined
                raised
                v-bind="attrs"
                small
                @click="scan"
              >
                <v-icon
                  left
                  small
                >
                  mdi-scanner
                </v-icon>
                {{ $tc('buttons.scan') }}
              </v-btn>
            </v-col>
          </v-row>
        </template>
        <v-form
          ref="form"
          @submit.prevent="onFileAddSubmit"
        >
          <v-card class="mx-auto">
            <v-card
              flat
            >
              <v-card-text>
                <v-card>
                  <v-card-title class="bg-secondary text-white p-2" />

                  <v-card-text>
                    <v-row>
                      <v-col
                        cols="12"
                        sm="12"
                        md="10"
                      >
                        <v-textarea
                          v-model="fileToAdd.description"
                          counter
                          outlined
                          prepend-icon="mdi-comment"
                          :label="$t('studentTabs.description')"
                          autocomplete="description"
                        />
                      </v-col>
                    </v-row>
                    <v-row>
                      <v-col
                        cols="12"
                        sm="12"
                        md="10"
                      >
                        <v-file-input
                          v-model="fileToAdd.file"
                          show-size
                          truncate-length="50"
                          :accept="accept"
                          :label="$tc('buttons.addFile', 1)"
                          :clearable="false"
                          :rules="[$validator.required()]"
                          class="required"
                        >
                          <template v-slot:selection="{ text }">
                            <v-chip
                              close
                              @click:close="deleteFile()"
                            >
                              {{ text }}
                            </v-chip>
                          </template>
                        </v-file-input>
                      </v-col>
                    </v-row>
                  </v-card-text>
                </v-card>
              </v-card-text>

              <v-card-actions>
                <v-spacer />
                <v-btn
                  ref="submit"
                  raised
                  color="primary"
                  type="submit"
                >
                  <v-icon left>
                    fas fa-save
                  </v-icon>
                  {{ $t('buttons.ok') }}
                </v-btn>

                <v-btn
                  raised
                  color="error"
                  @click="onFileAddReset"
                >
                  <v-icon left>
                    fas fa-times
                  </v-icon>
                  {{ $t('buttons.cancel') }}
                </v-btn>
              </v-card-actions>
            </v-card>
          </v-card>
        </v-form>
      </v-dialog>
    </v-card-actions>

    <diploma-document-add
      ref="diplomaDocumentAdd"
    />

    <v-overlay :value="scanning">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>

    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import DocDownloader from '@/components/common/DocDownloader.vue';
import DiplomaDocumentAdd from '@/components/tabs/diplomas/DiplomaDocumentAdd.vue';
import { DocumentModel } from '@/models/documentModel.js';

export default {
  name: 'FileManager',
  components: { DocDownloader, DiplomaDocumentAdd },
  props: {
    value: {
      type: Array,
      default() {
        return [];
      },
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    accept: {
      type: String,
      default() {
        return '*';
      }
    }
  },
  data() {
    return {
      files: this.value,
      scanning: false,
      dialog: false,
      fileToAdd: {
        description: null,
        file: null
      }
    };
  },
  computed: {
    totalSizeToUploadInKb() {
      if (!Array.isArray(this.files) || this.files.length === 0) {
        return 0;
      }

      let sum = 0.0;

      this.files.filter(x => x.sizeInKb).forEach(el => {
        sum += el.sizeInKb;
      });

      return sum;
      // return this.files.reduce((acc, el) => {
      //   return acc + el.sizeInKb || 0.00;
      // }, 0.00);
    }
  },
  watch: {
    value() {
      this.files = this.value;
    }
  },
  methods: {
    async onDelete(file) {
      if(!file) return;

      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))) {
        // this.files[index].deleted = true;

        if(file.id) {
          file.deleted = true;
        } else {
          const fileIndex = this.files.indexOf(file);
          this.files.splice(fileIndex, 1);
        }
      }
    },
    async onFileAddSubmit(){
      let hasErrors = this.$validator.validate(this);
      if(hasErrors) {
        this.$notifier.error('', this.$t('validation.hasErrors'));
        return;
        //Todo: notification
      }


      //DocumentModel
      const fileModel = await this.$helper.getFileContent(this.fileToAdd.file);
      if(!fileModel) return;
      //Todo: notification
      fileModel.description = this.fileToAdd.description;
      fileModel.uid = this.$uuid.v4();

      this.files.push(fileModel);
      this.onFileAddReset();
    },
    onFileAddReset() {
      this.fileToAdd.description = null;
      this.fileToAdd.file = null;
      this.dialog = false;
    },
    deleteFile(){
      this.fileToAdd.description = null;
      this.fileToAdd.file = null;
    },
    async scan(){
       this.scanning = true;

       await this.$api.scanner.scan('JPEG').then((response) => {

        const scannedFile = new DocumentModel({
                                noteFileName: `scan${this.files.filter(item => item.noteFileName.startsWith("scan") === true).length + 1}.jpg`,
                                noteFileType: 'image/jpeg',
                            });

        scannedFile.noteContents = this.$helper.base64ToByteArray(response.data.contents);
        scannedFile.uid = this.$uuid.v4();
        scannedFile.description = 'scan';

        this.files.push(scannedFile);
      }).catch((error) => {
            var ignoreError = false;

            if(error.message !== undefined && error.message.startsWith("Network error")){
                this.$notifier.error('', this.$t('errors.scan.localServerError'));
            }
            else if(error.response !== undefined && error.response.status === 400){
                if( error.response.data.includes("Грешка при сканиране 0x80210015")){
                  this.$notifier.error('', this.$t('errors.scan.noScannersFound'));
                }
                else if(error.response.data.includes("You must select a device for scanning.")){
                    ignoreError = true;
                }
            }
            else{
                this.$notifier.error('', error);
            }

           if(!ignoreError){
              console.log(error);
           }
        }).finally(() => {
           this.scanning = false;
        });
    }
  }
};
</script>
