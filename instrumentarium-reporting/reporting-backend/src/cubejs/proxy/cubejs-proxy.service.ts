import { HttpService } from '@nestjs/axios';
import {
  BadRequestException,
  ForbiddenException,
  Inject,
  Injectable,
  Scope,
} from '@nestjs/common';
import { REQUEST } from '@nestjs/core';
import { AxiosResponse } from 'axios';
import { lastValueFrom } from 'rxjs';
import { SchemaRoleAccessService } from 'src/domain/schema-role-access/schema-role-access.service';
import { AuthedRequest } from 'src/shared/interfaces/authed-request.interface';
import { CubeJSLoadResponseModel } from '../models/load.response.model';
import { CubeJSMetaResponseModel } from '../models/meta.response.model';
import { CubeJSLoadOptions } from './cubejs-load-options.interface';

@Injectable({ scope: Scope.REQUEST })
export class CubeJSProxyService {
  constructor(
    private readonly httpService: HttpService,
    private readonly sraService: SchemaRoleAccessService,
    @Inject(REQUEST) private request: AuthedRequest,
  ) {}

  private getCubeJSMeta(): Promise<AxiosResponse<CubeJSMetaResponseModel>> {
    return lastValueFrom(
      this.httpService.get(
        `${process.env.CUBEJS_ENDPOINT}/v1/meta?${JSON.stringify(
          this.request.query,
        )}`,
        {
          headers: { Authorization: this.request.headers['authorization'] },
        },
      ),
    );
  }

  private getCubeJSLoad(): Promise<AxiosResponse<CubeJSLoadResponseModel>> {
    const method = this.request.method;
    if (method === 'GET') {
      const query = encodeURIComponent(this.request.query.query as string);
      const queryType = this.request.query.queryType;
      return lastValueFrom(
        this.httpService.get(
          `${process.env.CUBEJS_ENDPOINT}/v1/load?query=${query}&queryType=${queryType}`,
          {
            headers: { Authorization: this.request.headers['authorization'] },
          },
        ),
      );
    } else if (method === 'POST') {
      return lastValueFrom(
        this.httpService.post(
          `${process.env.CUBEJS_ENDPOINT}/v1/load`,
          { ...this.request.body },
          {
            headers: { Authorization: this.request.headers['authorization'] },
          },
        ),
      );
    }
  }

  public async getFilteredCubeJSMetaResponse() {
    let cubes = (await this.getCubeJSMeta())?.data?.cubes;
    const allowedSchemas = await this.sraService.find({
      loadEagerRelations: true,
      relations: ['AllowedSysRole'],
      where: {
        AllowedSysRole: {
          SysRoleID: this.request._authObject.selectedRole.SysRoleID,
        },
      },
    });

    cubes = cubes.filter((c) =>
      allowedSchemas.find((as) => as.SchemaName === c.name),
    );

    return { cubes: cubes };
  }

  public async getFilteredCubeJSLoad(
    options: CubeJSLoadOptions = {},
  ): Promise<CubeJSLoadResponseModel> {
    const { limit = 100, offset = 0 } = options;

    let dimensionsPath: any;
    if (this.request.method === 'GET') {
      dimensionsPath = JSON.parse(this.request.query.query as string);
    } else if (this.request.method === 'POST') {
      dimensionsPath = this.request.body.query;
    } else {
      throw new ForbiddenException(`Load only accepts GET and POST requests.`);
    }

    const requestedSchemaName = dimensionsPath.dimensions?.[0]?.split('.')[0];
    if (!requestedSchemaName) {
      throw new BadRequestException('No dimension provided in query.');
    }

    const allowedSchemas = await this.sraService.find({
      loadEagerRelations: true,
      relations: ['AllowedSysRole'],
      where: {
        AllowedSysRole: {
          SysRoleID: this.request._authObject.selectedRole.SysRoleID,
        },
      },
    });

    if (!allowedSchemas.find((as) => as.SchemaName === requestedSchemaName)) {
      throw new ForbiddenException();
    }

    // Inject pagination into query
    if (this.request.method === 'POST') {
      this.request.body.query = {
        ...this.request.body.query,
        limit,
        offset,
      };
    } else {
      const parsed = JSON.parse(this.request.query.query as any);
      parsed.limit = limit;
      parsed.offset = offset;
      this.request.query.query = JSON.stringify(parsed);
    }

    try {
      return (await this.getCubeJSLoad())?.data;
    } catch (e) {
      console.error(e);
      throw new BadRequestException(e.response?.data?.error);
    }
  }

  async *streamCubeRows(options: CubeJSLoadOptions = {}) {
    const pageSize = options.limit || 100;
    let offset = options.offset || 0;
    let totalRows = 0;
    const maxRows = options.maxRows || Infinity;

    while (true) {
      const chunk = await this.getFilteredCubeJSLoad({
        limit: pageSize,
        offset,
      });

      for (const row of chunk.data) {
        yield row;
        totalRows++;
        if (totalRows >= maxRows) return;
      }

      if (chunk.data.length < pageSize) break;
      offset += pageSize;
    }
  }
}
