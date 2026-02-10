/**
 * Generic type that combines a base type with an index signature
 */
export type WithExtras<T> = T & { [key: string]: unknown };
