<template>
  <v-card>
    <v-card-title>Моля, изчакайте...</v-card-title>
  </v-card>
</template>

<script>
import AuthService  from '@/services/auth.service';

export default{
    name: 'Login',
    props: {
      redirect: {
        type: String,
        required: false,
        default: null
      }
    },
    mounted() {
        let auth = new AuthService();
        auth.login(this.redirect);
    }
};

</script>
