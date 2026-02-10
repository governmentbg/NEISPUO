// This file is required by karma.conf.js and loads recursively all the .spec and framework files

// Zone.js must be the first import, so disable prettier-plugin-organize-imports
// organize-imports-ignore

import 'zone.js/testing';
import { getTestBed } from '@angular/core/testing';
import { BrowserDynamicTestingModule, platformBrowserDynamicTesting } from '@angular/platform-browser-dynamic/testing';

declare const require: {
  context(
    path: string,
    deep?: boolean,
    filter?: RegExp
  ): {
    keys(): string[];
    <T>(id: string): T;
  };
};

// First, initialize the Angular testing environment.
getTestBed().initTestEnvironment(BrowserDynamicTestingModule, platformBrowserDynamicTesting());
// Then we find all the tests.
const context = require.context('./', true, /\.spec\.ts$/);
const contextShared = require.context('../../shared/', true, /\.spec\.ts$/);
// And load the modules.
context.keys().map(context);
contextShared.keys().map(contextShared);
