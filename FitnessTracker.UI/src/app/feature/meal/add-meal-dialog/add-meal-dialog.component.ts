import { Component } from '@angular/core';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { SharedMaterialModule } from '../../../core/shared.module';
import { Subject, takeUntil } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { MealService } from '../../../core/service/meal.service';

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
    private fb: FormBuilder,
     private mealService: MealService
  ) {
this.mealForm = this.fb.group({
  date: [new Date(), Validators.required], // Add date if missing
  mealName: ['', Validators.required],
  calories: [{ value: 0, disabled: true }, [Validators.required, Validators.min(0)]],
  protein: [0, [Validators.required, Validators.min(0)]],
  carbs: [0, [Validators.required, Validators.min(0)]],
  fat: [0, [Validators.required, Validators.min(0)]],
  notes: [''],
  mealPrompt: [''] 
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

    const proteinControl = this.mealForm.get('protein');
    const carbsControl = this.mealForm.get('carbs');
    const fatControl = this.mealForm.get('fat');
    const caloriesControl = this.mealForm.get('calories');

    if (proteinControl && carbsControl && fatControl && caloriesControl) {
    
      this.mealForm.valueChanges
        .pipe(takeUntil(this.unsubscribe$)) 
        .subscribe(values => {
      
          const protein = values.protein || 0;
          const carbs = values.carbs || 0;
          const fat = values.fat || 0;

          if (
            typeof protein === 'number' && !isNaN(protein) &&
            typeof carbs === 'number' && !isNaN(carbs) &&
            typeof fat === 'number' && !isNaN(fat)
          ) {
            const calories = this.calculateCalories(protein, carbs, fat);

            caloriesControl.patchValue(calories, { emitEvent: false });
          }
        });
    }
  }


  private calculateCalories(proteinGrams: number, carbsGrams: number, fatGrams: number): number {
    const proteinCalories = proteinGrams * 4;
    const carbCalories = carbsGrams * 4;
    const fatCalories = fatGrams * 9;
    return Math.round(proteinCalories + carbCalories + fatCalories); // Round to nearest whole number
  }
  fetchMacroSuggestion(): void {
  const prompt = this.mealForm.get('mealPrompt')?.value;
  if (!prompt) return;

  const payload = { prompt: prompt };

  this.mealService.fetchMacroSuggestion(payload).subscribe({
    next: (res) => {
      this.mealForm.patchValue({
        protein: res.protein ?? 0,
        carbs: res.carbs ?? 0,
        fat: res.fat ?? 0,
        calories: res.calories ?? 0
      });
    },
    error: (err) => {
      console.error('GPT API error', err);
      alert('Failed to fetch macro suggestion.');
    }
  });
}

}