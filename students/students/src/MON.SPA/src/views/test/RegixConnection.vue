<template>
  <div>
    <v-card>
      <v-card-title>Връзка с Regix</v-card-title>
      <v-card-text>
        <v-form :ref="'form' + _uid">
          <v-row>
            <v-col>
              <c-text-field
                v-model="identifier"
                label="Идентификатор"
                dense
                outlined
                persistent-placeholder
                class="required"
                :rules="[$validator.required()]"
              />
            </v-col>
          </v-row>
        </v-form>
        <v-row v-show="result">
          <vue-json-pretty
            :data="result"
            show-length
            show-line
            show-icon
          />
        </v-row>
      </v-card-text>
      <v-card-actions>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getValidPerson"
        >
          Валидност на физическо лице
        </v-btn>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getPersonSearch"
        >
          Детайли на физическо лице
        </v-btn>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getEmploymentContracts"
        >
          Списък на трудови договори в НАП
        </v-btn>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getRelations"
        >
          Свързани лица
        </v-btn>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getStateOfPlay"
        >
          Състояние на фирма
        </v-btn>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getActualState"
        >
          Актуално състояние на фирма
        </v-btn>
        <v-btn
          text
          small
          color="primary"
          @click.stop="getCompanyDetails"
        >
          Детайли на фирма
        </v-btn>
      </v-card-actions>
    </v-card>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>


<script>
import VueJsonPretty from 'vue-json-pretty';

export default {
  name: 'RegixConnectionView',
  components: {
    VueJsonPretty
  },
  data() {
    return {
      identifier: null,
      loading: false,
      result: null
    };
  },
  methods: {
    getEmploymentContracts() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getEmploymentContracts(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
          this.loading = false;
      });
    },
    getRelations() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getRelations(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
    getValidPerson() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getValidPerson(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
    getStateOfPlay() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getStateOfPlay(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
    getActualState() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getActualState(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
    getCompanyDetails() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getCompanyDetails(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
    getPersonSearch() {
      if(!this.validate()) return;

      this.result = null;
      this.loading = true;

      this.$api.regix.getPersonSearch(this.identifier)
      .then((response) => {
        if(response) {
          this.result = response.data;
        }
      })
      .finally(() => {
        this.loading = false;
      });
    },
    validate() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      return isValid;
    }
  },

};
</script>
