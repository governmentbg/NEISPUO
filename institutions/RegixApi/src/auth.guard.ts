import { Injectable, ExecutionContext } from "@nestjs/common";
import { JwtService } from "@nestjs/jwt";
import { AuthGuard } from "@nestjs/passport";
import { Observable } from "rxjs";

@Injectable()
export class JwtAuthGuard extends AuthGuard("jwt") {
  constructor(private jwtService: JwtService) {
    super();
  }

  canActivate(context: ExecutionContext): boolean | Promise<boolean> | Observable<boolean> {
    return new Promise((resolve, reject) => {
      const res: any = super.canActivate(context);
      const request = context.switchToHttp().getRequest();

      const token = request?.headers?.authorization ? request.headers.authorization.split(" ")[1] : null;
      const decodedToken: any = token ? this.jwtService.decode(token) : null;

      res
        .then(() => {
          if (decodedToken && ["inst_basic", "reg_inst"].includes(decodedToken.aud)) {
            resolve(true);
          } else {
            resolve(false);
          }
        })
        .catch((err) => {
          reject(err);
        });
    });
  }
}
