import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import Swal from 'sweetalert2';
import { MessagingService } from '../../../core/core services/messaging-service';
import { AuthPhotoComponent } from '../../../shared/Background_Photo/background';
import * as AuthDtos from '../../../shared/shared_models/Auth-models';
import { AuthService } from '../../../shared/shared_services/auth.service';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, AuthPhotoComponent],
  templateUrl: './login.html',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private presence = inject(MessagingService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  form = this.fb.group({
    email: ['', { validators: [Validators.required, Validators.email], nonNullable: true }],
    password: ['', { validators: [Validators.required], nonNullable: true }],
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const formData = this.form.getRawValue() as AuthDtos.RegistrationDto;
    this.auth.login(formData).subscribe({
      next: (res: any) => {
        Swal.fire({
          title: 'Welcome Back!',
          text: 'Logging you in...',
          icon: 'success',
          timer: 1500,
          showConfirmButton: false,
        }).then(() => {
          this.router.navigateByUrl('/ChatPage');
        });
      },
      error: (err) => {
        const message = err?.error?.errors?.join(', ') || 'Something went wrong. Please try again.';
        Swal.fire({
          title: 'Login Failed',
          text: message,
          icon: 'error',
          confirmButtonColor: '#0891b2',
        });
      },
    });
  }
}
