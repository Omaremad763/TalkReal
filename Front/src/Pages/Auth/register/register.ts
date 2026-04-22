import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import { ApiResponse } from '../../../shared/shared_models/api-response.model';
import * as AuthDtos from '../../../shared/shared_models/Auth-models';
import { AuthService } from '../../../shared/shared_services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, AuthPhotoComponent],
  templateUrl: './register.html',
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const formData = this.form.getRawValue() as AuthDtos.RegistrationDto;
    this.auth.register(formData).subscribe({
      next: (res: ApiResponse<any>) => {
        Swal.fire({
          title: 'Registration Successful!',
          text: 'Your account has been created. You can now log in.',
          icon: 'success',
          confirmButtonColor: '#06b6d4',
          confirmButtonText: 'Go to Login',
          allowOutsideClick: false,
        }).then((result) => {
          if (result.isConfirmed) {
            this.router.navigate(['/login']);
          }
        });
      },
      error: (err) => {
        const errorMessage = err.error?.message || err.message || 'Registration failed';
        Swal.fire({
          icon: 'error',
          title: 'Registration Error',
          text: errorMessage,
          confirmButtonColor: '#0f172a',
        });
      },
    });
  }
}
