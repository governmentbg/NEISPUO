<template>
  <v-card>
    <v-card-text>
      <v-row
        dense
        class="mb-2"
        align="center"
        justify="space-around"
      >
        <button-group>
          <v-btn
            color="primary"
            outlined
            :disabled="saving"
            @click.stop="onGetCacheServerInfo"
          >
            <v-icon
              color="primary"
              left
            >
              mdi-server-outline
            </v-icon>
            <span class="ml-1">Redis server info</span>
          </v-btn>
          <v-btn
            color="primary"
            outlined
            :disabled="saving"
            @click.stop="onGetCacheKeys"
          >
            <v-icon
              color="primary"
              left
            >
              mdi-list-box-outline
            </v-icon>
            <span class="ml-1">{{ $t('buttons.cacheKeys') }}</span>
          </v-btn>
          <v-btn
            color="primary"
            outlined
            :disabled="saving"
            @click.stop="onClearCache"
          >
            <v-icon
              color="primary"
              left
            >
              mdi-cached
            </v-icon>
            <span class="ml-1">{{ $t('buttons.clearCache') }}</span>
          </v-btn>
        </button-group>
      </v-row>
      <v-row
        v-if="cacheServerInfo"
        dense
      >
        <vue-json-pretty
          :data="cacheServerInfo"
          show-length
          show-line
          show-icon
          :show-double-quotes="false"
        />
      </v-row>
      <v-row
        v-if="cacheKeys"
        dense
      >
        <v-col
          cols="6"
        >
          <vue-json-pretty
            :data="cacheKeys"
            show-length
            show-line
            show-icon
            :show-double-quotes="false"
          >
            <template v-slot:[`nodeValue`]="{ node, defaultValue }">
              {{ defaultValue }}
              <button-group>
                <button-tip
                  icon
                  icon-name="mdi-eye"
                  icon-color="primary"
                  tooltip="buttons.review"
                  bottom
                  iclass=""
                  small
                  @click="onGetCacheKeyValue(cacheKeys[node.index])"
                />
                <button-tip
                  icon
                  icon-name="mdi-cached"
                  icon-color="error"
                  tooltip="buttons.clear"
                  bottom
                  iclass=""
                  small
                  @click="onClearCacheKey(cacheKeys[node.index])"
                />
              </button-group>
            </template>
          </vue-json-pretty>
        </v-col>
        <v-col
          cols="6"
        >
          <div class="my-2">
            <v-chip
              v-if="selectedCacheKey"
              small
              class="ma-1"
            >
              {{ selectedCacheKey }}
            </v-chip>
          </div>
          <vue-json-pretty
            :data="selectedCacheKeyValue"
            show-length
            show-line
            show-icon
          />
        </v-col>
      </v-row>
    </v-card-text>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

export default {
  name: 'CacheServiceManager',
  components: {
    VueJsonPretty
  },
  data() {
    return {
      saving: false,
      cacheServerInfo: null,
      cacheKeys: null,
      selectedCacheKey: null,
      selectedCacheKeyValue: null
    };
  },
  methods: {
    clearData() {
      this.cacheServerInfo = null;
      this.cacheKeys = null;
      this.selectedCacheKey = null;
      this.selectedCacheKeyValue = null;
    },
    onClearCache() {
      this.saving = true;
      this.$api.administration.clearCache()
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'));
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('common.saveError'));
      })
      .then(() => {
        this.saving = false;
      });
    },
    onGetCacheKeys() {
      this.saving = true;
      this.clearData();

      this.$api.administration.getCacheKeys()
      .then((response) => {
        this.cacheKeys = response.data;
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('common.loadError'));
      })
      .then(() => {
        this.saving = false;
      });
    },
    onGetCacheServerInfo() {
      this.saving = true;
      this.clearData();

      this.$api.administration.getCacheServerInfo()
      .then((response) => {
        this.cacheServerInfo = response.data;
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('common.loadError'));
      })
      .then(() => {
        this.saving = false;
      });
    },
    onGetCacheKeyValue(cacheKey) {
      this.selectedCacheKey = null;
      this.selectedCacheKeyValue = null;
      this.saving = true;

      this.$api.administration.getCacheKeyFull(cacheKey)
      .then((response) => {
        this.selectedCacheKey = cacheKey;
        this.selectedCacheKeyValue = response.data;
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('common.loadError'));
      })
      .then(() => {
        this.saving = false;
      });
    },
    async onClearCacheKey(cacheKey) {
      if (await this.$refs.confirm.open(this.$t('buttons.clear'), this.$t('common.confirm'))) {
        this.saving = true;
        this.clearData();

        this.$api.administration.clearCacheKey(cacheKey)
        .then(() => {
          this.onGetCacheKeys();
        })
        .catch((error) => {
          console.log(error.response);
          this.$notifier.error('', this.$t('common.loadError'));
        })
        .then(() => {
          this.saving = false;
        });
      }
    }
  }
};
</script>
