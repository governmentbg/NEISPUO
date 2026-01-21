<template>
  <div>
    <v-card>
      <v-card-text>
        <v-alert
          border="left"
          colored-border
          :color="attachedImagesInfo.color"
          elevation="2"
        >
          {{ attachedImagesInfo.message }}
        </v-alert>

        <CoolLightBox
          :items="images"
          :index="index"
          @close="index = null"
        />

        <draggable
          :list="items"
          class="row"
          ghost-class="ghost"
          @start="onStartDrag"
          @end="onEndDrag"
        >
          <v-col
            v-for="(image, imageIndex) in items"
            :key="imageIndex"
            class="d-flex child-flex"
            cols="4"
          >
            <v-card>
              <v-toolbar dense>
                <v-btn icon>
                  <v-icon
                    color="primary"
                    @click="onViewDocument(imageIndex)"
                  >
                    fa-eye
                  </v-icon>
                </v-btn>
                <v-spacer />

                <v-btn
                  v-if="hasManagePermission && diplomaCanBeEdit"
                  icon
                >
                  <v-icon
                    color="error"
                    @click="onRemoveDocument(image.id)"
                  >
                    fa-trash
                  </v-icon>
                </v-btn>
              </v-toolbar>
              <v-card-text class="px-0">
                <v-img
                  :src="getBlobDownloadUri(image)"
                  :lazy-src="getBlobDownloadUri(image)"
                  class="grey lighten-2"
                  @click="index = imageIndex"
                >
                  <template #placeholder>
                    <v-row
                      class="fill-height ma-0"
                      align="center"
                      justify="center"
                    >
                      <v-progress-circular
                        indeterminate
                        color="grey lighten-5"
                      />
                    </v-row>
                  </template>
                </v-img>
              </v-card-text>
            </v-card>
          </v-col>
        </draggable>
      </v-card-text>
      <v-card-actions>
        <v-btn
          v-if="diplomaCanBeEdit && hasManagePermission"
          raised
          color="primary"
          @click="addNewDocument"
        >
          <v-icon left>
            mdi-file-image
          </v-icon>
          {{ $t('buttons.add') }}
        </v-btn>
      </v-card-actions>
    </v-card>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <DiplomaDocumentAdd
      ref="diplomaDocumentAdd"
      :diploma-id="diplomaId"
      @AddDocumentCanceled="onCancelAddDocument"
      @AddDocument="onAddDocument"
    />
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { NotificationSeverity, Permissions } from '@/enums/enums';
import CoolLightBox from 'vue-cool-lightbox';
import 'vue-cool-lightbox/dist/vue-cool-lightbox.min.css';
import Draggable from 'vuedraggable';
import DiplomaDocumentAdd from "./DiplomaDocumentAdd.vue";
import { config } from "@/common/config";
import { mapGetters } from "vuex";

export default {
  components: {
      DiplomaDocumentAdd, CoolLightBox, Draggable
  },
  props: {
    diplomaId: {
      type: Number,
      required: true
    },
    isValidationDocument: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      saving: false,
      APIBaseUrl: config.apiBaseUrl,
      index: null,
      items: [],
      images: [],
      attachedImagesCountMin: null,
      attachedImagesCountMax: null,
      dragging: false,
      diplomaCanBeEdit: false
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage);
    },
    attachedImagesInfo(){
      if(this.attachedImagesCountMin !== null && this.items.length < this.attachedImagesCountMin) {
        return {color: 'error', message:`${this.$t("diplomas.attachedImagesCountMinMsg")} ${this.attachedImagesCountMin}`};
      }
      if(this.attachedImagesCountMax !== null && this.items.length > this.attachedImagesCountMax) {
        return {color: 'error', message:`${this.$t("diplomas.attachedImagesCountMaxMsg")} ${this.attachedImagesCountMax}`};
      }

      return {color:'info', message: this.$t("diplomas.attachedImagesCountOkMsg")};
    }
  },
  mounted() {
    this.load();
  },
  methods: {
    addNewDocument(){
      this.$refs.diplomaDocumentAdd.dialog = true;
    },
    onCancelAddDocument(){
      this.$refs.diplomaDocumentAdd.dialog = false;
    },
    getDiplomaBasicDetails() {
      this.$api.diploma.getBasicDetails(this.diplomaId)
        .then((response) => {
          if(response.data) {
            this.attachedImagesCountMin = response.data.attachedImagesCountMin;
            this.attachedImagesCountMax = response.data.attachedImagesCountMax;
            this.diplomaCanBeEdit = response.data.canBeEdit;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('diplomas.getBasicDocumentAttachedImagesCountErrorMsg'));
          console.log(error);
        });
    },
    onAddDocument(document){
      this.saving = true;
      this.$refs.diplomaDocumentAdd.saveInProgress(true);
      if (document.scannedFile){
        this.$api.diploma
            .uploadDiplomaImage(document.scannedFile, this.diplomaId, document.description)
            .then(() => {
                this.load();
            })
            .catch((error) => {
                this.$notifier.error("",  this.$t(error.response.data.message));
            }).finally(() => {
                this.$refs.diplomaDocumentAdd.dialog = false;
                this.$refs.diplomaDocumentAdd.fileToAdd = null;
                this.$refs.diplomaDocumentAdd.description = null;
                this.saving = false;
                this.$refs.diplomaDocumentAdd.saveInProgress(false);
            });
      }
      else{
        let formData = new FormData();
        formData.append("diplomaId", this.diplomaId);
        formData.append("description", document.description);
        formData.append("file", document.fileToAdd);

        this.$api.diploma
            .uploadDiplomaDocument(formData)
            .then(() => {
                this.load();
            })
            .catch((error) => {
                this.$notifier.error("",  this.$t(error.response.data.message));
            }).finally(() => {
                this.$refs.diplomaDocumentAdd.dialog = false;
                this.$refs.diplomaDocumentAdd.fileToAdd = null;
                this.$refs.diplomaDocumentAdd.description = null;
                this.saving = false;
                this.$refs.diplomaDocumentAdd.saveInProgress(false);
            });
      }
    },
    async onViewDocument(imageIndex){
        this.index = imageIndex;
    },
    async onRemoveDocument(id){
      if(await this.$refs.confirm.open('Изтриване', this.$t('common.confirm'))) {
        await this.$api.diploma.removeDiplomaDocument(id);
        await this.load();
        this.$notifier.success('', this.$t('common.saveSuccess'));
      }
    },
    async load(){
        this.items = (await this.$api.diploma.getDiplomaDocuments(this.diplomaId)).data;
        this.images = this.items.map(image => this.getBlobDownloadUri(image));
        this.getDiplomaBasicDetails();
    },
    getBlobDownloadUri(item) {
      return item
        ? `${item.blobServiceUrl}/${item.blobId}?t=${item.unixTimeSeconds}&h=${item.hmac}`
        : '';
    },
    onStartDrag(){
      this.dragging = true;
    },
    async onEndDrag(){
      this.dragging = false;
      let newDocumentOrder = this.items.map((item,index) =>  ({position: index, id: item.id}));
      await this.$api.diploma.reorderDiplomaDocuments(this.diplomaId, newDocumentOrder);
      this.$notifier.toast('',this.$t('common.saveSuccess'), NotificationSeverity.Success);
    }
  }
};
</script>

<style lang="scss" scoped>
  .v-progress-linear {
    position: absolute;
    top: 0;
    right: 0;
    left: 0;
  }
</style>
