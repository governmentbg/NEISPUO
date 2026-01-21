import { Logger } from '@nestjs/common';
import { ValueTransformer } from 'typeorm';

/**
 * JSON transformer for TypeORM columns.
 *
 * Introduced in TypeORM v0.2.30 and designed for SQL Server 2022’s NVARCHAR(MAX).
 * We avoid using the built-in `@Column('simple-json')` on SQL Server
 * because it maps to deprecated ntext/text types. Instead, we store raw JSON
 * in an NVARCHAR(MAX) column for unlimited capacity, and use this transformer
 * to handle serialization and deserialization.
 *
 * - **to**: Called before persisting—stringifies the value (defaulting
 *   to an empty array if the input is null or undefined).
 * - **from**: Called on retrieval—parses the JSON string back into JS,
 *   returning an empty array if the column is null or if parsing fails.
 *
 * @since TypeORM v0.2.30
 * @supports SQL Server 2022 (NVARCHAR(MAX) JSON storage)
 */
export const jsonTransformer: ValueTransformer = {
  to: (val: any) => JSON.stringify(val ?? []),
  from: (dbVal: string) => {
    try {
      return dbVal == null ? [] : JSON.parse(dbVal);
    } catch (e) {
      Logger.error('Failed to parse JSON from database:', e);
      return [];
    }
  },
};
