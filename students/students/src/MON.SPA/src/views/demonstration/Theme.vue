<template>
  <div>
    <h2>*Typography*</h2>

    <div class="text-h1">
      Hello from h1
    </div>
    <div class="text-h2">
      Hello from h2
    </div>
    <div class="text-h3">
      Hello from h3
    </div>
    <div class="text-h4">
      Hello from h4
    </div>
    <div class="text-h5">
      Hello from h5
    </div>
    <p>Hello from paragraph</p>
    <div class="v-small">
      Hello from .v-small text 14px
    </div>
    <div class="v-xsmall">
      Hello from .v-xsmall text 12px
    </div>

    <hr style="margin: 20px 0">

    <h2>*Toolbar*</h2>
    <v-toolbar>
      <v-btn
        icon
        class="example-icon"
        aria-label="Example icon-button with menu icon"
      >
        <v-icon>menu</v-icon>
      </v-btn>
      <span>My App</span>
      <span class="example-spacer" />
      <v-btn
        icon
        class="example-icon favorite-icon"
        aria-label="Example icon-button with heart icon"
      >
        <v-icon>favorite</v-icon>
      </v-btn>
      <v-btn
        icon
        class="example-icon"
        aria-label="Example icon-button with share icon"
      >
        <v-icon>share</v-icon>
      </v-btn>
    </v-toolbar>

    <hr style="margin: 20px 0">

    <h2>*Buttons*</h2>
    <section>
      <div class="example-label">
        Basic
      </div>
      <div class="example-button-row">
        <v-btn>
          Basic
        </v-btn>
        <v-btn
          color="primary"
        >
          Primary
        </v-btn>
        <v-btn
          color="accent"
        >
          Accent
        </v-btn>
        <v-btn
          color="warning"
        >
          Warn
        </v-btn>
        <v-btn
          disabled
        >
          Disabled
        </v-btn>
        <v-btn
          disabled
          href="https://www.google.com/"
          target="_blank"
        >
          Link
        </v-btn>
      </div>
    </section>
    <v-divider />
    <section>
      <div class="example-label">
        Raised
      </div>
      <div class="example-button-row">
        <v-btn raised>
          Basic
        </v-btn>
        <v-btn
          raised
          color="primary"
        >
          Primary
        </v-btn>
        <v-btn
          raised
          color="accent"
        >
          Accent
        </v-btn>
        <v-btn
          raised
          color="warning"
        >
          Warn
        </v-btn>
        <v-btn
          raised
          disabled
        >
          Disabled
        </v-btn>
        <v-btn
          raised
          href="https://www.google.com/"
          target="_blank"
        >
          Link
        </v-btn>
      </div>
    </section>
    <v-divider />
    <section>
      <div class="example-label">
        Stroked
      </div>
      <div class="example-button-row">
        <v-btn outlined>
          Basic
        </v-btn>
        <v-btn
          outlined
          color="primary"
        >
          Primary
        </v-btn>
        <v-btn
          outlined
          color="accent"
        >
          Accent
        </v-btn>
        <v-btn
          outlined
          color="warning"
        >
          Warn
        </v-btn>
        <v-btn
          outlined
          disabled
        >
          Disabled
        </v-btn>
        <v-btn
          outlined
          href="https://www.google.com/"
          target="_blank"
        >
          Link
        </v-btn>
      </div>
    </section>
    <v-divider />
    <section>
      <div class="example-label">
        Flat
      </div>
      <div class="example-button-row">
        <v-btn text>
          Basic
        </v-btn>
        <v-btn
          text
          color="primary"
        >
          Primary
        </v-btn>
        <v-btn
          text
          color="accent"
        >
          Accent
        </v-btn>
        <v-btn
          text
          color="warning"
        >
          Warn
        </v-btn>
        <v-btn
          text
          disabled
        >
          Disabled
        </v-btn>
        <v-btn
          text
          href="https://www.google.com/"
          target="_blank"
        >
          Link
        </v-btn>
      </div>
    </section>
    <v-divider />
    <section>
      <div class="example-label">
        Icon
      </div>
      <div class="example-button-row">
        <div class="example-flex-container">
          <v-btn
            icon
            aria-label="Example icon button with a vertical three dot icon"
          >
            <v-icon>more_vert</v-icon>
          </v-btn>
          <v-btn
            icon
            color="primary"
            aria-label="Example icon button with a home icon"
          >
            <v-icon>home</v-icon>
          </v-btn>
          <v-btn
            icon
            color="accent"
            aria-label="Example icon button with a menu icon"
          >
            <v-icon>menu</v-icon>
          </v-btn>
          <v-btn
            icon
            color="warning"
            aria-label="Example icon button with a heart icon"
          >
            <v-icon>favorite</v-icon>
          </v-btn>
          <v-btn
            icon
            disabled
            aria-label="Example icon button with a open in new tab icon"
          >
            <v-icon>open_in_new</v-icon>
          </v-btn>
        </div>
      </div>
    </section>
    <v-divider />
    <section>
      <div class="example-label">
        FAB
      </div>
      <div class="example-button-row">
        <div class="example-flex-container">
          <div class="example-button-container">
            <v-btn
              fab
              color="primary"
              aria-label="Example icon button with a delete icon"
            >
              <v-icon>delete</v-icon>
            </v-btn>
          </div>
          <div class="example-button-container">
            <v-btn
              fab
              color="accent"
              aria-label="Example icon button with a bookmark icon"
            >
              <v-icon>bookmark</v-icon>
            </v-btn>
          </div>
          <div class="example-button-container">
            <v-btn
              fab
              color="warning"
              aria-label="Example icon button with a home icon"
            >
              <v-icon>home</v-icon>
            </v-btn>
          </div>
          <div class="example-button-container">
            <v-btn
              fab
              disabled
              aria-label="Example icon button with a heart icon"
            >
              <v-icon>favorite</v-icon>
            </v-btn>
          </div>
        </div>
      </div>
    </section>
    <v-divider />
    <section>
      <div class="example-label">
        Mini FAB
      </div>
      <div class="example-button-row">
        <div class="example-flex-container">
          <div class="example-button-container">
            <v-btn
              fab
              small
              color="primary"
              aria-label="Example icon button with a menu icon"
            >
              <v-icon>menu</v-icon>
            </v-btn>
          </div>
          <div class="example-button-container">
            <v-btn
              fab
              small
              color="accent"
              aria-label="Example icon button with a plus one icon"
            >
              <v-icon>plus_one</v-icon>
            </v-btn>
          </div>
          <div class="example-button-container">
            <v-btn
              fab
              small
              color="warning"
              aria-label="Example icon button with a filter list icon"
            >
              <v-icon>filter_list</v-icon>
            </v-btn>
          </div>
          <div class="example-button-container">
            <v-btn
              fab
              small
              disabled
              aria-label="Example icon button with a home icon"
            >
              <v-icon>home</v-icon>
            </v-btn>
          </div>
        </div>
      </div>
    </section>

    <hr style="margin: 20px 0">

    <h2>*Form fields appearance="outline"*</h2>
    <form>
      <v-text-field
        v-model="fullName"
        label="Name"
        outlined
        dense
      />

      <v-text-field
        v-model="email"
        label="Email"
        outlined
        dense
      >
        <span class="v-error"> Please enter a valid email address </span>
      </v-text-field>

      <v-text-field 
        v-model="password"
        label="Password"
        outlined
        dense
        type="password"
      />

      <v-text-field
        v-model="disabledInput"
        outlined
        label="Disabled"
        disabled
      />
      <v-select
        v-model="country"
        label="Country"
        :items="items"
      />
    </form> 

    <hr style="margin: 20px 0">

    <h2>*Card (background .bg-selago)*</h2>
    <p
      class="bg-selago"
      style="padding: 30px"
    >
      <v-card color="primary">
        <v-card-title class="bg-cornflower-blue">
          Card title
        </v-card-title>
        <v-card-text>
          <p>
            Lorem ipsum dolor sit amet consectetur adipisicing elit. Debitis quasi
            quod, eligendi dicta modi sint rerum perspiciatis enim voluptatum
            recusandae minima ipsum cupiditate veritatis exercitationem aliquid
            numquam quia, repellat nesciunt.
          </p>
        </v-card-text>
        <v-card-footer>50 Lessons (footer)</v-card-footer>
      </v-card>
    </p>

    <hr style="margin: 20px 0">

    <h2>*Checkbox*</h2>
    <section>
      <v-checkbox 
        v-for="ch in checkboxes"
        :key="ch"
        v-model="ch.checked"
        :label="ch.name"
        :color="ch.color" 
        :value="ch.value"
      >
        {{ ch.name }}
      </v-checkbox>
    </section>

    <hr style="margin: 20px 0">

    <h2>*Radio*</h2>
    <form
      novalidate
    >
      <v-radio-group v-model="value">
        <v-radio
          color="primary"
          :value="1"
          label="Primary"
        />
        <v-radio
          color="accent"
          label="Accent"
          :value="2"
        />
        <v-radio
          color="warning"
          :value="3"
          label="Warning"
        />
      </v-radio-group>
    </form>

    <hr style="margin: 20px 0">
    <div v-demo:foo.a.b="message" />    
  </div>
</template>
<style>
section {
  display: table;
  padding: 20px;
}

.example-label {
  display: table-cell;
  font-size: 14px;
  margin-left: 8px;
  min-width: 120px;
}

.example-button-row {
  display: table-cell;
  width: 490px;
}

.example-button-row .mat-button-base {
  margin: 8px 8px 8px 0;
}

.example-flex-container {
  display: flex;
  justify-content: space-between;
  flex-wrap: wrap;
}

.example-button-container {
  display: flex;
  justify-content: center;
  width: 120px;
}
</style>
<script>
export default {
  name: 'Theme',
  data() {
      return{
          array: [],
        fullName: "",
        email: "",
        password: "",
        country: "",
        disabledInput: "",
        value: false,
        items: ["Great Britain", "United States", "Canada"],
        checkboxes : [
        { name: "Primary", checked: true, color: "primary" },
        { name: "Accent", checked: true, color: "accent" },
        { name: "Warning", checked: true, color: "warning" },
        ]

      };
  }
};
</script>