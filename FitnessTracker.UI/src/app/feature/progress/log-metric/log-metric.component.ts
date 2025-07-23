// src/app/features/progress-tracking/log-metrics-dialog/log-metrics-dialog.component.ts

import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { CommonModule } from '@angular/common'; // For NgIf, NgClass etc.
import { MatIconModule } from '@angular/material/icon';
import { share } from 'rxjs';
import { SharedMaterialModule } from '../../../core/shared.module';

// Define the data structure for the dialog result
export interface LogMetricsDialogData {
  logDate: Date ;
  weight: number | null; // Allow null
  waistSizeCm: number | null; // Allow null
  bodyFatPercentage: number | null; // Allow null
  notes: string;
}

@Component({
  selector: 'app-log-metrics-dialog',
  standalone: true,
  imports: [
    SharedMaterialModule
  ],
  templateUrl: './log-metric.component.html',
  styleUrls: ['./log-metric.component.scss']
})
export class LogMetricsDialogComponent implements OnInit {
  logForm!: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<LogMetricsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any, // You can pass initial data if needed
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.logForm = this.fb.group({
      logDate: [new Date(), Validators.required],
      weight: [null, [Validators.required, Validators.min(1), Validators.max(500)]], // Add reasonable min/max
      waistSizeCm: [null, [Validators.required, Validators.min(10), Validators.max(200)]],
      bodyFatPercentage: [null, [Validators.required, Validators.min(1), Validators.max(100)]],
    //   notes: ['']
    });
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.logForm.valid) {
      this.dialogRef.close(this.logForm.value);
    } else {
      this.logForm.markAllAsTouched(); // Show validation errors
    }
  }
}