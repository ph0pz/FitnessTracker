import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { format } from 'date-fns';
import { Meal, SavedItem } from '../../models/meal.model';
import { SavedMealsComponent } from '../saved-meal/saved-meals.component';
import { AddMealDialogComponent } from './add-meal-dialog/add-meal-dialog.component';
import { SharedMaterialModule } from '../../core/shared.module';
import { MealService } from '../../core/service/meal.service';

@Component({
  selector: 'app-meal',
  templateUrl: './meal.component.html',
  styleUrls: ['./meal.component.scss'],
  standalone: true,
  imports: [
    SharedMaterialModule,
    SavedMealsComponent
  ]
})
export class MealComponent implements OnInit {
  selectedDate = new FormControl(new Date());
  meals: Meal[] = [];
  filteredMeals: Meal[] = [];
  isLoading = false;

  @ViewChild(SavedMealsComponent) savedMealsComponent!: SavedMealsComponent;

  constructor(private router: Router, private dialog: MatDialog, private mealService: MealService) {}

  ngOnInit(): void {
    this.fetchMealsForDate(this.selectedDate.value);
    this.selectedDate.valueChanges.subscribe(date => {
      this.fetchMealsForDate(date);
    });
  }

  fetchMealsForDate(date: Date | null): void {
    if (!date) return;
    this.isLoading = true;
    const formattedDate = format(date, 'yyyy-MM-dd');
    this.mealService.getMealsByDate(formattedDate).subscribe({
      next: (meals: Meal[]) => {
        this.meals = meals;
        this.filteredMeals = meals;
        this.isLoading = false;
      },
      error: () => {
        this.meals = [];
        this.filteredMeals = [];
        this.isLoading = false;
      }
    });
  }

  filterMeals(): void {
    const dateStr = this.selectedDate.value?.toISOString().slice(0, 10);
    this.filteredMeals = this.meals.filter(m => m.date === dateStr);
  }

  goToDashboard(): void {
    this.router.navigate(['/dashboard']);
  }

  openAddMealDialog(): void {
    const dialogRef = this.dialog.open(AddMealDialogComponent, {
      width: '400px',
      height: 'auto'
    });

    dialogRef.afterClosed().subscribe((result: Meal | undefined) => {
      if (result && this.selectedDate.value) { // <-- Add null check here
        this.isLoading = true;
        const mealToAdd = {
          ...result,
          date: this.selectedDate.value.toISOString()
        };
        this.mealService.addMeal(mealToAdd).subscribe({
          next: () => {
            this.fetchMealsForDate(this.selectedDate.value);
          },
          error: () => {
            this.isLoading = false;
          }
        });
      }
    });
  }

  editMeal(meal: Meal): void {
    // Open edit dialog
  }

  deleteMeal(meal: Meal): void {
    if (!meal?.id) return;
    this.isLoading = true;
    this.mealService.deleteMeal(meal.id).subscribe({
      next: () => {
        this.fetchMealsForDate(this.selectedDate.value);
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  addToSaved(meal: Meal): void {
    const savedItem: SavedItem = {
      id: '',
      calories: meal.calories,
      protein: meal.protein,
      carbs: meal.carbs,
      fat: meal.fat,
      name: meal.mealName,
    };
    this.mealService.addSavedMeal(savedItem).subscribe({
      next: () => {
        // Refresh the saved meals list in the child component
        if (this.savedMealsComponent) {
          this.savedMealsComponent.loadSavedItems();
        }
      }
    });
  }
}