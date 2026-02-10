import { ModifierResult } from '../interfaces/modifier-result.interface';
import { TemplateEnv } from '../interfaces/template-env.interface';

export type TemplateModifier = (
  tpl: string,
  env: TemplateEnv,
) => ModifierResult;
