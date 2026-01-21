import { NgModule } from '@angular/core';
import { FaConfig, FontAwesomeModule } from '@fortawesome/angular-fontawesome';

@NgModule({
  declarations: [],
  imports: [FontAwesomeModule],
  exports: [FontAwesomeModule]
})
export class FontAwesomeWithConfigModule {
  constructor(faConfig: FaConfig) {
    faConfig.fixedWidth = true;
  }
}
