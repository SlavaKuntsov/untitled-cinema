import { HttpErrorResponse } from "@angular/common/http";

export interface IError extends HttpErrorResponse {
  error: {
    title: string;
    status: number;
    detail: string;
  };
}
