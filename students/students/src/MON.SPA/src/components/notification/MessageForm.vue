<!-- Форма за създаване на ново съобщение -->
<template>
  <v-form :disabled="disabled">
    <v-row>
      <v-dialog
        v-model="dialog"
        persistent
        max-width="1300"
      >
        <v-spacer />
        <button-tip
          small
          icon-name="fas fa-plus"
          color="primary"
          iclass="mx-2"
          bclass="mr-1"
          bottom
          fab
          @click="dialog = true"
        />
        <v-card>
          <v-toolbar
            color="primary"
            dark
          >
            <v-toolbar-title>Ново съобщение</v-toolbar-title>
            <v-spacer />
            <v-icon @click="dialog = false">
              mdi-close
            </v-icon>
          </v-toolbar>
          <v-card-text>
            <v-form>
              <v-text-field
                label="Получатели"
                required
              />

              <v-text-field
                v-model="subject"
                label="Тема"
                required
              />

              <v-file-input
                v-model="file"
                label="Файл"
                show-size
                truncate-length="50"
              />
            </v-form>
            <v-textarea
              rows="20"
              solo
            />
          </v-card-text>
          <v-card-actions>
            <v-spacer />

            <v-btn
              color="primary"
              dark
            >
              Изпращане
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-row>
  </v-form>
</template>

<script>
export default {
  name: "MessageForm",
  props: {
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      subject: "",
      file: null,
      dialog: false,
    };
  },
};
</script>