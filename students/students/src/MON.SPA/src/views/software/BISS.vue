<template>
  <div>
    <v-container
      fluid
      class="mt-5"
    >
      <v-card
        :loading="loadingCheck"
      >
        <v-card-title>Текуща инсталация</v-card-title>
        <v-card-subtitle>Проверява се работоспособността на инсталацията на текущия компютър</v-card-subtitle>
        <v-card-text>
          <v-simple-table>
            <tbody>
              <tr>
                <td>
                  Версия
                </td>
                <td class="text-right">
                  {{ version }}
                </td>
              </tr>
            </tbody>
          </v-simple-table>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="primary"
            @click="checkInstallation"
          >
            <v-icon
              left
              small
            >
              fa-sync
            </v-icon> Провери отново
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-container>

    <v-container
      fluid
      class="mt-5"
    >
      <v-card :loading="loadingSign">
        <v-card-title>
          Проверка на инсталация <v-spacer />
          <div v-if="signature">
            <div v-if="signature && signature.isError">
              <v-chip color="error">
                <v-icon
                  small
                  left
                >
                  fa-times
                </v-icon> Настъпила е грешка при изпълнение
              </v-chip>
            </div> <div v-else>
              <v-chip color="success">
                <v-icon
                  small
                  left
                >
                  fa-check
                </v-icon>Успешно изпълнение
              </v-chip>
            </div>
          </div>
        </v-card-title>
        <v-card-subtitle>За проверка се изпълнява тестово подписване</v-card-subtitle>
        <v-card-text>
          <v-simple-table v-if="signature">
            <tbody>
              <tr>
                <td>
                  Данни
                </td>
                <td>
                  <v-textarea
                    v-model="signature.contents"
                    readonly
                  />
                </td>
              </tr>
              <tr>
                <td>
                  Съобщение
                </td>
                <td class="text-right">
                  {{ signature.message }}
                </td>
              </tr>
              <tr>
                <td>
                  Допълнителна информация
                </td>
                <td class="text-right">
                  {{ signature.additionalInformation }}
                </td>
              </tr>
            </tbody>
          </v-simple-table>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            color="primary"
            @click="signXml"
          >
            <v-icon
              left
              small
            >
              fa-signature
            </v-icon> Подпиши
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-container>
  </div>
</template>

<script>
import { VChip } from "vuetify/lib";
export default {
  name: "LocalServerView",
  components: { VChip },
  data() {
    return {
      version: "няма връзка",
      xml: "<neispuo><data></data></neispuo>",
      signature: null,
      loadingCheck: false,
      loadingSign: false
    };
  },
  mounted() {
    this.checkInstallation();
  },
  methods: {
    async checkInstallation() {
      this.loadingCheck = true;
      this.version = await this.$api.biss.version();
      this.loadingCheck = false;
    },
    async signXml(){
      this.loadingSign = true;
      this.signature = null;
      let signer = await this.$api.biss.getSigner();
      let signData = await this.$api.regix.sign(this.xml);

      let result = await this.$api.biss.signXml(this.xml, signer, signData);
      console.log(result);
      this.loadingSign = false;
    }
  },
};
</script>
