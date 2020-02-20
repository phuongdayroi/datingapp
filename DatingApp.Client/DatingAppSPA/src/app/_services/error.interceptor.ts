import { catchError } from "rxjs/operators";
import { Injectable } from "@angular/core";
import {
  HttpInterceptor,
  HttpEvent,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
  HTTP_INTERCEPTORS
} from "@angular/common/http";
import { Observable, throwError } from "rxjs";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(error => {
        if (error.status === 401) {
          return throwError(error.statusText);
        }
        if (error instanceof HttpErrorResponse) {
          const applicationError = error.headers.get("Aplication-Error");
          if (applicationError) {
            return throwError(applicationError);
          }
          const serverError = error.error;
          let modalStateErrors = "";
          if (serverError.error && typeof serverError.error === "object") {
            for (const key in serverError.errors) {
              if (serverError.errors[key]) {
                modalStateErrors += serverError.error[key] + "\n";
              }
            }
          }
          return throwError(modalStateErrors || serverError || "Server Error");
        }
      })
    );
  }
}

// tslint:disable-next-line: one-variable-per-declaration
export const ErrorInterceptorProvider = {
  provider: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
