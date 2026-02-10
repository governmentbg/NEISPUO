<template>
  <div>
    <v-row
      justify="center"
      class="mt-5"
    >
      <v-sheet
        width="800"
        elevation="3"
      >
        <v-simple-table>
          <template v-slot:default>
            <thead>
              <tr>
                <th class="text-left">
                  Вариант
                </th>
                <th class="text-left" />
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>
                  Локален сървър за достъп до електронни сертификати
                  <v-chip small>
                    1.0.8
                  </v-chip>
                  <p class="text--secondary">
                    Поддържа се <v-icon small>
                      fab fa-windows
                    </v-icon> Windows
                    версия 7 и по-нова
                  </p>
                </td>
                <td>
                  <v-btn
                    color="primary"
                    download
                    :href="`${spaBaseUrl}download/LocalServer_NEISPUO_1.0.8.exe`"
                  >
                    <v-icon left>
                      mdi-cloud-download
                    </v-icon>
                    {{ $t("common.download") }}
                  </v-btn>
                </td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
      </v-sheet>
    </v-row>

    <v-container
      fluid
      class="mt-5"
    >
    <v-alert
      border="top"
      colored-border
      :type="lnaStatusType"
      elevation="2"
    >
      <div v-html="lnaStatus"></div>
    </v-alert>
      
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
              <tr>
                <td>
                  Вариант
                </td>
                <td class="text-right">
                  {{ edition }}
                </td>
              </tr>
              <tr>
                <td>
                  Възможности
                </td>
                <td class="text-right">
                  {{ caps }}
                </td>
              </tr>
              <tr>
                <td>
                  Настройки
                </td>
                <td class="text-right">
                  {{ settings }}
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
import { config } from '@/common/config';

export default {
  name: "LocalServerView",
  components: { VChip },
  data() {
    return {
      version: "няма връзка",
      edition: "няма връзка",
      caps: "няма връзка",
      settings: "няма връзка",
      xml: "<neispuo><data></data></neispuo>",
      signature: null,
      loadingCheck: false,
      loadingSign: false,
      spaBaseUrl: config.spaBaseUrlRelative,
      lnaStatus: null,
      lnaStatusType: "info"
    };
  },
  mounted() {
    this.checkLNA();
    this.checkInstallation();
  },
  methods: {
    async checkLNA(){
      try {
        // --- New Chrome 142+ API ---
        const permission = await navigator.permissions.query({ name: "local-network-access" });

        const updateUI = (state) => {
          switch (state) {
            case "granted":
              this.lnaStatusType = "success";
              this.lnaStatus = "✅ Достъп до локални устройства е позволен.";
              break;
            case "prompt":
              this.lnaStatusType = "warning";
              this.lnaStatus = "ℹ️ Ще е необходимо разрешение за достъп до локални устройства. За повече информация вижте <a href='" + config.spaBaseUrlRelative + "docs/guide/localServer/troubleshooting.html'>тук</a>";
              break;
            case "denied":
              this.lnaStatusType = "error";
              this.lnaStatus = "🚫 Достъпът до локални устройства е блокиран. За повече информация и как да отблокирате вижте <a href='" + config.spaBaseUrlRelative + "docs/guide/localServer/troubleshooting.html'>тук</a> ";
              break;
            default:
              this.lnaStatusType = "info";
              this.lnaStatus = "❔ Неразпознат статус за достъп до локални устройства: " + state;
          }
        };

        updateUI(permission.state);

        // React to live permission changes (user action)
        permission.onchange = () => updateUI(permission.state);

      } catch (err) {
        // --- Fallback for browsers without LNA permission descriptor ---
        console.warn("LNA Permissions API not supported:", err);
        this.lnaStatus = "⚙️ Проверка за достъп до локален сървър…";

        try {
          const controller = new AbortController();
          const timeout = setTimeout(() => controller.abort(), 3000);

          const resp = await fetch("http://127.0.0.1:5339/api/server/version", {
            mode: "cors",
            signal: controller.signal
          });
          clearTimeout(timeout);

          if (resp.ok) {
            this.lnaStatusType = "success";
            this.lnaStatus = "✅ Достъп до локални устройства е позволен.";
          } else {
            this.lnaStatusType = "warning";
            this.lnaStatus = `⚠️ Достъпът до локални устройства даде грешка ${resp.status}.`;
          }
        } catch (fetchErr) {
          this.lnaStatusType = "error";
          this.lnaStatus = "🚫 Достъпът до локални устройства е блокиран или Локален сървър не е стартиран. За повече информация и как да отблокирате вижте <a href='" + config.spaBaseUrlRelative + "docs/localserver/troubleshooting'>тук</a> ";
          console.error(fetchErr);
        }
      }
    },
    async checkInstallation() {
      this.loadingCheck = true;
      await this.$api.localServer.version().then( (data) => {this.version = data.data;}).catch((error) => {
        this.version = error;
      });
      await this.$api.localServer.edition().then( (data) => {this.edition = data.data;}).catch((error) => {
        this.edition = error;
      });
      await this.$api.localServer.capabilities().then( (data) => {this.caps = data.data;}).catch((error) => {
        this.caps = error;
      });
      await this.$api.localServer.settings().then( (data) => {this.settings = data.data;}).catch((error) => {
        this.settings = error;
      });
      this.loadingCheck = false;
    },
    signXml(){
      this.loadingSign = true;
      this.signature = null;

      this.$api.certificate.signXml(this.xml)
      .then((response) => {
        this.signature = response;
      }).catch((error) => {
        this.signature = {isError: true, message : error.message};
      })
      .then(() => {
        this.loadingSign = false;
      });
    }
  },
};
</script>
