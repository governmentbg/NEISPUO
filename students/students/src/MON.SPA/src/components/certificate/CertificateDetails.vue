<template>
  <v-card>
    <v-card-title>{{ $t('certificate.title') }}</v-card-title>
    <v-card-text>
      <v-simple-table>
        <template v-slot:default>
          <tbody>
            <tr>
              <th>{{ $t('certificate.issuer') }}</th>
              <td> {{ x509Certificate.Issuer }}</td>
            </tr>
            <tr>
              <th>{{ $t('certificate.subject') }}</th>
              <td>{{ x509Certificate.Subject }}</td>
            </tr>
            <tr>
              <th>{{ $t('certificate.validFrom') }}</th>
              <td> {{ x509Certificate.NotBefore | date }}</td>
            </tr>
            <tr>
              <th>{{ $t('certificate.validTo') }}</th>
              <td>{{ x509Certificate.NotAfter | date }}</td>
            </tr>
            <tr>
              <th>{{ $t('certificate.thumbprint') }}</th>
              <td>{{ x509Certificate.Thumbprint }}</td>
            </tr>
            <tr>
              <th>{{ $t('certificate.serialNumber') }}</th>
              <td>{{ x509Certificate.SerialNumber }}</td>
            </tr>
          </tbody>
        </template>
      </v-simple-table>
    </v-card-text>
  </v-card>
</template>

<script>
export default{
  name: "CertificateDetails",
  props: {
    certificate: {
      type: String,
      required: true,
      default: ''
    }
  },
  data() {
    return {
      x509Certificate: Object
    };
  },
  mounted() {
    this.x509Certificate = JSON.parse(this.certificate);
  }
};
</script>
