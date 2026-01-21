<template>
  <div>
    <v-form ref="searchForm" v-model="valid" @submit.prevent="submit">
      <v-card>
        <v-toolbar flat outlined dense color="deep-purple lighten-3">
          <v-card-title>{{ this.$t("search.title") }}</v-card-title>
        </v-toolbar>
        <v-card-subtitle class="text-center">
          {{ this.$t("search.description") }}
        </v-card-subtitle>
        <v-card-text>
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="model.idNumber"
                :label="$t('search.idNumberLabel')"
                :rules="[rules.required]"
              />
            </v-col>

            <v-col cols="12" class="pt-0 pb-0">
              <v-radio-group v-model="model.idType" row class="mt-0">
                <v-radio :label="$t('search.egn')" :value="1" />
                <v-radio :label="$t('search.lin')" :value="2" />
              </v-radio-group>
            </v-col>

            <v-col cols="12">
              <v-text-field
                v-model="model.docNumber"
                :label="$t('search.docNumberLabel')"
                :rules="[rules.required]"
              />
              <p class="text-center">
                {{ $t("search.docNumberHint") }}
              </p>
            </v-col>
          </v-row>
        </v-card-text>

        <v-card-actions class="justify-center">
          <v-btn type="submit" :disabled="!valid" color="primary">
            <v-icon left> fa-search </v-icon>{{ $t("buttons.search") }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
    <v-overlay :value="searching">
      <v-progress-circular indeterminate size="64" />
    </v-overlay>
  </div>
</template>

<script>
import { SearchModel } from "@/models/searchModel";
import { NotificationSeverity } from "@/enums/enums";

export default {
  name: "DiplomaSearch",
  data: (vm) => ({
    model: new SearchModel(),
    valid: false,
    rules: {
      required: (value) => !!value || vm.$t("validation.requiredField"),
    },
    error: "",
    timeout: 2000,
    searching: false,
  }),
  methods: {
    async submit() {
      this.$refs.searchForm.validate();
      if (!this.valid) {
        this.$notifier.error("", this.$t("validation.hasErrors"));
        return;
      }

      const reCaptchaToken = await this.recaptcha();
      if (!reCaptchaToken) {
        this.$notifier.error("", this.$t("search.invalidRecaptchaToken"));
        return;
      }
      this.model.reCaptchaToken = reCaptchaToken;

      this.searching = true;

      this.$http
        .post("/api/diploma/search", this.model)
        .then((result) => {
          if (result.data && result.data.length > 0) {
            this.$router.push({
              name: "Details",
              params: {
                diplomas: result.data,
              },
            });
          } else {
            this.$notifier.snackbar(
              "",
              this.$t("search.noResultsFound"),
              NotificationSeverity.info
            );
          }
        })
        .catch((err) => {
          this.$notifier.error("", err.message);
        })
        .then(() => {
          this.searching = false;
        });
    },
    async recaptcha() {
      // (optional) Wait until recaptcha has been loaded.
      await this.$recaptchaLoaded();

      // Execute reCAPTCHA with action "diplomaSearch".
      const token = await this.$recaptcha("diplomaSearch");
      return token;
    },
  },
};
</script>
