<template>
  <v-dialog
    v-model="dialog"
    persistent
    max-width="900px"
  >
    <v-form
      ref="form"
      @submit.prevent="validate"
    >
      <v-card class="mx-auto">
        <v-card flat>
          <v-card-text>
            <v-card>
              <v-card-title class="bg-secondary text-white p-2">
                {{ $tc('common.addingOf') + ' ' + $tc('documents.title', 1).toLowerCase() }}
              </v-card-title>

              <v-card-text>
                <v-row>
                  <v-col
                    cols="12"
                    sm="12"
                    md="12"
                  >
                    <v-textarea
                      v-model="description"
                      counter
                      outlined
                      prepend-icon="mdi-comment"
                      :label="$t('studentTabs.description')"
                      :value="description"
                      autocomplete="description"
                    />
                  </v-col>
                </v-row>
                <v-row v-if="scannedFile">
                  <v-col cols="10">
                    <v-slider
                      v-model="width"
                      append-icon="mdi-magnify-plus-outline"
                      prepend-icon="mdi-magnify-minus-outline"
                      class="align-self-stretch"
                      min="200"
                      max="1000"
                      step="10"
                      @click:append="zoomIn"
                      @click:prepend="zoomOut"
                    />
                  </v-col>
                  <v-col cols="2">
                    <button-group>
                      <button-tip
                        icon
                        icon-color="primary"
                        icon-name="fas fa-redo"
                        tooltip="buttons.rotateClockWise"
                        bottom
                        small
                        @click="rotate(true)"
                      />
                      <button-tip
                        icon
                        icon-color="primary"
                        icon-name="fas fa-undo"
                        tooltip="buttons.rotateAntiClockWise"
                        bottom
                        small
                        @click="rotate(false)"
                      />
                    </button-group>
                  </v-col>
                </v-row>
                <v-row
                  v-if="scannedFile"
                  align="center"
                  justify="center"
                >
                  <v-col>
                    <v-card
                      class="overflow-auto"
                      style="height:300px"
                    >
                      <v-img
                        :aspect-ratio="aspectRatio"
                        :src="scannedImage"
                        :width="width"
                      />
                    </v-card>
                  </v-col>
                </v-row>
                <v-row
                  align="center"
                  justify="center"
                >
                  <v-col
                    cols="12"
                    sm="6"
                    md="10"
                  >
                    <v-file-input
                      v-model="fileToAdd"
                      show-size
                      truncate-length="50"
                      accept="image/*"
                      :label="$tc('buttons.addFile', 1)"
                      :disabled="sending"
                      :clearable="false"
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
                  <v-col
                    cols="12"
                    sm="6"
                    md="2"
                  >
                    <v-btn
                      v-show="!disabled"
                      color="primary"
                      outlined
                      raised
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
              @click="onReset"
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
    <confirm-dlg ref="confirm" />
    <v-divider />
  </v-dialog>
</template>

<script>

import helper from '@/components/helper.js';
import { validationMixin } from 'vuelidate';
import { DocumentModel } from '@/models/documentModel.js';
import ButtonGroup from '../../wrappers/ButtonGroup.vue';

export default {
  name: "DiplomaDocumentAdd",
  components: { ButtonGroup },
  mixins: [validationMixin],
  props: {
    diplomaId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      description: '',
      fileToAdd: null,
      scannedFile: null,
      sending: false,
      helper: helper,
      dialog: false,
      scanning: false,
      width: 200,
      aspectRatio: 1,
      disabled: false,
      scannedImage: null
    };
  },
  methods: {
    deleteFile() {
      this.fileToAdd = null;
    },
    saveInProgress(value) {
      this.$emit('saveInProgress', value);
      this.sending = value;
    },
    async validate() {

      const payload = {
        fileToAdd: this.fileToAdd,
        scannedFile: this.scannedFile,
        description: this.description
      };

      this.$emit("AddDocument", payload);
    },
    onReset() {
      this.fileToAdd = null;
      this.scannedFile = null;
      this.scannedImage = null;
      this.$emit('AddDocumentCanceled');
    },
    rotate(isClockwise) {
      // create an off-screen canvas
      var offScreenCanvas = document.createElement('canvas');
      var offScreenCanvasCtx = offScreenCanvas.getContext('2d');

      // cteate Image
      var img = new Image();
      img.src = 'data:image/jpeg;base64,' + this.scannedFile.base64Contents;

      // set its dimension to rotated size
      offScreenCanvas.height = img.width;
      offScreenCanvas.width = img.height;

      // rotate and draw source image into the off-screen canvas:
      this.aspectRatio = img.height / img.width;
      if (isClockwise) {
        offScreenCanvasCtx.rotate(90 * Math.PI / 180);
        offScreenCanvasCtx.translate(0, -offScreenCanvas.width);
      } else {
        offScreenCanvasCtx.rotate(-90 * Math.PI / 180);
        offScreenCanvasCtx.translate(-offScreenCanvas.height, 0);
      }
      offScreenCanvasCtx.drawImage(img, 0, 0);

      // encode image to data-uri with base64
      this.scannedImage = offScreenCanvas.toDataURL("image/jpeg", 100);
      this.scannedFile.base64Contents = this.scannedImage.replace('data:image/jpeg;base64,', '');
    },
    zoomOut() {
      this.width = (this.width - 20);
    },
    zoomIn() {
      this.width = (this.width + 20);
    },
    async scan() {
      this.scanning = true;

      await this.$api.scanner.scan('JPEG').then((response) => {

        this.scannedFile = new DocumentModel({
          noteFileName: `scan.jpg`,
          noteFileType: 'image/jpeg',
        });
        this.scannedFile.base64Contents = response.contents[0];
        this.scannedFile.uid = this.$uuid.v4();
        this.scannedFile.description = 'scan';
        this.scannedImage = 'data:image/jpeg;base64,' + this.scannedFile.base64Contents;

        var img = new Image();
        img.src = this.scannedImage;
        this.aspectRatio = img.height / img.width;
      }).catch((error) => {
        var ignoreError = false;

        if (error.message !== undefined && error.message.startsWith("Network error")) {
          this.$notifier.error('', this.$t('errors.scan.localServerError'));
        }
        else if (error.response !== undefined && error.response.status === 400) {
          if (error.response.data.includes("Грешка при сканиране 0x80210015")) {
            this.$notifier.error('', this.$t('errors.scan.noScannersFound'));
          }
          else if (error.response.data.includes("You must select a device for scanning.")) {
            ignoreError = true;
          }
        }
        else {
          this.$notifier.error('', error);
        }

        if (!ignoreError) {
          console.log(error);
        }
      }).finally(() => {
        this.scanning = false;
      });
    }

  }
};
</script>
