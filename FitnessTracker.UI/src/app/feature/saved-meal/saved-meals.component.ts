import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { Meal, SavedItem } from '../../models/meal.model';
import { SharedMaterialModule } from '../../core/shared.module';
import { MealService } from '../../core/service/meal.service';

@Component({
  selector: 'app-saved-meals',
  templateUrl: './saved-meals.component.html',
  styleUrls: ['./saved-meals.component.scss'],
  standalone: true,
  imports: [SharedMaterialModule]
})
export class SavedMealsComponent implements OnInit {
  @Input() selectedDate: Date | null = null;
  @Output() mealAdded = new EventEmitter<void>();
  savedItems: SavedItem[] = [];
  isLoading = false;

  constructor(private mealService: MealService) {}

  ngOnInit(): void {
    this.loadSavedItems();
  }

  loadSavedItems(): void {
    this.isLoading = true;
    this.mealService.getSavedMeals().subscribe({
      next: items => {
        this.savedItems = items;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  addItem(item: SavedItem): void {
    const date = this.selectedDate ? this.selectedDate.toISOString() : new Date().toISOString();
    const meal: Meal = {
      id: '', // or omit if backend generates
      date: date, // Use selected date from parent
      mealName: 'From Saved Item',
      name: item.name,
      calories: item.calories,
      protein: item.protein,
      carbs: item.carbs,
      fat: item.fat,
      notes: item.name
    };
    this.mealService.addMeal(meal).subscribe({
      next: () => {
        this.mealAdded.emit();
      }
    });
  }

  deleteItem(item: SavedItem): void {
    this.isLoading = true;
    this.mealService.deleteSavedMeal(item.id).subscribe({
      next: () => {
        this.loadSavedItems();
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  addSavedItem(): void {
    // Open add dialog
  }
}