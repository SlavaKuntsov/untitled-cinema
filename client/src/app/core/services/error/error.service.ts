import { HttpErrorResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { IError } from "../../../../entities/error/model/error";

@Injectable({
  providedIn: "root",
})
export class ErrorService {
  getErrorMessage(error: IError): string {
    if (error instanceof HttpErrorResponse) {
      if (error.error && typeof error.error === "string") {
        try {
          const errorObj = JSON.parse(error.error);
          return (
            errorObj.detail || errorObj.message || "An unknown error occurred"
          );
        } catch (e) {
          return error.error;
        }
      } else if (error.error && error.error.detail) {
        return error.error.detail;
      } else if (error.message) {
        return error.message;
      }
    }
    return "An unknown error occurred";
  }
}
