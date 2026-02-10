import { Injectable } from "@nestjs/common";
import { Version } from "./model/version.model";

@Injectable()
export class VersionService {
  constructor() {}

  getVersion(): Version {
    const fs = require('fs');
    const packageJSON = JSON.parse(fs.readFileSync('./package.json', 'utf8'));
    
    return { name: packageJSON.name, version: packageJSON.version }
  }
}