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
      next: (res: string) => {
        Swal.fire({
          title: 'Welcome Back!',
          text: 'Logging you in...',
          icon: 'success',
          timer: 1500,
          timerProgressBar: true,
          showConfirmButton: false,
        }).then(() => {
          const ChatUI = this.route.snapshot.queryParams['returnUrl'] || '/ChatPage';
          this.auth.extractAndSaveClaims(res);
          this.router.navigateByUrl(ChatUI, { replaceUrl: true });
        });
      },
      error: (err) => {
        let errorMessage = 'Our systems are down, please try again later.';
        let iconType: 'error' | 'warning' = 'warning';

        if (err.status === 401) {
          errorMessage = 'Invalid email or password. Please try again.';
          iconType = 'error';
        }

        Swal.fire({
          title: iconType === 'error' ? 'Login Failed' : 'Server Error',
          text: errorMessage,
          icon: iconType,
          confirmButtonColor: '#0891b2',
        });
      },
    });
  }
}
