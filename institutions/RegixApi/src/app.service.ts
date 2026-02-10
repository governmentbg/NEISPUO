import { HttpService } from "@nestjs/axios";
import { BadRequestException, Injectable, InternalServerErrorException } from "@nestjs/common";
import { lastValueFrom, switchMap } from "rxjs";
import { Message } from "./messages";

const https = require("https");
const fs = require("fs");
const parseStringPromise = require("xml2js").parseStringPromise;

@Injectable()
export class AppService {
  constructor(private httpClient: HttpService) {}

  async getRegixData(query) {
    if (!query.operation || !query.requestName || !query.xmlns || !query.params) {
      throw new BadRequestException(Message.InvalidRequest);
    }

    let queryParams,
      params = "";

    try {
      queryParams = JSON.parse(query.params);
      for (let key in queryParams) {
        params += `<${key}>${queryParams[key]}</${key}>`;
      }
    } catch (err) {
      throw new BadRequestException(Message.InvalidRequest);
    }

    let agent = new https.Agent({
      rejectUnauthorized: false,
      pfx: fs.readFileSync("regix.mon.bg.pfx"),
      passphrase: "mon"
    });

    const uri = "https://service-regix.egov.bg/RegiXEntryPoint.svc";
    const body = `
    <s:Envelope 
    xmlns:a="http://www.w3.org/2005/08/addressing" 
    xmlns:s="http://www.w3.org/2003/05/soap-envelope"
    >
  <s:Header>
    <a:Action s:mustUnderstand="1">http://tempuri.org/IRegiXEntryPoint/ExecuteSynchronous</a:Action>
  </s:Header>
  <s:Body xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    <ExecuteSynchronous xmlns="http://tempuri.org/">
      <request>
        <Operation>${query.operation}</Operation>
        <Argument>
          <${query.requestName} 
          xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
          xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
          xmlns="${query.xmlns}"
          >
            ${params}
          </${query.requestName}>
        </Argument>
      </request>
    </ExecuteSynchronous>
  </s:Body>
</s:Envelope>
    `;

    const headers = {
      "Content-Type": "application/soap+xml; charset=utf-8"
    };

    const result = await lastValueFrom(
      this.httpClient.post(uri, body, { httpsAgent: agent, headers }).pipe(
        switchMap(async (res) => {
          return await parseStringPromise(res.data);
        })
      )
    );

    const filteredResult = this.filterResult(
      result["s:Envelope"]?.["s:Body"]?.[0]?.["ExecuteSynchronousResponse"]?.[0]?.["ExecuteSynchronousResult"]
    );

    if (filteredResult.HasError === "true") {
      throw new InternalServerErrorException(filteredResult.Error);
    } else {
      return filteredResult["Data"]["Response"];
    }
  }

  private filterResult(data) {
    if (data && typeof data === "object" && data.length === undefined) {
      delete data["$"];

      if (data["_"]) {
        return data["_"];
      }

      for (let key in data) {
        data[key] = this.filterResult(data[key]);
      }
    } else if (data && typeof data === "object") {
      if (data.length === 1) {
        data = this.filterResult(data[0]);
      } else {
        const filteredList = [];
        for (let elem of data) {
          filteredList.push(this.filterResult(elem));
        }
        data = filteredList;
      }
    }

    return data;
  }
}
