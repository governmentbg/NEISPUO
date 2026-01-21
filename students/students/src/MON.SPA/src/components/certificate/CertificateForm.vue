<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row>
        <v-col
          v-if="editMode"
          cols="4"
          sm="4"
          md="4"
        >
          <v-text-field
            v-model="model.name"
            :label="$t('certificate.name')"
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="4"
          sm="4"
          md="4"
        >
          <v-select
            ref="certificateType"
            v-model="model.certificateType"
            :items="[{name:'Root', text:'Root', value: 1},{name:'Intermediate', text:'Intermediate', value: 2}]"
            :label="$t('certificate.type')"  
            clearable
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="4"
          sm="4"
          md="4"
        >
          <v-checkbox
            v-model="model.isValid"
            color="primary"
            :label="$t('certificate.isValid')"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col
          cols="12"
          sm="12"
          md="10"
        >
          <v-textarea
            v-model="model.description"
            counter
            outlined
            prepend-icon="mdi-comment"
            :label="$t('certificate.description')"
            :value="model.description"
            autocomplete="description"
          />
        </v-col>
      </v-row>

      <v-row v-if="!editMode">
        <v-col
          cols="12"
          sm="12"
          md="10"
        >
          <v-file-input
            v-model="model.fileToAdd.file"
            show-size
            truncate-length="50"
            accept=".cer"
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
    </v-form>
  </div>
</template>

<script>

export default {
  name: "CertificateForm",
  components: {
  
  },
  props: {
    model: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
    editMode: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      
    };
  },
  methods: {
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
    deleteFile(){
      this.model.fileToAdd.file = null;
    }
  },
};
</script>
