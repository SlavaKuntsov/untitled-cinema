import { CommonModule } from "@angular/common";
import { Component, EventEmitter, Input, Output } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ConfirmationService, MessageService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { ConfirmDialogModule } from "primeng/confirmdialog";
import { DatePickerModule } from "primeng/datepicker";
import { DialogModule } from "primeng/dialog";
import { ToastModule } from "primeng/toast";

@Component({
  selector: "app-confirmation-dialog",
  imports: [
    ReactiveFormsModule,
    ToastModule,
    ButtonModule,
    CommonModule,
    DialogModule,
    ConfirmDialogModule,
    FormsModule,
    DatePickerModule,
  ],
  providers: [ConfirmationService, MessageService],
  templateUrl: "./confirmation-dialog.component.html",
  styleUrl: "./confirmation-dialog.component.scss",
})
export class ConfirmationDialogComponent {
  @Input() label: string = "Confirm";
  @Input() buttonClasses: string = "";
  @Input() dialogKey: string = "";
  @Input() header: string = "Are you sure?";
  @Input() message: string = "Please confirm to proceed.";
  @Input() acceptLabel: string = "Confirm";
  @Input() colorClass: string = "bg-green-600";
  @Input() acceptButtonClass: string =
    "!bg-green-600 !text-white !transition-colors !duration-500 !easy-in-out hover:!text-green-500 !border-[2px] hover:!border-white !border-green-600 hover:!bg-white w-32 !text-2xl !px-8 !py-3";

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  constructor(private confirmationService: ConfirmationService) {}

  onClick() {
    this.confirmationService.confirm({
      header: this.header,
      message: this.message,
      key: this.dialogKey,
      accept: () => {
        this.confirm.emit();
      },
      reject: () => {
        this.cancel.emit();
      },
    });
  }
}
