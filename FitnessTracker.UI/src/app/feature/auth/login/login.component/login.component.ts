import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/auth/auth.service';

// Angular Material Imports
import { MatInputModule } from '@angular/material/input';       // For <mat-form-field> and <input matInput>
import { MatFormFieldModule } from '@angular/material/form-field'; // For <mat-form-field>
import { MatButtonModule } from '@angular/material/button';     // For <button mat-button>
import { MatCardModule } from '@angular/material/card';         // For <mat-card>

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatInputModule,        // <--- Add this
    MatFormFieldModule,    // <--- Add this
    MatButtonModule,       // <--- Add this
    MatCardModule          // <--- Add this
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    this.errorMessage = null;
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (response: any) => {
          console.log('Login successful!', response);
          this.router.navigate(['/dashboard']);
        },
        error: (err: { error: { message: string; }; }) => {
          console.error('Login failed:', err);
          this.errorMessage = err.error?.message || 'Invalid credentials or an error occurred.';
        }
      });
    }
  }
}