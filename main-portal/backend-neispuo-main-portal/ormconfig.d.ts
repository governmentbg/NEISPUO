// https://github.com/microsoft/TypeScript/issues/15031
declare module 'ormconfig' {
    interface OrmConfig {
        [key: string]: any;
    }

    const _ormconfig: OrmConfig[];
    export = _ormconfig;
}
