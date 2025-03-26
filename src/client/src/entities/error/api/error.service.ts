import { Injectable } from "@angular/core";
import { IError } from "../model/error";

@Injectable({
  providedIn: "root",
})
export class ErrorService {
  getErrorMessage(error: IError): string {
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
    } else if (error.error && error.error.title) {
      return error.error.title;
    } else if (error.message) {
      return error.message;
    }
    return "An unknown error occurred";
  }
}
