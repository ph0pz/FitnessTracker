import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common'; // For ngIf etc.

// Material Modules
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { MatIconModule } from '@angular/material/icon'; // If you want icons in the dialog
import { SharedMaterialModule } from '../../../core/shared.module';
import { EditMacrosDialogData } from '../../../models/dashboard.model';



@Component({
  selector: 'app-macro-dialog.component',
  templateUrl: './macro-dialog.component.html',
  styleUrls: ['./macro-dialog.component.scss'],
     standalone: true,
  imports: [
    SharedMaterialModule,
  ],
})
export class EditMacrosDialogComponent implements OnInit {
  macroForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<EditMacrosDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EditMacrosDialogData // To pass in existing goal
  ) {}

  ngOnInit(): void {
    const currentGoal = this.data.currentGoal;

    this.macroForm = this.fb.group({
      goalType: [''], // Default to 'Maintenance' or existing
      protein: [currentGoal?.protein || null, [Validators.required, Validators.min(0)]],
      carbs: [currentGoal?.carbs || null, [Validators.required, Validators.min(0)]],
      fat: [currentGoal?.fat || null, [Validators.required, Validators.min(0)]],
      calories: [{ value: currentGoal?.calories || 0, disabled: true }], // Calories disabled by default
    });

    // Subscribe to value changes of protein, carbs, fat to calculate calories
    this.macroForm.valueChanges.pipe(
      debounceTime(300), // Wait for user to stop typing
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)) // Only emit if values actually changed
    ).subscribe(values => {
      const protein = values.protein || 0;
      const carbs = values.carbs || 0;
      const fat = values.fat || 0;
      const calculatedCalories = this.calculateCalories(protein, carbs, fat);

      // Only update if the calculated value is different to prevent infinite loops
      if (this.macroForm.get('calories')?.value !== calculatedCalories) {
        this.macroForm.get('calories')?.setValue(calculatedCalories, { emitEvent: false }); // Avoid infinite loop
      }
    });

    // Initial calculation if pre-filled data exists
    if (currentGoal) {
      this.macroForm.get('protein')?.updateValueAndValidity({ emitEvent: true });
      this.macroForm.get('carbs')?.updateValueAndValidity({ emitEvent: true });
      this.macroForm.get('fat')?.updateValueAndValidity({ emitEvent: true });
    }
  }

  private calculateCalories(proteinGrams: number, carbsGrams: number, fatGrams: number): number {
    const proteinCalories = proteinGrams * 4;
    const carbCalories = carbsGrams * 4;
    const fatCalories = fatGrams * 9;
    return Math.round(proteinCalories + carbCalories + fatCalories); // Round to nearest whole number
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.macroForm.valid) {
      // Get the current values, including the disabled calories field
      const formValue = this.macroForm.getRawValue(); // getRawValue includes disabled fields
      this.dialogRef.close(formValue);
    }
  }
}