import { Injectable, OnModuleInit } from '@nestjs/common';
import * as Handlebars from 'handlebars';
import { registerAllHelpers } from './helpers';
import { dataTablePartial } from './partials/data-table.partial';

@Injectable()
export class HandlebarsService implements OnModuleInit {
  private readonly hbs = Handlebars;

  onModuleInit() {
    this.hbs.registerPartial(dataTablePartial.name, dataTablePartial.template);
    registerAllHelpers(this.hbs);
  }

  /**
   * Compile a template string with a given context, returning the rendered string.
   * Because partials and helpers were registered in onModuleInit(), you can
   * reference them freely in your templates.
   *
   * @example
   *   const tpl = `<p>{{name}}</p> {{> data‐table}}`;
   *   const ctx = { name: 'alice', table: { … } };
   *   return handlebarsService.compile(tpl, ctx);
   */
  public compile<T = unknown>(template: string, context: T): string {
    const fn = this.hbs.compile(template);
    return fn(context);
  }
}
