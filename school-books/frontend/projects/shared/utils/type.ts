export type ArrayElementType<TResult> = TResult extends Array<infer T> ? T : never;

export type Nullable<T> = { [P in keyof T]: T[P] | null };
