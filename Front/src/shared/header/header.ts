import { CommonModule } from '@angular/common';
import { Component, ElementRef, inject, Input, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { UserService } from '../../core/core services/user-service';
import { ImageUploadComponent } from '../modal/photo-cropper/photo-cropper';
import { AuthService } from '../shared_services/auth.service';
import { NotificationService } from '../shared_services/notification.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.html',
  standalone: true,
  imports: [CommonModule, ImageUploadComponent],
})
export class HeaderComponent implements OnInit {
  @ViewChild('imageUploader') imageUploader!: ImageUploadComponent;
  private auth = inject(AuthService);
  private notification = inject(NotificationService);
  private userService = inject(UserService);
  private router = inject(Router);

  @Input() title: string = 'TalkReal';
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  username: string = '';
  userId: string = '';
  ImageURL: string | null = null;
  menuOpened = false;

  ngOnInit(): void {
    this.extractUserData();
  }

  private extractUserData(): void {
    const userData = localStorage.getItem('user_data');
    if (userData) {
      try {
        const parsedData = JSON.parse(userData);
        this.username = parsedData.userName || 'Guest';
        this.userId = parsedData.userId || '';

        if (this.userId) {
          this.loadProfileImage();
        }
      } catch (error) {
        console.error('Failed to parse user data:', error);
      }
    }
  }
  loadProfileImage(): void {
    if (!this.userId) return;

    this.userService.GetImagetById(this.userId).subscribe({
      next: (res) => {
        if (res && res.imageURL) {
          this.ImageURL = `${res.imageURL}?t=${new Date().getTime()}`;
        } else {
          this.ImageURL = null;
        }
      },
      error: (err) => {
        console.error('Error fetching image:', err);
        this.ImageURL = null;
      },
    });
  }
  handleImageUpload(file: File): void {
    console.error('File Received:', file);
    const formData = new FormData();
    formData.append('file', file);

    this.userService.AddPhoto(formData, this.userId).subscribe({
      next: () => {
        this.notification.showSuccess('Photo updated successfully');
        this.loadProfileImage();
        this.menuOpened = false;
      },
      error: () => this.notification.showError('Failed to upload photo'),
    });
  }
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (!file) return;

    const formData = new FormData();
    formData.append('file', file);

    this.userService.AddPhoto(formData, this.userId).subscribe({
      next: () => {
        this.notification.showSuccess('Photo updated successfully');
        this.loadProfileImage();
        this.menuOpened = false;
      },
      error: () => this.notification.showError('Failed to upload photo'),
      complete: () => {
        this.fileInput.nativeElement.value = '';
      },
    });
  }
  deletePhoto(): void {
    Swal.fire({
      title: 'Are you sure?',
      text: 'This will remove your profile picture.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#2563eb',
      cancelButtonColor: '#ef4444',
      confirmButtonText: 'Yes, delete it!',
      customClass: {
        popup: 'rounded-none border border-blue-100',
        confirmButton: 'rounded-none px-6 uppercase text-xs font-bold',
      },
    }).then((result) => {
      if (result.isConfirmed) {
        this.userService.DeleteImagetById(this.userId).subscribe({
          next: (isDeleted) => {
            if (isDeleted) {
              this.ImageURL = null;
              this.notification.showSuccess('Photo removed successfully');
              this.menuOpened = false;
            }
          },
          error: () => this.notification.showError('Failed to delete photo'),
        });
      }
    });
  }

  toggleMenu(): void {
    this.menuOpened = !this.menuOpened;
  }

  logout(): void {
    this.auth.logout();
  }
}
