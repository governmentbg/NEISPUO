<template>
  <v-card>
    <v-card-title>
      <v-icon left>
        fa-server
      </v-icon>{{ this.$t('serverInfo.title') }}
    </v-card-title>
    <v-card-text>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.osPlatform') }}
        </v-col>
        <v-col cols="6">
          {{ serverInfo.osPlatform }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.aspDotnetVersion') }}
        </v-col>
        <v-col cols="6">
          {{ serverInfo.aspDotnetVersion }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.allocated') }}
        </v-col>
        <v-col cols="6">
          {{ formatBytes(serverInfo.allocated) }}
        </v-col>
      </v-row>

      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.privateMemorySize64') }}
        </v-col>
        <v-col cols="6">
          {{ formatBytes(serverInfo.privateMemorySize64) }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.virtualMemorySize64') }}
        </v-col>
        <v-col cols="6">
          {{ formatBytes(serverInfo.virtualMemorySize64) }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.workingSet64') }}
        </v-col>
        <v-col cols="6">
          {{ formatBytes(serverInfo.workingSet64) }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.cpuUsage') }}
        </v-col>
        <v-col cols="6">
          {{ serverInfo.cpuUsage }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.processorCount') }}
        </v-col>
        <v-col cols="6">
          {{ serverInfo.processorCount }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.minThreads') }}
        </v-col>
        <v-col cols="6">
          IO: {{ serverInfo.minIOThreads }} / Worker: {{ serverInfo.minWorkerThreads }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.maxThreads') }}
        </v-col>
        <v-col cols="6">
          IO: {{ serverInfo.maxIOThreads }} / Worker: {{ serverInfo.maxWorkerThreads }}
        </v-col>
      </v-row>
      <v-row>
        <v-col
          cols="6"
          right
        >
          {{ this.$t('serverInfo.availableThreads') }}
        </v-col>
        <v-col cols="6">
          IO: {{ serverInfo.availableIOThreads }} / Worker: {{ serverInfo.availableWorkerThreads }}
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions>
      <button-tip
        icon
        icon-name="fas fa-database"
        icon-color="primary"
        tooltip="buttons.dataBaseTooltip"
        iclass="mx-2"
        right
        @click="onDbDgml()"
      />
    </v-card-actions>
  </v-card>
</template>

<script>
    import {ServerInfo} from '@/models/admin/serverInfo.js';
    export default {
        name: 'ServerInfo',
        data() {
            return{
                serverInfo: new ServerInfo()
            };
        },
        beforeMount(){
            this.init();
        },
        methods:{
            async init(){
                this.$api.admin.getServerInfo().then((result) =>{
                    this.serverInfo = new ServerInfo(result.data);
                });
            },
            formatBytes(bytes, decimals = 2) {
                if (bytes === 0) return '0 Bytes';

                const k = 1024;
                const dm = decimals < 0 ? 0 : decimals;
                const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

                const i = Math.floor(Math.log(bytes) / Math.log(k));

                return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
            },
            async onDbDgml(){
              let blob = new Blob([(await this.$api.admin.getDBDgml()).data], { type: 'application/octet-stream' }),
              url = window.URL.createObjectURL(blob);
              var link = document.createElement('a');
              link.href = url;
              link.download = "DB.dgml";
              link.click();
            }
        }
    };
</script>
