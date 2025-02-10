import { inject, Injectable } from "@angular/core";
import { MessageService } from "primeng/api";

@Injectable({
  providedIn: "root",
})
export class ToastService {
  private messageService = inject(MessageService);

  showToast = (status: string, detail: string) => {
    this.messageService.add({
      severity: status,
      summary: this.capitalizeFirstLetter(status),
      detail: detail,
    });
  };

  private capitalizeFirstLetter = (str: string): string => {
    return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
  };
}
