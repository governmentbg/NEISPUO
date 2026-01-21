<template>
  <v-main>
    <v-container
      fill-height
      fluid
    >
      <v-row
        align="center"
        justify="center"
      >
        <v-col>
          <v-alert
            outlined
            type="error"
            prominent
          >
            <p class="text-h3 text-center">
              {{ $t("errors.pageNotFound") }}
            </p>
          </v-alert>
        </v-col>
      </v-row>
    </v-container>
  </v-main>
</template>

<style>
  .paragraph-text {
    font-size: 18px;
  }
</style>

<script>
export default 
{
  name: "Http404"
};
</script>