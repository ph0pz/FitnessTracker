import { Component } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SharedMaterialModule } from '../../../core/shared.module';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-add-meal-dialog',
  templateUrl: './add-meal-dialog.component.html',
  standalone: true,
  imports: [SharedMaterialModule , MatDialogModule ]
})
export class AddMealDialogComponent {
  mealForm: FormGroup;
private unsubscribe$ = new Subject<void>(); 
  constructor(
    private dialogRef: MatDialogRef<AddMealDialogComponent>,
    private fb: FormBuilder
  ) {
    this.mealForm = this.fb.group({
    //   date: [new Date().toISOString().slice(0, 16), Validators.required],
      mealName: ['', Validators.required],
      calories: [{ value: 0, disabled: true }, [Validators.required, Validators.min(0)]], // Make calories disabled
      protein: [0, [Validators.required, Validators.min(0)]],
      carbs: [0, [Validators.required, Validators.min(0)]],
      fat: [0, [Validators.required, Validators.min(0)]],
      notes: ['']
    });
    this.setupCalorieCalculation();
  }
    ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.mealForm.valid) {
        const formData = this.mealForm.getRawValue();
     
        
      this.dialogRef.close(formData);
    }
  }
  setupCalorieCalculation(): void {
    // Get references to the form controls
    const proteinControl = this.mealForm.get('protein');
    const carbsControl = this.mealForm.get('carbs');
    const fatControl = this.mealForm.get('fat');
    const caloriesControl = this.mealForm.get('calories');

    if (proteinControl && carbsControl && fatControl && caloriesControl) {
      // Subscribe to value changes of protein, carbs, and fat
      this.mealForm.valueChanges
        .pipe(takeUntil(this.unsubscribe$)) // Automatically unsubscribe when component is destroyed
        .subscribe(values => {
          // Only re-calculate if the relevant fields have valid numbers
          const protein = values.protein || 0;
          const carbs = values.carbs || 0;
          const fat = values.fat || 0;

          if (
            typeof protein === 'number' && !isNaN(protein) &&
            typeof carbs === 'number' && !isNaN(carbs) &&
            typeof fat === 'number' && !isNaN(fat)
          ) {
            const calories = this.calculateCalories(protein, carbs, fat);
            // Use patchValue to update the disabled control without marking it as dirty by user interaction
            // { emitEvent: false } prevents an infinite loop as this is also a valueChanges listener
            caloriesControl.patchValue(calories, { emitEvent: false });
          }
        });
    }
  }

  /**
   * Calculates total calories based on macronutrient grams.
   * Standard conversion factors:
   * - Protein: 4 calories/gram
   * - Carbs: 4 calories/gram
   * - Fat: 9 calories/gram
   */
  private calculateCalories(proteinGrams: number, carbsGrams: number, fatGrams: number): number {
    const proteinCalories = proteinGrams * 4;
    const carbCalories = carbsGrams * 4;
    const fatCalories = fatGrams * 9;
    return Math.round(proteinCalories + carbCalories + fatCalories); // Round to nearest whole number
  }

}