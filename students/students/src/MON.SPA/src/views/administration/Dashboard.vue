<template>
  <v-row>
    <v-col cols="6">
      <server-info />
    </v-col>
  </v-row>
</template>

<script>
    import ServerInfo from '@/components/admin/serverInfo';


    export default{
        name: 'Dashboard',
        components: { ServerInfo}
    };
</script>